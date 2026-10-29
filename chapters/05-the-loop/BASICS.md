# Chapter 05 — basics: a loop over a list that grows

Three things this chapter needs. Nothing else.

New words are in [`GLOSSARY.md`](../../docs/GLOSSARY.md).

---

## 1. A loop you cannot count in advance

Most loops you have written know how long they are before they start.

```ts
for (const file of files) { ... }        // as many times as there are files
for (let i = 0; i < 10; i++) { ... }     // ten times
```

This chapter's loop is the other kind. You do not know how many times it runs. You find out by
running it.

```ts
while (true) {
  const reply = await ask(messages);
  if (noToolCalls(reply)) break;         // the only way out
  // ...run the tools, add the results...
}
```

The exit is not a count. It is **something that comes back from the thing inside the loop**.
You go round again because of what the last trip told you.

**C# writes the same shape:**

```csharp
while (true)
{
    Message reply = await Helper.Ask(messages);
    if (reply.ToolCalls is null or { Count: 0 }) break;
    // ...run the tools, add the results...
}
```

Two things to be clear about, because they bite.

**The `break` must be reachable.** If every reply has tool calls, you never break. Nothing in
`while (true)` promises to end. A `for (int step = 0; step < 10; step++)` promises to end, and
that one word — promises — is the whole of chapter 07.

**You already have one of these.** Chapters 01 to 04 have `while (true)` around
`Console.ReadLine()`, and it ends when you type an empty line. This chapter puts a second
`while` **inside** it. Outer loop: one question. Inner loop: one question's steps.

```text
while (true)            one question from you
    while (true)        one step of the model's work
```

---

## 2. Adding to a list while you walk it

The loop sends `messages` every time round. And every time round, it also **adds** to
`messages`. So the thing you send grows while you are sending it.

That is fine here, and it is worth seeing why. You are not iterating over the list — you are
not inside a `foreach (var m in messages)`. You send the whole list, get a reply, and append.
The list is the loop's memory, and the `while` is what makes it grow.

Count them. One question that took three steps ended with **seven messages**, in this order:

| # | role | what it is |
|---|---|---|
| 1 | `system` | the prompt. Sent every time, never changes. |
| 2 | `user` | your question |
| 3 | `assistant` | "run `list_files` with `path: .`" — `content` is empty |
| 4 | `tool` | the six file names your code found |
| 5 | `assistant` | "run `read_file` with `path: STYLE.md`" |
| 6 | `tool` | the text of `STYLE.md` |
| 7 | `assistant` | the answer, in words |

Real run, real count. The pattern is two messages per tool step, plus the answer, plus the two
you started with.

**Why both of each pair.** Row 4 on its own is six file names with no question attached. The
model needs row 3 in front of it to know why it is looking at them. Drop the `assistant`
message and the model asks for the same listing again — that is a row in the chapter's
"When it breaks", and it is the most common way to break this loop.

**Why this gets expensive.** Row 6 is a whole file. Every later send carries it again, because
the model remembers nothing and the list is the memory. Three big files and you are posting
them on every turn. Chapter 06 is about what you keep and what you drop.

---

## 3. An array of calls, not one call

`tool_calls` is a **list**, even when it has one thing in it.

```json
"tool_calls": [
  {"id": "call_ldu1w03r", "function": {"name": "list_files", "arguments": {"path": "."}}}
]
```

One call is what you will see nearly every time with a small model. The field is still an array,
because a model is allowed to ask for two tools at once.

So loop over it, and add **one `tool` message per call**:

```ts
for (const call of reply.tool_calls ?? []) {
  const result = run(call.function.name, call.function.arguments);
  messages.push({ role: "tool", content: result });
}
```

```csharp
foreach (ToolCall call in reply.ToolCalls)
{
    string result = Tools.Run(call);
    messages.Add(new("tool", result));
}
```

No `?? []` is needed in the C#, because the `break` above it already ran if the list was null
or empty. That is the shape in [`Program.cs`](code/csharp/Program.cs) — read the check and the
loop together.

If you write `reply.tool_calls[0]` and ignore the rest, the day a model asks for two tools your
agent answers half the question and you will not know why.

**`arguments` is an object, not a string.** Some other providers send the arguments as a string
of JSON that you have to parse a second time. Ollama does not — the keys are already there,
with the names you put in your [tool schema](../../docs/GLOSSARY.md#tool-schema). Checked on a
real reply.

In C# that means they arrive on a typed record, `ToolArguments(string? Path, string? Query)`,
and every field is nullable for the same reason as chapter 04: `list_files` sends a path and no
query, and `search_text` sends the other one.

---

## Check yourself

Before the video:

- [ ] you can write a `while` that ends because of something the body returned, not a count
- [ ] you can say how many messages a 2-step answer leaves in the list *(the answer is 5, and
      it was measured)*
- [ ] you can say why the `assistant` message has to go in the list before the `tool` message
- [ ] you can say what happens if a model asks for two tools and you only run the first
