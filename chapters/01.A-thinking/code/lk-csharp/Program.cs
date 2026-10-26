// Chapter 01 — the model.
//
// The whole point of this file: a model is an HTTP endpoint. 
// You send it a list of messages.
// It sends back one more message. 
// There is no magic, no SDK, and no state on the server.

using System.Net.Http.Json;
using System.Text;
using System.Text.Json;


List<Message> messages = [];
int counter = 0;

using HttpClient http = Helper.OllamaHttpClient;

while (true)
{
    Console.WriteLine($"*****--run: {counter++}--*****");
    Console.Write("You: ");
    string prompt = Console.ReadLine();

    messages.Add(new("user", prompt));

    ChatRequest request = new (Helper.model, messages, Stream: true, Think: Helper.Think);

    using HttpRequestMessage message = 
        new (HttpMethod.Post, "/api/chat")
        {
            Content = JsonContent.Create(request, options: Helper.AgentJsonOptions),
        };

    using var response = await http.SendAsync(message, HttpCompletionOption.ResponseHeadersRead);

    if (!response.IsSuccessStatusCode) 
        throw new HttpRequestException(
            $"{(int)response.StatusCode} {response.ReasonPhrase}: {await response.Content.ReadAsStringAsync()}"
        );        

    var content = new StringBuilder();
    var thinking = new StringBuilder();

    // A stream is one JSON object per line. ReadLineAsync waits for a whole line, so a network
    // chunk that stops mid-line is not our problem here.
    using var reader = new StreamReader(await response.Content.ReadAsStreamAsync());
    while (await reader.ReadLineAsync() is { } line)
    {
        if (line.Length == 0) continue;
        Message part = JsonSerializer.Deserialize<ChatResponse>(line, Helper.AgentJsonOptions)!.Message;

        if (!string.IsNullOrEmpty(part.Thinking))
        {
            Console.Write(Helper.Grey + part.Thinking + Helper.Reset);
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
    messages.Add(new Message("assistant", content.ToString(), thinking.ToString()));
}

