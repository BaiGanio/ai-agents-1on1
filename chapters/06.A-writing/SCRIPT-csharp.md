# Chapter 06.A — camera script, C#

Target 12 minutes. Read this file top to bottom while you record. Nothing else needs to be open:
every command and every line of code is here.

Every step is **Say / Type / Proof**, and some add a **Note** — the one sentence you must not
forget. **Type** is exactly what goes on air in that step, and nothing else (decision 38). The
finished files are [`code/csharp/Program.cs`](code/csharp/Program.cs),
[`code/csharp/ChatProtocol.cs`](code/csharp/ChatProtocol.cs) and
[`code/csharp/Tools.cs`](code/csharp/Tools.cs), and each code step names the lines you are
typing, so a bad take is one paste away.

This chapter starts as a copy of chapter 06's code: four files, and a `prompts/` folder. No new
file is created, and the prompt does not change. **59 new code lines go on air**, almost all of
them in `Tools.cs`.

> **Never point `REPO` at a real folder in this video.** These tools delete, and nothing asks.
> Every run below points `REPO` at `/tmp/repo-06A`, a copy of this repo's `docs/` folder. P7
> makes it, and Act 3 makes it again. The output in each **Proof** is from a real run against a
> copy like it.

The order follows the file. `Tools.cs` is typed top to bottom: first what the model is told,
then the switch, then the bodies. So nothing runs until Act 3. Say that once, at the start of
Act 1, so the wait has a reason.

The TypeScript video is `SCRIPT-ts.md`, which is not written yet. Acts 0 and 4 are the shared
ones.

| Act | Steps | Minutes |
|---|---|---|
| 0 — it cannot write | 1 | 0:00–1:30 |
| 1 — what the model is told | 2–7 | 1:30–4:30 |
| 2 — what your code does | 8–11 | 4:30–7:30 |
| 3 — let it loose | 12–15 | 7:30–10:30 |
| 4 — break it, hand off | 16–17 | 10:30–12:00 |

---

## Before you record — off camera

Run these in order. Every one is a copy-paste.

```bash
# P1. the model server, in its own terminal window. Leave it running.
ollama serve
```

```bash
# P2. the model is pulled
ollama pull gemma4:e2b
```

```bash
# P3. the model is warm. Without this, your first on-air answer costs 20 seconds of silence.
ollama run gemma4:e2b "hi" --verbose
```

```bash
# P4. everything else is installed
cd /Users/lk/Projects/BaiGanio/ai-agents-1on1
./scripts/check.sh
```

```bash
# P5. warm the SDK too, or the first `dotnet run` costs you 20 more seconds of silence
cd /Users/lk/Projects/BaiGanio/ai-agents-1on1/chapters/06.A-writing/code/csharp
dotnet build
```

```bash
# P6. keep the finished code, then put chapter 06's code in its place so you can type the
#     difference on air. Chapter 06 has all four files — this chapter creates none.
cd /Users/lk/Projects/BaiGanio/ai-agents-1on1
rm -rf /tmp/take-06A && mkdir -p /tmp/take-06A && cp -R chapters/06.A-writing/code /tmp/take-06A/
rm -rf chapters/06.A-writing/code
cp -R chapters/06-memory/code chapters/06.A-writing/code
rm -rf chapters/06.A-writing/code/csharp/bin chapters/06.A-writing/code/csharp/obj
rm -f chapters/06.A-writing/code/notes.md
```

```bash
# P7. the folder the agent may wreck. A copy, never the real docs/.
rm -rf /tmp/repo-06A && cp -R /Users/lk/Projects/BaiGanio/ai-agents-1on1/docs /tmp/repo-06A
```

```bash
# P8. chapter 06's code has to still work, because Act 0 records it failing to write
cd /Users/lk/Projects/BaiGanio/ai-agents-1on1/chapters/06-memory/code/csharp
dotnet build
```

```bash
# P9. after the take, put the finished code back, and clear the note Act 0 saves
cd /Users/lk/Projects/BaiGanio/ai-agents-1on1
rm -rf chapters/06.A-writing/code
cp -R /tmp/take-06A/code chapters/06.A-writing/
rm -f chapters/06.A-writing/code/notes.md chapters/06-memory/code/notes.md
rm -rf /tmp/repo-06A
git status
```

Last checks, by eye: terminal font is sized up, the terminal is wide enough that a file listing
does not wrap, the editor has no other tab open, and notifications are off.

---

## Act 0 — It cannot write · 0:00–1:30

### 1. Ask chapter 06 for a file

