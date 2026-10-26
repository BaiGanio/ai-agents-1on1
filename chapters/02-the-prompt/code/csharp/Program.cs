// Chapter 02 — the prompt.
//
// The model now gets a system message first, read from a text file. Swap the file and the agent
// is somebody else, with no code change. A loop keeps that message in front of every turn.

using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

var ollamaUrl = Environment.GetEnvironmentVariable("OLLAMA_URL") ?? "http://localhost:11434";
var model = Environment.GetEnvironmentVariable("MODEL") ?? "gemma4:e2b";

// on, off, or unset. Unset sends nothing, so the model does what it ships with. We do not send
// true by default: a model that cannot think answers that with an error, not with an answer.
bool? think = Environment.GetEnvironmentVariable("THINK") switch
{
    "on" => true,
    "off" => false,
    _ => null,
};

// Names a file in ../prompts. That file is the whole personality of the agent.
var promptName = Environment.GetEnvironmentVariable("PROMPT") ?? "librarian";

const string Grey = "\x1b[90m";
const string Reset = "\x1b[0m";

// One HttpClient for the process. A new one per call leaks sockets, and a local model can take
// minutes on the first answer while it loads into RAM, so the 100-second default is too short.
using var http = new HttpClient
{
    BaseAddress = new Uri(ollamaUrl),
    Timeout = TimeSpan.FromMinutes(5),
};

// Ollama's field names are lower case. C# properties are not, so we say so once, here, and
// never think about it again. WhenWritingNull is what makes `think: null` mean "not sent".
var json = new JsonSerializerOptions
{
    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
};

// Read once, before anything else happens. Edit the file mid-session and nothing changes until
// you restart. The path is relative to the working directory, which `dotnet run` sets to this
// project folder. Run the binary out of bin/ instead and this throws.
var system = await File.ReadAllTextAsync($"../prompts/{promptName}.md");

// The model remembers nothing between calls. Every turn sends the whole history back, so the
// system message at index 0 is re-sent every single time. Drop it and the agent forgets itself.
List<Message> messages = [new("system", system)];

if (args.Length > 0)
{
    messages.Add(new("user", string.Join(' ', args)));
    await ChatAsync(messages);
}
else
{
    Console.Write("you> ");

    // `is { } line` means "not null, and call it line". ReadLine returns null at end of input,
    // which is Ctrl-D, so that pattern is also how the loop ends.
    while (Console.ReadLine() is { } line)
    {
        if (line.Trim().Length > 0)
        {
            messages.Add(new("user", line));
            // The list only ever grows. It has no limit until chapter 06 gives it one.
            messages.Add(await ChatAsync(messages));
        }
        Console.Write("you> ");
    }

    Console.WriteLine();
}

// Sends the conversation so far. Prints the reply as it streams. Returns the whole message.
async Task<Message> ChatAsync(List<Message> history)
{
    var request = new ChatRequest(model, history, Stream: true, Think: think);

    // ResponseHeadersRead: return as soon as the headers arrive. Without it HttpClient waits for
    // the whole body first, and the stream stops being a stream.
    using var message = new HttpRequestMessage(HttpMethod.Post, "/api/chat")
    {
        Content = JsonContent.Create(request, options: json),
    };
    using var response = await http.SendAsync(message, HttpCompletionOption.ResponseHeadersRead);

    if (!response.IsSuccessStatusCode)
    {
        // Ollama puts the real reason in the body — an unknown model, a bad field. Without this
        // line you get a bare 500 and no idea which of the two it was.
        var body = await response.Content.ReadAsStringAsync();
        throw new HttpRequestException(
            $"{(int)response.StatusCode} {response.ReasonPhrase}: {body}");
    }

    var content = new StringBuilder();
    var thinking = new StringBuilder();

    // A stream is one JSON object per line. ReadLineAsync waits for a whole line, so a network
    // chunk that stops mid-line is not our problem here.
    using var reader = new StreamReader(await response.Content.ReadAsStreamAsync());
    while (await reader.ReadLineAsync() is { } line)
    {
        if (line.Length == 0) continue;
        var part = JsonSerializer.Deserialize<ChatResponse>(line, json)!.Message;

        if (!string.IsNullOrEmpty(part.Thinking))
        {
            Console.Write(Grey + part.Thinking + Reset);
            thinking.Append(part.Thinking);
        }
        if (!string.IsNullOrEmpty(part.Content))
        {
            if (content.Length == 0 && thinking.Length > 0) Console.Write("\n\n");
            Console.Write(part.Content);
            content.Append(part.Content);
        }
    }

    Console.WriteLine();
    return new Message("assistant", content.ToString(), thinking.ToString());
}

record Message(string Role, string Content, string? Thinking = null);
record ChatRequest(string Model, List<Message> Messages, bool Stream, bool? Think);
record ChatResponse(Message Message);
