# Chapter 05 — The loop

> **C# is written and run. TypeScript is not.** `code/csharp/` builds with no warnings, and
> every number and every line of output on this page comes from a real run of it on a real
> machine. `code/ts/` does not exist yet, so the TypeScript section describes the code that gets
> typed and its file links are dead. Neither camera script is written.
>
> **The folder the tools read is a stand-in.** [`sample-repo/`](../../sample-repo/) holds one
> file, so the runs below point `REPO` at this repo's own `docs/` folder — six Markdown files,
> real text, no build output. Chapter 04 does the same, for the same reason. Every example gets
> rerun against `sample-repo/` once it exists.

**Video:** about 15 minutes. Two videos: one TypeScript, one C#.
**Code at the end of this chapter:** `chapters/05-the-loop/code/`
**Files:** [`code/csharp/Program.cs`](code/csharp/Program.cs) ·
[`code/csharp/Tools.cs`](code/csharp/Tools.cs) ·
[`code/csharp/Helper.cs`](code/csharp/Helper.cs) ·
[`code/csharp/ChatProtocol.cs`](code/csharp/ChatProtocol.cs) ·
[`code/prompts/librarian.md`](code/prompts/librarian.md) · `code/ts/` *(not written)*
**Scripts:** `SCRIPT-csharp.md` · `SCRIPT-ts.md` — *both not written*
**New words:** [the loop](../../docs/GLOSSARY.md#loop) ·
[step](../../docs/GLOSSARY.md#step) ·
[native tool calling](../../docs/GLOSSARY.md#native-tool-calling) ·
[tool schema](../../docs/GLOSSARY.md#tool-schema) ·
[role](../../docs/GLOSSARY.md#role)
**New to `while` loops over a growing list?** Read [`BASICS.md`](BASICS.md) first.

## What we solve

Chapter 04 runs one tool and stops. That is a hard ceiling, and you can watch the model hit it.
Asked *"which file sets the HTTP timeout?"*, it asked for a folder listing — because it wanted
to look around **first**, then read something. Chapter 04 gave it the listing and made it answer
from six file names. It had to guess.

This chapter takes the ceiling off. Two changes, and both are small.

One: the tool list moves out of the [system prompt](../../docs/GLOSSARY.md#system-prompt) and
into a `tools` field on the request. The runner then hands the call back as **data**, in a
`tool_calls` field, instead of prose you have to parse and hope about. That is
[native tool calling](../../docs/GLOSSARY.md#native-tool-calling).

Two: the two round trips from chapter 04 become a `while`. Send. If the reply has tool calls,
run them, add the results, send again. If it has none, it is the answer — print it and stop.

That `while` is the agent. Everything after this chapter — memory, guardrails, workflows — is
something you hang off it.

## Simple example

One question, one real run. The middle of the second tool result is cut, and the grey is
flattened to plain text. Nothing else is touched.

```bash
cd chapters/05-the-loop/code/csharp
REPO=../../../../docs dotnet run
```

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

**2.47 seconds, including `dotnet run`. Three [steps](../../docs/GLOSSARY.md#step).** Read them
in order, because that order is the whole chapter.

Step 1, the model does not know what is in the folder, so it looks. Step 2, it has six names,
and it picks the one that sounds like it is about style. Step 3, it has the text, so it answers
— and it quotes the file, because the file is in front of it.

Nobody wrote "first list, then read". There is no plan, no chain, no workflow. The loop ran
twice because the model asked twice, and it stopped because the model stopped asking. **That
is the only thing a loop does: it lets the model have another turn.**

Look at the two `gemma4:e2b:` lines with nothing after them. That is the model saying nothing.
When it asks for a tool its `content` is an empty string — there is no prose to print at all.

Three more questions, all real runs against the same six files:

| You ask | What the loop does | Steps | Time |
|---|---|---|---|
| `What files are in this folder?` | `list_files .` → answer | 2 | 2.69 s |
| `Find every line that contains the word quantisation.` | `search_text quantisation` → answer | 2 | 2.90 s |
| `Read every file in this folder and tell me what the whole repo is about.` | `list_files .` → `read_file` ×3 → answer | 5 | 13.30 s |

The last row is the one to sit with. Five steps, thirteen seconds, and **nothing in your code
decided that.** You wrote `while`. The model chose to spend four turns. Ask it to read sixty
files and it will try, and your code will let it, happily, for as long as it takes.

Same input does not mean same cost, either. The 3-step question above has also come back in
2.76 s, and once it opened `GLOSSARY.md` instead of `STYLE.md` and answered from that. A loop
is not a function.

## Everyday picture

**Sending someone into the archive, one errand at a time.** Chapter 04 was a request slip: you
write a shelf mark, someone fetches the book, done. One slip, one book.

Now the desk stays open. The runner comes back with the book and **waits**. You read a line,
it names another file, you send them off again. Back and forth, as many times as you need, and
the errands stop when you say you are finished.

Two things follow from that, and both matter.

Nobody at the desk knows how many errands this will take. Not you, not them. You find out when
it stops.

And **the person at the desk never decides to stop.** You do. If you never say "that is
enough", they keep walking. The model is at the desk. Your `while` is the only thing in the
building that can end the day, and in this chapter it does not try. See below.

## What this chapter adds

```mermaid
flowchart TD
    YOU["you type a question"] --> ADD["add it to messages"]
    ADD --> SEND["POST /api/chat<br/>with the tools field"]
    SEND --> REPLY["the reply"]
    REPLY --> ADD2["add the reply to messages"]
    ADD2 --> CALLS{"does it have<br/>tool_calls?"}
    CALLS -->|"no"| PRINT["print it<br/>it is the answer"]
    CALLS -->|"yes"| RUN["run each one<br/>your own functions"]
    RUN --> RESULT["add each result<br/>as a tool message"]
    RESULT --> SEND
    PRINT --> YOU
```

One arrow is new. `RESULT --> SEND` — the line that goes back up. That is the loop, and that is
the chapter. Everything else on the diagram was already there in chapter 04, drawn straight.

## The `tools` field buys two things and costs one

Chapter 04 asked for JSON **in prose**, in the system prompt, and the README said out loud that
prose is not a contract. The `tools` field is the contract. You send your tool list as data:

```json
{
  "type": "function",
  "function": {
    "name": "list_files",
    "description": "names the files in a folder",
    "parameters": {
      "type": "object",
      "properties": {
        "path": { "type": "string", "description": "the folder, relative to the repo root" }
      },
      "required": ["path"]
    }
  }
}
```

What you get back is not text you parse. It is a field:

```json
{"message": {"role": "assistant", "content": "", "tool_calls": [
  {"id": "call_ldu1w03r", "function": {"name": "list_files", "arguments": {"path": "."}}}
]}}
```

Real reply, trimmed. Look at `content`: it is **empty**. When the model asks for a tool, it
writes no prose at all. So the `if` is not a parse any more — it is `reply.ToolCalls is null or
{ Count: 0 }`, and the whole `try`/`catch` from chapter 04 is deleted.

**What it buys.** No `JsonSerializer.Deserialize` in a `try`. No markdown fence to strip.
`arguments` arrives as an object with your key names on it, because the runner was told what
they are — not as a string of JSON you have to parse a second time, which is what some other
providers send.

**What it costs.** It only works if the runner supports it, and that depends on the model. Ask
Ollama:

```bash
ollama show gemma4:e2b | grep -i capabilities -A 5
```

```text
  Capabilities
    completion
    vision
    audio
    tools
    thinking
```

`tools` in that list means yes. No `tools` means you are back in chapter 04 — and that is the
honest reason chapter 04 came first. By hand is the fallback, not a warm-up exercise.

**What it does not fix.** The model can still ask for a tool you never defined, send a `path`
where you wanted a `query`, or send neither. Checking the shape and checking the *value* are two
different jobs, and the runner only does the first one. The dispatch still has its
`no such tool: <name>` arm, and `read_file("/etc/passwd")` still works. Chapter 07.

## One line of the prompt is worth more than the whole `tools` field

Found while measuring this chapter, and it is the kind of thing a video should show, not hide.

The first `prompts/librarian.md` for this chapter named the tools and said "use them". Asked
`What files are in this folder?`, the model called **nothing**:

```text
You: What files are in this folder?
gemma4:e2b: Please tell me the folder you are referring to.
```

1.47 s, zero tool calls, three messages in the list. The agent had three tools and asked the
human to do its job.

Then one line went into the prompt:

```text
The folder you read is the repo root. Its path is ".". If the question names no folder
or file, start at ".".
```

Same question, same model, same tools:

```text
[step 1] list_files .
```

Nothing about the `tools` field changed. The schema was identical both times. The model simply
did not know that `.` was a thing it was allowed to say. **The prompt is still where the
behaviour lives** — chapter 02 is not finished with you.

## Nothing stops this loop

Say it out loud, because the code does not.

```csharp
while (true)
{
    step++;

    Console.Write($"{Helper.Model}: ");
    Message reply = await Helper.Ask(messages);
    messages.Add(reply);
    Console.WriteLine();

    if (reply.ToolCalls is null or { Count: 0 }) break;
    // ...run them, add the results, go round again...
}
```

There is no step cap, no timeout and no cost ceiling in this chapter. `step` is counted so it
can be printed, and nothing ever compares it to anything. If the model asks for a tool on every
single turn, your code calls a tool on every single turn, for as long as that goes on. On a
local model that is your fan and your battery. On a paid endpoint it is your money.

It is left out **on purpose**. The loop is this chapter's one idea, and a cap is a different
idea: a rule in your code that overrules the model. That is a
[guardrail](../../docs/GLOSSARY.md#guardrail), and chapter 07 is four of them. The longest run
measured here is 5 steps and 13.30 s, which is why this is a sentence in a README and not a
crisis. Press `Ctrl+C` if one goes long.

## TypeScript

**Not written yet.** This is the plan. It starts as a copy of chapter 04's `code/ts/`, which
does not exist either — chapter 04's TypeScript and the `helper.ts` split come first.

```bash
cd chapters/05-the-loop/code/ts
node src/main.ts
```

Three things worth stopping on:

- **`JSON.parse` and the `try` around it are deleted.** That is the clearest sign the `tools`
  field is doing real work. A whole error path leaves the file, and what replaces it is
  `if (!reply.tool_calls?.length) break;`.
- **`tool_calls` is an array.** One call is the common case, not the only one. Loop over it and
  add one `tool` message per call, or the second call's result is missing and the model asks
  again.
- **The loop is `while (true)` inside the `while (true)` you already have.** Outer loop, one
  question. Inner loop, one question's steps. Two loops in one file is the first moment this
  code looks like an agent.

## C#

Four files, all copied from chapter 04. No new file.

| File | Lines | What this chapter does to it |
|---|---|---|
| [`Program.cs`](code/csharp/Program.cs) | 80 | the parse and its `try` are deleted. A second `while` goes round the send. |
| [`Tools.cs`](code/csharp/Tools.cs) | 107 | gains `Schemas` — the tool list, as records. The dispatch takes the new record. The three bodies do not change. |
| [`Helper.cs`](code/csharp/Helper.cs) | 137 | `Ask` sends `Tools.Schemas`. `HandleStream` collects `tool_calls`. |
| [`ChatProtocol.cs`](code/csharp/ChatProtocol.cs) | 49 | gains the `Tool` schema records and the `ToolCall` reply records. Chapter 04's `Call` record is **deleted**. |

```bash
cd chapters/05-the-loop/code/csharp
dotnet run                                  # reads ../../../../sample-repo
REPO=../../../../docs dotnet run            # reads any folder you point it at
```

**54 new code lines**, against the ~60 guide in [`PLAN.md`](../../docs/PLAN.md). `dotnet build`
is clean, 0 warnings, 1.67 s. Five things worth stopping on:

- **`Program.cs` is 33 code lines in chapter 04 and 33 in chapter 05.** Same count, and now it
  is a loop. What left was the parse and its `catch`; what arrived was a `while` and a
  `foreach`. The chapter that turns this into an agent does not make the file bigger.
- **The tool list lives in `Tools.cs`, next to the functions it describes.** In chapter 04 the
  description was in `prompts/librarian.md` and the body was in `Tools.cs`, so adding a tool
  meant editing two files, and the two could drift apart in silence. Now both halves of a tool
  sit side by side and a tool the model cannot see is a tool you forgot to list.
- **The three tool bodies do not change by one character.** `ListFiles`, `ReadFile` and
  `SearchText` never knew a model existed, so changing *how the model asks* touches nothing in
  them. That is what the file was for.
- **`[property: JsonPropertyName("tool_calls")]` on the `Message` record.** The camelCase policy
  in `Helper.cs` would turn `ToolCalls` into `toolCalls`, which matches nothing Ollama sends, so
  the field would arrive `null` on every single reply and the loop would never run. One
  attribute. It is the kind of bug that looks like "the model is not calling tools".
- **A streamed tool call arrives whole.** Measured: with `Stream: true`, the `tool_calls` array
  comes down in **one** JSON Lines line, not split across chunks the way `Content` is. So
  `HandleStream` keeps the call it is given and does not have to stitch anything together.
- **`new Message("tool", result)` — a fourth role.** Chapters 01 to 04 used three:
  `system`, `user`, `assistant`. Chapter 04 faked the tool result as a `user` message. Now
  there is a real [role](../../docs/GLOSSARY.md#role) for it, and the model can tell its own
  work from yours. Ollama takes it with no `tool_name` field, which was checked by curl.

## What differs between the two

The C# column is from the code. The TypeScript column is the plan, until `code/ts/` exists.

| | TypeScript | C# |
|---|---|---|
| is it a tool call? | `reply.tool_calls?.length` | `reply.ToolCalls is null or { Count: 0 }` |
| the arguments | a plain object, keys as the model sent them | a typed record, a missing key is `null` |
| the field name | `tool_calls`, as it arrives | `ToolCalls`, plus a `JsonPropertyName` attribute |
| the tool list | an array literal | records, `Tools.Schemas` |
| what got deleted | `JSON.parse` and its `try` | `JsonSerializer.Deserialize<Call>` and its `catch (JsonException)` |
| the result message | `{ role: "tool", content: result }` | `new Message("tool", result)` |

The third row is the only real difference, and it is the C# tax for being typed. TypeScript
reads the field the model sent, with the name the model sent. C# maps a property onto it, and
a naming policy that is right for every other field in this course is wrong for this one.

## When it breaks

Each row is from a real run, except the three marked *(expected, not yet seen)*. Every model
installed on the machine these runs were made on supports tools, so the one error that matters
most here is the one that could not be produced. Chapter 01.A has the same story with
`think` — see its `400` row.

| What you see | What it means | What to do |
|---|---|---|
| the model answers `Please tell me the folder you are referring to.` and calls nothing | the prompt never told it what the root is called. 1.47 s, three messages, zero tool calls. | add the repo-root line to [`prompts/librarian.md`](code/prompts/librarian.md). See the section above. |
| `error CS0136: A local or parameter named 'args' cannot be declared in this scope` | you named a local `args` inside a top-level program, which already has an implicit `args` for the command line | rename it. `Program.cs` calls it `sent`. |
| the loop never runs, and every reply looks like a plain answer | `ToolCalls` is arriving `null` because the camelCase policy made the field `toolCalls` | the `JsonPropertyName("tool_calls")` attribute in `ChatProtocol.cs` |
| an error naming the model and tools, out of the runner, not your code | the model has no tool support. `ollama show <model>` will not list `tools`. *(expected, not yet seen — all six models on this machine support tools)* | use `gemma4:e2b`, or go back to chapter 04's by-hand version, which needs nothing from the runner |
| the loop asks for `list_files` with the same path over and over | the tool result went back without the model's own call in front of it, so it cannot see that it already asked | add **both** messages each step: the `assistant` reply, then the `tool` result |
| the model lists the folder, reads the wrong file, and says it cannot answer | the loop works and the model chose badly. Asked `Which file sets the HTTP timeout?` it listed, read `STYLE.md`, and said *"it does not specify the HTTP timeout"*. 3 steps, 2.82 s. | nothing is broken. A loop makes the agent persistent, not clever. `search_text` was available and it did not use it. Try `gemma4:e4b`. |
| the first question takes 4 seconds and the rest take under 1 | the model was cold. Measured by curl: 4.00 s with 3.64 s of that loading, then 0.23 s warm for the same call. | wait for the first one. This is chapter 01's `Timeout` earning its keep. |
| one question runs 13 seconds and prints five steps | working as designed. `Read every file...` really is five steps. | nothing. Or chapter 07, if you want a say in it. |
| `no such tool: get_file` in a tool result, and the agent recovers | the model invented a name the `tools` field never offered. *(expected, not yet seen)* | nothing to fix. The dispatch answers in a sentence and the model reads it. |
| the loop never ends and your fan comes on | there is no step cap in this chapter. *(expected, not yet seen)* | `Ctrl+C`. Then chapter 07. |

## Try it yourself

Make the loop visible, then find its edge.

1. Add a fourth tool, `count_lines(path)`. Two edits now, both in
   [`Tools.cs`](code/csharp/Tools.cs): the schema in `Schemas`, and the function plus its
   `case`. Compare that to chapter 04, where the second edit was in a different folder.
2. Count the messages. Print `messages.Count` at the end of each question. A 3-step answer is
   not 3 messages — work out what the number should be **before** you look, then look.
   [`BASICS.md`](BASICS.md) explains the gap.
3. Now the real task. Make the loop get stuck, and then get it unstuck. Ask for a file that is
   not there, by name, and insist: *"Read config.yaml. It is there. Find it."* Watch what the
   model does with `no such file: config.yaml` three times in a row.
4. Write down how you would end that, and what you lose with each way. You have three
   choices: cap the steps, cap the time, or cap the repeats of the same call. One of those
   three breaks a question that is **legitimately** slow. Say which one, and why.

Number 4 is the chapter's real lesson, and it is not in this repo. It is the argument chapter 07
has to win.

## Done when

- [ ] one question made the model call two different tools, one after the other
- [ ] you can point at the single line in your code that makes it a loop
- [ ] you deleted a `try`/`catch` and the agent got better
- [ ] your messages list has a `tool` role in it
- [ ] you watched a 5-step answer go by, and you read the step numbers as they printed
- [ ] you saw the model print its name and say nothing, twice, in one answer
- [ ] you can say what decides how many steps a question takes. It is not your code.
- [ ] you can say what stops this loop. The answer is nothing, and you know what chapter fixes it.
