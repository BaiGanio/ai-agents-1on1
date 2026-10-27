# Chapter 04 — camera script, C#

Target 15 minutes. Read this file top to bottom while you record. Nothing else needs to be open:
every command and every line of code is here.

Every step is **Say / Type / Proof**, and some add a **Note** — the one sentence you must not
forget. **Type** is exactly what goes on air in that step, and nothing else (decision 38). The
finished files are [`code/prompts/librarian.md`](code/prompts/librarian.md),
[`code/csharp/ChatProtocol.cs`](code/csharp/ChatProtocol.cs),
[`code/csharp/Helper.cs`](code/csharp/Helper.cs),
[`code/csharp/Tools.cs`](code/csharp/Tools.cs) and
[`code/csharp/Program.cs`](code/csharp/Program.cs), and each code step names the lines you are
typing, so a bad take is one paste away.

This chapter starts as a copy of chapter 02's code: three files, and a `prompts/` folder. You
create `Tools.cs` on camera. **71 lines of code go on air** — the most of any chapter so far,
and the chapter still only has one idea in it.

> **`sample-repo/` is empty.** Until it is filled, every run below points `REPO` at this
> repo's own `docs/` folder. The output in each **Proof** is from a real run against it. When
> `sample-repo/` exists, drop the `REPO=` prefix and re-shoot the proofs.

The TypeScript video is `SCRIPT-ts.md`, which is not written yet. Acts 0, 1 and 7 are the
shared ones.

