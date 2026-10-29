using System.Text.Json.Serialization;

// The wire format, and nothing else. Helper.cs is the plumbing. Program.cs is the conversation.

// Thinking has a default, so every `new Message("user", prompt)` from chapter 01 still compiles.
// ToolCalls needs the attribute: Ollama writes tool_calls, and the camelCase policy in
// Helper.cs would make it toolCalls, which matches nothing and arrives as null every time.
public record Message(
    string Role,
    string Content,
    string? Thinking = null,
    [property: JsonPropertyName("tool_calls")] List<ToolCall>? ToolCalls = null
);

public record ChatRequest(
    string Model,
    List<Message> Messages,
    List<Tool> Tools,
    bool Stream = false,
    bool? Think = false
);

// Ollama counts the whole prompt it read and puts the number on the last line of the stream.
// Same trap as tool_calls: camelCase makes it promptEvalCount, which arrives as null.
public record ChatResponse(
    Message Message,
    [property: JsonPropertyName("prompt_eval_count")] int? PromptEvalCount = null
);

// What we send: the tools the model is allowed to ask for. Chapter 04 wrote this list in
// English in the system prompt and hoped. Now the runner reads it and holds the model to it.
public record ToolProperty(string Type, string Description);

public record ToolParameters(
    string Type,
    Dictionary<string, ToolProperty> Properties,
    List<string> Required
);

public record ToolFunction(string Name, string Description, ToolParameters Parameters);

// Type has a default because "function" is the only value Ollama takes today.
public record Tool(ToolFunction Function, string Type = "function");

// What comes back: the call, as data. Chapter 04's `Call` record is gone — it parsed JSON out
// of the model's prose, and there is no prose here. Content is an empty string when it asks.
// Arguments stays nullable per field, same as chapter 04: list_files sends a path and no query,
// search_text the other way round, and a model that sends neither arrives as nulls, not a crash.
// Text is save_note's one argument. The other three tools leave it null.
public record ToolArguments(string? Path, string? Query, string? Text);

public record ToolCallFunction(string? Name, ToolArguments? Arguments);

// Id is Ollama's, not ours. We never send it back, and nothing in this course needs it.
public record ToolCall(string? Id, ToolCallFunction? Function);
