
using System.Text.Json;
using System.Text.Json.Serialization;

public static class Helper
{   
    public static readonly string ollamaUrl = "http://localhost:11434";
    public static readonly string model = "gemma4:e2b";
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

record Message(string Role, string Content);
record ChatRequest(string Model, List<Message> Messages, bool Stream);
record ChatResponse(Message Message);