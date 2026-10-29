// The three tools: what the model is told about them, what they do, and the switch that picks
// one. The bodies are ordinary functions that know no model exists.
//
// Chapter 04 kept the description in prompts/librarian.md and the body here, so adding a tool
// meant editing two files and the two could drift apart in silence. Now both halves of a tool
// live side by side, and a tool the model cannot see is a tool you forgot to list below.
public static class Tools
{
    // What goes in the request's `tools` field, every call. The descriptions are the only thing
    // the model reads to choose between these three, so they are written for it, not for you.
    public static readonly List<Tool> Schemas =
    [
        new(new(
            "list_files",
            "names the files in a folder",
            new("object",
                new()
                {
                    ["path"] = new("string", "the folder, relative to the repo root"),
                },
                ["path"]))),
        new(new(
            "read_file",
            "gives back the text of one file",
            new("object",
                new()
                {
                    ["path"] = new("string", "the file, relative to the repo root"),
                },
                ["path"]))),
        new(new(
            "search_text",
            "finds the lines that contain some text",
            new("object",
                new()
                {
                    ["query"] = new("string", "the text to look for"),
                },
                ["query"]))),
    ];

    // Every path the model wrote is joined onto RepoRoot. In this chapter that is the only
    // thing between the model and the rest of your disk, and it is not a rule — Path.Combine
    // happily accepts "../../../.ssh". Chapter 07 makes it a real one.
    static string Under(string? path) => Path.Combine(Helper.RepoRoot, path ?? ".");

    static string Name(string file) => Path.GetRelativePath(Helper.RepoRoot, file);

    // Dispatch: a name the model wrote, picking a function you wrote. People sell this as a
    // tool registry. An unknown name is not a crash — the model reads the sentence and retries.
    // The `tools` field makes an unknown name unlikely, not impossible, so the arm stays.
    public static string Run(ToolCall call) =>
        call.Function?.Name switch
        {
            "list_files" => ListFiles(call.Function.Arguments?.Path),
            "read_file" => ReadFile(call.Function.Arguments?.Path),
            "search_text" => SearchText(call.Function.Arguments?.Query),
            _ => $"no such tool: {call.Function?.Name}",
        };

    static string ListFiles(string? path)
    {
        string folder = Under(path);
        if (!Directory.Exists(folder)) return $"no such folder: {path}";

        // Names are relative to the folder, because the model hands them straight back to
        // read_file. Give it an absolute path and it will send you an absolute path.
        var names = Directory
            .EnumerateFiles(folder, "*", SearchOption.AllDirectories)
            .Select(f => Path.GetRelativePath(folder, f))
            .Order();

        return string.Join("\n", names);
    }

    static string ReadFile(string? path)
    {
        string file = Under(path);

        // A missing file is a sentence, not an exception. A throw ends the run and teaches the
        // model nothing. This goes back as a tool result and it tries another path.
        if (!File.Exists(file)) return $"no such file: {path}";

        return File.ReadAllText(file);
    }

    static string SearchText(string? query)
    {
        if (string.IsNullOrWhiteSpace(query)) return "search_text needs a query";

        List<string> hits = [];

        foreach (string file in Directory.EnumerateFiles(
            Helper.RepoRoot, "*", SearchOption.AllDirectories))
        {
            int number = 0;
            foreach (string line in File.ReadLines(file))
            {
                number++;
                if (line.Contains(query, StringComparison.OrdinalIgnoreCase))
                    hits.Add($"{Name(file)}:{number}: {line.Trim()}");
            }
        }

        return hits.Count == 0 ? $"no line contains {query}" : string.Join("\n", hits);
    }
}
