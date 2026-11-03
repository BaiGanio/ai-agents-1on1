// What goes in the request's `tools` field, every call. The descriptions are the only thing the
// model reads to choose between them, so they are written for it, not for you.
//
// Moved out of Tools.cs in chapter 07, word for word. Tools.cs had hit 199 lines, and the guards
// need room there. A tool is still two halves: its schema here, its body in Tools.cs.
public static class Schemas
{
    public static readonly List<Tool> All =
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

    // The router's word picks the list. The order of All is the contract: three readers, then
    // save_note, then three writers. An unknown word gets the readers, the safe list.
    public static List<Tool> For(string route) =>
        route switch
        {
            "change" => [.. All.Take(3), .. All.Skip(4)],
            "remember" => [All[3]],
            _ => [.. All.Take(3)],
        };
}
