// Chapter 08 — workflows.
//
// Chapter 07's agent, with a call before it and a call after it. The router picks the tools.
// The agent runs the loop. The checker reads the answer against what the tools said. Code
// picks each step, so the order below is the order on screen.

string system = await File.ReadAllTextAsync($"../prompts/{Helper.PromptName}.md");
List<Message> messages = [new("system", system)];

if (File.Exists(Helper.NotesPath))
{
    string notes = await File.ReadAllTextAsync(Helper.NotesPath);
    messages.Add(new("system", $"Notes from earlier sessions:\n{notes}"));
}

Console.WriteLine($"Reading {Path.GetFullPath(Helper.RepoRoot)}");
Console.WriteLine("Type a question, or /overview. An empty line stops the app.");

while (true)
{
    Console.Write("You: ");
    string? prompt = Console.ReadLine();
    if (string.IsNullOrWhiteSpace(prompt)) break;

    // The clock and the step count cover everything below, every call in it.
    Guards.Clock.Restart();
    Guards.Steps = 0;

    // A command, not a question. The chain runs and the history stays as it was.
    if (prompt == "/overview")
    {
        await Workflows.Overview();
        continue;
    }

    messages.Add(new("user", prompt));

    // 1. Router: one word, no tools.
    string route = await Workflows.Route(prompt);

    // 2. Agent: chapter 07's loop, with only that word's tools.
    Outcome outcome = await Agent.Run(messages, Schemas.For(route));

    // 3. Checker: only when a tool ran. With none, the answer has nothing to be checked against.
    if (outcome.Answer != "" && outcome.Evidence.Count > 0)
    {
        string problem = await Workflows.Check(outcome.Evidence, outcome.Answer);
        if (problem != "")
        {
            Console.WriteLine($"{Helper.Grey}[checker: FAIL, {problem} Trying once more.]"
                + Helper.Reset);

            // One retry. The model's first answer is already in the history, so it sees what
            // it said, and why that was refused.
            messages.Add(new("user", $"A checker says your answer is wrong: {problem} "
                + "Answer again, and say only what the tool results show."));
            Outcome second = await Agent.Run(messages, Schemas.For(route));
            outcome = new(second.Answer, [.. outcome.Evidence, .. second.Evidence]);

            if (outcome.Answer != "")
            {
                problem = await Workflows.Check(outcome.Evidence, outcome.Answer);
                if (problem != "")
                    Console.WriteLine(
                        $"{Helper.Grey}[checker: FAIL again, {problem}]{Helper.Reset}");
            }
        }
    }

    // Short memory. After the checker, because the checker needs the tool results.
    string stub = "[dropped. Call the tool again to see it.]";
    for (int i = 0; i < messages.Count; i++)
    {
        if (messages[i].Role == "tool") messages[i] = messages[i] with { Content = stub };
    }

    Console.WriteLine();
}
