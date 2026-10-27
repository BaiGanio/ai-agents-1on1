// Chapter 03 — the chat page.
//
// The agent gets a window. One HTML page, one POST /chat endpoint, served by the same process
// that calls the model. The stream from chapter 01.A now runs all the way to the browser.

using System.Net;
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

// 11434 is Ollama. 3000 is us. Two servers, two ports, one machine.
var port = Environment.GetEnvironmentVariable("PORT") ?? "3000";

const string Grey = "\x1b[90m";
const string Reset = "\x1b[0m";

// Set once thinking has been printed, so the answer starts on a line of its own.
var grey = false;

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
    await ChatAsync(messages, ToTerminal);
    Console.WriteLine();
    return;
}

// Read once, at start. The page is not the lesson, so we do not reload it per request.
var page = Encoding.UTF8.GetBytes(await File.ReadAllTextAsync("../web/index.html"));

using var listener = new HttpListener();
// The trailing slash is not optional. Prefixes.Add throws ArgumentException without it.
listener.Prefixes.Add($"http://localhost:{port}/");
listener.Start();
Console.WriteLine($"Repo Librarian on http://localhost:{port}");

// One request at a time, on purpose. There is one history and one model, so a second question
// would only queue behind the first anyway.
while (true)
{
    var context = await listener.GetContextAsync();
    var request = context.Request;
    var response = context.Response;

    if (request.HttpMethod != "POST" || request.Url?.AbsolutePath != "/chat")
    {
        // There is one page and one endpoint, so there is no router. Everything else is the page.
        response.ContentType = "text/html; charset=utf-8";
        await response.OutputStream.WriteAsync(page);
        response.Close();
        continue;
    }

    using var body = new StreamReader(request.InputStream, Encoding.UTF8);
    messages.Add(new("user", await body.ReadToEndAsync()));

    // SendChunked sends the body in pieces with no content-length, which we do not know yet.
    // That is what lets the browser show the first word before the last one is written.
    response.ContentType = "application/x-ndjson; charset=utf-8";
    response.SendChunked = true;

    try
    {
        messages.Add(await ChatAsync(messages, part => ToBrowser(part, response)));
    }
    catch (Exception error)
    {
        // One bad request must not take the server down with it, and an uncaught exception in
        // here would. The headers are already sent, so the only place left to say so is the
        // stream. Drop the question too: a user turn with no answer confuses the next call.
        ToBrowser(new JsonError(error.Message), response);
        messages.RemoveAt(messages.Count - 1);
    }

    response.Close();
}

// Sends the conversation so far. Hands each streamed piece to `send`. Returns the whole reply.
async Task<Message> ChatAsync(List<Message> history, Action<Message> send)
{
    var body = new ChatRequest(model, history, Stream: true, Think: think);

    // ResponseHeadersRead: return as soon as the headers arrive. Without it HttpClient waits for
    // the whole body first, and the stream stops being a stream.
    using var message = new HttpRequestMessage(HttpMethod.Post, "/api/chat")
    {
        Content = JsonContent.Create(body, options: json),
    };
    using var response = await http.SendAsync(message, HttpCompletionOption.ResponseHeadersRead);

    if (!response.IsSuccessStatusCode)
    {
        // Ollama puts the real reason in the body — an unknown model, a bad field. Without this
        // line you get a bare 500 and no idea which of the two it was.
        var error = await response.Content.ReadAsStringAsync();
        throw new HttpRequestException(
            $"{(int)response.StatusCode} {response.ReasonPhrase}: {error}");
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

        // ChatAsync no longer decides where the words go. The caller does. That one change is
        // what lets the same method feed a terminal and a browser.
        send(part);
        thinking.Append(part.Thinking);
        content.Append(part.Content);
    }

    return new Message("assistant", content.ToString(), thinking.ToString());
}

// A `send` for the terminal: thinking in grey, a blank line, then the answer.
void ToTerminal(Message part)
{
    if (!string.IsNullOrEmpty(part.Thinking))
    {
        Console.Write(Grey + part.Thinking + Reset);
        grey = true;
    }
    if (!string.IsNullOrEmpty(part.Content))
    {
        if (grey)
        {
            Console.Write("\n\n");
            grey = false;
        }
        Console.Write(part.Content);
    }
}

// A `send` for the browser: one JSON object per line. SendChunked is what makes each line
// leave at once. The Flush is belt and braces — it arrives in pieces without it.
void ToBrowser(object part, HttpListenerResponse response)
{
    var line = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(part, json) + "\n");
    response.OutputStream.Write(line);
    response.OutputStream.Flush();
}

record Message(string Role, string Content, string? Thinking = null);
record JsonError(string Error);
record ChatRequest(string Model, List<Message> Messages, bool Stream, bool? Think);
record ChatResponse(Message Message);
