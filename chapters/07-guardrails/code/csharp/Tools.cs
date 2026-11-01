// The tools: what they do, and the switch that picks one. Their schemas are in Schemas.cs.
// The bodies are ordinary functions that know no model exists.
public static class Tools
{
    static string Name(string file) => Path.GetRelativePath(Helper.RepoRoot, file);

    // One sentence for every refused path. Not "no such file": the file may well be there, and
    // the model should learn the rule, not go looking for another spelling of the same path.
    static string Outside(string? path) => $"refused: {path} is outside the repo";

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
        // Chapter 06.A joined the path on and hoped. Now the path is judged before the disk
        // is touched. A null is a no.
        if (Guards.Readable(path) is not string folder) return Outside(path);
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
        if (Guards.Readable(path) is not string file) return Outside(path);

        // A missing file is a sentence, not an exception. A throw ends the run and teaches the
        // model nothing. This goes back as a tool result and it tries another path.
        if (!File.Exists(file)) return $"no such file: {path}";

        return File.ReadAllText(file);
    }

    // Takes no path, so the allow-list has nothing to check. It walks the root and only the
    // root, which is the same rule by construction.
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

    // No ask here. It adds one line, to a file the model cannot name, outside the repo.
    static string SaveNote(string? text)
    {
        if (string.IsNullOrWhiteSpace(text)) return "save_note needs a text";

        File.AppendAllText(Helper.NotesPath, $"- {text.Trim()}\n");
        return "saved";
    }

    // The write side has one more no: the repo root itself.
    static string Locked(string? path) => $"refused: {path} is the whole repo, or outside it";

    // Replaces a whole file, so it can lose text. The path check first, then the folder, then
    // you — so you are only asked about a write that would really happen.
    static string WriteFile(string? path, string? text)
    {
        if (string.IsNullOrWhiteSpace(path)) return "write_file needs a path";

        // A new file, yes. A new folder, no — and a sentence, because the throw ends the app.
        if (Guards.Writable(path) is not string file) return Locked(path);
        if (!Directory.Exists(Path.GetDirectoryName(file))) return $"no such folder for {path}";

        // The size, so a one-line answer about to replace a 300-line file shows on screen.
        string size = File.Exists(file) ? $"replace {path}" : $"make {path}";
        if (!Guards.Ask($"write_file: {size} with {(text ?? "").Length} characters"))
            return $"NOT written. The user said no. Tell the user {path} was not changed.";

        File.WriteAllText(file, text ?? "");
        return $"wrote {path}";
    }

    // No ask. It only adds to the end of a file that is there, so nothing is lost. AppendAllText
    // would make a missing file, so a typo would be a new file, not an error the model reads.
    // And the model ends files with no newline, so we start a new line first. Measured.
    static string AppendFile(string? path, string? text)
    {
        if (string.IsNullOrWhiteSpace(path)) return "append_file needs a path";

        if (Guards.Writable(path) is not string file) return Locked(path);
        if (!File.Exists(file)) return $"no such file: {path}";
        bool glued = !File.ReadAllText(file).EndsWith('\n');
        File.AppendAllText(file, (glued ? "\n" : "") + text);
        return $"appended to {path}";
    }

    // No recycle bin. A folder goes with everything in it, so the ask says which it is. The
    // root itself never reaches the ask: Writable refuses it first.
    static string DeleteFile(string? path)
    {
        if (Guards.Writable(path) is not string target) return Locked(path);

        string kind;
        if (File.Exists(target)) kind = "file";
        else if (Directory.Exists(target)) kind = "folder and everything in it";
        else return $"no such file or folder: {path}";

        if (!Guards.Ask($"delete_file: {path} ({kind})"))
            return $"NOT deleted. The user said no. Tell the user {path} is still there.";

        if (File.Exists(target)) File.Delete(target);
        else Directory.Delete(target, recursive: true);

        return $"deleted {path}";
    }
}