| Act | Steps | Minutes |
|---|---|---|
| 0 — it still cannot see | 1 | 0:00–1:00 |
| 1 — the whole trick, by curl | 2–3 | 1:00–3:30 |
| 2 — the prompt grows three tools | 4–6 | 3:30–5:00 |
| 3 — `Call`, one record | 7 | 5:00–5:30 |
| 4 — `Tools.cs`, three functions and a switch | 8–12 | 5:30–9:00 |
| 5 — `Helper.cs`, the send moves out | 13–15 | 9:00–10:30 |
| 6 — `Program.cs`, parse and run | 16–22 | 10:30–13:00 |
| 7 — run it, break it, hand off | 23–26 | 13:00–15:00 |

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
cd /Users/lk/Projects/BaiGanio/ai-agents-1on1/chapters/04-tools-by-hand/code/csharp
dotnet build
```

```bash
# P6. keep the finished code, then put chapter 02's code in its place so you can type the
#     difference on air. Chapter 02 has no Tools.cs — you create it on camera.
cd /Users/lk/Projects/BaiGanio/ai-agents-1on1
mkdir -p /tmp/take-04 && cp -R chapters/04-tools-by-hand/code /tmp/take-04/
rm -rf chapters/04-tools-by-hand/code
cp -R chapters/02-the-prompt/code chapters/04-tools-by-hand/code
rm -rf chapters/04-tools-by-hand/code/ts
rm -rf chapters/04-tools-by-hand/code/csharp/bin chapters/04-tools-by-hand/code/csharp/obj
```

```bash
# P7. after the take, put the finished code back
cd /Users/lk/Projects/BaiGanio/ai-agents-1on1
rm -rf chapters/04-tools-by-hand/code
git checkout -- chapters/04-tools-by-hand/code
git status
```

Last checks, by eye: terminal font is sized up, the terminal is wide enough that a file listing
does not wrap, the editor has no other tab open, and notifications are off.

---

## Act 0 — It still cannot see · 0:00–1:00

### 1. The sentence it keeps saying

**Say.** "Three chapters in, and it keeps telling us the same thing. Listen to it."

**Type.**

```bash
cd /Users/lk/Projects/BaiGanio/ai-agents-1on1/chapters/02-the-prompt/code/csharp
dotnet run
```

At the `You:` prompt:

```text
Which file starts the HTTP server?
```

**Proof.** *"I cannot see the files yet."*

**Note.** Say it once, and be flat about it: **it is right.** It has no hands. It has never
opened a folder in its life. Today it gets three tools, and the surprise is how little that
takes — no plugin system, no registry, no framework. Then say the Everyday Picture from
[`README.md`](README.md): the request slip at a library desk. You write the shelf mark on a
slip and slide it across. Someone else has the legs.

---

## Act 1 — The whole trick, by curl · 1:00–3:30

### 2. Ask for JSON, and get JSON

**Say.** "Before any code. The whole of tool calling is in this one curl, and there is no tool
in it anywhere."

**Type.**

```bash
curl -s http://localhost:11434/api/chat -d '{
  "model": "gemma4:e2b", "stream": false, "think": false,
  "messages": [
    {"role":"system","content":"You are the Repo Librarian. You have three tools: list_files(path), read_file(path), search_text(query). To use one, answer with one line of JSON and nothing else, like {\"tool\": \"list_files\", \"path\": \".\"}. No prose. No markdown fence."},
    {"role":"user","content":"What files are in this folder?"}
  ]
}' | python3 -m json.tool
```

**Proof.** The whole of `message.content`, from a real run on this machine:

```text
{"tool": "list_files", "path": "."}
```

**Note.** This is the beat of the chapter and it is thirty seconds in. Point at the screen and
say all three.

Thirty-five characters, and every one is for your code, not for a human.

Nothing has run. **The model asked.** There is no folder on the other end of this curl, no
function, no filesystem. It wrote a sentence in a shape we invented.

And look where we described the tools: in **English, in the system prompt.** That is the whole
interface.

### 3. Hand the result back

**Say.** "So now I am the code. I read the JSON, I run the function in my head, and I give the
answer back the only way there is — as another message."

**Type.**

```bash
curl -s http://localhost:11434/api/chat -d '{
  "model": "gemma4:e2b", "stream": false, "think": false,
  "messages": [
    {"role":"system","content":"You are the Repo Librarian. You have three tools: list_files(path), read_file(path), search_text(query). To use one, answer with one line of JSON and nothing else, like {\"tool\": \"list_files\", \"path\": \".\"}. No prose. No markdown fence. When a tool result is given to you, answer the question in at most three sentences."},
    {"role":"user","content":"What files are in this folder?"},
    {"role":"assistant","content":"{\"tool\": \"list_files\", \"path\": \".\"}"},
    {"role":"user","content":"tool result of list_files:\nGLOSSARY.md\nMODELS.md\nPLAN.md\nPROGRESS.md\nSETUP.md\nSTYLE.md"}
  ]
}' | python3 -m json.tool
```

**Proof.** From a real run on this machine:

```text
There are six files in this folder: GLOSSARY.md, MODELS.md, PLAN.md, PROGRESS.md, SETUP.md,
and STYLE.md. These files appear to be related to some kind of project or documentation.
```

**Note.** Count the array out loud: **four messages, and two of them are ours.** The model's
own JSON goes back in as `assistant`, and the result goes after it as `user`. Drop the
assistant message and the model gets a file listing with no idea why.

Then say the cost, because nobody warns about it: **one tool use is two round trips.** You wait
for the model, your code runs, you wait for the model again.

Everything from here is that curl, in C#, in a loop.

---

## Act 2 — The prompt grows three tools · 3:30–5:00

### 4. Delete the lie, name the tools

**Say.** "Chapter 02's prompt says it has no tools. That is now false, so it goes."

**Type.** In [`code/prompts/librarian.md`](code/prompts/librarian.md), delete these two lines
from chapter 02:

```text
You cannot see any files yet. You have no tools. If a question needs a file,
say "I cannot see the files yet" and stop. Never guess a file name.
```

Then type **lines 5-9**:

```markdown
You have three tools:

list_files(path)      names the files in a folder
read_file(path)       gives back the text of one file
search_text(query)    finds the lines that contain some text
```

**Note.** This is a Markdown file. No schema, no JSON Schema, no attribute, no decorator. Say
that out loud while it is on screen, because in chapter 05 it becomes a `tools` field and the
viewer should remember it started here, as a sentence.

**Proof.** Three tools, described in English.

### 5. The shape to answer in

**Say.** "And now the contract. Which is not a contract, and we will watch it break later."

**Type.** **Lines 11-17**:

```markdown
To use a tool, answer with one line of JSON and nothing else:

