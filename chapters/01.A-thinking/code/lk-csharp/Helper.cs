
using System.Text.Json;
using System.Text.Json.Serialization;

public static class Helper
{
    public static readonly string OllamaUrl = "http://localhost:11434";

    // A model that cannot think answers Think: true with an error, not with an answer.
    public static readonly string Model = "Qwen3.5:4b";
    public static readonly bool? Think = true;

    public static readonly string Grey = "\x1b[90m";
    public static readonly string Reset = "\x1b[0m";

    // One client for the whole process. A new one per call leaks sockets.
    public static readonly HttpClient OllamaHttpClient =
        new()
        {
            BaseAddress = new Uri(OllamaUrl)
        };

    public static readonly JsonSerializerOptions AgentJsonOptions =
        new()
        {
            // Ollama's field names are lower case. C# properties are not, so we say so once, here.
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        };
}

record Message(string Role, string Content, string? Thinking = null);
record ChatRequest(string Model, List<Message> Messages, bool Stream, bool? Think);
record ChatResponse(Message Message);
