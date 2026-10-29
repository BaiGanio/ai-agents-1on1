# Chapter 06 — camera script, C#

Target 15 minutes. Read this file top to bottom while you record. Nothing else needs to be open:
every command and every line of code is here.

Every step is **Say / Type / Proof**, and some add a **Note** — the one sentence you must not
forget. **Type** is exactly what goes on air in that step, and nothing else (decision 38). The
finished files are [`code/prompts/librarian.md`](code/prompts/librarian.md),
[`code/csharp/ChatProtocol.cs`](code/csharp/ChatProtocol.cs),
[`code/csharp/Tools.cs`](code/csharp/Tools.cs),
[`code/csharp/Helper.cs`](code/csharp/Helper.cs) and
[`code/csharp/Program.cs`](code/csharp/Program.cs), and each code step names the lines you are
typing, so a bad take is one paste away.

This chapter starts as a copy of chapter 05's code: four files, and a `prompts/` folder. No new
file is created. **34 new code lines go on air**, plus one line of prompt.

> **`sample-repo/` is empty.** Until it is filled, every run below points `REPO` at this
> repo's own `docs/` folder. The output in each **Proof** is from a real run against it. When
> `sample-repo/` exists, drop the `REPO=` prefix and re-shoot the proofs.

The order is long memory first, then short memory. That is not the order of the README, and it
is on purpose: each file is typed top to bottom, and `Program.cs` reads the notes near the top
and drops tool results near the bottom.

The TypeScript video is `SCRIPT-ts.md`, which is not written yet. Acts 0 and 4 are the shared
ones.

| Act | Steps | Minutes |
|---|---|---|
| 0 — it forgets you | 1 | 0:00–1:30 |
| 1 — read the notebook | 2–5 | 1:30–4:30 |
| 2 — write the notebook | 6–12 | 4:30–8:30 |
| 3 — clear the desk | 13–18 | 8:30–13:00 |
| 4 — break it, hand off | 19–20 | 13:00–15:00 |

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
cd /Users/lk/Projects/BaiGanio/ai-agents-1on1/chapters/06-memory/code/csharp
dotnet build
```

```bash
# P6. keep the finished code, then put chapter 05's code in its place so you can type the
#     difference on air. Chapter 05 has all four files — this chapter creates none.
cd /Users/lk/Projects/BaiGanio/ai-agents-1on1
rm -rf /tmp/take-06 && mkdir -p /tmp/take-06 && cp -R chapters/06-memory/code /tmp/take-06/
rm -rf chapters/06-memory/code
cp -R chapters/05-the-loop/code chapters/06-memory/code
rm -rf chapters/06-memory/code/csharp/bin chapters/06-memory/code/csharp/obj
```

```bash
# P7. no notes left over from a rehearsal. Act 1 writes the first one by hand, on air.
rm -f /Users/lk/Projects/BaiGanio/ai-agents-1on1/chapters/06-memory/code/notes.md
```

```bash
# P8. chapter 05's code has to still work, because Act 0 records it forgetting you
cd /Users/lk/Projects/BaiGanio/ai-agents-1on1/chapters/05-the-loop/code/csharp
dotnet build
```

```bash
# P9. after the take, put the finished code back
cd /Users/lk/Projects/BaiGanio/ai-agents-1on1
rm -rf chapters/06-memory/code
cp -R /tmp/take-06/code chapters/06-memory/
rm -f chapters/06-memory/code/notes.md
git status
```

Last checks, by eye: terminal font is sized up, the terminal is wide enough that a file listing
does not wrap, the editor has no other tab open, and notifications are off.

---

## Act 0 — It forgets you · 0:00–1:30

### 1. Tell it your name, twice

**Say.** "Last video it became an agent. Watch me introduce myself to it."

**Type.**

```bash
cd /Users/lk/Projects/BaiGanio/ai-agents-1on1/chapters/05-the-loop/code/csharp
REPO=../../../../docs dotnet run
```

At the `You:` prompt:

```text
My name is Lyuben.
```

Type an empty line to quit. Run the same `dotnet run` again, and ask:

```text
What is my name?
```

**Proof.** From two real runs on this machine:

```text
You: My name is Lyuben.
gemma4:e2b: Hello Lyuben. How may I assist you with the code repository today?
```

```text
You: What is my name?
gemma4:e2b: I do not have access to your personal information, so I do not know your name.
```

**Note.** It said hello to you by name. One restart later, you are a stranger. Say why in one
sentence: **the message list lives in a variable, and a variable dies with the app.**

Then say the second problem, which you cannot see yet: the list also **never shrinks**. Every
file it reads stays in it and is sent again on every step. Too little memory across runs, too
much inside one. Today fixes both.

Then the Everyday Picture from [`README.md`](README.md): the desk and the notebook. The desk is
cleared when a job is done. The notebook goes home with you.

Type an empty line to quit, and leave that terminal.

---

## Act 1 — Read the notebook · 1:30–4:30

### 2. Say what the chapter is

**Say.** "Chapter 06's folder, with chapter 05's code in it. Top of `Program.cs` first."

**Type.** In [`code/csharp/Program.cs`](code/csharp/Program.cs), **lines 1-4**. Replace chapter
05's five-line comment with this:

```csharp
// Chapter 06 — memory.
//
// Chapter 05's loop, with two kinds of memory. Short: the message list, which we now shrink
// after each answer. Long: a notes file, which we read into the system message at the start.
```

**Proof.** Nothing visible. Keep moving.

### 3. Where the notebook lives

**Say.** "A notebook needs a place on the shelf. One line in `Helper.cs`, with the other
settings."

**Type.** In [`code/csharp/Helper.cs`](code/csharp/Helper.cs), **lines 26-29**, after
`RepoRoot`:

```csharp
    // Long memory. Next to the prompts, outside RepoRoot, so list_files never shows the agent
    // its own notes as if they were part of the repo.
    public static readonly string NotesPath =
        Environment.GetEnvironmentVariable("NOTES") ?? "../notes.md";
