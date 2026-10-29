# Chapter 05 — camera script, C#

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

This chapter starts as a copy of chapter 04's code: four files, and a `prompts/` folder. No new
file is created. **54 lines of code go on air** — the fewest of any chapter that adds a part,
and it is the chapter that turns this into an agent.

> **`sample-repo/` is empty.** Until it is filled, every run below points `REPO` at this
> repo's own `docs/` folder. The output in each **Proof** is from a real run against it. When
> `sample-repo/` exists, drop the `REPO=` prefix and re-shoot the proofs.

The TypeScript video is `SCRIPT-ts.md`, which is not written yet. Acts 0, 1 and 7 are the
shared ones.

| Act | Steps | Minutes |
|---|---|---|
| 0 — the question it could not finish | 1 | 0:00–1:30 |
| 1 — the `tools` field, by curl | 2–3 | 1:30–4:00 |
| 2 — the prompt gives its tools away | 4–5 | 4:00–5:00 |
| 3 — `ChatProtocol.cs`, the new shapes | 6–10 | 5:00–7:30 |
| 4 — `Tools.cs`, the list moves in | 11–12 | 7:30–9:00 |
| 5 — `Helper.cs`, send the list, keep the calls | 13–16 | 9:00–10:30 |
| 6 — `Program.cs`, the loop | 17–22 | 10:30–13:00 |
| 7 — run it, break it, hand off | 23–27 | 13:00–15:00 |

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
# P4. the model supports the tools field. If `tools` is not in this list, the whole chapter
#     does not work and you want to know now, not on camera.
ollama show gemma4:e2b | grep -i capabilities -A 5
```

```bash
# P5. everything else is installed
cd /Users/lk/Projects/BaiGanio/ai-agents-1on1
./scripts/check.sh
```

```bash
# P6. warm the SDK too, or the first `dotnet run` costs you 20 more seconds of silence
cd /Users/lk/Projects/BaiGanio/ai-agents-1on1/chapters/05-the-loop/code/csharp
dotnet build
```

```bash
# P7. keep the finished code, then put chapter 04's code in its place so you can type the
#     difference on air. Chapter 04 has all four files — this chapter creates none.
cd /Users/lk/Projects/BaiGanio/ai-agents-1on1
mkdir -p /tmp/take-05 && cp -R chapters/05-the-loop/code /tmp/take-05/
rm -rf chapters/05-the-loop/code
cp -R chapters/04-tools-by-hand/code chapters/05-the-loop/code
rm -rf chapters/05-the-loop/code/csharp/bin chapters/05-the-loop/code/csharp/obj
```

```bash
# P8. chapter 04's code has to still work, because Act 0 records it failing on purpose
cd /Users/lk/Projects/BaiGanio/ai-agents-1on1/chapters/04-tools-by-hand/code/csharp
dotnet build
```

```bash
# P9. after the take, put the finished code back
cd /Users/lk/Projects/BaiGanio/ai-agents-1on1
rm -rf chapters/05-the-loop/code
git checkout -- chapters/05-the-loop/code
git status
```

Last checks, by eye: terminal font is sized up, the terminal is wide enough that a file listing
does not wrap, the editor has no other tab open, and notifications are off.

---

## Act 0 — The question it could not finish · 0:00–1:30

### 1. Chapter 04 hangs up on it

**Say.** "Last video we gave it hands. Watch me ask it one question that it cannot finish, and
watch where it stops."

**Type.**

```bash
cd /Users/lk/Projects/BaiGanio/ai-agents-1on1/chapters/04-tools-by-hand/code/csharp
REPO=../../../../docs dotnet run
```

At the `You:` prompt:

```text
Which file sets the HTTP timeout?
```

**Proof.** From a real run on this machine, grey thinking left out:

```text
Reading /Users/lk/Projects/BaiGanio/ai-agents-1on1/docs
Type a question. An empty line stops the app.
You: Which file sets the HTTP timeout?
gemma4:e2b: {"tool": "list_files", "path": "."}
[list_files]
GLOSSARY.md
MODELS.md
PLAN.md
PROGRESS.md
SETUP.md
STYLE.md

