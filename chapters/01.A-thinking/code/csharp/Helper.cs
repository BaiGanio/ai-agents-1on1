using System.Text.Json;
using System.Text.Json.Serialization;

// Everything needed to reach the model, in one place, so Program.cs is only the conversation.
public static class Helper
{
    // Both read the environment first. Nobody has to edit code to point at another machine
    // or swap the model.
    public static readonly string Model =
        Environment.GetEnvironmentVariable("MODEL") ?? "gemma4:e2b";

    public static readonly string OllamaUrl =
        Environment.GetEnvironmentVariable("OLLAMA_URL") ?? "http://localhost:11434";

    // Flip this to false to get the answer without the wait. A model that cannot think answers
    // Think: true with a 400, not with an answer, so leave it off on those.
    public static readonly bool? Think = true;

    // The thinking is grey so your eye skips it. These are ANSI colour codes, not a library.
    public static readonly string Grey = "\x1b[90m";
    public static readonly string Reset = "\x1b[0m";

    // One client for the whole process. A new one per call leaks sockets, and a local model can
    // take minutes on the first answer while it loads into RAM, so 100 seconds is too short.
    public static readonly HttpClient OllamaHttpClient =
        new()
        {
            BaseAddress = new Uri(OllamaUrl),
            Timeout = TimeSpan.FromMinutes(5),
        };

    public static readonly JsonSerializerOptions AgentJsonOptions =
        new()
        {
            // Ollama's field names are lower case. C# properties are not, so we say so once, here.
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            // This is what makes Think: null mean "field not sent". Sent as null, it means
            // "no thinking", which is a different answer.
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        };
}

record Message(string Role, string Content, string? Thinking = null);
record ChatRequest(string Model, List<Message> Messages, bool Stream, bool? Think);
record ChatResponse(Message Message);
