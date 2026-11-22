// The wire format, and nothing else. Helper.cs is the plumbing. Program.cs is the conversation.

using System.Text.Json.Serialization;

// Thinking has a default, so every `new Message("user", prompt)` from chapter 01 still compiles.
public record Message(string Role, string Content, string? Thinking = null);

public record ChatRequest(
    string Model,
    List<Message> Messages,
    bool Stream = false,
    bool? Think = false
);

// One line of the stream. Only the last line has the counts, so on every other line they are
// null. Ollama writes these four names in snake_case, and camelCase would never find them.
public record ChatResponse(
    Message Message,
    [property: JsonPropertyName("prompt_eval_count")] int? PromptEvalCount = null,
    [property: JsonPropertyName("eval_count")] int? EvalCount = null,
    [property: JsonPropertyName("eval_duration")] long? EvalDuration = null,
    [property: JsonPropertyName("total_duration")] long? TotalDuration = null
);
