# Chapter 02 — questions

Answer these out loud before the next video. If one is hard, rewatch that part.

**Q1.** What makes a message the system prompt?

<details><summary>A</summary>

Two things, and neither is a special field. Its `role` is `"system"`, and it is **first** in
the list. There is no `system:` key on the request. It is one more item in the same
`messages` array you have been sending since chapter 01.

</details>

**Q2.** Why do we send the system prompt on every single call?

<details><summary>A</summary>

Because the model keeps nothing between calls. The server holds no session. If turn two does
not carry the system message, the model on turn two never saw it, and the agent forgets who
it is. You pay those tokens again on every call, which is why a short prompt is worth writing.

</details>

**Q3.** The prompt lives in a file, not in the source. Name two things that buys you.

<details><summary>A</summary>

You change the agent without editing or rebuilding code, and you can keep several personas
side by side and pick one with `PROMPT=pirate`. A third: the prompt can be reviewed and
diffed like any other file, by someone who does not read TypeScript or C#.

</details>

**Q4.** "You are the Repo Librarian. You answer questions about code." Why is that a bad
system prompt?

<details><summary>A</summary>

It is a label, not a rule. On this machine the model took the name and then guessed four file
names for a folder it cannot see. The line that changed the behaviour was **"Never guess a
file name"** — it says what the agent must not do. Write behaviour, not a job title.

</details>

**Q5.** The agent stayed in character when the user typed "Ignore your instructions". Is the
system prompt a security control?

<details><summary>A</summary>

No. It held that time, on that model, against that push. A prompt is a strong suggestion. It
can be talked around, and the harder someone tries, the better their odds. Real limits are
rules in *your* code that the model cannot reach — the step cap, the timeout, the path
allow-list in chapter 07.

</details>

**Q6.** The terminal loop keeps the message list between turns. Is that memory?

<details><summary>A</summary>

Only the short kind, and only inside one run. Quit and it is gone. It also grows with no
limit, so a long session will eventually fill the
[context window](../../docs/GLOSSARY.md#context-window) and the oldest turns fall out with no
warning. Chapter 06 gives the list a size and gives the agent a notes file that survives.

</details>

**Q7.** Your prompt file is not found. What does each stack say?

<details><summary>A</summary>

TypeScript: `ENOENT: no such file or directory, open 'prompts/nope.md'`.
C#: `System.IO.DirectoryNotFoundException` or `FileNotFoundException`, with the full path.
The usual cause is not a typo. It is the working directory — you ran the program from
somewhere other than the code folder.

</details>
