// Chapter 01.A — thinking.
//
// Chapter 01 showed a model is an HTTP endpoint. This chapter shows it thinks before it answers,
// and lets you choose: watch the thinking, or turn it off. The reply now streams, word by word.

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

const string Grey = "\x1b[90m";
const string Reset = "\x1b[0m";

// One HttpClient for the process. A new one per call leaks sockets, and a local model can take
// minutes on the first answer while it loads into RAM, so the 100-second default is too short.
using var http = new HttpClient { BaseAddress = new Uri(ollamaUrl), Timeout = TimeSpan.FromMinutes(5) };

// Ollama's field names are lower case. C# properties are not, so we say so once, here, and
// never think about it again. WhenWritingNull is what makes `think: null` mean "not sent".
var json = new JsonSerializerOptions
{
    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
};

var prompt = args.Length > 0 ? string.Join(' ', args) : "Say hello in five words.";

// The model remembers nothing between calls. Every turn sends the whole history back.
// That list is the agent's short memory, and it is the only memory it has until chapter 06.
List<Message> messages = [new("user", prompt)];

await ChatAsync(messages);

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
        throw new HttpRequestException($"{(int)response.StatusCode} {response.ReasonPhrase}: {body}");
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
