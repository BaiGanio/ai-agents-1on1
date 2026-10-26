
using System.Text.Json;
using System.Text.Json.Serialization;

public static class Helper
{   
    public static readonly string ollamaUrl = "http://localhost:11434";
    public static readonly string model = "qwen2.5:3b";
    public static bool? Think = null;
    public static string Grey = "\x1b[90m";
    public static string Reset = "\x1b[0m";
    public static HttpClient OllamaHttpClient = 
        new() 
        { 
            BaseAddress = new Uri(ollamaUrl) 
        };
    public static JsonSerializerOptions AgentJsonOptions = 
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