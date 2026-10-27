# Chapter 04 — questions

Answer these out loud before the next video. If one is hard, rewatch that part.

**Q1.** The model "called a tool". What did the model actually do?

<details><summary>A</summary>

It wrote text. 35 characters of it: `{"tool": "list_files", "path": "."}`. That is all. It did
not open a folder, run a function, or touch your disk. It cannot. Your code read that text,
decided to believe it, and called a function you wrote. **The model runs nothing.** If you
delete the `switch` in your code, the model still happily writes the same JSON and nothing
happens.

</details>

**Q2.** Where is the list of tools the model knows about?

<details><summary>A</summary>

In [`prompts/librarian.md`](../02-the-prompt/code/prompts/librarian.md) — a text file. In
English. There is no registry, no plugin folder, no `@tool` attribute. You describe the three
tools and the JSON shape in the [system prompt](../../docs/GLOSSARY.md#system-prompt), and that
is the whole interface. Chapter 05 moves it to a `tools` field, which is the same list in a
shape the runner checks for you.

</details>

**Q3.** Your parse threw a `SyntaxError`. Is something broken?

<details><summary>A</summary>

Almost certainly not. In this chapter a parse failure means the model wrote prose instead of
JSON, and the usual reason is that **no tool was needed** — you said hello. So the `catch` is
the normal path for a plain answer: print the text and carry on. The error to worry about is a
different one, which is why you catch `JsonException` in C# and not `Exception`.

</details>

**Q4.** How many calls to the model does one tool use cost?

<details><summary>A</summary>

Two. One [round trip](../../docs/GLOSSARY.md#round-trip) to get the JSON, then your function
runs, then a second round trip carrying the result so the model can answer in words. That is
why a tool-using agent feels slower than a chat: you are waiting for the model twice, plus your
own code in the middle. Chapter 05's loop can make it many more than two.

</details>

**Q5.** You sent the tool result back and the model ignored it. What did you forget?

<details><summary>A</summary>

The model's own message. The list has to carry both: the `assistant` message that asked for the
tool, then the result after it. If you drop the assistant message, the model is reading a
folder listing that nobody asked for, with no idea why it is there. Append both, in order,
every time — the same rule as the system prompt in chapter 02.

</details>

**Q6.** A tool cannot do its job — the file is not there. Throw, or return a string?

<details><summary>A</summary>

Return a string, like `no such file: nope.ts`. A throw kills the run and the model never learns
anything. A string goes back as a [tool result](../../docs/GLOSSARY.md#tool-result), the model
reads it, and it can try a different path or tell the user. **Your tools talk to a reader, not
to a debugger.** Make the sentence say what went wrong.

</details>

**Q7.** `read_file("/etc/passwd")` in this chapter's code. What happens?

<details><summary>A</summary>

It reads it, and sends the contents to the model. Nothing in this chapter stops it. The tool
takes whatever path arrives and hands it to the filesystem, and the model is a program that
writes plausible text — including plausible paths. That is the honest state of the code after
this chapter. [Guardrails](../../docs/GLOSSARY.md#guardrail) — a path allow-list, a size cap,
asking before a write — are chapter 07, and this question is why that chapter exists.

</details>

**Q8.** Asked *"Which file sets the HTTP timeout?"*, the model replied
`{"tool": "list_files", "path": "."}` instead of searching. Was that wrong?

<details><summary>A</summary>

No, it was sensible, and nobody taught it to do that. It cannot search for a file name it does
not know, so it asked to look around first. That is one step of a plan. But this chapter runs
exactly one tool and then stops, so the model gets a listing and has to answer from it. Chapter
05 adds the loop that lets it take the second step.

</details>

**Q9.** Chapter 05 uses Ollama's native `tools` field. What does it buy, and what does it not?

<details><summary>A</summary>

It buys the shape: the runner is told what the tools are and hands you a structured tool call,
so the model wrapping JSON in a markdown fence stops being your problem. It does **not** buy
correctness. The model can still pick the wrong tool, invent a path, forget an argument, or ask
for a file that is not there. Every one of those is still yours to handle, with the same code
you wrote by hand here. That is why this chapter comes first.

</details>
