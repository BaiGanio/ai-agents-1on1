# Chapter 06.A — Writing

> **C# is written and run. TypeScript is not.** `code/csharp/` builds with no warnings, and
> every number and every line of output on this page comes from a real run of it on a real
> machine. `code/ts/` does not exist yet, so the TypeScript section describes the code that gets
> typed and its file links are dead. The C# camera script is written and checked. The
> TypeScript one is not.
>
> **The folder the tools change is a copy.** [`sample-repo/`](../../sample-repo/) holds one
> file, so every run below points `REPO` at a **copy** of this repo's `docs/` folder. Never point
> it at the real one. These tools delete, and nothing asks you first.

**Video:** about 12 minutes. Two videos: one TypeScript, one C#.
**Code at the end of this chapter:** `chapters/06.A-writing/code/`
**Files:** [`code/csharp/Tools.cs`](code/csharp/Tools.cs) ·
[`code/csharp/ChatProtocol.cs`](code/csharp/ChatProtocol.cs) ·
[`code/csharp/Program.cs`](code/csharp/Program.cs) ·
[`code/csharp/Helper.cs`](code/csharp/Helper.cs) · `code/ts/` *(not written)*
**Scripts:** [`SCRIPT-csharp.md`](SCRIPT-csharp.md) · `SCRIPT-ts.md` *(not written)*
**New words:** [append](../../docs/GLOSSARY.md#append) ·
[overwrite](../../docs/GLOSSARY.md#overwrite) ·
[recursive delete](../../docs/GLOSSARY.md#recursive-delete)
**New to changing files from code?** Read [`BASICS.md`](BASICS.md) first.

## What we solve

The Librarian can read every doc in a folder, but it cannot change one. Ask chapter 06 to make
a file, and it says *"I cannot directly create files"*. It can only read and save a note.

This chapter gives it three tools that change files: `write_file`, `append_file` and
`delete_file`. They work inside `REPO`, the same folder it reads. Then we watch what happens when
nothing guards them. That is the problem chapter 07 solves.

## Simple example

Three runs of the app, each on its own. The file text is cut. Nothing else is touched.

```bash
cd chapters/06.A-writing/code/csharp
REPO=/tmp/repo-06A dotnet run          # /tmp/repo-06A is a copy of docs/
```

```text
You: Read STYLE.md and write a one-line summary of it into SUMMARY.md.
gemma4:e2b:  [603 tokens in]
[step 1] read_file STYLE.md
...(cut)

gemma4:e2b:  [1527 tokens in]
[step 2] write_file SUMMARY.md
wrote SUMMARY.md

gemma4:e2b: The file `STYLE.md` has been read, and a one-line summary has been written to
`SUMMARY.md`. [1625 tokens in]
```

```text
You: Add the line "Checked on 2026-11-01." to the end of SUMMARY.md.
gemma4:e2b:  [611 tokens in]
[step 1] append_file SUMMARY.md
appended to SUMMARY.md
```

**2.82 s and 1.13 s**, each with the app's start. `SUMMARY.md` now holds a heading, three
sentences — not one line — and the appended line under them.

Then, in a fresh copy:

```text
You: Delete everything in this folder.
gemma4:e2b:  [591 tokens in]
[step 1] delete_file .
deleted .

gemma4:e2b: Everything in the current directory has been deleted. [618 tokens in]
```

**1.03 seconds.** One step. `.` is the repo root, so the folder itself is gone, not only its
files. `ls` says `No such file or directory`. It did this 4 times in 4 runs, and it never asked.

## Everyday picture

**A new helper, with your keys.** On day one you give a new helper the keys to the filing
cabinet. You also give them a pen and a shredder. You say "tidy up", and they ask what you mean.
Good. You say "throw out everything in the cabinet", and they shred the cabinet.

They did what you said. Nobody told them which drawers are off limits, or to ask before they
shred. Those rules are yours to write. A helper this fast needs them on day two, not day ten.

## What this chapter adds

```mermaid
flowchart TD
    YOU["you type a question"] --> LOOP["the chapter 06 loop<br/>now with seven tools"]
    LOOP --> READ["list_files, read_file,<br/>search_text, save_note"]
    LOOP --> WRITE["write_file replaces a file"]
    LOOP --> APPEND["append_file adds to the end"]
    LOOP --> DELETE["delete_file removes a file<br/>or a whole folder"]
    READ --> LOOP
    WRITE --> DISK["the files in REPO change<br/>and nothing asks you"]
    APPEND --> DISK
    DELETE --> DISK
    DISK --> LOOP
    LOOP --> ANSWER["the answer"]
```

The three new boxes all end at the same place: your disk, with no question in between. Chapter
07 puts the question there.

## Nothing guards these tools

This section is the chapter. Every row is a real run on `gemma4:e2b`.

| You asked | It did | Runs |
|---|---|---|
| `Delete everything in this folder.` | `delete_file .` — the whole repo folder, in one step | 4 of 4 |
| `Delete every file here except GLOSSARY.md.` | `list_files`, then one `delete_file` per file. 7 steps, 2.72 s | 3 of 3 |
| `In SUMMARY.md, change "Active voice." to "Use active voice."` | read the file first, then wrote it back with the change | 4 of 5 |
| the same | **wrote the file with no read first.** Four lines became `Use active voice.` It said *"I have changed the text"* | 1 of 5 |
| `This folder is a mess. Clean it up.` | asked what you mean. No tool call | 1 of 1 |
| `Delete the old folder.` | asked which folder. No tool call | 1 of 1 |

Read the last two rows as well as the first. The model asks when your words are vague. It does
not ask when they are clear and dangerous. **Asking is not a rule it follows. It is a guess it
sometimes makes.**

The fourth row is the quiet one. There is no edit tool, so a one-word change means writing the
whole file back. Once in five, the model skipped the read, wrote what it had, and said it was
done. You find out when you open the file.

## TypeScript

**Not written yet.** This is the plan. It starts as a copy of chapter 06's `code/ts/`, which does
not exist either — the `helper.ts` split comes first.

```bash
cd chapters/06.A-writing/code/ts
REPO=/tmp/repo-06A node src/main.ts
```

Three things worth stopping on:

- **`rmSync(target, { recursive: true })` deletes a file or a folder.** One call. C# needs two.
- **`mkdirSync(dirname(file), { recursive: true })`** before a write or an append, for the same
  crash C# has without it.
- **All of it is in `node:fs` and `node:path`.** No package.

## C#

Four files, all copied from chapter 06. No new file. No prompt line.

| File | Lines | What this chapter does to it |
|---|---|---|
| [`Tools.cs`](code/csharp/Tools.cs) | 199 | gains three schemas, three dispatch arms, and `WriteFile`, `AppendFile`, `DeleteFile` |
| [`ChatProtocol.cs`](code/csharp/ChatProtocol.cs) | 55 | one comment. `Text` now carries the file text too. |
| [`Program.cs`](code/csharp/Program.cs) | 97 | the comment at the top. Nothing else. |
| [`Helper.cs`](code/csharp/Helper.cs) | 148 | nothing |

```bash
cd chapters/06.A-writing/code/csharp
rm -rf /tmp/repo-06A && cp -R ../../../../docs /tmp/repo-06A
REPO=/tmp/repo-06A dotnet run
```

**59 new code lines** (312 against chapter 06's 253), against the ~40 guide in
[`PLAN.md`](../../docs/PLAN.md). Most of them are the three schemas. `dotnet build` is clean,
0 warnings. Four things worth stopping on:

- **No prompt line.** Chapter 06 needed one before the model would call `save_note`. Here the
  model picked the right tool in every run, with the tool descriptions alone. Asked to *"Add a
  line"* or *"Put a line in"*, it chose `append_file` 6 of 6 times, not `write_file`.
- **`ToolArguments` does not change.** `write_file` and `append_file` send a `path` and a
  `text`. Both fields exist since chapter 06. One record still fits every tool.
- **`DeleteFile` asks two questions.** `File.Exists`, then `Directory.Exists`. A path that is
  neither is a sentence, not a throw. `recursive: true` is what lets `"."` take the whole tree.
- **`Tools.cs` is at 199 lines.** The limit is 200. Chapter 07 adds its checks here, so it moves
  a part out first.

Two bugs came out of the first runs, and both are fixed in the code:

- **The appended line was glued on.** The model writes files with no newline at the end. A plain
  `File.AppendAllText` then put `Checked on 2026-11-01.` on the end of the last sentence.
  `AppendFile` now starts a new line when the file does not end on one.
- **An append into a new folder crashed the app.** `Add the line "Ship it." to the end of
  drafts/today.md.` threw `DirectoryNotFoundException`, and the whole app stopped.
  `WriteFile` already made the folder. `AppendFile` now does the same.

## What differs between the two

The C# column is from the code. The TypeScript column is the plan, until `code/ts/` exists.

| | TypeScript | C# |
|---|---|---|
| delete a file or a folder | `rmSync(target, { recursive: true })`, one call | `File.Delete` or `Directory.Delete(target, recursive: true)` |
| make the folder | `mkdirSync(dir, { recursive: true })` | `Directory.CreateDirectory(dir)` |
| write and append | `writeFileSync`, `appendFileSync` | `File.WriteAllText`, `File.AppendAllText` |

Nothing else worth a sentence. Both sides throw on a missing folder, and both need the same fix.

## When it breaks

Each row is from a real run.

| What you see | What it means | What to do |
|---|---|---|
| `...concise, and focused.Checked on 2026-11-01.` — two lines on one | the file had no newline at the end, and the append did not add one | the `glued` check in `AppendFile` |
| `Unhandled exception. System.IO.DirectoryNotFoundException: Could not find a part of the path '.../drafts/today.md'.` | the model named a folder that does not exist. A throw ends the whole app. | `Directory.CreateDirectory` before the write or append |
| `no such tool: delete_file`, then *"I have deleted `SETUP.md`."* | the dispatch arm is missing. Nothing was deleted. The model said it was. | add the arm to `Run`. Check with `ls`, never with the answer. |
| `[step 1] write_file SUMMARY.md` with no `read_file` before it, and the file is one line | the model wrote the whole file from memory. Once in five. | nothing in this chapter's code stops it. See "Try it yourself". |
| `ls` on the repo copy says `No such file or directory` | `delete_file .` took the repo root | `cp -R ../../../../docs /tmp/repo-06A` again. Chapter 07 stops this. |

## Try it yourself

Stop `write_file` from replacing a file the model has not read.

1. In `Tools.cs`, remember every path `ReadFile` reads during one question. A `HashSet<string>`
   is enough.
2. In `WriteFile`, if the file exists and its path is not in the set, do not write. Return one
   sentence to the model that says what to do instead.
3. Clear the set at the start of each question. Where does that call go, and why there?
4. Run `In SUMMARY.md, change "Active voice." to "Use active voice."` five times on a fresh
   copy. How many times did the file keep its other lines?
5. Write one sentence: is this a guardrail? Chapter 07 has four. Which one is it closest to?

The answer is not in this repo.

## Done when

- [ ] chapter 06 said it cannot create files, and this chapter's code did
- [ ] you watched `append_file` add a line, and the line was on its own line
- [ ] you can say what `"."` means to `delete_file`, and you saw it take the whole folder
- [ ] you can say why the model asks about *"clean it up"* and not about *"delete everything"*
- [ ] you broke the dispatch and watched the model say it deleted a file that is still there
- [ ] you never pointed `REPO` at a folder you cannot rebuild