{"tool": "list_files", "path": "."}
{"tool": "read_file", "path": "src/server.ts"}
{"tool": "search_text", "query": "listen"}

No prose. No markdown fence. No explanation. Only the JSON.
```

**Note.** Three examples, not a description. A small model copies a shape far better than it
follows a rule about a shape — and `src/server.ts` is a made-up path on purpose, so it copies
the shape and not the value.

Then say the honest part: **you asked for JSON in English, and English is not a contract.** It
will mostly do this and sometimes not. Step 24 is where you watch it not.

### 6. What to do with a result

**Say.** "One more line, for the second half of the round trip."

**Type.** **Lines 19-20**:

```markdown
When a tool result is given to you, answer the question in at most three sentences.
Never guess a file name. If the tools did not show it to you, say so.
```

**Note.** *"Never guess a file name"* is chapter 02's line, surviving. It did the work then and
it does more work now: the tools can show it a listing, so a guess has no excuse left.

**Proof.** The file is 20 lines. Nothing has been compiled.

---

## Act 3 — `Call`, one record · 5:00–5:30

### 7. The model's JSON is a wire format too

**Say.** "The model writes a shape. Shapes live in `ChatProtocol.cs`. It does not care that we
invented this one."

**Type.** In [`code/csharp/ChatProtocol.cs`](code/csharp/ChatProtocol.cs), **lines 15-19**:

```csharp
// What the model writes when it asks for a tool. It is not part of Ollama's wire format — it
// is a shape we invented in the system prompt, and the model may break it any time.
// All three are nullable: list_files sends a path and no query, search_text the other way
// round, and a model that answers `{}` sends neither.
public record Call(string? Tool, string? Path, string? Query);
```

**Note.** Stop on the three question marks and give each five seconds, because they are the
only type design in the chapter.

`Path` is null for `search_text`. `Query` is null for the other two. One record covers all
three tools because the key that was not sent arrives as `null`.

`Tool` is nullable too, and that one is not about the tools — it is about the model. A reply of
`{}` parses perfectly and means nothing. One `null` check in step 20 catches it.

**Proof.** `dotnet build` is clean. Nothing uses it.

---

## Act 4 — `Tools.cs`, three functions and a switch · 5:30–9:00

### 8. A new file that has never heard of a model

**Say.** "New file. And the thing to notice is what is *not* in it."

**Type.** A new file, `code/csharp/Tools.cs`, **lines 1-10**:

```csharp
// The three tools, and the switch that picks one. Nothing in this file knows a model exists.
// They are ordinary functions. That is the whole point of the chapter.
public static class Tools
{
    // Every path the model wrote is joined onto RepoRoot. In this chapter that is the only
    // thing between the model and the rest of your disk, and it is not a rule — Path.Combine
    // happily accepts "../../../.ssh". Chapter 07 makes it a real one.
    static string Under(string? path) => Path.Combine(Helper.RepoRoot, path ?? ".");

