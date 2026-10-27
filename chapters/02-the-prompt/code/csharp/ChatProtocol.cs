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
