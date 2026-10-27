// The three tools, and the switch that picks one. Nothing in this file knows a model exists.
// They are ordinary functions. That is the whole point of the chapter.
public static class Tools
{
    // Every path the model wrote is joined onto RepoRoot. In this chapter that is the only
    // thing between the model and the rest of your disk, and it is not a rule — Path.Combine
    // happily accepts "../../../.ssh". Chapter 07 makes it a real one.
    static string Under(string? path) => Path.Combine(Helper.RepoRoot, path ?? ".");

    static string Name(string file) => Path.GetRelativePath(Helper.RepoRoot, file);

    // Dispatch: a name the model wrote, picking a function you wrote. People sell this as a
    // tool registry. An unknown name is not a crash — the model reads the sentence and retries.
    public static string Run(Call call) =>
        call.Tool switch
        {
            "list_files" => ListFiles(call.Path),
            "read_file" => ReadFile(call.Path),
            "search_text" => SearchText(call.Query),
            _ => $"no such tool: {call.Tool}",
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
