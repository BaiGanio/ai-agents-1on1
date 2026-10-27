// Chapter 01.A — thinking.
//
// Chapter 01 showed a model is an HTTP endpoint.
// This chapter shows it thinks before it answers, and the reply streams, word by word.
// The thinking is grey. The answer is plain.

using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

List<Message> messages = [];

HttpClient http = Helper.OllamaHttpClient;

Console.WriteLine("Type a question. An empty line stops the app.");

while (true)
{
    Console.Write("You: ");

    // ReadLine gives null when input ends, so this one check covers both quit and end of input.
    string? prompt = Console.ReadLine();
    if (string.IsNullOrWhiteSpace(prompt)) break;

    messages.Add(new("user", prompt));

    ChatRequest request = new (Helper.Model, messages, Stream: true, Think: Helper.Think);

    using HttpRequestMessage message =
        new (HttpMethod.Post, "/api/chat")
        {
            Content = JsonContent.Create(request, options: Helper.AgentJsonOptions),
        };

    // ResponseHeadersRead hands us the stream as it arrives, instead of waiting for the last byte.
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