```

**Note.** Point at **outside `RepoRoot`**. The tools read `REPO`. If the notes lived there,
`list_files` would show the agent its own notebook as if it were part of the code. `NOTES` moves
it, the same way `REPO` and `MODEL` move theirs.

**Proof.** Nothing visible.

### 4. Read it in, as its own message

**Say.** "And the start of the app reads it. Right after the system message."

**Type.** In `Program.cs`, **lines 15-22**, after `List<Message> messages = ...`:

```csharp
// Long memory. Every run starts knowing nothing, so last run's notes go back in as a second
// system message. Measured with four tools: glued onto the end of the first one, the model
// missed them three times in five and said it had no record of your name. Here, five in five.
if (File.Exists(Helper.NotesPath))
{
    string notes = await File.ReadAllTextAsync(Helper.NotesPath);
    messages.Add(new("system", $"Notes from earlier sessions:\n{notes}"));
}
```

**Note.** Two things to say, and the second is the finding.

`File.Exists` first. The first run has no notes, and the app must start the same way chapter 05
did. [`BASICS.md`](BASICS.md) has the exception you get without it.

Then: **why a second system message, and not the end of the first?** Because the end of the
first does not work. Measured, real code, five runs each: glued onto the end, the model found
your name 4 of 5 times with three tools, and **2 of 5** with four. As its own message, 5 of 5
both ways. The failing runs said *"I do not have a record of your name."* — with the note right
there in the request. Do not type the wrong version on air. With three tools it usually works,
and the take proves nothing.

**Proof.** `dotnet build`. Clean.

### 5. Write a note by hand, and read it back

**Say.** "No tool yet. I write the first note myself, to prove the reading half on its own."

**Type.**

```bash
cd /Users/lk/Projects/BaiGanio/ai-agents-1on1/chapters/06-memory/code/csharp
echo "- User's name is Lyuben." > ../notes.md
REPO=../../../../docs dotnet run
```

```text
What is my name?
```

**Proof.** From a real run on this machine, 1.91 s:

```text
You: What is my name?
gemma4:e2b: Your name is Lyuben.
```

**Note.** No tool call. It did not look anything up. The note was in front of it, in the list,
and it read it. Same question, same model as Act 0. **The only difference is a file.**

Same trick as chapter 04: by hand first, so you see there is nothing magic in it. Now we let
the model write the notebook.

Quit with an empty line.

---

## Act 2 — Write the notebook · 4:30–8:30

### 6. One more argument

**Say.** "The model needs a way to hand us a sentence. One more field on the arguments."

**Type.** In [`code/csharp/ChatProtocol.cs`](code/csharp/ChatProtocol.cs), **lines 42-43**.
Replace chapter 05's `ToolArguments` line:

```csharp
// Text is save_note's one argument. The other three tools leave it null.
public record ToolArguments(string? Path, string? Query, string? Text);
```

**Note.** Same design as chapter 05: one record for all the tools, every field nullable. A key
the model did not send arrives as `null`, not a crash.

**Proof.** Nothing visible.

### 7. The fourth tool, on the list

**Say.** "A fourth tool. You know the shape — this is the fourth copy of it."

**Type.** In [`code/csharp/Tools.cs`](code/csharp/Tools.cs), **lines 40-48**, after the
`search_text` schema:

```csharp
        new(new(
            "save_note",
            "saves one short fact to keep for the next session",
            new("object",
                new()
                {
                    ["text"] = new("string", "the fact, in one sentence"),
                },
                ["text"]))),