**Say.** "Last video it learned to remember. Today I want it to keep the docs. Watch me ask it
for a file."

**Type.**

```bash
cd /Users/lk/Projects/BaiGanio/ai-agents-1on1/chapters/06-memory/code/csharp
REPO=/tmp/repo-06A dotnet run
```

```text
Create a file called TODO.md with three short things to do in this repo.
```

**Proof.** From a real run on this machine, 6.77 s:

```text
You: Create a file called TODO.md with three short things to do in this repo.
gemma4:e2b:  [375 tokens in]
[step 1] list_files .
...(six names)

gemma4:e2b:  [426 tokens in]
[step 2] save_note Create a file named TODO.md with three tasks.
saved

gemma4:e2b: I cannot directly create files, but I can help you by suggesting the content for a
file named `TODO.md`. Here are three potential tasks for a repository:

1. Review the latest code changes.
2 Update documentation for key modules.
3 Refactor performance bottlenecks. [462 tokens in]
```

**Note.** Two things, and the second is funny. It says it **cannot create files** — true, it has
no tool for that. And look at step 2: it grabbed the only tool that writes anything, and saved
your request as a **note**. A model with the wrong tool uses the wrong tool.

Then the Everyday Picture from [`README.md`](README.md): a new helper, with your keys. Today we
hand over a pen and a shredder. We do not write any rules yet.

Type an empty line to quit. P9 deletes that note after the take.

---

## Act 1 — What the model is told · 1:30–4:30

### 2. Say what the chapter is

**Say.** "Chapter 06.A's folder, with chapter 06's code in it. Top of `Program.cs` first — and it
is the only thing in `Program.cs` that changes."

**Type.** In [`code/csharp/Program.cs`](code/csharp/Program.cs), **lines 1-4**. Replace chapter
06's four-line comment with this:

```csharp
// Chapter 06.A — writing.
//
// Chapter 06's agent, with three tools that change files: write, append, delete. Nothing below
// this comment changes. The new tools live in Tools.cs, and the loop runs them like any other.
```

**Note.** Say it plainly: **the loop does not change.** A tool that deletes runs through exactly
the same loop as a tool that reads. The loop cannot tell them apart. That is the problem.

**Proof.** Nothing visible. Keep moving.

### 3. One comment, no new field

**Say.** "The new tools need a path and some text. We have both fields already."

**Type.** In [`code/csharp/ChatProtocol.cs`](code/csharp/ChatProtocol.cs), **lines 42-43**.
Change the comment above `ToolArguments`. The record line stays:

```csharp
// Text is what save_note, write_file and append_file write. The read tools leave it null.
public record ToolArguments(string? Path, string? Query, string? Text);
```

**Note.** One record for every tool, every field nullable, since chapter 05. `write_file` sends a
`path` and a `text`. Both exist. Nothing to add.

**Proof.** Nothing visible.

### 4. "Three" is wrong now

**Say.** "`Tools.cs`. Two comments still say three tools. Fix those first."

**Type.** In [`code/csharp/Tools.cs`](code/csharp/Tools.cs), **lines 1-2**:

```csharp
// The tools: what the model is told about them, what they do, and the switch that picks one.
// The bodies are ordinary functions that know no model exists.
```

**Proof.** Nothing visible.

### 5. And the second one

**Type.** **Lines 9-10**:

```csharp
    // What goes in the request's `tools` field, every call. The descriptions are the only thing
    // the model reads to choose between them, so they are written for it, not for you.
```

**Note.** Then say how this act goes. The file is typed top to bottom: what the model is told,
then the switch, then the bodies. **Nothing runs until Act 3.** You will see red squiggles on the
way. That is fine.

**Proof.** Nothing visible.

### 6. write_file, on the list

**Say.** "Three new schemas. You know the shape. Read the descriptions out loud, because that is
all the model ever reads."

**Type.** **Lines 49-58**, after the `save_note` schema:

```csharp
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
```

**Note.** Read *"makes a file, or replaces all of its text"* out loud, and stress **all**. There
is no edit. A one-word change is the whole file, written back. Act 4 pays for that.

**Proof.** Nothing visible.

### 7. append_file and delete_file

**Type.** **Lines 59-77**, right after it:

```csharp
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
```

**Note.** Read the last description: *"deletes a file, or a folder and everything in it"*. We
are telling the model, in plain English, that it may delete a whole folder. It will believe us.

**Proof.** Nothing visible.

---

## Act 2 — What your code does · 4:30–7:30

### 8. Three arms on the switch

**Say.** "The switch. Three more arms."

