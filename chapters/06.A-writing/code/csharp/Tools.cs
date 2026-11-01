// The tools: what the model is told about them, what they do, and the switch that picks one.
// The bodies are ordinary functions that know no model exists.
//
// Chapter 04 kept the description in prompts/librarian.md and the body here, so adding a tool
// meant editing two files and the two could drift apart in silence. Now both halves of a tool
// live side by side, and a tool the model cannot see is a tool you forgot to list below.
public static class Tools
{
    // What goes in the request's `tools` field, every call. The descriptions are the only thing
    // the model reads to choose between them, so they are written for it, not for you.
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
        new(new(
            "save_note",
            "saves one short fact to keep for the next session",
            new("object",
                new()
                {
                    ["text"] = new("string", "the fact, in one sentence"),
                },
                ["text"]))),
        new(new(
            "write_file",
            "makes a file, or replaces all of its text",
            new("object",
                new()
                {
                    ["path"] = new("string", "the file, relative to the repo root"),
                    ["text"] = new("string", "the whole new text of the file"),
                },
                ["path", "text"]))),
        new(new(
            "append_file",
            "adds text to the end of a file",
            new("object",
                new()
                {
                    ["path"] = new("string", "the file, relative to the repo root"),
                    ["text"] = new("string", "the text to add"),
                },
                ["path", "text"]))),
        new(new(
            "delete_file",
            "deletes a file, or a folder and everything in it",
            new("object",
                new()
                {
                    ["path"] = new("string", "the file or folder, relative to the repo root"),
                },
                ["path"]))),
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
            "save_note" => SaveNote(call.Function.Arguments?.Text),
            "write_file" =>
                WriteFile(call.Function.Arguments?.Path, call.Function.Arguments?.Text),
            "append_file" =>
                AppendFile(call.Function.Arguments?.Path, call.Function.Arguments?.Text),
            "delete_file" => DeleteFile(call.Function.Arguments?.Path),
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

    // The first tool that writes. It can only add a line, and only to a file the model cannot
    // name. Chapter 07 makes it ask you first.
    static string SaveNote(string? text)
    {
        if (string.IsNullOrWhiteSpace(text)) return "save_note needs a text";

        File.AppendAllText(Helper.NotesPath, $"- {text.Trim()}\n");
        return "saved";
    }

    // From here on the model changes your files. Nothing below asks you, and nothing checks the
    // path. That is this chapter's lesson, and chapter 07's whole job.
    static string WriteFile(string? path, string? text)
    {
        if (string.IsNullOrWhiteSpace(path)) return "write_file needs a path";

        string file = Under(path);
        Directory.CreateDirectory(Path.GetDirectoryName(file)!);
        File.WriteAllText(file, text ?? "");
        return $"wrote {path}";
    }

    // Append makes the file and its folder if they are missing, so a wrong path is a new file,
    // not an error. The model writes files with no newline at the end. Measured: the appended
    // line was glued onto the last one, so we start a new line when the file does not end on one.
    static string AppendFile(string? path, string? text)
    {
        if (string.IsNullOrWhiteSpace(path)) return "append_file needs a path";

        string file = Under(path);
        Directory.CreateDirectory(Path.GetDirectoryName(file)!);
        bool glued = File.Exists(file) && !File.ReadAllText(file).EndsWith('\n');
        File.AppendAllText(file, (glued ? "\n" : "") + text);
        return $"appended to {path}";
    }

    // No recycle bin. A folder goes with everything in it, and "." is the whole repo.
    static string DeleteFile(string? path)
    {
        string target = Under(path);

        if (File.Exists(target)) File.Delete(target);
        else if (Directory.Exists(target)) Directory.Delete(target, recursive: true);
        else return $"no such file or folder: {path}";

        return $"deleted {path}";
    }
}
