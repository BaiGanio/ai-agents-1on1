
using System.Text.Json;
using System.Text.Json.Serialization;

public static class Helper
{
    public static readonly string OllamaUrl = "http://localhost:11434";
    public static readonly string Model = "gemma4:e2b";

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

record Message(string Role, string Content);
record ChatRequest(string Model, List<Message> Messages, bool Stream);
record ChatResponse(Message Message);
