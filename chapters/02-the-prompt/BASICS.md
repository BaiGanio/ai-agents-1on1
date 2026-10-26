# Chapter 02 — basics: text, files and stdin

Three things this chapter needs. Nothing else.

New words are in [`GLOSSARY.md`](../../docs/GLOSSARY.md).

---

## 1. A prompt is only a string

There is no prompt type. No class, no builder, no template engine. A system prompt is a
string, the same kind of string as `"hello"`. The model reads text, so everything you give it
ends up as text.

A prompt runs over many lines, so you want a string that can hold a line break.

**TypeScript** — backticks:

```ts
const prompt = `You are the Repo Librarian.

Never guess a file name.`;
```

**C#** — a raw string, three or more double quotes:

```csharp
var prompt = """
    You are the Repo Librarian.

    Never guess a file name.
    """;
```

The closing `"""` sets the left edge. C# strips that much indentation off every line, so the
string has no leading spaces even though the code does.

**We use neither.** Both of those put the prompt inside the code, and then changing the agent
means editing and recompiling. We read a file instead. The string forms are here because you
will see them everywhere else, and because you need to know a prompt is nothing special.

---

## 2. Reading a text file

**TypeScript:**

```ts
import { readFile } from "node:fs/promises";
const text = await readFile("../prompts/librarian.md", "utf8");
```

Without `"utf8"` you get raw bytes, not a string. Always pass it.

**C#:**

```csharp
var text = await File.ReadAllTextAsync("../prompts/librarian.md");
```

### The path is relative to where you ran the program

This trips up everyone once. `"../prompts/librarian.md"` does not mean "next to the source
file". It means "one folder up from the **working directory**" — the folder your shell was in
when you started the program.

So this works:

```bash
cd chapters/02-the-prompt/code/ts
node src/main.ts
```

And this does not, even though it runs the same file:

```bash
node chapters/02-the-prompt/code/ts/src/main.ts     # working directory is the repo root
```

In C#, `dotnet run` sets the working directory to the project folder for you. Running the
built binary out of `bin/Debug/net10.0/` does not, and the path breaks.

---

## 3. Reading lines from the terminal

**stdin** is the stream of text going *into* your program. When you type in a terminal, you
are writing to stdin. Reading a line means waiting until the user presses Enter.

**TypeScript:**

```ts
import * as readline from "node:readline/promises";
const rl = readline.createInterface({ input: process.stdin, output: process.stdout });

for await (const line of rl) {
  console.log("you said: " + line);
}
```

**C#:**

```csharp
while (Console.ReadLine() is { } line)
{
    Console.WriteLine("you said: " + line);
}
```

`is { } line` means "is not null, and call it `line`". It is a pattern, not a comparison.

### How a loop like this ends

Not with a magic word. With **end of input**, which you send by pressing **Ctrl-D** on macOS
and Linux, or **Ctrl-Z then Enter** on Windows.

| | What happens at end of input |
|---|---|
| TypeScript | the `for await` finishes and the code after it runs |
| C# | `Console.ReadLine()` returns `null`, so the `while` stops |

Ctrl-C is different. It kills the program at once, and nothing after the loop runs. Both work
for quitting this chapter's code, because there is nothing after the loop yet.
