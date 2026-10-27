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

    // Flip this to false to get the answer without the wait. A model that cannot think answers
    // Think: true with a 400, not with an answer, so leave it off on those.
    public static readonly bool? Think = true;

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

    // Prints each piece the moment it arrives, and fills the caller's two builders. Printing and
    // remembering are two jobs, so the caller owns what it wants to keep.
    public static async Task HandleStream(
        HttpResponseMessage response,
        StringBuilder content,
        StringBuilder thinking)
    {
        // A stream is one JSON object per line. ReadLineAsync waits for a whole line, so a
        // network chunk that stops mid-line is not our problem here.
        using var reader = new StreamReader(await response.Content.ReadAsStreamAsync());
        while (await reader.ReadLineAsync() is { } line)
        {
            if (line.Length == 0) continue;

            Message part =
                JsonSerializer.Deserialize<ChatResponse>(line, AgentJsonOptions)!.Message;

            if (!string.IsNullOrEmpty(part.Thinking))
            {
                Console.Write(Grey + part.Thinking + Reset);
                thinking.Append(part.Thinking);
            }

            if (!string.IsNullOrEmpty(part.Content))
            {
                // The first word of the answer needs a gap after the grey, or they run together.
                if (content.Length == 0 && thinking.Length > 0) Console.Write("\n\n");
                Console.Write(part.Content);
                content.Append(part.Content);
            }
        }
    }
}
