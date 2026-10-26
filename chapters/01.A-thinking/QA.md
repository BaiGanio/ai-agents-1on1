# Chapter 01.A — questions

Answer these out loud before the next video. If one is hard, rewatch that part.

**Q1.** What is the model doing during the six seconds before a five-word answer appears?

<details><summary>A</summary>

Writing notes to itself — thinking. For "Say hi in two words." on this machine that was 328
lines of notes for 2 lines of answer. The notes cost tokens and time. Chapter 01 waited for
all of them and threw them away, because it printed only `content`.

</details>

**Q2.** Why does `THINK` have three states and not two?

<details><summary>A</summary>

`on`, `off`, and **not set**. Not set sends no `think` field at all, so the model does what it
ships with. That matters because a model that cannot think answers `think: true` with a 400
error, not with an answer. Two states would force us to pick one for every model, and one of
the two would break on some of them.

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
