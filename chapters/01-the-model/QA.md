# Chapter 01 — questions

Answer these out loud before the next video. If one is hard, rewatch that part.

**Q1.** You ask the model the same question twice and get two different answers. Is something broken?

<details><summary>A</summary>

No. A model picks each next word from a set of likely ones, with some randomness.
Same input, different output, by design. If you need the same answer every time, you set the
temperature to 0 — and even then two different runners can differ.

</details>

**Q2.** Why is `messages` a list and not a string?

<details><summary>A</summary>

Because the server keeps nothing. Every call starts from zero. If you want the model to know
what was said two turns ago, you send those two turns again. That list is the only memory an
agent has, which is also why it fills up — chapter 06.

</details>

**Q3.** Your first call takes 28 seconds. The second takes 1 second. Why?

<details><summary>A</summary>

The first call loads the model into RAM — 9.6 GB off the disk. After that it is resident and
answers fast. This is why the C# side raises `HttpClient.Timeout`: the 100-second default can
be beaten by a cold load on a slow disk, and the error then looks like a network fault.

</details>

**Q4.** The model answered "Which file starts the HTTP server?" with a confident, wrong answer.
What is missing?

<details><summary>A</summary>

Hands. It has no way to look at your files, so it guessed from the shape of the question.
Tools (chapter 04) and the loop (chapter 05) are what turn a guess into a look.

</details>

**Q5.** We send `"stream": false`. What does `true` change, and why is it off?

<details><summary>A</summary>

`true` sends many small JSON objects, one per chunk of text, so a user sees words appear.
`false` sends one object when the model is done. Streaming is better for a person and worse
for a lesson — you would be parsing a stream before you understand the message. It comes back
in chapter 06.

</details>

**Q6.** What is the smallest change that would make this program talk to a cloud model instead?

<details><summary>A</summary>

The URL, an API key header, and the path. The message list and the `message` reply are nearly
the same shape everywhere, because everyone copied the same design. That is also the warning:
nearly is not the same, and the differences are in the tool-calling fields you meet in
chapter 05.

</details>
