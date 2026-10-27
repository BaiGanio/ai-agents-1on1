// Chapter 02 — the prompt.
//
// The model now gets a system message first, read from a text file. Swap the file and the agent
// is somebody else, with no code change. The loop keeps that message in front of every turn.

using System.Net.Http.Json;
using System.Text;

// Read once, before anything else happens. Edit the file mid-session and nothing changes until
// you restart. The path is relative to the working directory, which `dotnet run` sets to this
// project folder. Run the binary out of bin/ instead and this throws.
string system = await File.ReadAllTextAsync($"../prompts/{Helper.PromptName}.md");

// The model remembers nothing between calls. Every turn sends the whole history back, so the
// system message at index 0 is re-sent every single time. Drop it and the agent forgets itself.
List<Message> messages = [new("system", system)];

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