    static string Name(string file) => Path.GetRelativePath(Helper.RepoRoot, file);
```

**Note.** Read the comment on `Under` out loud, slowly, and then sit still for two seconds.
`Path.Combine` is not a guard. Hand it `../../../.ssh/id_rsa` and it builds that path for you
and the file gets read. **Say the quiet part: after this chapter your agent can read anything
your user account can read.** Then name the chapter that fixes it — 07 — and move on. Do not
fix it here. The viewer has to feel it.

`Helper.RepoRoot` does not exist yet. Let the editor underline it in red. You write it in step
14, and leaving it red for four minutes is a better explanation than a sentence.

**Proof.** One red squiggle, on purpose.

### 9. The dispatch

**Say.** "And here is the thing people sell you a framework for."

**Type.** **Lines 12-21**:

```csharp
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
```

**Note.** Point at the four arms and say it in one sentence: **a string the model wrote, picking
a function you wrote.** That is the entire mechanism. There is no registration, no attribute, no
reflection, no container.

Then point at the `_` arm. An invented tool name is not an exception. It is a sentence that goes
back to the model, and the model reads it and tries something else. **Your tools talk to a
reader, not to a debugger.** That rule decides every return value in the next three steps.

**Proof.** Three more red squiggles. Three functions to go.

### 10. `list_files`

**Say.** "Ten lines. First tool."

**Type.** **Lines 23-36**:

```csharp
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
```

**Note.** Two things, and the second one is the kind of bug you only find on camera.

It returns a **string**, not a list. A message is a string. Everything a tool gives back has to
end up as text, so it may as well start as text.

`Path.GetRelativePath` is there because the model copies. Give it
`/Users/you/code/src/server.ts` and the next thing it sends you is a `read_file` with that
whole absolute path in it. Give it `src/server.ts` and you get `src/server.ts` back.

**Proof.** Two squiggles left.

### 11. `read_file`

**Say.** "Second tool, and the important line is the one that does not throw."

**Type.** **Lines 38-47**:

```csharp
    static string ReadFile(string? path)
    {
        string file = Under(path);

        // A missing file is a sentence, not an exception. A throw ends the run and teaches the
        // model nothing. This goes back as a tool result and it tries another path.
        if (!File.Exists(file)) return $"no such file: {path}";

        return File.ReadAllText(file);
    }
```

**Note.** This is the rule from step 9, in code. Say both halves.

A `throw` here kills the run, and the model never learns that the file was not there.

`return $"no such file: {path}"` goes back as a tool result, the model reads it like any other
text, and it can try a different name or tell the user. You see this happen in step 24.

And say what is missing: no size check. `File.ReadAllText` on a 40 MB file puts 40 MB in your
prompt. It measured 20 seconds on a 42 KB file on this machine.

**Proof.** One squiggle left.

### 12. `search_text`

**Say.** "Third tool. A `foreach` inside a `foreach`, and that is the whole search engine."

**Type.** **Lines 49-69**:

```csharp
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
```

**Note.** Three small decisions, ten seconds each.

`File.ReadLines`, not `ReadAllLines`. It reads one line at a time instead of the whole file.

`file:number: line` is the shape `grep` uses, and the model has read a lot of `grep` output. A
shape it has seen before is a shape it uses well.

`OrdinalIgnoreCase`, because a human asking for *"timeout"* means `Timeout` too.

And the empty case is a sentence again — `no line contains Timeout` — not an empty string. An
empty string tells the model nothing happened. A sentence tells it what happened.

**Proof.** `Tools.cs` is 69 lines. It still will not build: `Helper.RepoRoot` is red.

---

## Act 5 — `Helper.cs`, the send moves out · 9:00–10:30

### 13. One using

**Say.** "The send is about to move into this file, so it needs the send's using."

**Type.** In [`code/csharp/Helper.cs`](code/csharp/Helper.cs), **line 1**:

```csharp
using System.Net.Http.Json;
```

**Proof.** Nothing visible. Keep moving.

### 14. The folder to read

**Say.** "And the red squiggle from four minutes ago."

**Type.** **Lines 22-24**, under `PromptName`:

```csharp
    // The folder the Librarian reads. Every path the model writes is joined onto this one.
    public static readonly string RepoRoot =
        Environment.GetEnvironmentVariable("REPO") ?? "../../../../sample-repo";
```

**Note.** Point at the four fields now sitting together — `Model`, `OllamaUrl`, `PromptName`,
`RepoRoot` — and say the rule one more time, because it has now paid off three chapters
running: **`Helper.cs` is the only file in this project that reads the environment.**

**Proof.** Every squiggle in `Tools.cs` is gone.

### 15. The send moves into `Ask`

**Say.** "Now the one refactor in this chapter, and the reason for it is one sentence: **today
we send twice.**"

**Type.** **Lines 95-122**. Cut the send out of `Program.cs` and paste it here, then make four
edits: wrap it in a method, drop the `Helper.` prefixes, drop the `Console.Write` of the model
name, and return the message instead of adding it to the list.

```csharp
    // One send, one streamed reply. It was inline in Program.cs until this chapter, where the
    // same send happens twice per question — once to get the tool call, once to get the answer.
    // It does not touch the history. The caller appends, so Program.cs still shows the memory.
    public static async Task<Message> Ask(List<Message> history)
    {
        ChatRequest request = new(Model, history, Stream: true, Think: Think);

        using HttpRequestMessage message =
            new(HttpMethod.Post, "/api/chat")
            {
                Content = JsonContent.Create(request, options: AgentJsonOptions),
            };

        // ResponseHeadersRead hands us the stream as it arrives. Without it HttpClient waits for
        // the last byte, and the stream stops being a stream.
        using var response =
            await OllamaHttpClient.SendAsync(message, HttpCompletionOption.ResponseHeadersRead);

        if (!response.IsSuccessStatusCode)
            await HandleResponseError(response);

        StringBuilder content = new();
        StringBuilder thinking = new();

        await HandleStream(response, content, thinking);

        return new Message("assistant", content.ToString(), thinking.ToString());
    }
```

**Note.** Read the third comment line out loud, because it is a choice and not a tidy-up:
**`Ask` does not touch the history.** It would be one line shorter if it appended the reply
itself. Then `Program.cs` would stop showing the message list being built, and "the list is the
memory" — the thing chapters 01 to 02 spent their time on — would disappear into a helper. The
caller appends. You will see `messages.Add` four times in the next act, and that is the point.

Say the forward pointer too, because it is one line away: **wrap `Ask` in a `while` and you
have chapter 05.**

**Proof.** `dotnet build` clean, 0 warnings. `Program.cs` is now broken — you cut its middle
out. That is the next act.

---

## Act 6 — `Program.cs`, parse and run · 10:30–13:00

### 16. Say what the chapter is

**Say.** "Top of the file, same as every chapter."

**Type.** In [`code/csharp/Program.cs`](code/csharp/Program.cs), **lines 1-4**. Delete chapter
02's comment and type this:

```csharp
// Chapter 04 — tools by hand.
//
// The model still only writes text. But now some of that text is JSON, and when it is, we run
// a function and give the answer back as one more message. That is all tool calling ever is.
```

**Proof.** The old comment is gone.

### 17. One using in, two out

**Say.** "The HTTP usings went into `Helper.cs` with the send. This file needs one thing now,
and it is the parser."

**Type.** **Line 6**. Delete chapter 02's `using System.Net.Http.Json;` and
`using System.Text;`, and leave only:

```csharp
using System.Text.Json;
```

**Note.** Worth five seconds: the file that talks to the model no longer imports anything about
HTTP. It imports a parser. That is what this chapter did to it.

**Proof.** Two usings gone, one left.

### 18. Say which folder, out loud

**Say.** "First line of output, every run. You will thank yourself for this in step 25."

**Type.** **Line 17**, and delete chapter 02's
`HttpClient http = Helper.OllamaHttpClient;` while you are here:

```csharp
Console.WriteLine($"Reading {Path.GetFullPath(Helper.RepoRoot)}");
```

**Note.** `GetFullPath`, not `RepoRoot`. A relative path printed back at you teaches nothing —
`../../../../sample-repo` is exactly as confusing on screen as it was in the code. The absolute
path tells you instantly whether you are pointed where you think you are.

**Proof.** It builds, and the first line names a folder.

### 19. Send, and keep the reply

**Say.** "The eighteen lines that used to be here are one line now."

**Type.** **Lines 30-33**, where the send used to be:

```csharp
    Console.Write($"{Helper.Model}: ");
    Message reply = await Helper.Ask(messages);
    messages.Add(reply);
    Console.WriteLine();
```

**Note.** `messages.Add(reply)` — that is one. Count them on camera as they arrive; there are
four in this file by the end of the act.

And the reply is kept in a variable this time, not just printed. Chapters 01 to 02 printed it
and forgot it. This chapter has to **look at it**.

**Proof.** It builds and it runs. It is chapter 02 again, with an extra variable.

### 20. Try to read it as JSON

**Say.** "And here is the whole of today, in C#."

**Type.** **Lines 35-47**:

```csharp
    // There is no "is this JSON?" function. You try to read it, and a throw means the model
    // wrote prose — which is a real answer, already printed above, not an error. So this catch
    // is the ordinary path. Catch JsonException and not Exception, or a bug in Tools.cs hides.
    Call? call = null;
    try
    {
        call = JsonSerializer.Deserialize<Call>(reply.Content, Helper.AgentJsonOptions);
    }
    catch (JsonException)
    {
    }