**Type.** **Lines 97-101**, after the `save_note` arm:

```csharp
            "write_file" =>
                WriteFile(call.Function.Arguments?.Path, call.Function.Arguments?.Text),
            "append_file" =>
                AppendFile(call.Function.Arguments?.Path, call.Function.Arguments?.Text),
            "delete_file" => DeleteFile(call.Function.Arguments?.Path),
```

**Proof.** Three red squiggles, one per function that does not exist yet.

### 9. WriteFile

**Say.** "Now the bodies. Every tool so far could only read, or add a note to one file. From
here, the model changes your files."

**Type.** **Lines 162-172**, at the bottom of the class:

```csharp
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
```

**Note.** Point at `Directory.CreateDirectory`. Without it, `drafts/today.md` with no `drafts/`
folder throws, and a throw ends the **whole app**, not the tool call. [`BASICS.md`](BASICS.md)
has the exception.

**Proof.** One squiggle gone.

### 10. AppendFile

**Type.** **Lines 174-186**:

```csharp
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
```

**Note.** Two lines to stop on.

`CreateDirectory` again. The first version of this function had no such line, and an append
into a new folder **crashed the app**. Measured, not guessed.

Then `glued`. Read the comment. The model writes files with **no newline at the end**. Append
right after that, and your line joins the last one. Act 4 shows it.

**Proof.** Another squiggle gone.

### 11. DeleteFile

**Type.** **Lines 188-198**:

```csharp
    // No recycle bin. A folder goes with everything in it, and "." is the whole repo.
    static string DeleteFile(string? path)
    {
        string target = Under(path);

        if (File.Exists(target)) File.Delete(target);
        else if (Directory.Exists(target)) Directory.Delete(target, recursive: true);
        else return $"no such file or folder: {path}";

        return $"deleted {path}";
    }
```

**Note.** Slow down here. `recursive: true` deletes a folder, every folder in it, and every file
in all of them. **No recycle bin.** And read the comment: `"."` is the whole repo, because every
path is joined onto `REPO`.

**Proof.** `dotnet build`. Clean, 0 warnings.

---

## Act 3 — Let it loose · 7:30–10:30

### 12. Write a file

**Say.** "Everything is wired. No prompt line this time — let us see if it needs one."

**Type.**

```bash
cd /Users/lk/Projects/BaiGanio/ai-agents-1on1/chapters/06.A-writing/code/csharp
REPO=/tmp/repo-06A dotnet run
```

```text
Read STYLE.md and write a one-line summary of it into SUMMARY.md.
```

**Proof.** From a real run on this machine, 2.82 s. The file is cut:

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

**Note.** It read first, then wrote. No prompt line needed: the descriptions were enough.
Chapter 06 needed a line before the model would call `save_note`. This time it did not.

Quit with an empty line.

### 13. Append a line

**Type.** `REPO=/tmp/repo-06A dotnet run`, then:

```text
Add the line "Checked on 2026-11-01." to the end of SUMMARY.md.
```

Quit, then:

```bash
cat /tmp/repo-06A/SUMMARY.md
```

**Proof.** From a real run on this machine, 1.13 s:

```text
You: Add the line "Checked on 2026-11-01." to the end of SUMMARY.md.
gemma4:e2b:  [611 tokens in]
[step 1] append_file SUMMARY.md
appended to SUMMARY.md
```

```text
# Style Summary

Style emphasizes short sentences, active voice, no hype, explaining "what" rather than "how".
Every chapter README must include specific sections like problem solved, simple example, and
error handling. Diagrams should be clear, and code comments should explain the "why", not just
the "what".
Checked on 2026-11-01.
```

**Note.** Two things. "One line" became three sentences — it is a 2B model. And **`Checked on`
sits on its own line.** That is the `glued` check working. Act 4 takes it out.

The `cat` output above is wrapped to fit this page. On screen, the summary is one long line.

### 14. Delete all but one

**Say.** "Now the shredder."

**Type.** `REPO=/tmp/repo-06A dotnet run`, then:

```text
Delete every file here except GLOSSARY.md.
```

Quit, then `ls /tmp/repo-06A`.

**Proof.** From a real run on this machine, 2.72 s:

