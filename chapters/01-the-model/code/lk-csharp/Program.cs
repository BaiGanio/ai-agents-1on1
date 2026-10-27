// Chapter 01 — the model.
//
// The whole point of this file: a model is an HTTP endpoint.
// You send it a list of messages.
// It sends back one more message.
// There is no magic, no SDK, and no state on the server.

using System.Net.Http.Json;

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

    ChatRequest request = new (Helper.Model, messages, Stream: false);

    var response = await http.PostAsJsonAsync("/api/chat", request, Helper.AgentJsonOptions);

    if (!response.IsSuccessStatusCode)
        throw new HttpRequestException(
            $"{(int)response.StatusCode} {response.ReasonPhrase}: {await response.Content.ReadAsStringAsync()}"
        );

    var reply = await response.Content.ReadFromJsonAsync<ChatResponse>(Helper.AgentJsonOptions);

    Console.WriteLine($"{Helper.Model} (role: {reply!.Message.Role}): {reply!.Message.Content.Trim()}");

    messages.Add(reply!.Message);
}