gemma4:e2b: {"tool": "read_file", "path": "SETUP.md"}
You:
```

**Note.** This is the best beat in the chapter and it is thirty seconds in. Point at the **last
line the model wrote** and read it out loud.

It is not an answer. **It is another tool call.** It listed the folder, looked at six names,
picked one, and asked to read it — and chapter 04 printed that request like it was prose and
went back to the `You:` prompt.

The model did everything right. **Our code hung up on it mid-sentence.**

Then say the whole chapter in one sentence: *"Today we stop hanging up. We go round again."*
Then the Everyday Picture from [`README.md`](README.md): chapter 04 was a request slip, one
slip, one book. Today the desk stays open and you send the runner back as many times as you
need.

Type an empty line to quit, and leave that terminal.

---

## Act 1 — The `tools` field, by curl · 1:30–4:00

### 2. Stop asking in English

**Say.** "Before any code. Chapter 04 described our tools in English, in the system prompt, and
I told you English is not a contract. Here is the contract."

**Type.**

```bash
curl -s http://localhost:11434/api/chat -d '{
  "model": "gemma4:e2b", "stream": false, "think": false,
  "messages": [
    {"role":"system","content":"You are the Repo Librarian. The folder you read is the repo root. Its path is \".\"."},
    {"role":"user","content":"What files are in this folder?"}
  ],
  "tools": [
    {"type":"function","function":{"name":"list_files","description":"names the files in a folder",
     "parameters":{"type":"object","properties":{"path":{"type":"string","description":"the folder, relative to the repo root"}},"required":["path"]}}}
  ]
}' | python3 -m json.tool
```

**Proof.** From a real run on this machine, trimmed to the message:

```text
"message": {
    "role": "assistant",
    "content": "",
    "tool_calls": [
        {
            "id": "call_czig04wc",
            "function": {
                "index": 0,
                "name": "list_files",
                "arguments": {
                    "path": "."
                }
            }
        }
    ]
}
```

**Note.** Three things, and give the first one real time.

Point at **`"content": ""`** and say nothing for two seconds. The model wrote **no words at
all.** In chapter 04 the JSON *was* the content and you had to parse the content to find out
whether it was an answer or a request. Now asking and talking are two different fields.

Point at `tool_calls`. It is a **list**, and it is data. No `JSON.parse`, no `try`, no markdown
fence to strip. The runner was told the shape, so the runner holds the model to it.

And point at `arguments`. It is an **object**, with `path` on it — the key name we chose,
spelled the way we spelled it. Some providers hand you a string of JSON here and make you parse
it twice. Ollama does not.

### 3. Hand the result back, as a tool

**Say.** "So now I am the loop. I read the call, I run the function in my head, and I give the
answer back — and this time there is a role for it."

**Type.**

```bash
curl -s http://localhost:11434/api/chat -d '{
  "model": "gemma4:e2b", "stream": false, "think": false,
  "messages": [
    {"role":"system","content":"You are the Repo Librarian. The folder you read is the repo root. Its path is \".\". Answer in at most three sentences."},
    {"role":"user","content":"What files are in this folder?"},
    {"role":"assistant","content":"","tool_calls":[{"function":{"name":"list_files","arguments":{"path":"."}}}]},
    {"role":"tool","content":"GLOSSARY.md\nMODELS.md\nPLAN.md\nPROGRESS.md\nSETUP.md\nSTYLE.md"}
  ],
  "tools": [
    {"type":"function","function":{"name":"list_files","description":"names the files in a folder",
     "parameters":{"type":"object","properties":{"path":{"type":"string","description":"the folder, relative to the repo root"}},"required":["path"]}}}
  ]
}' | python3 -c "import json,sys; r=json.load(sys.stdin); print(r['message']['content'])"
```

**Proof.** From a real run on this machine:

```text
There are five files in the current directory: GLOSSARY.md, MODELS.md, PLAN.md, PROGRESS.md,
and SETUP.md, along with STYLE.md.
```

**Note.** Four things. The third one is the chapter.

**`"role": "tool"`.** A fourth role, after `system`, `user` and `assistant`. Chapter 04 sent the
result as a plain `user` message and it worked — say that, it matters. There was never a
protocol. There was a list of strings, and now one of those strings has a better label on it.

**No `tool_name` field, and no `id`.** I dropped both and it answered correctly. Checked.

**Count the messages in that curl: four.** The prompt, the question, **the model's own request**,
and the result. The third one is the one people leave out. Take it out and the model gets six
file names with nothing explaining why they are there — so it asks for the listing again, and
again. Say that out loud now, because you will not have time in step 22.

**And it miscounted.** It said *five files* and then listed six. Leave that in and say one
sentence about it: the loop gives it more turns, it does not give it more care.

---

## Act 2 — The prompt gives its tools away · 4:00–5:00

### 4. Thirteen lines leave the prompt

**Say.** "First edit of the day is a delete. The tool list is moving out of the prompt and into
the request, so everything that described a tool in here goes."

**Type.** In [`code/prompts/librarian.md`](code/prompts/librarian.md), delete chapter 04's lines
5 to 17 — the three tool signatures, the three JSON examples, and *"No prose. No markdown fence.
No explanation. Only the JSON."* Then type **lines 5-8**:

```markdown
Use your tools to look things up. You may use several, one after another.