```text
You: Delete every file here except GLOSSARY.md.
gemma4:e2b:  [596 tokens in]
[step 1] list_files .
GLOSSARY.md
MODELS.md
PLAN.md
PROGRESS.md
SETUP.md
STYLE.md
SUMMARY.md

gemma4:e2b:  [651 tokens in]
[step 2] delete_file MODELS.md
deleted MODELS.md

gemma4:e2b:  [683 tokens in]
[step 3] delete_file PLAN.md
deleted PLAN.md

gemma4:e2b:  [714 tokens in]
[step 4] delete_file PROGRESS.md
deleted PROGRESS.md

gemma4:e2b:  [745 tokens in]
[step 5] delete_file SETUP.md
deleted SETUP.md

gemma4:e2b:  [776 tokens in]
[step 6] delete_file STYLE.md
deleted STYLE.md

gemma4:e2b:  [807 tokens in]
[step 7] delete_file SUMMARY.md
deleted SUMMARY.md

gemma4:e2b: I have deleted all files except GLOSSARY.md. [838 tokens in]
```

```text
GLOSSARY.md
```

**Note.** Seven steps, under three seconds. The longest run in the course so far, and it is a
delete. It did exactly what you said. It never asked.

### 15. Delete everything

**Say.** "One more. Fresh copy first."

**Type.**

```bash
rm -rf /tmp/repo-06A && cp -R /Users/lk/Projects/BaiGanio/ai-agents-1on1/docs /tmp/repo-06A
REPO=/tmp/repo-06A dotnet run
```

```text
Delete everything in this folder.
```

Quit, then `ls /tmp/repo-06A`.

**Proof.** From a real run on this machine, 1.03 s:

```text
You: Delete everything in this folder.
gemma4:e2b:  [591 tokens in]
[step 1] delete_file .
deleted .

gemma4:e2b: Everything in the current directory has been deleted. [618 tokens in]
```

```text
ls: /tmp/repo-06A: No such file or directory
```

**Note.** This is the beat of the chapter. Stop and point.

`delete_file .` — **one step, one second**. Not the files in the folder: **the folder**. 4 times
in 4 runs.

Then say the other half, from the README table. Asked *"This folder is a mess. Clean it up."*,
it asked what you meant. Asked *"Delete everything"*, it did not. **It asks when your words are
vague. It does not ask when they are clear and dangerous.** Asking is a guess, not a rule.

Make the copy again before Act 4:

```bash
rm -rf /tmp/repo-06A && cp -R /Users/lk/Projects/BaiGanio/ai-agents-1on1/docs /tmp/repo-06A
```

---

## Act 4 — Break it, hand off · 10:30–12:00

### 16. Break it

**Say.** "Why the `glued` line? Take it out and see."

**Type.** In `Tools.cs`, line 184. Replace it with a plain append:

```text
        File.AppendAllText(file, text);
```

Then run, and ask both questions from steps 12 and 13 in **one** session:

```bash
REPO=/tmp/repo-06A dotnet run
```

Quit, then `cat /tmp/repo-06A/SUMMARY.md`.

**Proof.** From a real run on this machine, 3.78 s for both. The last line of the file:

```text
...concise, and focused.Checked on 2026-11-01.
```

**Note.** Two lines became one. The model wrote the summary with no newline at the end, and
append put your text right after the last character. Nothing failed. No error. You find it when
you open the file.

Put line 184 back, `dotnet build`, clean.

### 17. The costs, and hand off

**Say.** "Three things before you go."

**Note.** Say them flat, with nothing on screen.

**It changes your files now.** Write, append, delete — inside the folder you pointed it at, with
the same loop that reads.

**It does not ask.** It asks when you are vague. It does not ask when you are clear and the
thing is dangerous. And once in five, asked to change one word, it wrote a whole file from
memory and said it was done.

**What it says is not what it did.** With a broken switch it said *"I have deleted
`SETUP.md`."* The file was still there. Check the disk, not the answer.

**Say.** "So it can keep the docs now, and it can wreck them in one second. What it needs is
rules in **your** code: a fence around the folder, a cap on the steps, and a question before it
writes or deletes. That is guardrails, next video."

**Proof.** Stop recording.

---

## After the take

1. Run **P9** above to restore the code and remove the scratch folder.
2. Check all four `.cs` files build clean with 0 warnings, and that no new file was created.
3. Check `prompts/librarian.md` and `prompts/pirate.md` are the same as chapter 06's.
4. Check `code/notes.md` is gone in both chapter 06 and 06.A.
5. Re-run the by-script check:
   `python3 scripts/check-script.py chapters/06.A-writing/SCRIPT-csharp.md` (decision 45).
6. Write the real run numbers and the machine into [`docs/PROGRESS.md`](../../docs/PROGRESS.md).
7. Tick the boxes in the chapter [`README.md`](README.md) "Done when" list.
