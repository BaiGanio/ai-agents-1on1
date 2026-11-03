using System.Diagnostics;

// The rules your code holds the model to. The model can ask for anything. Every line in this
// file is a place where the answer is no, and the model has no way to argue with it. The loop's
// rules come first, then the tools'.
public static class Guards
{
    // A real question takes a few steps. A model stuck asking for the same listing takes all of
    // them. The number is a guess you tune by watching, not a law.
    public static readonly int MaxSteps = 10;

    // Counted per question, and across a retry: the agent's second try spends the first one's
    // steps. Program.cs sets it to zero.
    public static int Steps;

    // One clock for the whole question, not one per call. Ten fast steps that go nowhere are
    // as stuck as one slow one, and only a shared clock sees both.
    public static readonly TimeSpan QuestionTime = TimeSpan.FromSeconds(60);

    // Started per question by Program.cs. Ask stops it while you read, so your thinking time
    // is never charged to the model.
    public static readonly Stopwatch Clock = new();

    public static TimeSpan TimeLeft()
    {
        TimeSpan left = QuestionTime - Clock.Elapsed;
        return left > TimeSpan.Zero ? left : TimeSpan.Zero;
    }

    // The tools' rules. Tools.cs calls these before it touches the disk.

    // The allow-list. The root and everything under it, and nothing else. GetFullPath folds
    // every "../" away first, so "docs/../../.ssh" is judged by where it lands, not how it
    // reads. A block-list of bad strings would miss the spelling you did not think of.
    public static string? Readable(string? path)
    {
        string root = Root();
        string full = Path.GetFullPath(Path.Combine(root, path ?? "."));

        // The separator matters. Without it "/tmp/repo-07-old" starts with "/tmp/repo-07".
        if (full == root || full.StartsWith(root + Path.DirectorySeparatorChar)) return full;
        return null;
    }

    static string Root() =>
        Path.TrimEndingDirectorySeparator(Path.GetFullPath(Helper.RepoRoot));

    // Stricter than reading: the root itself is off the list. In chapter 06.A "Delete
    // everything" became delete_file "." four times in four, and the whole folder went.
    public static string? Writable(string? path)
    {
        string? full = Readable(path);
        return full == Root() ? null : full;
    }

    // A yes from a human, read from the same terminal as your questions. Anything but "y" is
    // a no, so a stray Enter never deletes a file.
    public static bool Ask(string what)
    {
        Clock.Stop();
        Console.Write($"Allow {what}? [y/N] ");
        string? answer = Console.ReadLine();
        Clock.Start();

        return answer?.Trim().ToLowerInvariant() == "y";
    }
}
