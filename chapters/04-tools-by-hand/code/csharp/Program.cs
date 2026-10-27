// Chapter 04 — tools by hand.
//
// The model still only writes text. But now some of that text is JSON, and when it is, we run
// a function and give the answer back as one more message. That is all tool calling ever is.

using System.Text.Json;

// Read once, before anything else happens. Edit the file mid-session and nothing changes until
// you restart. The path is relative to the working directory, which `dotnet run` sets to this
// project folder. Run the binary out of bin/ instead and this throws.
string system = await File.ReadAllTextAsync($"../prompts/{Helper.PromptName}.md");

// The model remembers nothing between calls. Every turn sends the whole history back, so the
// system message at index 0 is re-sent every single time. Drop it and the agent forgets itself.
List<Message> messages = [new("system", system)];

Console.WriteLine($"Reading {Path.GetFullPath(Helper.RepoRoot)}");
Console.WriteLine("Type a question. An empty line stops the app.");

while (true)
{
    Console.Write("You: ");

    // ReadLine gives null when input ends, so this one check covers both quit and end of input.
    string? prompt = Console.ReadLine();
    if (string.IsNullOrWhiteSpace(prompt)) break;

    messages.Add(new("user", prompt));

    Console.Write($"{Helper.Model}: ");
    Message reply = await Helper.Ask(messages);
    messages.Add(reply);
    Console.WriteLine();

    // There is no "is this JSON?" function. You try to read it, and a throw means the model
    // wrote prose — which is a real answer, already printed above, not an error. So this catch
    // is the ordinary path. Catch JsonException and not Exception, or a bug in Tools.cs hides.
    Call? call = null;
    try
    {
        call = JsonSerializer.Deserialize<Call>(reply.Content, Helper.AgentJsonOptions);
    }
    catch (JsonException)
    {
    }

    if (call?.Tool is null) continue;

    // The model asked. Your code decided. The model ran nothing — it never could.
    string result = Tools.Run(call);
    Console.WriteLine($"{Helper.Grey}[{call.Tool}]{Helper.Reset}");
    Console.WriteLine(result);
    Console.WriteLine();

    // The result is just another message. No tool channel, no callback, no framework. The
    // model's own JSON has to stay in front of it, or the listing arrives with no question.
    messages.Add(new("user", $"tool result of {call.Tool}:\n{result}"));

    Console.Write($"{Helper.Model}: ");
    messages.Add(await Helper.Ask(messages));
    Console.WriteLine();
}
