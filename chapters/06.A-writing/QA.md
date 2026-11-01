# Chapter 06.A — questions

Answer these out loud before the next video. If one is hard, rewatch that part.

**Q1.** Chapter 06 already wrote to disk with `save_note`. What is different about these tools?

<details><summary>A</summary>

Who picks the file. `save_note` writes to one file your code names, outside `REPO`, and it can
only add a line. The new tools take a path from the model, inside `REPO`. `write_file` can
replace a whole file. `delete_file` can take a whole folder.

</details>

**Q2.** You asked *"Delete everything in this folder."* What did the model send, and what did
your code do with it?

<details><summary>A</summary>

It sent `delete_file .`, 4 times in 4 runs. Your code joined `.` onto `REPO`, found a folder, and
deleted it with `recursive: true`. So the repo folder itself was gone, not only the files in it.
One step, about one second, and no question.

</details>

**Q3.** The model asked *"what do you mean?"* when you said *"clean it up"*. So it asks before
risky things. True?

<details><summary>A</summary>

No. It asks when your words are vague. *"Delete everything in this folder."* is clear, and it
did it without a question. Asking is a guess the model sometimes makes, not a rule it follows.
A rule has to live in your code. That is chapter 07.

</details>

**Q4.** Why did one run turn a four-line file into one line?

<details><summary>A</summary>

There is no edit tool. To change one word, the model has to read the file, change the word, and
write the whole file back. In 1 of 5 runs it skipped the read and wrote only the line it was
asked about. Then it said *"I have changed the text"*.

</details>

**Q5.** You removed the `delete_file` arm from the dispatch. The model said *"I have deleted
`SETUP.md`."* Was it deleted?

<details><summary>A</summary>

No. The tool result was `no such tool: delete_file`. The model read that and still wrote the
words a person would say after a delete. Same lesson as chapter 06's *"I will remember"*: what
the model **says** happened is not what **did** happen. Check the disk.

</details>

**Q6.** Why does `AppendFile` look at the end of the file before it appends?

<details><summary>A</summary>

Because the model writes files with no newline at the end. A plain append then joined the new
line onto the last one: `...and focused.Checked on 2026-11-01.` So the code adds a `\n` first
when the file does not end on one.

</details>

**Q7.** Why did this chapter need no prompt line, when chapter 06 did?

<details><summary>A</summary>

Because the model used the tools on its own. In chapter 06 it said *"I will remember"* and
called nothing, 0 times in 12, until one prompt line told it when. Here, asked to write, add or
delete, it called the right tool in every run, with only the tool descriptions. A prompt line is
a fix for a measured problem. With no problem, there is no line.

</details>
