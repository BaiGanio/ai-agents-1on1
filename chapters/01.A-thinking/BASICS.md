# Chapter 01.A — basics: three states, and streams

Two things this chapter needs. Nothing else.

New words are in [`GLOSSARY.md`](../../docs/GLOSSARY.md).

---

## 1. A value that can be "not set"

Most switches have two states: on and off. The thinking switch in this chapter has **three**:
on, off, and **not asked for at all**. The third one is not a tidy-up. It is the only safe
setting for a model you have not tested, because a model that cannot think answers "think: true"
with an error instead of an answer.

So the code needs a third state, and both languages have one:

```csharp
bool? think = true;      // C#. true, false, or null. The ? is what buys you null.
```

```ts
let think: boolean | undefined = true;   // TypeScript. true, false, or undefined.
```

A plain `bool` cannot hold "not set", so the question mark is the point, not decoration.

The other half is making "not set" mean **nothing on the wire**. If you send the field as
`null`, the server reads that as "no thinking", which is a different answer from "I did not
ask". C# needs one option for that, `DefaultIgnoreCondition = WhenWritingNull`. TypeScript
gets it free: `JSON.stringify` drops a key whose value is `undefined`.

### Environment variables, from chapter 01

You met these already. A setting you hand a program when you start it, written in front of the
command:

```bash
MODEL=gemma4:e4b dotnet run
```

That runs with one variable, `MODEL`, set to `gemma4:e4b`. It lives for that one run and then
it is gone. Nothing is saved, and no file changed. The thinking switch is **not** one of these
— it is a line in the code you flip, because you choose it once per project, not once per run.

---

## 2. A stream

Chapter 01 asked for the answer and waited. The server built the whole reply, then sent it in
one piece. That is the normal way, and it is why you stared at nothing for six seconds.

A **stream** sends the reply in pieces, as it is made. You get the first word while the model
is still working on the tenth.

Ollama streams as **one JSON object per line**. Not one big JSON document — many small ones,
separated by `\n`. This is called JSON Lines. You cannot parse the whole body at once. You
parse each line on its own, as it arrives.

### Reading it in TypeScript

`fetch` gives you `response.body`, which you can walk with `for await`:

```ts
for await (const chunk of response.body!.pipeThrough(new TextDecoderStream())) {
  // chunk is a piece of text. It is NOT a line.
}
```

Read that comment again. A chunk is however many bytes arrived from the network. It may hold
three lines. It may hold half a line. So you keep the unfinished tail and glue the next chunk
to it:

```ts
let buffer = "";
buffer += chunk;
const lines = buffer.split("\n");
buffer = lines.pop()!;          // the last piece may be half a line. Keep it for next time.
for (const line of lines) { /* now these are whole lines */ }
```

### Reading it in C#

`StreamReader.ReadLineAsync` does the gluing for you. It returns when it has a whole line:

```csharp
using var reader = new StreamReader(await response.Content.ReadAsStreamAsync());
while (await reader.ReadLineAsync() is { } line)
{
    // line is a whole line. Always.
}
```

One catch. By default `HttpClient` downloads the entire body before it hands you anything, so
your stream arrives all at once at the end. You turn that off with one flag:

```csharp
await http.SendAsync(message, HttpCompletionOption.ResponseHeadersRead);
```

---

## 3. Grey text in a terminal

A terminal colours text when you print a special code. Two are used in this chapter:

| Code | What it does |
|---|---|
| `\x1b[90m` | everything after this is grey |
| `\x1b[0m` | back to normal |

They print as nothing. They are instructions to the terminal, not letters. If you forget the
second one, your whole terminal stays grey after the program ends.

We use grey for the thinking so your eye can skip it. The answer is the plain text.