The folder you read is the repo root. Its path is ".". If the question names no folder
or file, start at ".".
```

**Note.** Two sentences, and the second one is a story you have to tell.

*"You may use several, one after another"* is the only place in the prompt that mentions the
loop. One line. The model does the rest.

Now the repo-root line. **Say that you found this the hard way.** The first version of this
file had a perfect tool schema and no root line, and asked *"what files are in this folder?"*
the model called **nothing** and answered *"Please tell me the folder you are referring to."*
It had three tools and asked the human to do its job. One line fixed it.

Then the rule, flat: **the `tools` field tells the model what it may do. The prompt still tells
it what it should do.** A better field does not replace chapter 02.

**Proof.** The file is four lines shorter than chapter 04's and the word "JSON" is gone from it.

### 5. What is left is the persona

**Say.** "And two lines that survive from chapter 02, because they still earn it."

**Type.** **Lines 10-12**:

```markdown
Never guess a file name. If your tools did not show it to you, say so.

When you know the answer, answer the question in at most three sentences.
```

**Note.** *"Never guess a file name"* is chapter 02's line, three chapters old. It did the work
when the agent had no hands, and it does more work now, because the excuse is gone — the tools
can show it anything in the folder.

*"When you know the answer"* is new wording, and it is doing loop work. Chapter 04 said *"when a
tool result is given to you, answer"* — one result, then answer. This version lets it decide it
does not know yet.

**Proof.** The file is 12 lines. Nothing has been compiled.

---

## Act 3 — `ChatProtocol.cs`, the new shapes · 5:00–7:30

### 6. One using

**Say.** "One import, and in sixty seconds you will see why this file of all files needs it."

**Type.** In [`code/csharp/ChatProtocol.cs`](code/csharp/ChatProtocol.cs), **line 1**:

```csharp
using System.Text.Json.Serialization;
```

**Proof.** Nothing visible. Keep moving.

### 7. The reply can carry calls now

**Say.** "The reply has a new field on it, and one attribute that will save you an afternoon."

**Type.** **Lines 6-13**. Chapter 04's `Message` is one line — replace it with this:

```csharp
// ToolCalls needs the attribute: Ollama writes tool_calls, and the camelCase policy in
// Helper.cs would make it toolCalls, which matches nothing and arrives as null every time.
public record Message(
    string Role,
    string Content,
    string? Thinking = null,
    [property: JsonPropertyName("tool_calls")] List<ToolCall>? ToolCalls = null
);
```

**Note.** Stop on the attribute and spend a slow thirty seconds on it, because this is the bug
of the chapter and it has no error message.

`Helper.cs` sets `PropertyNamingPolicy = JsonNamingPolicy.CamelCase`, and that is right for
every other field in this course. It turns `ToolCalls` into **`toolCalls`**. Ollama sends
**`tool_calls`**. Nothing matches.

So without this attribute the field is `null` on every single reply, every reply looks like a
plain answer, **the loop never goes round once** — and you get no error, no warning and no
exception. Your agent just quietly refuses to use its tools.

Say the shape too: it has a default, so every `new Message("user", prompt)` from chapter 01
still compiles. Four fields, nothing else in the file changes.

**Proof.** Red squiggles on `ToolCall`. You write it in step 10. Leave it red.

### 8. The request carries the list

**Say.** "And the request gets the field this whole chapter is named after."

**Type.** **Line 18**, inside `ChatRequest`:

```csharp
    List<Tool> Tools,
```

**Note.** It goes **before** `Stream` and `Think`, and that is not style — those two have
defaults, so a parameter without one cannot come after them. Five seconds, then move.

Worth one sentence: the tools are part of the request, next to the model name. Not a startup
step, not a registration, not a container. Every call carries the whole list.

**Proof.** One more red squiggle, on `Tool`.

### 9. The shape we send

**Say.** "Four records to describe a tool. It looks like a lot. It is the JSON you already read
in step 2, and nothing more."

**Type.** **Lines 25-38**:

```csharp
// What we send: the tools the model is allowed to ask for. Chapter 04 wrote this list in
// English in the system prompt and hoped. Now the runner reads it and holds the model to it.
public record ToolProperty(string Type, string Description);

public record ToolParameters(
    string Type,
    Dictionary<string, ToolProperty> Properties,
    List<string> Required
);

public record ToolFunction(string Name, string Description, ToolParameters Parameters);

