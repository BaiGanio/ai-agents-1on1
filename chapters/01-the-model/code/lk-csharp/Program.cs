// Chapter 01 — the model.
//
// The whole point of this file: a model is an HTTP endpoint. 
// You send it a list of messages.
// It sends back one more message. 
// There is no magic, no SDK, and no state on the server.

using System.Net.Http.Json;

List<Message> messages = [];

using HttpClient http = Helper.OllamaHttpClient;

while (true)
{
    Console.Write("You: ");
    string prompt = Console.ReadLine();

    messages.Add(new("user", prompt));

    ChatRequest request = new (Helper.model, messages, Stream: false);

    var response = await http.PostAsJsonAsync("/api/chat", request, Helper.AgentJsonOptions);

    if (!response.IsSuccessStatusCode) 
        throw new HttpRequestException(
            $"{(int)response.StatusCode} {response.ReasonPhrase}: {await response.Content.ReadAsStringAsync()}"
        );        

    var reply = await response.Content.ReadFromJsonAsync<ChatResponse>(Helper.AgentJsonOptions);

    Console.WriteLine($"{Helper.model} (role: {reply!.Message.Role}): {reply!.Message.Content.Trim()}"); 
    
    messages.Add(reply!.Message);
}

