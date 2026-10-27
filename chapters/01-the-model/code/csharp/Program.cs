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

    ChatRequest request = new(Helper.Model, messages, Stream: false);

    var response = await http.PostAsJsonAsync("/api/chat", request, Helper.AgentJsonOptions);

    if (!response.IsSuccessStatusCode)
    {
        // Ollama puts the real reason in the body — an unknown model, a bad field. Without this
        // line you get a bare 500 and no idea which of the two it was.
        var body = await response.Content.ReadAsStringAsync();
        throw new HttpRequestException(
            $"{(int)response.StatusCode} {response.ReasonPhrase}: {body}");
    }

    var reply = await response.Content.ReadFromJsonAsync<ChatResponse>(Helper.AgentJsonOptions);

    Console.WriteLine(
        $"{Helper.Model} (role: {reply!.Message.Role}): {reply.Message.Content.Trim()}");

    // The reply goes back into the list. The model remembers nothing, so the list is the memory.
    messages.Add(reply.Message);
}
