// The wire format, and nothing else. Helper.cs is the plumbing. Program.cs is the conversation.

// Thinking has a default, so every `new Message("user", prompt)` from chapter 01 still compiles.
public record Message(string Role, string Content, string? Thinking = null);

public record ChatRequest(
    string Model,
    List<Message> Messages,
    bool Stream = false,
    bool? Think = false
);

public record ChatResponse(Message Message);

// What the model writes when it asks for a tool. It is not part of Ollama's wire format — it
// is a shape we invented in the system prompt, and the model may break it any time.
// All three are nullable: list_files sends a path and no query, search_text the other way
// round, and a model that answers `{}` sends neither.
public record Call(string? Tool, string? Path, string? Query);