    if (call?.Tool is null) continue;
```

**Note.** Four things. This is the slowest step in the video — take the time.

**There is no "is this JSON?" function.** You try, and failure is the answer. That reads
backwards the first time and it is correct.

**The empty `catch` is not laziness.** A throw here means the model wrote prose, which means it
answered the question — and the answer is already on screen, because `Ask` streamed it. There
is nothing to do. Say "the catch is the normal path" out loud.

**`JsonException`, not `Exception`.** Point at it. The day `Tools.cs` has a bug, a wide catch
swallows it, prints the model's text, and costs you an afternoon. Two characters.

**`call?.Tool is null`** covers three cases in one line: the parse threw, the model sent `{}`,
or it sent the word `null`. All three mean "that was a plain answer", and `continue` goes back
to the `You:` prompt.

**Proof.** Build clean. Run it and say hello — you get an answer and no crash.

### 21. Run the function

**Say.** "It asked. Now we decide."

**Type.** **Lines 49-53**:

```csharp
    // The model asked. Your code decided. The model ran nothing — it never could.
    string result = Tools.Run(call);
    Console.WriteLine($"{Helper.Grey}[{call.Tool}]{Helper.Reset}");
    Console.WriteLine(result);
    Console.WriteLine();
```

**Note.** Read the comment out loud as written. It is the sentence the whole chapter exists for,
and this is the last chance to say it before the thing works and distracts everybody.

`[{call.Tool}]` in grey is not decoration. It is your code saying *I chose to believe that text
and call this function.* Without it, a viewer watching the output cannot tell where the model
stopped and your program started.

**Proof.** Run it and ask for a listing. You see the JSON, then `[list_files]`, then real file
names. Then it stops, because nothing hands the result back yet.

### 22. Hand the result back

**Say.** "Which is step 3's curl, in code."

**Type.** **Lines 55-61**:

```csharp
    // The result is just another message. No tool channel, no callback, no framework. The
    // model's own JSON has to stay in front of it, or the listing arrives with no question.
    messages.Add(new("user", $"tool result of {call.Tool}:\n{result}"));

