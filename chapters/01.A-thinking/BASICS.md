# Chapter 01.A — basics: streams and environment variables

Two things this chapter needs. Nothing else.

New words are in [`GLOSSARY.md`](../../docs/GLOSSARY.md).

---

## 1. An environment variable

A setting you hand a program when you start it, from outside the program. You write it in
front of the command:

```bash
THINK=off node src/main.ts "hello"
```

That runs `node` with one variable, `THINK`, set to `off`. The variable lives for that one
run and then it is gone. Nothing is saved, and no file changed.

You read it inside the program:

```ts
const think = process.env.THINK;          // TypeScript. A string, or undefined.
```

```csharp
var think = Environment.GetEnvironmentVariable("THINK");   // C#. A string, or null.
```

Three states, not two. `"on"`, `"off"`, and **not set at all**. The third one matters in this
chapter, so we keep it instead of turning it into `false`.

Why not a command-line argument? Because the question is already the argument. A setting that
is not part of the question stays out of the question.

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