```

**Note.** Read the description out loud: *saves one short fact to keep for the next session.*
That sentence is all the model knows about this tool. Hold that thought for step 11.

**Proof.** Nothing visible.

### 8. The dispatch

**Say.** "One more arm on the switch."

**Type.** **Line 67**, after the `search_text` arm:

```csharp
            "save_note" => SaveNote(call.Function.Arguments?.Text),
```

**Proof.** A red squiggle on `SaveNote`.

### 9. The first tool that writes

**Say.** "And the body. Every tool so far has only read. This one writes."

**Type.** **Lines 118-126**, at the bottom of the class:

```csharp
    // The first tool that writes. It can only add a line, and only to a file the model cannot
    // name. Chapter 07 makes it ask you first.
    static string SaveNote(string? text)
    {
        if (string.IsNullOrWhiteSpace(text)) return "save_note needs a text";

        File.AppendAllText(Helper.NotesPath, $"- {text.Trim()}\n");
        return "saved";
    }
```

**Note.** Say what it **cannot** do, because that is the design. It cannot pick the file —
your code names it. It cannot overwrite — `AppendAllText` only adds to the end. The worst it can
do is add lines you did not want. And it does not ask you first. Chapter 07 fixes that.

**Proof.** The squiggle is gone.

### 10. Show the note on screen

**Say.** "The step line prints the path or the query. Teach it the third argument."

**Type.** In `Program.cs`, **line 71**:

```csharp
                    + $"{sent?.Path ?? sent?.Query ?? sent?.Text}{Helper.Reset}");
```

**Proof.** `dotnet build`. Clean.

### 11. Ask it to remember

**Say.** "Everything is wired. Let us ask it to remember a rule."

**Type.**

```bash
REPO=../../../../docs dotnet run
```

```text
Remember that we always write diagrams as flowchart TD.
```

Quit, then:

```bash
cat ../notes.md
```

**Proof.** From a real run on this machine, 2.22 s:

```text
You: Remember that we always write diagrams as flowchart TD.
gemma4:e2b: Understood. I will format any diagrams you ask me to generate in the flowchart TD
syntax. How can I assist you with the code in the current repository?
```

```text
- User's name is Lyuben.
```

**Note.** This is the beat of the chapter. Slow down.

It said *"Understood."* It promised. There is **no `[step 1]` line**, so no tool ran. And the
notes file still has one line — the one **you** wrote in Act 1.

The model did not lie on purpose. It wrote what a person would say, and saying is all a model
can do. **Only a tool call writes a file.** Measured by curl: 0 calls in 12 tries, across four
questions like this one, and every answer promised to remember.

### 12. The prompt says when

**Say.** "Chapter 05 taught us this once already. The `tools` field says what it may do. The
prompt says when."

**Type.** In [`code/prompts/librarian.md`](code/prompts/librarian.md), **line 10**, after the
repo-root line, with a blank line after it:

```markdown
When the user asks you to remember something, call save_note with one short sentence.
```

Run it again, same question, then `cat ../notes.md`.

**Proof.** From a real run on this machine, 2.29 s:

```text
You: Remember that we always write diagrams as flowchart TD.
gemma4:e2b:
[step 1] save_note We always write diagrams as flowchart TD.
saved