// Type has a default because "function" is the only value Ollama takes today.
public record Tool(ToolFunction Function, string Type = "function");
```

**Note.** Put the step 2 curl back on screen next to it and walk them together, outside in:
`Tool` is `{type, function}`. `ToolFunction` is `{name, description, parameters}`.
`ToolParameters` is `{type, properties, required}`. `ToolProperty` is `{type, description}`.
**Four records, four levels of that JSON.** There is nothing clever in here.

Then point at `Dictionary<string, ToolProperty>` and say why it is a dictionary and not a
record: the keys are **your** argument names. `path`, `query`, and whatever you add next. A
record would pin them.

And the comment on `Tool`: `"function"` is the only `type` Ollama takes today, so it gets a
default and you never type it again.

**Proof.** The `Tool` squiggle in `ChatRequest` is gone.

### 10. The shape that comes back — and a delete

**Say.** "And now the other direction. This is the record that replaces chapter 04's whole
parsing problem."

**Type.** **Lines 40-49**. Then delete chapter 04's `Call` record, which sat right here:

```csharp
// What comes back: the call, as data. Chapter 04's `Call` record is gone — it parsed JSON out
// of the model's prose, and there is no prose here. Content is an empty string when it asks.
// Arguments stays nullable per field, same as chapter 04: list_files sends a path and no query,
// search_text the other way round, and a model that sends neither arrives as nulls, not a crash.
public record ToolArguments(string? Path, string? Query);

public record ToolCallFunction(string? Name, ToolArguments? Arguments);

