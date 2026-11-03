# Chapter 08 — questions

Answer these out loud before the next video. If one is hard, rewatch that part.

**Q1.** The router says `look`. The model then asks for `delete_file`. What happens?

<details><summary>A</summary>

`Agent.Run` checks the name against the list the router chose, and sends back `no such tool:
delete_file`. The router limits what the model can ask for, not only what it is shown. In our
runs the model never asked for a tool outside its list, so we did not see this fire.

</details>

**Q2.** Why does the checker get no question?

<details><summary>A</summary>

Measured. With the question, an honest *"it was not written"* failed 6 of 6, because it did not
do what the question asked. Without it, it passed 6 of 6. The checker compares the tool results
with the answer. It does not judge the task.

</details>

**Q3.** The checker says `PASS`. Is the answer true?

<details><summary>A</summary>

Not always. The checker is a small model reading a model. It said `FAIL` to a correct
answer in the delete run. With long tool results it caught 0 of 10 lies. The grey tool line and the disk are the
truth.

</details>

**Q4.** `/overview` or a question like *"What is this repo about?"* — which is the workflow, and
which is the agent?

<details><summary>A</summary>

`/overview` is the workflow: your code lists the files and loops. The question goes to the
agent: the model picks which files to read. The workflow gives the same steps every time, 7
calls for 6 files. The agent can take 2 steps or 6.

</details>
