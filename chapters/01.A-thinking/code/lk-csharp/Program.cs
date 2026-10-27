using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

HttpClient http = Helper.OllamaHttpClient;
List<Message> messages = [];

while (true)
{
    Console.Write("You: ");

    string? prompt = Console.ReadLine();

    if (string.IsNullOrWhiteSpace(prompt)) break;

    messages.Add(new Message("user", prompt));

    ChatRequest req = new(Helper.Model, messages, Stream: true, Think: Helper.Think);

    using HttpRequestMessage message =
        new(HttpMethod.Post, "/api/chat")
        {
            Content = JsonContent.Create(req, options: Helper.AgentJsonOptions),
        };
    using var response =
        await http.SendAsync(message, HttpCompletionOption.ResponseHeadersRead);

    if (!response.IsSuccessStatusCode)
        await Helper.HandleResponseError(response);

    StringBuilder content = new();
    StringBuilder thinking = new();

    Console.Write($"{Helper.Model}: ");

    await Helper.HandleStream(response, content, thinking);

    messages.Add(new Message("assistant", content.ToString(), thinking.ToString()));
    
    Console.WriteLine("\n");
}