gemma4:e2b:
```

```text
- User's name is Lyuben.
- We always write diagrams as flowchart TD.
```

**Note.** One line of English. Measured by curl: **0 of 12 without it, 15 of 15 with it.** And
*"What files are in this folder?"* still calls `list_files`, so the line did not turn it into a
note-taker for everything.

The second `gemma4:e2b:` is empty. The model chose to say nothing after `saved`. It happens; the
note is on disk, and `cat` is the proof.

Quit with an empty line.

---

## Act 3 — Clear the desk · 8:30–13:00

### 13. Move one record, and give it a count

**Say.** "Now short memory. To shrink something, you have to see its size first. Ollama tells
us — we have been throwing the number away."

**Type.** In `ChatProtocol.cs`, delete chapter 05's one-line `ChatResponse` record (under
`ChatRequest`), and type it again at the bottom of the file, **lines 50-55**:

```csharp
// One line of the stream. Ollama counts the whole prompt it read and puts the number on the last
// line only. Same trap as tool_calls: camelCase makes it promptEvalCount, which arrives as null.
public record ChatResponse(
    Message Message,
    [property: JsonPropertyName("prompt_eval_count")] int? PromptEvalCount = null
);
```

**Note.** Why it moves: it is a shape that **comes back**, so it goes with the other shapes that
come back. Five seconds.

Then the attribute, and say it is chapter 05's trap a second time. The camelCase policy turns
`PromptEvalCount` into `promptEvalCount`. Ollama sends `prompt_eval_count`. Without the
attribute nothing fails — the number is `null` every time, and you never see it. Step 19 shows
that.

**Proof.** Nothing visible.

### 14. Print the count

**Say.** "The stream's last line carries it. We print it in grey."

**Type.** In `Helper.cs`, inside `HandleStream`, **lines 82-89**. Replace chapter 05's two-line
`Message part = ...!.Message;` with this:

```csharp
            ChatResponse chunk =
                JsonSerializer.Deserialize<ChatResponse>(line, AgentJsonOptions)!;
            Message part = chunk.Message;

            // Only the last line carries it. It is the size of everything we just sent, which
            // is the number that grows on every step, and the number this chapter cuts.
            if (chunk.PromptEvalCount is int tokens)
                Console.Write($"{Grey} [{tokens} tokens in]{Reset}");
```

**Note.** The count is the size of **everything** we sent for that one step: both system
messages, the whole list, and the tool list. Measured by curl, it stays honest when Ollama
reuses its cache: the same request twice reported 924 and 924.

**Proof.** `dotnet build`. Clean.

### 15. Watch the bill

**Say.** "Two questions. Watch the grey numbers, and watch what happens to the second
question."

**Type.**

```bash
REPO=../../../../docs dotnet run
```

```text
Which file explains how we write diagrams, and what does it say about them?
```

```text
What files are in this folder?
```

**Proof.** From a real run on this machine, 6.56 s for both. The file is cut:

```text
You: Which file explains how we write diagrams, and what does it say about them?
gemma4:e2b:  [404 tokens in]
[step 1] list_files .
...(six names)

gemma4:e2b:  [455 tokens in]
[step 2] read_file GLOSSARY.md
...(cut)

gemma4:e2b: The file `GLOSSARY.md` explains terms used in the course. It defines terms like
"agent," "tool," "system prompt," and "tool schema." [2239 tokens in]

You: What files are in this folder?
gemma4:e2b:  [2291 tokens in]
[step 1] list_files .
...(six names)

gemma4:e2b: The files in this folder are `GLOSSARY.md`, `MODELS.md`, `PLAN.md`,
`PROGRESS.md`, `SETUP.md`, and `STYLE.md`. [2342 tokens in]
```

**Note.** Point at **2291**. The second question is about file names. It needs none of the
glossary. **It sent the whole glossary anyway**, and every step of every question after it
will too. That is the desk with the old job still on it.

Quit with an empty line.

### 16. The drop

**Say.** "So when the job is done, we clear the desk. After the answer, not during."

**Type.** In `Program.cs`, **lines 87-94**, after the inner loop closes and before the last
`Console.WriteLine();`:

```csharp
    // Short memory. The answer is on screen, so the files it read have done their job. Left in,
    // each one is sent again on every step of every question after this. The call stays, so
    // the model still sees which file it read. Only the text goes. Need it? Read it again.
    string stub = "[dropped. Call the tool again to see it.]";
    for (int i = 0; i < messages.Count; i++)
    {
        if (messages[i].Role == "tool") messages[i] = messages[i] with { Content = stub };
    }
```

**Note.** Three choices in eight lines. Say each one.

**After the answer, not during.** Mid-question, the model is using the file. That is why it
asked for it.

**Only the text goes. The call stays.** The `assistant` message that said `read_file
GLOSSARY.md` is untouched, so the model still sees what it read, and every `tool` message still
follows its call.

**`with`, and a `for` with an index.** A record cannot change, so `with` makes a copy with one
field different, and the copy goes back in the same place. `foreach` would throw — `BASICS.md`
has the message.

**Proof.** `dotnet build`. Clean.

### 17. The bill, after

**Say.** "Same two questions."

**Type.** `REPO=../../../../docs dotnet run`, and the same two questions as step 15.

**Proof.** From a real run on this machine, 5.18 s for both. The file is cut:

```text
You: Which file explains how we write diagrams, and what does it say about them?
gemma4:e2b:  [404 tokens in]
[step 1] list_files .
...(six names)

