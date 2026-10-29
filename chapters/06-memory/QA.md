# Chapter 06 — questions

Answer these out loud before the next video. If one is hard, rewatch that part.

**Q1.** The model said "I will remember that". Why was nothing saved?

<details><summary>A</summary>

Because saying is all a model can do. It writes text. Only your code writes a file, and your
code writes one only when the model asks for `save_note`.

With no line about `save_note` in the prompt, the model called it 0 times in 12. Every answer
said some form of *"Understood, I will remember that."* Those are the words a person would say.
One line in the prompt made it 15 of 15.

</details>

**Q2.** Why drop the old tool results? The model needs to know what it read.

<details><summary>A</summary>

It still knows. Only the text goes. The `assistant` message that asked for
`read_file GLOSSARY.md` stays in the list, so the model can see it read the glossary.

What goes is about 1800 tokens that are sent again on every step after the answer. Measured: the
next question started at 576 tokens with the drop, 2271 without.

</details>

**Q3.** Why after the answer, and not after each step?

<details><summary>A</summary>

Because in the middle of a question the model is still using the file. That is why it asked for
it. Drop it then and the next step sends a stub where the text was, so the model has nothing
to answer from.

Once the answer is on screen, the file has done its job.

</details>

**Q4.** What does a follow-up question about the same file cost now?

<details><summary>A</summary>

One more `read_file`. Asked *"What does that same file say a stub is?"*, the model read
`GLOSSARY.md` again. That one step went to 2298 tokens, then the drop brought the list back down.

That is the trade. One call when you need the file, against the whole file on every step when
you do not.

</details>

**Q5.** Why are the notes a second system message, and not the end of the first one?

<details><summary>A</summary>

Because at the end of the first one, the model ignored them. Measured by curl with the `tools`
field: 0 of 5 right at the end, 5 of 5 as a second message, 5 of 5 at the top. With no `tools`
field, all three were 5 of 5.

We do not know why the tool list does that. We know where the notes work, and that is where the
code puts them.

</details>

**Q6.** Why does `notes.md` live outside `REPO`?

<details><summary>A</summary>

So the agent does not find its own notes when it looks at the repo. Inside `REPO`, `list_files`
would list `notes.md`, and `search_text` would find your name in it, as if it were part of the
code.

</details>

**Q7.** `save_note` writes to disk. Is that safe?

<details><summary>A</summary>

Safer than most writes. It can only add a line, and only to one file. The model never names the
file — your code does. The worst it can do is fill `notes.md` with lines you did not want.

It still writes without asking you. Chapter 07 adds "ask before you write".

</details>
