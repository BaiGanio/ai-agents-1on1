// Chapter 07 — guardrails.
//
// Chapter 06.A's agent, with five rules it cannot talk its way past. Three live here, in the
// loop: a step cap, a clock, and a repeat check. Two live in Guards.cs, at the tools: a path
// allow-list, and a yes from you before a file is replaced or deleted.

// Read once, before anything else happens. Edit the file mid-session and nothing changes until
// you restart. The path is relative to the working directory, which `dotnet run` sets to this
// project folder. Run the binary out of bin/ instead and this throws.
string system = await File.ReadAllTextAsync($"../prompts/{Helper.PromptName}.md");

// The model remembers nothing between calls. Every turn sends the whole history back, so the
// system message at index 0 is re-sent every single time. Drop it and the agent forgets itself.
List<Message> messages = [new("system", system)];

// Long memory. Every run starts knowing nothing, so last run's notes go back in as a second
// system message. Measured with four tools: glued onto the end of the first one, the model
// missed them three times in five and said it had no record of your name. Here, five in five.
if (File.Exists(Helper.NotesPath))
{
    string notes = await File.ReadAllTextAsync(Helper.NotesPath);
    messages.Add(new("system", $"Notes from earlier sessions:\n{notes}"));
}

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

    // Per question, not per run. A file read for the last question may be read again for this
    // one, and should be: short memory dropped its text.
    HashSet<string> asked = [];
    Guards.Clock.Restart();

    // The inner loop: one step of the model's work. Chapter 05 left it with no way out but the
    // model's own say-so. Now your code has two: too many steps, or too much time.
    int step = 0;
    while (true)
    {
        step++;

        // Your code ends the question, so the model gets no turn to explain. The grey line is
        // the only explanation there is, which is why it names the rule and the number.
        if (step > Guards.MaxSteps)
        {
            Console.WriteLine($"{Helper.Grey}[stopped: {Guards.MaxSteps} steps]{Helper.Reset}");
            break;
        }

        // The label prints before the send, so you watch the model say nothing. When it asks
        // for a tool its Content is an empty string — there is no prose to stream at all.
        Console.Write($"{Helper.Model}: ");

        // The token fires when this question's time is up, mid-send or mid-stream. Whatever the
        // model streamed so far stays on screen, but it never goes into the history.
        using CancellationTokenSource timer = new(Guards.TimeLeft());
        Message reply;
        try
        {
            reply = await Helper.Ask(messages, timer.Token);
        }
        catch (OperationCanceledException)
        {
            Console.WriteLine(
                $"\n{Helper.Grey}[stopped: {Guards.QuestionTime.TotalSeconds} s]{Helper.Reset}");
            break;
        }
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
                    + $"{sent?.Path ?? sent?.Query ?? sent?.Text}{Helper.Reset}");

            // The model asked. Your code decided. The model ran nothing — it never could.
            // The same call twice in one question gets the same answer twice, so it is not run
            // again. The model is told why, which is the whole difference from a step cap.
            string key = $"{call.Function?.Name} {sent?.Path} {sent?.Query} {sent?.Text}";
            string result = asked.Add(key)
                ? Tools.Run(call)
                : "you already made this exact call. Use the result you have.";

            // A change makes every earlier read out of date. Measured without this line: read,
            // append, read again to check, and the check was refused three times in three.
            if (call.Function?.Name is "write_file" or "append_file" or "delete_file")
                asked.Clear();

            Console.WriteLine(result);
            Console.WriteLine();

            // Role "tool" — the fourth one, after system, user and assistant. Chapter 04 faked
            // this as a user message. Both messages have to go in, in this order: the model's
            // own request above, then its result here. Add only the result and the model sees a
            // file listing with nothing explaining why, so it asks for the same listing again.
            messages.Add(new("tool", result));
        }
    }

    // Short memory. The answer is on screen, so the files it read have done their job. Left in,
    // each one is sent again on every step of every question after this. The call stays, so
    // the model still sees which file it read. Only the text goes. Need it? Read it again.
    string stub = "[dropped. Call the tool again to see it.]";
    for (int i = 0; i < messages.Count; i++)
    {
        if (messages[i].Role == "tool") messages[i] = messages[i] with { Content = stub };
    }

    Console.WriteLine();
}
