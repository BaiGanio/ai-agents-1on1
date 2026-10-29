# Chapter 05 — questions

Answer these out loud before the next video. If one is hard, rewatch that part.

**Q1.** What makes this an agent, when chapter 04 was not one?

<details><summary>A</summary>

One `while`. That is the honest answer and it is worth being annoyed about.

Chapter 04 sends twice: once to get the tool call, once to give the result back. Two round
trips, hard-coded, then it stops. Chapter 05 puts a loop around that and lets the model have as
many turns as it asks for. Nothing else changed about what the model can do.

The [glossary](../../docs/GLOSSARY.md#agent) says an agent is a model, a prompt, some tools and
a loop. Chapters 01, 02 and 04 built the first three. This one is the fourth, and it is twelve
lines.

</details>

**Q2.** Who decides how many steps a question takes?

<details><summary>A</summary>

The model. Not you.

`What files are in this folder?` took 2 steps and 2.69 s. `Read every file in this folder and
tell me what the whole repo is about.` took 5 steps and 13.30 s. Your code is identical in both.
You wrote `while` — you did not write "two" or "five".

This is the thing that makes an agent feel different from a program, and it is also the thing
that makes it hard to budget, hard to test and hard to bill for. Hold on to that feeling until
chapter 07.

</details>

**Q3.** What does the `tools` field actually save you?

<details><summary>A</summary>

A parse, and the error path around it.

Chapter 04 asked for JSON in prose, so the reply was text that you had to `JSON.parse` inside a
`try`, with a `catch` for "that was prose, not a call" and a fence to strip when the model got
helpful. Chapter 05 sends the tool list as data, and the call comes back in a `tool_calls`
field. The `if` becomes `reply.ToolCalls is null or { Count: 0 }`. The `try` is deleted, and
`Program.cs` is no bigger than it was: 33 code lines in both chapters.

What it does **not** save you: checking that the values make sense. The runner guarantees the
shape, never the content. A `path` of `../../../.ssh/id_rsa` is a perfectly valid string.

</details>

**Q4.** The model asked for a tool. What did it actually do?

<details><summary>A</summary>

The same thing as chapter 04. It wrote something. The runner put that something in a field
instead of leaving it in `content`, which is nicer for you and changes nothing for the model.

It did not open a folder, run a function, or touch your disk. **The model runs nothing.** If
you delete your dispatch, the model still asks, and nothing happens.

The field is a tidier envelope. There is no new power in it.

</details>

**Q5.** Why is `content` empty when the model asks for a tool?

<details><summary>A</summary>

Because it is asking, not talking. Real reply, trimmed:

```json
{"message": {"role": "assistant", "content": "", "tool_calls": [...]}}
```

That empty string is useful: it is why your `if` can look at `tool_calls` alone and never has
to guess whether some prose was an answer or a preamble. In chapter 04 the JSON *was* the
content, and telling an answer from a call meant trying to parse it.

</details>

**Q6.** A 3-step answer leaves how many messages in the list, and why that many?

<details><summary>A</summary>

**Seven.** Measured: `system`, `user`, `assistant`, `tool`, `assistant`, `tool`, `assistant`.

Two to start — the prompt and your question. Then two per tool step, because the model's own
request has to stay in front of its result. Then one for the answer.

The pair is the part people get wrong. Add only the `tool` message and the model sees six file
names with nothing explaining why, so it asks for the listing again, and again.

</details>

**Q7.** You gave the agent a `tools` field and it still asked *you* which folder to look in.
Whose fault is that?

<details><summary>A</summary>

The prompt's. This really happened while the chapter was measured.

The tool list was correct. The schema was correct. The model answered `Please tell me the
folder you are referring to.` in 1.47 s and called nothing, because no one had told it that the
root is called `.`. Three messages in the list, zero tool calls. One line in
`prompts/librarian.md` fixed it, and the next run called `list_files .`.

The lesson: the `tools` field tells the model what it **may** do. The prompt tells it what it
**should** do. A better field never replaces chapter 02.

</details>

**Q8.** What stops this loop?

<details><summary>A</summary>

Nothing in your code. Only the model choosing to stop asking.

There is no step cap, no timeout and no cost ceiling in this chapter, on purpose — the loop is
the one idea, and a cap is a different idea. If every reply asks for a tool, your code runs a
tool every time, for as long as that goes on.

`Ctrl+C` is the guardrail in chapter 05. Chapter 07 writes four real ones.

</details>

**Q9.** `tool_calls` has one item in it every time you run it. Why is it an array?

<details><summary>A</summary>

Because a model is allowed to ask for two tools in one turn, and one day yours will.

Write `for (const call of reply.tool_calls)` and add one `tool` message per call. Write
`reply.tool_calls[0]` and your agent will one day answer half a question, with no error
anywhere, and you will spend an afternoon on it.

</details>

**Q10.** Chapter 04 is by hand and chapter 05 has a field for it. Was chapter 04 a waste?

<details><summary>A</summary>

No, and it is not only for the lesson. It is the fallback.

The `tools` field only works if the runner and the model support it. `ollama show <model>` lists
`tools` under Capabilities, or it does not. When it does not, you are back to describing tools
in the prompt and parsing text — which is chapter 04 exactly, and it needs nothing from anyone.

That is why the by-hand chapter comes first. Not as a warm-up. As the version that always works.

</details>

**Q11.** The loop read the wrong file and answered confidently from it. Is the loop broken?

<details><summary>A</summary>

No. Asked `Which file sets the HTTP timeout?` it listed the folder, read `STYLE.md` — a file
about writing style — and then said *"it does not specify the HTTP timeout"*. 3 steps, 2.82 s,
all working as built.

A loop makes an agent **persistent**, not clever. It gets more turns to look things up. It does
not get better judgement about which file to open. More steps make a bad guess slower, not
righter.

What helps: a bigger model (`gemma4:e4b`), or a better prompt, or a tool that fits the question
— `search_text` was right there and it did not use it. Give it credit for one thing, though:
it did not invent an answer. Chapter 04, asked the same question, had six file names and had to
guess from them.

</details>

**Q12.** The three tool bodies did not change by one character. Why not?

<details><summary>A</summary>

Because none of them ever knew a model existed. `ListFiles`, `ReadFile` and `SearchText` take a
string and return a string. The whole change in this chapter is *how the model asks*, which is
wire format, so it lands in `ChatProtocol.cs`, `Helper.cs` and `Program.cs`.

`Tools.cs` as a file does change, twice. The dispatch takes the new record. And the tool list —
`Schemas` — moves **into** it, out of `prompts/librarian.md`.

That second one is worth a minute. In chapter 04 a tool was described in one folder and
implemented in another, so adding a fourth meant two edits that nothing checked against each
other. Now both halves sit in one file. A tool the model cannot see is a tool you forgot to
list, ten lines above the function.

</details>

**Q13.** Your agent compiles, runs, and never calls a tool. Where do you look first?

<details><summary>A</summary>

At the field name. In C# this is the bug you will actually hit.

`Helper.cs` sets `PropertyNamingPolicy = JsonNamingPolicy.CamelCase`, which is right for every
other field in this course. It turns `ToolCalls` into `toolCalls`. Ollama sends `tool_calls`.
Nothing matches, so the property is `null` on every reply, every reply looks like a plain
answer, and the loop never goes round once.

The fix is `[property: JsonPropertyName("tool_calls")]` in `ChatProtocol.cs`. No error, no
warning, no exception — which is exactly why it is worth knowing about before it happens.

</details>