gemma4:e2b:  [455 tokens in]
[step 2] read_file STYLE.md
...(cut)

gemma4:e2b: The file `STYLE.md` explains how diagrams are written. It states that diagrams
should be in the `flowchart TD` format, and there should be only one diagram per chapter.
[1379 tokens in]

You: What files are in this folder?
gemma4:e2b:  [533 tokens in]
[step 1] list_files .
...(six names)

gemma4:e2b: The files in this folder are `GLOSSARY.md`, `MODELS.md`, `PLAN.md`,
`PROGRESS.md`, `SETUP.md`, and `STYLE.md`. [584 tokens in]
```

**Note.** Point at **533** against **2291**.

Be honest about one thing on air: this run read `STYLE.md`, not the glossary. Same question,
different file — chapter 05's "a loop is not a function". So say the clean number too, from a
run where both read the glossary: the second question started at **506 with the drop and 2271
without**.

### 18. Need it again? Read it again

**Say.** "What did we lose? Ask a follow-up about a file it already read."

**Type.** `REPO=../../../../docs dotnet run`, then:

```text
Read GLOSSARY.md and tell me what a token is.
```

```text
What does that same file say a stub is?
```

**Proof.** From a real run on this machine. The file is cut:

```text
You: Read GLOSSARY.md and tell me what a token is.
gemma4:e2b:  [402 tokens in]
[step 1] read_file GLOSSARY.md
...(cut)

gemma4:e2b: As per the GLOSSARY.md, a token is a piece of a word that models read and write, not
letters. [2186 tokens in]

You: What does that same file say a stub is?
gemma4:e2b:  [488 tokens in]
[step 1] read_file GLOSSARY.md
...(cut)

gemma4:e2b: A token is a piece of a word that models read and write, not letters. [2272 tokens in]
```

**Note.** Point at the second `[step 1] read_file GLOSSARY.md`. **It went and got the file
again.** The call it kept told it which file, and the stub told it what to do. That is the cost
of the drop: one call when you need the file, against the whole file on every step when you do
not.

Then the answer, because it is wrong, and do not hide it. It read the right file and answered
the **old** question. An earlier run got it right: *"A stub is a short line put where a long
text was..."*. Memory is fixed. The model is still a 2B model.

Quit with an empty line.

---

## Act 4 — Break it, hand off · 13:00–15:00

### 19. Break it

**Say.** "One break, and it is the one with no error message."

**Type.** In `ChatProtocol.cs`, delete `[property: JsonPropertyName("prompt_eval_count")] ` from
line 54, and run:

```bash
REPO=../../../../docs dotnet run
```

```text
What is my name?
```

**Proof.** From a real run on this machine, 2.36 s:

```text
You: What is my name?
gemma4:e2b: Your name is Lyuben.
```

**Note.** It works. Your name is there. **The count is gone**, and nothing told you. No error,
no warning. The field arrived as `null` and the `if` skipped it. Same bug as chapter 05's
`tool_calls`, and it will not be the last time.

Put the attribute back, run it once more, and watch the grey count come back.

### 20. The costs, and hand off

**Say.** "Three things before you go."

**Note.** Say them flat, with nothing on screen.

**The list shrinks now.** After an answer, every file it read is a one-line stub. The next
question starts small. A follow-up pays one more tool call.

**The notebook is a file.** Two lines, plain text, `cat` reads it. Nothing goes in unless a tool
call writes it, and the prompt is what makes the model call.

**And it writes without asking.** `save_note` is the first tool that changes your disk. It can
only add a line, to one file. It still never asks you. And the loop from chapter 05 still has no
cap.

**Say.** "So it remembers you now, and it stops carrying old files around. Two things it still
does not do: ask before it writes, and stop when it has gone round too many times. Both are
rules in your code, not in the model. That is guardrails, next video."

**Proof.** Stop recording.

---

## After the take

1. Run **P9** above to restore the code.
2. Check all four `.cs` files build clean with 0 warnings, and that no new file was created.
3. Check `prompts/librarian.md` is 14 lines and has the `save_note` line in it.
4. Check `prompts/pirate.md` is untouched, and `code/notes.md` is gone.
5. Re-run the by-script check:
   `python3 scripts/check-script.py chapters/06-memory/SCRIPT-csharp.md` (decision 45).
6. Write the real run numbers and the machine into [`docs/PROGRESS.md`](../../docs/PROGRESS.md).
7. Tick the boxes in the chapter [`README.md`](README.md) "Done when" list.
