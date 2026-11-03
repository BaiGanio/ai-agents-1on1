using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

// Everything needed to reach the model, in one place, so Program.cs is only the conversation.
public static class Helper
{
    // Both read the environment first. Nobody has to edit code to point at another machine
    // or swap the model.
    public static readonly string Model =
        Environment.GetEnvironmentVariable("MODEL") ?? "gemma4:e2b";

    public static readonly string OllamaUrl =
        Environment.GetEnvironmentVariable("OLLAMA_URL") ?? "http://localhost:11434";

    // Names a file in ../prompts. That file is the whole personality of the agent, and it is
    // the only thing you change to get a different one.
    public static readonly string PromptName =
        Environment.GetEnvironmentVariable("PROMPT") ?? "librarian";

    // The folder the Librarian reads. Every path the model writes is joined onto this one.
    public static readonly string RepoRoot =
        Environment.GetEnvironmentVariable("REPO") ?? "../../../../sample-repo";

    // Long memory. Next to the prompts, outside RepoRoot, so list_files never shows the agent
    // its own notes as if they were part of the repo.
    public static readonly string NotesPath =
        Environment.GetEnvironmentVariable("NOTES") ?? "../notes.md";

    // Flip this to false to get the answer without the wait. A model that cannot think answers
    // Think: true with a 400, not with an answer, so leave it off on those.
    public static readonly bool? Think = false;

    // The thinking is grey so your eye skips it. These are ANSI colour codes, not a library.
    public static readonly string Grey = "\x1b[90m";
    public static readonly string Reset = "\x1b[0m";

    // One client for the whole process. A new one per call leaks sockets, and a local model can
    // take minutes on the first answer while it loads into RAM, so 100 seconds is too short.
    public static readonly HttpClient OllamaHttpClient =
        new()
        {
            BaseAddress = new Uri(OllamaUrl),
            Timeout = TimeSpan.FromMinutes(5),
        };

    public static readonly JsonSerializerOptions AgentJsonOptions =
        new()
        {
            // Ollama's field names are lower case. C# properties are not, so we say so once, here.
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            // This is what makes Think: null mean "field not sent". Sent as null, it means
            // "no thinking", which is a different answer.
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        };

    // Ollama puts the real reason in the body — an unknown model, a model that cannot think.
    // Without this you get a bare 400 and no idea which of the two it was.
    public static async Task HandleResponseError(HttpResponseMessage response)
    {
        var body = await response.Content.ReadAsStringAsync();
        throw new HttpRequestException(
            $"{(int)response.StatusCode} {response.ReasonPhrase}: {body}");
    }

    // Prints each piece the moment it arrives, and fills the caller's three collections. Printing
    // and remembering are two jobs, so the caller owns what it wants to keep.
    public static async Task HandleStream(
        HttpResponseMessage response,
        StringBuilder content,
        StringBuilder thinking,
        List<ToolCall> calls,
        Func<Task> quiet,
        CancellationToken stop)
    {
        // A stream is one JSON object per line. ReadLineAsync waits for a whole line, so a
        // network chunk that stops mid-line is not our problem here.
        using var reader = new StreamReader(await response.Content.ReadAsStreamAsync(stop));
        while (await reader.ReadLineAsync(stop) is { } line)
        {
            if (line.Length == 0) continue;

            ChatResponse chunk =
                JsonSerializer.Deserialize<ChatResponse>(line, AgentJsonOptions)!;
            Message part = chunk.Message;

            // Only the last line carries it. It is the size of everything we just sent, which
            // is the number that grows on every step, and the number this chapter cuts.
            if (chunk.PromptEvalCount is int tokens)
            {
                await quiet();
                Console.Write($"{Grey} [{tokens} tokens in]{Reset}");
            }

            if (!string.IsNullOrEmpty(part.Thinking))
            {
                await quiet();
                Console.Write(Grey + part.Thinking + Reset);
                thinking.Append(part.Thinking);
            }

            // Measured: a tool call arrives whole, in one line. Unlike Content, which comes a
            // few characters at a time, it is never split across lines — so nothing to stitch.
            if (part.ToolCalls is not null) calls.AddRange(part.ToolCalls);

            if (!string.IsNullOrEmpty(part.Content))
            {
                await quiet();
                // The first word of the answer needs a gap after the grey, or they run together.
                if (content.Length == 0 && thinking.Length > 0) Console.Write("\n\n");
                Console.Write(part.Content);
                content.Append(part.Content);
            }
        }
    }

    // Grey dots until the first word. A big file can take the model a long time to read, and
    // without these the screen sits still the whole while. \b moves the cursor back, so each
    // frame draws over the last one.
    static async Task Spin(CancellationToken stop)
    {
        try
        {
            for (int i = 0; ; i++)
            {
                Console.Write($"{Grey}{new string('.', i % 4),-3}{Reset}\b\b\b");
                await Task.Delay(300, stop);
            }
        }
        catch (OperationCanceledException) { }

        Console.Write("   \b\b\b");
    }

    // One send, one streamed reply. It does not touch the history — the caller appends, so
    // Program.cs still shows the memory being built. Chapter 04 called this twice by hand.
    // Chapter 05 calls it in a while loop, and that loop is the whole chapter. Chapter 07 hands
    // it a token: when the question's clock runs out, the send and the stream both stop.
    public static async Task<Message> Ask(
        List<Message> history, List<Tool>? tools, CancellationToken stop)
    {
        // The tool list goes on every single request. The model remembers nothing, so there is
        // no registration step anywhere — the tools exist for exactly as long as one call.
        // Chapter 08 passes the list in. Null leaves the field out, and the model gets no tools.
        ChatRequest request = new(Model, history, tools, Stream: true, Think: Think);

        StringBuilder content = new();
        StringBuilder thinking = new();
        List<ToolCall> calls = [];

        // The dots run until the first thing is printed, or until anything goes wrong. Quiet is
        // safe to call twice, so the finally can always call it.
        using CancellationTokenSource dots = CancellationTokenSource.CreateLinkedTokenSource(stop);
        Task spinner = Spin(dots.Token);
        async Task Quiet()
        {
            dots.Cancel();
            await spinner;
        }

        try
        {
            using HttpRequestMessage message =
                new(HttpMethod.Post, "/api/chat")
                {
                    Content = JsonContent.Create(request, options: AgentJsonOptions),
                };

            // ResponseHeadersRead hands us the stream as it arrives. Without it HttpClient waits
            // for the last byte, and the stream stops being a stream.
            using var response =
                await OllamaHttpClient.SendAsync(
                    message, HttpCompletionOption.ResponseHeadersRead, stop);

            if (!response.IsSuccessStatusCode)
                await HandleResponseError(response);

            await HandleStream(response, content, thinking, calls, Quiet, stop);
        }
        finally
        {
            await Quiet();
        }

        // An empty list, not null, would be sent back as `"tool_calls": []`. Null is left out of
        // the JSON entirely by WhenWritingNull, which is what a plain answer should look like.
        return new Message(
            "assistant",
            content.ToString(),
            thinking.ToString(),
            calls.Count > 0 ? calls : null);
    }
}
