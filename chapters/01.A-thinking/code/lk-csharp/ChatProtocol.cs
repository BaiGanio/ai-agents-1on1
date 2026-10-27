public record Message(string Role, string Content, string? Thinking = null);
public record ChatRequest(
    string Model, 
    List<Message> Messages, 
    bool Stream = false, 
    bool? Think = false
);
public record ChatResponse(Message Message);