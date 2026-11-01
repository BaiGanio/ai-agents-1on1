# Chapter 07 — questions

Answer these out loud before the next video. If one is hard, rewatch that part.

**Q1.** The system prompt could say "never read outside the repo". Why is that not a
guardrail?

<details><summary>A</summary>

Because the model can ignore it, and nothing would stop it. A guardrail is a rule in **your**
code. The model fills in a request. Your code decides whether to run it. In chapter 06.A the
model read `/etc/hosts` the moment it was asked. Here `Guards.Readable` refuses it, 7 of 8
times it was sent. The eighth time the model sent `etc/hosts`, which is inside the repo.

</details>

**Q2.** You asked *"Delete everything in this folder."* Why did no `y/n` show up?

<details><summary>A</summary>

Because the path check comes first. The model sent `delete_file .`, and `.` is the repo root.
`Guards.Writable` refuses the root, so the tool returned `refused: . is the whole repo, or
outside it` before it got to the ask. 5 of 5 runs. You are only asked about a change that
would really happen.

</details>

**Q3.** You said `n` to a write. The model said *"The file SUMMARY.md has been created."* Was
it?

<details><summary>A</summary>

No. `ls` shows no `SUMMARY.md`. The tool result said `NOT written. The user said no.` The model
read it and still wrote the words a person says after a write. 7 of 8 runs. A clearer sentence
and a prompt line did not fix it. So the lesson is the one from chapters 06 and 06.A: what the
model **says** happened is not what **did** happen. Read the tool's line. Check the disk.

</details>

**Q4.** Why does `append_file` not ask, when `write_file` does?

<details><summary>A</summary>

Because append cannot lose text. It adds to the end of a file that is there. `write_file`
replaces the whole file, and `delete_file` removes it. Those two can lose work, so those two
ask. Fewer asks also means you read each one, instead of typing `y` without looking.

</details>

**Q5.** The step cap stopped the question after 3 steps. What happened to the 2 files it had
already deleted?

<details><summary>A</summary>

They stay deleted. A cap stops new steps. It does not undo old ones. The same is true of the
clock. That is why the ask sits at the tool, not at the loop: a guard that comes after the
damage is too late for that damage.

</details>

**Q6.** Why one clock for the whole question, and not one per call?

<details><summary>A</summary>

Because a stuck loop can be made of fast calls. Each fast call ends inside its own clock, so a
clock per call never fires. One clock for the question adds them all up. The `HttpClient`
already had a per-call timeout of 5 minutes, and it cannot stop a loop.

</details>

**Q7.** The first repeat check refused a real read. Why, and what fixed it?

<details><summary>A</summary>

The model read `SUMMARY.md`, appended a line, then read it again to check. The second read had
the same name and the same path, so the check called it a repeat, 3 of 3. But the file had
changed, so the answer would be new. Now any write, append or delete clears the list of past
calls. Rerun: 3 of 3 read the file again.

</details>

**Q8.** Why does `Guards.Readable` call `Path.GetFullPath`, and not look for `..` in the
string?

<details><summary>A</summary>

Because a string check is a block-list, and a block-list misses the spelling you did not think
of. `GetFullPath` walks every `.` and `..` first, and an absolute path like `/etc/hosts`
replaces the root. Then one question is enough: is the result the repo folder, or inside it?
That is an allow-list.

</details>
