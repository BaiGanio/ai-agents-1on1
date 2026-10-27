// Chapter 01.A — thinking.
//
// Chapter 01 showed a model is an HTTP endpoint.
// This chapter shows it thinks before it answers, and that the reply arrives word by word.
// The thinking is grey. The answer is plain.

using System.Net.Http.Json;
using System.Text;

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

    ChatRequest request = new(Helper.Model, messages, Stream: true, Think: Helper.Think);

    using HttpRequestMessage message =
        new(HttpMethod.Post, "/api/chat")
        {
            Content = JsonContent.Create(request, options: Helper.AgentJsonOptions),
        };

    // ResponseHeadersRead hands us the stream as it arrives. Without it HttpClient waits for the
    // last byte, and the stream stops being a stream.
    using var response =
        await http.SendAsync(message, HttpCompletionOption.ResponseHeadersRead);

    if (!response.IsSuccessStatusCode)
        await Helper.HandleResponseError(response);

    StringBuilder content = new();
    StringBuilder thinking = new();

    Console.Write($"{Helper.Model}: ");

    await Helper.HandleStream(response, content, thinking);

    Console.WriteLine();

    // The reply goes back into the list. The model remembers nothing, so the list is the memory.
    messages.Add(new Message("assistant", content.ToString(), thinking.ToString()));
}