// Id is Ollama's, not ours. We never send it back, and nothing in this course needs it.
public record ToolCall(string? Id, ToolCallFunction? Function);
```

**Note.** Delete `Call` on camera and say what you are deleting: **that was the shape we
invented and hoped for.** This is the shape the runner publishes. Same idea, one fewer
hope.

Then the nullables, and say they are the same design as chapter 04, surviving on purpose.
`Path` is null for `search_text`. `Query` is null for the other two. One record covers all
three tools, because a key that was not sent arrives as `null` instead of throwing.

`Name` is nullable too, and that one is not about tools — it is about the model. Step 20 has the
one check that covers it.

And `Id`: Ollama's, not ours. Step 3's curl left it out and the answer was correct. Say that and
move on — it stops a viewer worrying about correlation ids they do not need.

**Proof.** `dotnet build`. **It fails**, and read the error out loud: `Helper.cs` and
`Program.cs` are both broken now. `Tools` has no value and `Call` is gone. That is the next
two acts.

---

## Act 4 — `Tools.cs`, the list moves in · 7:30–9:00

### 11. The list lives next to the functions

**Say.** "Here is the one design decision in this chapter. The tool descriptions just left the
prompt. They are not going into the HTTP file. They are going here, next to the functions they
describe."

**Type.** In [`code/csharp/Tools.cs`](code/csharp/Tools.cs), **lines 1-40**. Replace chapter
04's two comment lines at the top, then add the list under the `{`:

```csharp
// The three tools: what the model is told about them, what they do, and the switch that picks
// one. The bodies are ordinary functions that know no model exists.
//
// Chapter 04 kept the description in prompts/librarian.md and the body here, so adding a tool
// meant editing two files and the two could drift apart in silence. Now both halves of a tool
// live side by side, and a tool the model cannot see is a tool you forgot to list below.
public static class Tools
{
    // What goes in the request's `tools` field, every call. The descriptions are the only thing
    // the model reads to choose between these three, so they are written for it, not for you.
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
    ];
```

**Note.** Three things, and the first one is the reason for the whole step.

**Scroll so `Schemas` and `ListFiles` are on screen together.** In chapter 04 the description
of `list_files` lived in a Markdown file in a different folder from the function, and nothing
on earth checked one against the other. Rename the function and the prompt still advertises the
old name. Now they are forty lines apart in one file, and a tool the model cannot see is a tool
you forgot to list. Say the payoff: *"adding a fourth tool is now two edits in one file."*

**Then read one description out loud and say who it is for.** *"names the files in a folder"* is
not a comment. It is the **only** thing the model reads to choose between these three. Write
them for the model.

And `new(new(` — say it is ugly and say why you left it. C# infers the record from the target
type, so the type names would be noise three levels deep. If it bothers you, that is a
`Tool.Of(...)` helper and ten minutes, and it teaches nothing.

**Proof.** `Helper.cs` still will not build. `Tools.cs` itself is clean.

### 12. The dispatch takes the new record

**Say.** "And the switch. Two lines change, and the three functions underneath do not change by
one character."

**Type.** **Lines 49-59**:

```csharp
    // Dispatch: a name the model wrote, picking a function you wrote. People sell this as a
    // tool registry. An unknown name is not a crash — the model reads the sentence and retries.
    // The `tools` field makes an unknown name unlikely, not impossible, so the arm stays.
    public static string Run(ToolCall call) =>
        call.Function?.Name switch
        {
            "list_files" => ListFiles(call.Function.Arguments?.Path),
            "read_file" => ReadFile(call.Function.Arguments?.Path),
            "search_text" => SearchText(call.Function.Arguments?.Query),
            _ => $"no such tool: {call.Function?.Name}",
        };
```

**Note.** Scroll down past it so `ListFiles`, `ReadFile` and `SearchText` are all on screen, and
say the thing that is easy to miss: **not one character of those three functions changes in this
chapter.**

They never knew a model existed. We changed *how the model asks*, which is wire format, and wire
format does not reach them. That is what the file was for.

Then the `_` arm, in one sentence. The `tools` field makes an invented tool name unlikely. It
does not make it impossible — the runner checks the shape of the call, never the sense of it. So
the sentence stays, and the model reads it and tries something else.

**Proof.** `dotnet build`. Still failing, and now only on `Helper.cs` and `Program.cs`.

---

## Act 5 — `Helper.cs`, send the list, keep the calls · 9:00–10:30

### 13. The stream has a third thing to collect

**Say.** "The stream used to fill two things. Now there are three."

**Type.** In [`code/csharp/Helper.cs`](code/csharp/Helper.cs), **lines 62-69**:

```csharp
    // Prints each piece the moment it arrives, and fills the caller's three collections. Printing
    // and remembering are two jobs, so the caller owns what it wants to keep.
    public static async Task HandleStream(
        HttpResponseMessage response,
        StringBuilder content,
        StringBuilder thinking,
        List<ToolCall> calls)
    {
```

**Proof.** One more parameter. It builds no better than before.

### 14. Keep the calls as they arrive

**Say.** "And three lines inside the loop, one of which is a measurement."

**Type.** **Lines 86-88**, above the `Content` block:

```csharp
            // Measured: a tool call arrives whole, in one line. Unlike Content, which comes a
            // few characters at a time, it is never split across lines — so nothing to stitch.
            if (part.ToolCalls is not null) calls.AddRange(part.ToolCalls);
```

**Note.** Read the comment out loud, because it is the answer to a question the viewer is about
to have. `Content` arrives a few characters at a time — that is what chapter 01.A's streaming
was all about. So does a tool call arrive in pieces too? **No.** It comes down in one JSON Lines
line, whole, and that was checked by curl. So there is nothing to buffer and nothing to stitch
back together. `AddRange` and move on.

`AddRange`, not `Add`, because `part.ToolCalls` is itself a list.

**Proof.** Nothing visible yet.

### 15. Send the list

**Say.** "And the line this whole chapter has been walking towards. One argument."

**Type.** **Lines 100-107**:

```csharp
    // One send, one streamed reply. It does not touch the history — the caller appends, so
    // Program.cs still shows the memory being built. Chapter 04 called this twice by hand.
    // Chapter 05 calls it in a while loop, and that loop is the whole chapter.
    public static async Task<Message> Ask(List<Message> history)
    {
        // The tool list goes on every single request. The model remembers nothing, so there is
        // no registration step anywhere — the tools exist for exactly as long as one call.
        ChatRequest request = new(Model, history, Tools.Schemas, Stream: true, Think: Think);
```

**Note.** Point at `Tools.Schemas` sitting between `history` and `Stream:` and say the sentence
the comment makes: **the tools go on every single request.**

There is no registration step in this course. There never will be. The model remembers nothing
between calls — that is the rule from chapter 01 — so the tool list has exactly the same
lifetime as the message list. One call. Both get re-sent every time.

And read the third comment line out loud as the forward pointer it is: chapter 04 called `Ask`
twice by hand, and this chapter wraps it in a `while`. That is the next act, and it is nine
lines.

**Proof.** `Helper.cs` is nearly clean.

### 16. Return the calls with the message

**Say.** "And `Ask` gives back what it collected."

**Type.** **Lines 123-136**:

```csharp
        StringBuilder content = new();
        StringBuilder thinking = new();
        List<ToolCall> calls = [];

        await HandleStream(response, content, thinking, calls);

        // An empty list, not null, would be sent back as `"tool_calls": []`. Null is left out of
        // the JSON entirely by WhenWritingNull, which is what a plain answer should look like.
        return new Message(
            "assistant",
            content.ToString(),
            thinking.ToString(),
            calls.Count > 0 ? calls : null);
    }
```

**Note.** One thing, and it is a callback to chapter 01.A. Point at
`calls.Count > 0 ? calls : null`.

This message goes straight into the history and gets re-sent on the next call. An empty list
would be serialised as `"tool_calls": []`, and that is a sentence about tools in a message that
had nothing to do with tools. `null` is left out of the JSON entirely, because of
`DefaultIgnoreCondition = WhenWritingNull` — the same option, four fields along, that chapter
01.A needed so `think: null` could mean "do not send the field".

Say it in one line: **a plain answer should look like a plain answer on the wire.**

**Proof.** `dotnet build`. `Helper.cs` is clean. Only `Program.cs` is red now — it still has
chapter 04's parse in it, and `Call` no longer exists.

---

## Act 6 — `Program.cs`, the loop · 10:30–13:00

### 17. Say what the chapter is, and delete a using

**Say.** "Top of the file. And the first thing to go is the parser, because we are not parsing
anything today."

**Type.** **Lines 1-5**. Delete chapter 04's comment and its
`using System.Text.Json;`, then type this:

```csharp
// Chapter 05 — the loop.
//
// Chapter 04 sent twice per question and stopped. This sends as many times as the model asks
// for, and stops when it stops asking. That while loop, and nothing else, is what makes this
// an agent. Nothing in this file decides how many steps a question takes. The model does.
```

**Note.** Worth five seconds on the deleted `using`. Chapter 04 took HTTP out of this file and
left a parser in. Chapter 05 takes the parser out too. **This file now imports nothing.** It is
a conversation and a loop, and that is all it is.

Read the last sentence of the comment out loud: *nothing in this file decides how many steps a
question takes.*

**Proof.** No usings at the top. Still red.

### 18. The second while

**Say.** "And here it is. The whole chapter, in four lines, inside the loop you already have."

**Type.** **Lines 30-36**, after `messages.Add(new("user", prompt));`:

```csharp
    // The inner loop: one step of the model's work. Nothing here counts the steps or caps them.
    // If every reply asks for a tool, this runs a tool every time, for as long as that goes on.
    // That is deliberate, it is the one thing chapter 05 leaves unsafe, and chapter 07 fixes it.
    int step = 0;
    while (true)
    {
        step++;
```

**Note.** Scroll out so both `while (true)` are on screen at once and say what each one is:
**outer loop, one question from you. Inner loop, one step of the model's work.** Two loops, one
file. That picture is the chapter.

Then read the second comment line and let it sit. **Nothing here caps anything.** `step` is
counted so it can be printed, and it is never compared to a number. If the model asks for a
tool on every turn, this runs a tool on every turn, for as long as that goes on.

Say that it is left out **on purpose**, and say why in one sentence: the loop is today's idea,
and a cap is the opposite idea — a rule in your code that overrules the model. That is chapter
07, and it is four rules, not one. For now, `Ctrl+C` is the guardrail.

**Proof.** Deeply red. Keep going.

### 19. Send, inside the loop

**Say.** "The four lines from chapter 04 move inside, and one of them starts doing something new."

**Type.** **Lines 38-43**. Re-indent chapter 04's send one level and add the two comment lines:

```csharp
        // The label prints before the send, so you watch the model say nothing. When it asks
        // for a tool its Content is an empty string — there is no prose to stream at all.
        Console.Write($"{Helper.Model}: ");
        Message reply = await Helper.Ask(messages);
        messages.Add(reply);
        Console.WriteLine();
```

**Note.** Point at `Console.Write` being **before** the send and say what you are about to see
on screen: the model prints its name, and then says nothing. Twice, in one answer. That empty
space is `content: ""` from step 2, live.

And `messages.Add(reply)` — that is one. There are two `messages.Add` in this loop by the end of
the act, and the reply has to be one of them.

**Proof.** Still red, on the parse below.

### 20. Delete the parse

**Say.** "And now the delete I have been waiting four chapters for."

**Type.** **Lines 45-49**. Delete chapter 04's thirteen lines — the `Call? call = null`, the
`try`, the `JsonSerializer.Deserialize`, the `catch (JsonException)` and the
`if (call?.Tool is null) continue;` — and type this:

```csharp
        // No parse, and no try around it. Chapter 04 needed both, because the call arrived as
        // prose it had to read. The runner hands it to us as a field, so asking has become one
        // question about a list. A reply with no calls is the answer, and the answer is already
        // on screen — Ask streamed it as it arrived.
        if (reply.ToolCalls is null or { Count: 0 }) break;
```

**Note.** Do the delete slowly and out loud. **Thirteen lines out, one line in.** Name each
thing that leaves: the `try`, the empty `catch`, the "is this JSON?" problem, and the whole
question of whether prose was an answer or a request.

Then the line that replaces them. `is null or { Count: 0 }` covers both nothings in one pattern:
the field was not sent, or it was sent empty. Either way the model has stopped asking, so it has
answered — and the answer is **already on screen**, because `Ask` streamed it. There is nothing
to print. `break`.

Say the forward pointer, because the next chapter needs it: this is the only exit. The model
decides when to take it.

**Proof.** Half the red is gone.

### 21. Run each call

**Say.** "It asked. Now we decide — and `each` is doing work in that sentence."

**Type.** **Lines 51-69**:

```csharp
        // A list, not one call, because one turn may ask for two tools. Take only the first and
        // the model gets half an answer back with no error anywhere to tell you why.
        foreach (ToolCall call in reply.ToolCalls)
        {
            // The step number and the argument go on screen *before* the tool runs, so a slow
            // search shows you what it is waiting on. Without this line you cannot tell a
            // 2-step answer from a 5-step one, and you will want to.
            // Not named `args`: a top-level program already has an implicit `args` parameter
            // for the command line, and the compiler refuses the shadow with CS0136.
            ToolArguments? sent = call.Function?.Arguments;
            Console.WriteLine(
                $"{Helper.Grey}[step {step}] {call.Function?.Name} "
                    + $"{sent?.Path ?? sent?.Query}{Helper.Reset}");

            // The model asked. Your code decided. The model ran nothing — it never could.
            string result = Tools.Run(call);

            Console.WriteLine(result);
            Console.WriteLine();
```

**Note.** Three things.

**`foreach`, not `[0]`.** A model is allowed to ask for two tools in one turn, and one day yours
will. Take only the first and your agent answers half the question, with no error anywhere to
tell you why, and you lose an afternoon.

**The `sent` variable, and the reason for its name.** Try naming it `args` and the build fails:
`error CS0136`. A top-level program already has an implicit `args` for the command line. Thirty
seconds, and the viewer never loses that half hour.

**The label before `Tools.Run`, not after.** `search_text` walks every file in the folder. Print
after it and the screen sits blank while it works. Print before and the viewer — and you — can
see what it is waiting on.

Then read the comment above `Tools.Run` out loud, as written. It is the sentence the last two
chapters exist for, and this is the last chance to say it before the thing works and distracts
everybody: **the model asked. Your code decided. The model ran nothing.**

**Proof.** One brace short. Nearly there.

### 22. Hand the result back

**Say.** "Which is step 3's curl, in code. Four lines of comment for one line of work, and the
comment is the lesson."

**Type.** **Lines 71-80**:

```csharp
            // Role "tool" — the fourth one, after system, user and assistant. Chapter 04 faked
            // this as a user message. Both messages have to go in, in this order: the model's
            // own request above, then its result here. Add only the result and the model sees a
            // file listing with nothing explaining why, so it asks for the same listing again.
            messages.Add(new("tool", result));
        }
    }

    Console.WriteLine();
}
```

**Note.** Scroll so the whole inner loop is on screen and count the two `messages.Add` out loud:
**the reply, then the result.** Then say the failure, because it is the most common way to break
this loop and it looks nothing like a bug: leave out the first one and the model gets a file
listing with no question attached, so it asks for the same listing again, and again, and you
watch your agent loop forever doing nothing wrong.

Then the role. `"tool"`, the fourth one. Say what chapter 04 did — a plain `user` message — and
that it worked. **There was never a protocol here. There was a list of strings.** The role is a
better label on one of them, and the model can now tell its own work from yours.

**Proof.** `dotnet build` clean, 0 warnings. That is the chapter — 54 lines.

---

## Act 7 — Run it, break it, hand off · 13:00–15:00

### 23. Finish the question from Act 0

**Say.** "Four files, fifty-four lines, and no new file. Let us go back and finish that
question."

**Type.**

```bash
cd /Users/lk/Projects/BaiGanio/ai-agents-1on1/chapters/05-the-loop/code/csharp
REPO=../../../../docs dotnet run
```

At the `You:` prompt:

```text
Which file explains how we write diagrams, and what does it say about them?
```

**Proof.** From a real run on this machine, 2.47 seconds including `dotnet run`, with the middle
of the second tool result cut:

```text
Reading /Users/lk/Projects/BaiGanio/ai-agents-1on1/docs
Type a question. An empty line stops the app.
You: Which file explains how we write diagrams, and what does it say about them?
gemma4:e2b:
[step 1] list_files .
GLOSSARY.md
MODELS.md
PLAN.md
PROGRESS.md
SETUP.md
STYLE.md

gemma4:e2b:
[step 2] read_file STYLE.md
# Style

How we write, so fourteen chapters read like one person wrote them. Borrowed from
[`agent-ready-payments`](https://github.com/BaiGanio/agent-ready-payments) and cut down.
...(cut)

gemma4:e2b: The file `STYLE.md` explains how to write content, including diagrams. It states
that for diagrams, you should use `flowchart TD` and never use a semicolon inside a diagram
line.
```

**Note.** Do not explain it. Point at the steps in order and let the viewer read, then say three
things.

**Nobody wrote "first list, then read".** There is no plan in that code, no chain, no workflow.
The loop went round twice because the model asked twice, and it stopped because the model
stopped asking.

**Point at the two `gemma4:e2b:` lines with nothing after them.** That is `content: ""` from
step 2, on screen. The model saying nothing, because it was asking.

**And it quotes the file.** In Act 0 it had six file names and had to guess. Now it has the text.

### 24. Two steps, and five

**Say.** "Same run. Two more questions, and watch the step numbers."

**Type.** At each `You:` prompt in turn:

```text
Find every line that contains the word quantisation.
Read every file in this folder and tell me what the whole repo is about.
```

**Proof.** Both from real runs on this machine. The first, 2 steps, 2.90 s:

```text
[step 1] search_text quantisation
GLOSSARY.md:35: | <a id="quantisation"></a>**quantisation** | making a model smaller by ...
PLAN.md:57: | 09 | `llama-cpp` | swap the runner. Flags, quantisation, context size. ...

gemma4:e2b: I found lines mentioning "quantisation" in `GLOSSARY.md` and `PLAN.md`.
```

The second, **5 steps, 13.30 s**:

```text
[step 1] list_files .
[step 2] read_file GLOSSARY.md
[step 3] read_file MODELS.md
[step 4] read_file PLAN.md
gemma4:e2b: The repository appears to be a collection of educational materials for a course
on building AI agents, specifically focusing on how an agent interacts with a local language
model.
```

**Note.** Put the two side by side and say the number that matters: **two steps, then five.
Your code is identical in both.**

You wrote `while`. You did not write "two" or "five". The model spent four turns because it
decided to. Ask it to read sixty files and it will try, and your code will let it, happily, for
as long as that takes.

Say the honest bit too: it read three files out of six and then answered. Nobody told it to stop
at three. Nobody could have.

### 25. The loop is persistent, not clever

**Say.** "One more. The question from Act 0 — the one chapter 04 could not finish."

**Type.**

```text
Which file sets the HTTP timeout?
```

**Proof.** From a real run on this machine, 3 steps, 2.82 s:

```text
[step 1] list_files .
[step 2] read_file STYLE.md
gemma4:e2b: The file `STYLE.md` discusses the writing style for the documentation, but it
does not specify the HTTP timeout. I do not see any other files that seem relevant to network
configurations.
```

**Note.** Sit with this one, because it is the most useful thirty seconds in the chapter.

**It finished the question.** Chapter 04 could not. So the loop worked.

**And it read the wrong file.** `STYLE.md` is about writing style. `search_text` was sitting
right there in its tool list and it did not use it.

Say the rule and say it flat: **a loop makes an agent persistent, not clever.** More turns do
not buy better judgement. A bad guess just gets more chances to be slow.

Then give it the credit it earned: *"it does not specify the HTTP timeout"* — **it did not
invent an answer.** That is chapter 02's *"never guess a file name"* still working, three
chapters later.

### 26. Break it

**Say.** "One break, and it is the one you will actually hit."

**Type.** Quit, then in `prompts/librarian.md` delete lines 7 and 8 — the repo-root line — and
run again:

```bash
REPO=../../../../docs dotnet run
```

```text
What files are in this folder?
```

**Proof.** From a real run on this machine, 1.47 s:

```text
You: What files are in this folder?
gemma4:e2b: Please tell me the folder you are referring to.
```

**Note.** Zero steps. Zero tool calls. Three messages in the list.

Point at the terminal and say it: the tool list was **perfect**. The schema was byte for byte
what it was a minute ago. The model had three tools and asked the human to go and look.

It did not know that `.` was a thing it was allowed to say. **One line of prose beat the entire
`tools` field.**

Put the line back on camera, run it again, watch `[step 1] list_files .` appear, and say the
rule one last time: the `tools` field says what it **may** do. The prompt says what it
**should** do. You are never finished with chapter 02.

### 27. The costs, and hand off

**Say.** "Three numbers before you go."

**Note.** All three from real runs on this machine. Say them flat, with nothing on screen.

**Two round trips is now the floor, not the number.** A listing is 2 steps. Reading three files
is 5 steps and 13.3 seconds. One question, one `while`, and you cannot tell in advance which
one you are getting.

**Every step re-sends everything.** A 3-step answer leaves **seven** messages in the list, and
one of them is the whole of `STYLE.md`. Every later step posts that file again. The list is the
memory and the list only grows — that is the next video.

**And nothing stops it.** No step cap, no timeout, no cost ceiling. `step` is counted and never
checked. On a local model that is your fan. On a paid endpoint it is your money.

**Say.** "So it is an agent now, and that took one `while`. It looks things up until it is done,
and it tells you when it cannot find something. Two problems left, and you just watched both.
It re-sends every file it has ever read, on every single step — that is memory, next video. And
it will do that forever if the model asks it to — that is guardrails, the video after. One of
those makes it cheaper. The other one makes it safe."

**Proof.** Stop recording.

---

## After the take

1. Run **P9** above to restore the code.
2. Check all four `.cs` files build clean with 0 warnings, and that no new file was created.
3. Check `prompts/librarian.md` is 12 lines and has the repo-root line back in it.
4. Check `prompts/pirate.md` is untouched.
5. Re-run the by-script check: every code block above must still be verbatim in the file it
   names, in the same order, and every line range in a step heading must be right (decision 38).
6. Write the real run numbers and the machine into [`docs/PROGRESS.md`](../../docs/PROGRESS.md).
7. Tick the boxes in the chapter [`README.md`](README.md) "Done when" list.