    Console.Write($"{Helper.Model}: ");
    messages.Add(await Helper.Ask(messages));
    Console.WriteLine();
```

**Note.** Scroll up so the whole loop is on screen and count the four `messages.Add` out loud:
the question, the model's JSON, the tool result, the answer. **Four messages per tool use, and
every one of them goes back on the next call.**

Then say the role, because it surprises people: the tool result goes in as **`user`**. Not a
tool role, not a function role, not a channel. Ollama has a `tool` role and chapter 05 uses it.
Today a plain `user` message does the job, and that is worth knowing: **there was never a
protocol here. There was a list of strings.**

**Proof.** `dotnet build` clean, 0 warnings. That is the chapter.

---

## Act 7 — Run it, break it, hand off · 13:00–15:00

### 23. The full round trip

**Say.** "Four files, seventy-one lines, and it has hands."

**Type.**

```bash
cd /Users/lk/Projects/BaiGanio/ai-agents-1on1/chapters/04-tools-by-hand/code/csharp
REPO=../../../../docs dotnet run
```

At the `You:` prompt:

```text
What files are in this folder?
```

**Proof.** From a real run on this machine, 9.58 seconds including `dotnet run`, with the grey
thinking left out here:

```text
Reading /Users/lk/Projects/BaiGanio/ai-agents-1on1/docs
Type a question. An empty line stops the app.
You: What files are in this folder?
gemma4:e2b: {"tool": "list_files", "path": "."}

[list_files]
GLOSSARY.md
MODELS.md
PLAN.md
PROGRESS.md
SETUP.md
STYLE.md

gemma4:e2b: The files in this folder are GLOSSARY.md, MODELS.md, PLAN.md, PROGRESS.md,
SETUP.md, and STYLE.md.
```

**Note.** Do not explain it. Point at the three blocks in order — model, your code, model — and
let the viewer read. Then say the one thing the picture does not show: **the model ran nothing.**

### 24. The other two tools, and the one that fails

**Say.** "Same run. Three more questions."

**Type.** At each `You:` prompt in turn:

```text
What is in STYLE.md?
Find every line that contains the word quantisation.
What is in server.ts?
```

**Proof.** All three from a real run on this machine.

`read_file`, then a correct summary of the real file:

```text
gemma4:e2b: {"tool": "read_file", "path": "STYLE.md"}
...
gemma4:e2b: STYLE.md outlines strict writing rules for documentation, emphasizing short
sentences, active voice, and avoiding hype.
```

`search_text`, with two real hits:

```text
gemma4:e2b: {"tool": "search_text", "query": "quantisation"}

[search_text]
GLOSSARY.md:32: | <a id="quantisation"></a>**quantisation** | making a model smaller by ...
PLAN.md:60: | 09 | `llama-cpp` | swap the runner. Flags, quantisation, context size. ...
```

And the third one, which is the one to slow down for:

```text
gemma4:e2b: {"tool": "read_file", "path": "server.ts"}

[read_file]
no such file: server.ts

gemma4:e2b: ... the file was not found.
```

**Note.** Stop there. **Nothing crashed.** The tool returned a sentence, the sentence went back
as a message, the model read it like any other text and told the user. That is step 11's `if`,
earning its keep. Say the rule one last time: your tools talk to a reader.

### 25. Break it twice

**Say.** "Two errors you will hit on day one."

**Type.**

```bash
REPO=../../../../nope dotnet run
```

Ask anything.

**Proof.** From a real run on this machine:

```text
Reading /Users/lk/Projects/BaiGanio/ai-agents-1on1/nope
...
[list_files]
no such folder: .
```

**Note.** Point at the first line of output. That is step 18 paying for itself — the path is
wrong and you can see it is wrong, instead of guessing.

**Type.** Then the one that catches everybody:

```bash
REPO=../../../02-the-prompt/code dotnet run
```

```text
What files are in this folder?
```

**Proof.** From a real run on this machine, 36 names, five of them like this:

```text
[list_files]
csharp/bin/Debug/net10.0/RepoLibrarian
csharp/bin/Debug/net10.0/RepoLibrarian.deps.json
csharp/bin/Debug/net10.0/RepoLibrarian.dll
...
gemma4:e2b: There are many files in this folder. They include DLLs, source files like
ChatProtocol.cs and Helper.cs, project files, and prompt files.
```

**Note.** Your tools have no filter. They walked `bin/` and `obj/`, and the model is now
answering questions about build output. Say what that cost: thirty-six file names in the prompt
instead of nine, and an answer about DLLs.

Do not fix it on camera. Say the fix out loud — skip `bin` and `obj` in `ListFiles` — and say
why it is not in this chapter: **the moment you start deciding which files a tool may touch,
you are writing chapter 07.**

### 26. The costs, and hand off

**Say.** "Three numbers before you go."

**Note.** All three from real runs on this machine. Say them flat, with nothing on screen.

**Two round trips per tool use.** One to get the JSON, one to get the answer. A listing took
9.58 seconds end to end.

**`read_file` has no size cap.** A 42 KB file went into the prompt whole and the same question
took **19.92 seconds** instead of 9.58. Twice the wait, for one unguarded line.

**And it can read anything you can read.** `Path.Combine` took whatever path arrived. Nothing in
these 71 lines says no.

**Say.** "So it has hands, and no manners. It runs one tool and stops — ask it *which file sets
the timeout* and it asks for a listing, because it wants to look before it searches. We do not
let it. Next video we do: the native `tools` field, and a `while` loop around `Ask`. One of
those is a convenience. The other one turns this into an agent."

**Proof.** Stop recording.

---

## After the take

1. Run **P7** above to restore the code.
2. Check `Tools.cs` exists and all four `.cs` files build clean.
3. Check `prompts/librarian.md` is 20 lines and `prompts/pirate.md` is untouched.
4. Re-run the by-script check: every code block above must still be verbatim in the file it
   names, in the same order (decision 38).
5. Write the real run numbers and the machine into [`docs/PROGRESS.md`](../../docs/PROGRESS.md).
6. Tick the boxes in the chapter [`README.md`](README.md) "Done when" list.
