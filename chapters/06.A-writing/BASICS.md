# Chapter 06.A — basics: changing files

Three things this chapter needs. Nothing else.

New words are in [`GLOSSARY.md`](../../docs/GLOSSARY.md).

---

## 1. Write or append

There are two ways to put text in a file, and they do opposite things to what was there.

**Write** [overwrites](../../docs/GLOSSARY.md#overwrite). The old text is gone, and the new text
takes its place. If the file does not exist, write makes it.

**[Append](../../docs/GLOSSARY.md#append)** adds to the end. The old text stays. If the file
does not exist, append makes it too.

```ts
import { writeFileSync, appendFileSync } from "node:fs";

writeFileSync("SUMMARY.md", "# Summary\n");          // the file is now one line
appendFileSync("SUMMARY.md", "Checked.\n");          // the file is now two lines
writeFileSync("SUMMARY.md", "Gone.\n");              // the file is one line again
```

**C# writes the same shape:**

```csharp
File.WriteAllText("SUMMARY.md", "# Summary\n");      // the file is now one line
File.AppendAllText("SUMMARY.md", "Checked.\n");      // the file is now two lines
File.WriteAllText("SUMMARY.md", "Gone.\n");          // the file is one line again
```

There is no "edit one word" in either language. To change one word, you read the whole file,
change the word, and write the whole file back. The model has to do the same, and that is where
it can go wrong: skip the read, and it writes back only what it remembers.

**The newline at the end.** Append adds your text right after the last character. If the last
line has no `\n`, your text joins that line. The model often writes files with no `\n` at the
end, so the code in this chapter checks for one first.

---

## 2. A file in a folder that does not exist

Write and append make a missing **file**. They do not make a missing **folder**. Ask for
`drafts/today.md` when there is no `drafts/`, and C# throws:

```text
Unhandled exception. System.IO.DirectoryNotFoundException: Could not find a part of the path
'.../drafts/today.md'.
```

That ends the whole app, not only the tool call. So the code makes the folder first:

```csharp
Directory.CreateDirectory(Path.GetDirectoryName(file)!);   // does nothing if it is there
```

```ts
mkdirSync(dirname(file), { recursive: true });             // does nothing if it is there
```

---

## 3. Delete, and the folder called `.`

A file and a folder are deleted by different calls in C#:

```csharp
File.Delete("SUMMARY.md");
Directory.Delete("old", recursive: true);
```

`recursive: true` is a [recursive delete](../../docs/GLOSSARY.md#recursive-delete): the folder,
every folder inside it, and every file in all of them. Without it, a folder with anything in it
is not deleted, and C# throws.

TypeScript does both with one call: `rmSync(path, { recursive: true })`.

**There is no recycle bin.** A file deleted from code does not go to the Trash. It is gone.

**`.` means "this folder".** The tools join every path onto `REPO`. So `delete_file .` is not
"the files in here". It is `REPO` itself, and everything in it.

**The way back is a copy.** This chapter points `REPO` at a copy of a folder, never the real one.
If the folder is in git, `git restore <folder>` also brings back the files git knows about.
A file git never saw is gone for good.

---

## Check yourself

Before the video:

- [ ] you can say what is left in a file after a write, and after an append
- [ ] you can say why an append can land on the end of the last line
- [ ] you can say what happens when you write a file into a folder that does not exist
- [ ] you can say what `delete_file .` deletes, and why
- [ ] you have a copy of a folder to point `REPO` at, and you know how to make it again
