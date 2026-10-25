// Chapter 01 — the model.
//
// The whole point of this file: a model is an HTTP endpoint. You send it a list of messages.
// It sends back one more message. There is no magic, no SDK, and no state on the server.

using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;

var ollamaUrl = Environment.GetEnvironmentVariable("OLLAMA_URL") ?? "http://localhost:11434";
var model = Environment.GetEnvironmentVariable("MODEL") ?? "gemma4:e2b";

// One HttpClient for the process. A new one per call leaks sockets, and a local model can take
// minutes on the first answer while it loads into RAM, so the 100-second default is too short.
using var http = new HttpClient { BaseAddress = new Uri(ollamaUrl), Timeout = TimeSpan.FromMinutes(5) };

// Ollama's field names are lower case. C# properties are not, so we say so once, here, and
// never think about it again.
var json = new JsonSerializerOptions
{
    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
};

var prompt = args.Length > 0 ? string.Join(' ', args) : "Say hello in five words.";

// The model remembers nothing between calls. Every turn sends the whole history back.
// That list is the agent's short memory, and it is the only memory it has until chapter 06.
List<Message> messages = [new("user", prompt)];

var answer = await ChatAsync(messages);
Console.WriteLine(answer.Content.Trim());

// Sends the conversation so far. Returns the one message the model wrote.
async Task<Message> ChatAsync(List<Message> history)
{
    // stream:false means "one JSON object when you are done", not a drip of tokens.
    // Streaming is nicer for a user and noisier for a lesson, so it stays off until chapter 01.A.
    var request = new ChatRequest(model, history, Stream: false);

    var response = await http.PostAsJsonAsync("/api/chat", request, json);

    if (!response.IsSuccessStatusCode)
    {
        // Ollama puts the real reason in the body — an unknown model, a bad field. Without this
        // line you get a bare 500 and no idea which of the two it was.
        var body = await response.Content.ReadAsStringAsync();
        throw new HttpRequestException($"{(int)response.StatusCode} {response.ReasonPhrase}: {body}");
    }

    var reply = await response.Content.ReadFromJsonAsync<ChatResponse>(json);
    return reply!.Message;
}

record Message(string Role, string Content);
record ChatRequest(string Model, List<Message> Messages, bool Stream);
record ChatResponse(Message Message);
