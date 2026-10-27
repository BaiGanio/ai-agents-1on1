# Chapter 03 — questions

Answer these out loud before the next video. If one is hard, rewatch that part.

**Q1.** There are now two HTTP servers running. Name them, and say which one the browser talks
to.

<details><summary>A</summary>

Ollama on port 11434, and ours on port 3000. The browser only ever talks to ours. Our server is
a **client** of Ollama's — it takes the question from the browser, calls `POST /api/chat`, and
passes the words back. Three programs in a row, and the browser can only see the middle one.

</details>

**Q2.** Why can the message list not live in the browser?

<details><summary>A</summary>

Close the tab and it is gone. More importantly, from chapter 04 the agent runs tools that read
files on your disk, and a browser cannot read your disk. The loop has to live in the process
that can. The page is a window onto that process, not the thing itself.

</details>

**Q3.** Chapter 02's `chat` function printed to the terminal. Chapter 03's takes a `send`
parameter instead. What did that buy?

<details><summary>A</summary>

One function that works for two destinations. `chat` now does one job — talk to the model and
hand over each piece — and the caller decides where the pieces go. The terminal caller passes
`toTerminal`. The server caller passes a function that writes a JSON line into the response.
It also means chapter 05's loop will not have to touch `chat` at all.

</details>

**Q4.** Why does the server send no `content-length` header?

<details><summary>A</summary>

Because it does not know the length. The model has not finished writing the answer when the
headers go out. With no length, HTTP falls back to
[chunked](../../docs/GLOSSARY.md#chunked) encoding — the body goes out in pieces, and the
browser can read the first piece before the last one exists. That is the whole reason the page
fills in word by word.

</details>

**Q5.** Why does the page get one JSON object per line, instead of just the words?

<details><summary>A</summary>

Because two different things are coming back: thinking and the answer. Plain words would land
in the same place on screen. A small object per line keeps the two labelled, so the page can
put `thinking` in the grey box and `content` in the answer. It is the same
[JSON Lines](../../docs/GLOSSARY.md#json-lines) shape Ollama sends us, passed on.

</details>

**Q6.** Both the server and the page keep a `buffer` variable and call `split("\n")`. Why
twice?

<details><summary>A</summary>

Because the problem happens twice. A network body is split wherever the network likes, which
may be in the middle of a line. The server hits that reading Ollama's stream, and the page hits
it reading the server's. Same problem, same fix, one layer apart. `buffer` holds the unfinished
tail until the rest arrives.

</details>

**Q7.** You stop Ollama and ask a question. What should happen, and what happens without the
`try` around the model call?

<details><summary>A</summary>

It should show the error in the page and leave the server running. Without the `try`, the throw
is uncaught inside an async request handler, which ends the whole process — and because the
headers were already sent, the browser gets a closed connection with no message at all. One bad
request must never end the server. Chapters 04 to 08 all live in this process.

</details>

**Q8.** Two people open the page and both ask a question. What goes wrong?

<details><summary>A</summary>

There is one message list for the whole process, so they share a conversation and each can read
the other's questions. On the TypeScript server, asking at the same moment is worse: both
handlers push onto the same list and both send it, so the second question gets an answer to the
first. Measured — two curls asking for `RED` and `BLUE` both came back `RED`. The C# server
handles one request at a time, so it returns `RED` and `BLUE`. Chapter 06 is where a list stops
being one list.

</details>

**Q9.** The agent still answers "I cannot see the files yet". Did this chapter fail?

<details><summary>A</summary>

No. This chapter added a window, not an ability. The agent has no tools until chapter 04, so
the honest answer is still the refusal from chapter 02. What changed is that you can now watch
it refuse in a browser, and from chapter 04 every tool call will appear in the same place.

</details>
