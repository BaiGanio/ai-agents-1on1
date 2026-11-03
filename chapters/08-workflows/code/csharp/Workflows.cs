// The three workflows. Each one is code that calls the model, not a model that calls code.
// Router and checker send no tools, so they cannot do anything but say a word.
public static class Workflows
{
    // One call, no tools, no history. The model sees the system text and one user message,
    // so it cannot be pulled along by the conversation. Empty when the clock ran out.
    static async Task<string> Once(string label, string system, string user)
    {
        Console.Write($"{Helper.Grey}{label}:{Helper.Reset} ");
        using CancellationTokenSource timer = new(Guards.TimeLeft());
        try
        {
            Message reply = await Helper.Ask(
                [new("system", system), new("user", user)], null, timer.Token);
            Console.WriteLine();
            return reply.Content.Trim();
        }
        catch (OperationCanceledException)
        {
            Console.WriteLine($"\n{Helper.Grey}[stopped: clock]{Helper.Reset}");
            return "";
        }
    }

    static string Prompt(string name) => File.ReadAllText($"../prompts/{name}.md");

    // Router. Anything but the other two words is "look", the list that cannot change a file.
    public static async Task<string> Route(string question)
    {
        string word = (await Once("router", Prompt("router"), question)).ToLowerInvariant();
        return word.Contains("change") ? "change" : word.Contains("remember") ? "remember" : "look";
    }

    // Checker. It reads what the tools said and what the model said, and compares. Empty means
    // PASS, so a checker that failed to run never blocks an answer.
    public static async Task<string> Check(List<string> evidence, string answer)
    {
        // No question in it. Measured: with the question, the honest "it was not written" failed
        // 6 of 6, because it did not do what the question asked. Without it, 6 of 6 passed.
        string seen = $"Tool results:\n{string.Join("\n", evidence)}\n\nAnswer: {answer}";
        string verdict = await Once("checker", Prompt("checker"), seen);
        return verdict.StartsWith("FAIL", StringComparison.OrdinalIgnoreCase)
            ? verdict[4..].Trim(' ', ',', ':', '-')
            : "";
    }

    // Chain. Code lists the files, code loops over them, code joins the results. The model is
    // asked three kinds of small question and picks no step.
    public static async Task Overview()
    {
        string root = Path.GetFullPath(Helper.RepoRoot);
        List<string> lines = [];
        foreach (string file in Directory.GetFiles(root).Order())
        {
            // Cut for the same reason as the evidence: a long file is a long wait for one line.
            string text = File.ReadAllText(file);
            string one = await Once(
                Path.GetFileName(file),
                "Write one line, at most 15 words, that says what this file is for.",
                text.Length > 4000 ? text[..4000] : text);
            lines.Add($"{Path.GetFileName(file)}: {one}");
        }

        await Once("join", "Join these lines into one paragraph of at most three sentences.",
            string.Join("\n", lines));
    }
}
