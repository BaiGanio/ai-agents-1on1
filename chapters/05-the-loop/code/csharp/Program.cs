// Chapter 05 — the loop.
//
// Chapter 04 sent twice per question and stopped. This sends as many times as the model asks
// for, and stops when it stops asking. That while loop, and nothing else, is what makes this
// an agent. Nothing in this file decides how many steps a question takes. The model does.

// Read once, before anything else happens. Edit the file mid-session and nothing changes until
// you restart. The path is relative to the working directory, which `dotnet run` sets to this
// project folder. Run the binary out of bin/ instead and this throws.
string system = await File.ReadAllTextAsync($"../prompts/{Helper.PromptName}.md");

// The model remembers nothing between calls. Every turn sends the whole history back, so the
// system message at index 0 is re-sent every single time. Drop it and the agent forgets itself.
List<Message> messages = [new("system", system)];

Console.WriteLine($"Reading {Path.GetFullPath(Helper.RepoRoot)}");
Console.WriteLine("Type a question. An empty line stops the app.");

// The outer loop: one question from you.
while (true)
{
    Console.Write("You: ");

    // ReadLine gives null when input ends, so this one check covers both quit and end of input.
    string? prompt = Console.ReadLine();
    if (string.IsNullOrWhiteSpace(prompt)) break;

    messages.Add(new("user", prompt));

    // The inner loop: one step of the model's work. Nothing here counts the steps or caps them.
    // If every reply asks for a tool, this runs a tool every time, for as long as that goes on.
    // That is deliberate, it is the one thing chapter 05 leaves unsafe, and chapter 07 fixes it.
    int step = 0;
    while (true)
    {
        step++;

        // The label prints before the send, so you watch the model say nothing. When it asks
        // for a tool its Content is an empty string — there is no prose to stream at all.
        Console.Write($"{Helper.Model}: ");
        Message reply = await Helper.Ask(messages);
        messages.Add(reply);
        Console.WriteLine();

        // No parse, and no try around it. Chapter 04 needed both, because the call arrived as
        // prose it had to read. The runner hands it to us as a field, so asking has become one
        // question about a list. A reply with no calls is the answer, and the answer is already
        // on screen — Ask streamed it as it arrived.
        if (reply.ToolCalls is null or { Count: 0 }) break;

        // A list, not one call, because one turn may ask for two tools. Take only the first and
        // the model gets half an answer back with no error anywhere to tell you why.
        foreach (ToolCall call in reply.ToolCalls)
        {
            // The step number and the argument go on screen *before* the tool runs, so a slow
            // search shows you what it is waiting on. Without this line you cannot tell a
            // 2-step answer from a 5-step one, and you will want to.
            // Not named `args`: a top-level program already has an implicit `args` parameter
            // for the command line, and the compiler refuses the shadow with CS0136.
            ToolArguments? sent = call.Function?.Arguments;
            Console.WriteLine(
                $"{Helper.Grey}[step {step}] {call.Function?.Name} "
                    + $"{sent?.Path ?? sent?.Query}{Helper.Reset}");

            // The model asked. Your code decided. The model ran nothing — it never could.
            string result = Tools.Run(call);

            Console.WriteLine(result);
            Console.WriteLine();

            // Role "tool" — the fourth one, after system, user and assistant. Chapter 04 faked
            // this as a user message. Both messages have to go in, in this order: the model's
            // own request above, then its result here. Add only the result and the model sees a
            // file listing with nothing explaining why, so it asks for the same listing again.
            messages.Add(new("tool", result));
        }
    }

    Console.WriteLine();
}
