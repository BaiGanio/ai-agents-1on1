# Chapter 01.A — questions

Answer these out loud before the next video. If one is hard, rewatch that part.

**Q1.** What is the model doing during the six seconds before a five-word answer appears?

<details><summary>A</summary>

Writing notes to itself — thinking. For "Say hi in two words." on this machine that was 328
lines of notes for 2 lines of answer. The notes cost tokens and time. Chapter 01 waited for
all of them and threw them away, because it printed only `content`.

</details>

**Q2.** Why is the thinking switch a `bool?` and not a `bool`?

<details><summary>A</summary>

Because there are three states, not two: `true`, `false`, and **not sent at all**. Not sent
means the model does what it ships with, and that is the only safe setting for a model you have
not tested — a model that cannot think answers `think: true` with a 400 error, not with an
answer. A plain `bool` cannot hold the third state, so it would force you to pick one of the
two for every model, and one of the two breaks on some of them.

The second half is `DefaultIgnoreCondition = WhenWritingNull`. Without it, `null` goes on the
wire as `"think": null`, which Ollama reads as "no thinking" — a different thing from "I did
not ask". Measured on this machine: no field gives 714 characters of thinking, `"think": null`
gives none.

</details>

**Q3.** `stream: true` changes the shape of the reply. How?

<details><summary>A</summary>

`stream: false` gives one JSON object at the end. `stream: true` gives one JSON object **per
line**, one per token. A thinking line has a `thinking` field. An answer line has `content`.
The last line has `done: true` and the counts, like `eval_count`.

</details>

**Q4.** In TypeScript, what breaks if you delete the `buffer = lines.pop()` line?

<details><summary>A</summary>

`JSON.parse` throws `Unexpected end of JSON input`, sometimes. A network chunk can stop in the
middle of a line. Without the buffer you try to parse half a line. It works most of the time,
which is worse than never working — the bug shows up on a slow network and not on yours.

</details>

**Q5.** In C#, what breaks if you forget `HttpCompletionOption.ResponseHeadersRead`?

<details><summary>A</summary>

Nothing crashes. That is the trap. `HttpClient` downloads the whole body first, so your
"stream" prints everything in one burst at the end. The code is right, the output is right,
and the feature is gone.

</details>

**Q6.** Is thinking worth it?

<details><summary>A</summary>

It depends on your model and your question, and the only honest answer is "measure it". On
this machine, for "In five words, what is an AI agent?", thinking cost 5.4 s instead of 0.4 s
and the answer was no better. For a question with steps in it, it often is better. You now
have a flag, so you can test instead of guess.

</details>

**Q7.** The C# code is three files now. What goes in each one, and why is that worth a file?

<details><summary>A</summary>

`ChatProtocol.cs` is **what** goes down the wire: the three records, and nothing else.
`Helper.cs` is **how** we reach the model: the config, the one `HttpClient`, the JSON options,
and the two methods that read a reply. `Program.cs` is the conversation, and nothing else —
send, check, stream, remember.

It is worth a file because every chapter from here changes the wire. Chapter 04 adds a tool
call to `Message`, chapter 05 adds a `tools` field to `ChatRequest`. Those edits all land in
one small file you can read in ten seconds. And it keeps `Program.cs` at 53 lines, so the
conversation stays the thing you see when you open the project.

</details>
