# Chapter 06 — basics: a file that outlives the app

Three things this chapter needs. Nothing else.

New words are in [`GLOSSARY.md`](../../docs/GLOSSARY.md).

---

## 1. A file that outlives the app

Everything in a variable is gone when the app stops. The message list too. A file on disk is
still there tomorrow. That is the whole idea of long memory.

This chapter needs two things from a file: add one line at the end, and read it all back.

```ts
import { appendFileSync, existsSync, readFileSync } from "node:fs";

appendFileSync("notes.md", "- User's name is Lyuben.\n");   // makes the file if it is missing
if (existsSync("notes.md")) {
  const notes = readFileSync("notes.md", "utf8");
}
```

**C# writes the same shape:**

```csharp
File.AppendAllText("notes.md", "- User's name is Lyuben.\n");  // makes the file if it is missing
if (File.Exists("notes.md"))
{
    string notes = await File.ReadAllTextAsync("notes.md");
}
```

Two things to be clear about, because they bite.

**Append, not write.** `File.WriteAllText` throws the old text away and puts the new text in.
Use it for notes and every note but the last one is gone. `AppendAllText` adds to the end.

**A missing file is normal.** The first run has no notes. So the code asks `File.Exists` first,
and an app with no notes yet starts the same way chapter 05 did. Read a missing file without
the check and the very first run stops with
`FileNotFoundException: Could not find file '.../notes.md'.`

**A relative path starts where you ran the app.** `../notes.md` from `code/csharp/` is
`code/notes.md`. Run from somewhere else and the notes go somewhere else. Chapter 02's prompt
path has the same rule.

---

## 2. Replacing an item in a list

To drop old tool results, the code puts a new message where an old one was. Same place in the
list, different text.

In TypeScript an object can change, so you can set the field:

```ts
for (const m of messages) {
  if (m.role === "tool") m.content = "[dropped. Call the tool again to see it.]";
}
```

In C# a `record` cannot change after it is made. `m.Content = ...` does not compile. So you make
a copy with one field different, and put the copy back in the same place:

```csharp
for (int i = 0; i < messages.Count; i++)
{
    if (messages[i].Role == "tool") messages[i] = messages[i] with { Content = stub };
}
```

`with` is the C# word for "a copy of this, except". The role, and the place in the list, stay.

This is why the loop is `for`, with an index, and not `foreach`. A `foreach` hands you the
item, not its place. Find the place and set it anyway, and the next turn of the `foreach`
throws `InvalidOperationException: Collection was modified; enumeration operation may not
execute.`

---

## 3. Tokens, not characters

The model does not read letters. It reads [tokens](../../docs/GLOSSARY.md#token), which are
pieces of words. Each reply from Ollama ends with a count of the tokens it read for that step:

```json
{"done": true, "prompt_eval_count": 2270, ...}
```

That number is the size of **everything** sent: the system message, every message in the list,
and the tool list. It is the number that gets slower as it grows, and the number with a ceiling
— the [context window](../../docs/GLOSSARY.md#context-window).

A rough rule: one token is about four characters of English. Do not trust the rule for code.
Read the count.

---

## Check yourself

Before the video:

- [ ] you can add a line to a file without losing the lines already in it
- [ ] you can say what your code should do when the notes file does not exist yet
- [ ] you can replace one item in a C# list of records, and say why `foreach` will not do it
- [ ] you can say what `prompt_eval_count` counts, and why it grows on every step
