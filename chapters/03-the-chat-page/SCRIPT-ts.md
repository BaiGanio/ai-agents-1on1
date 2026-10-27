# Chapter 03 — camera script, TypeScript

Target 12 minutes. Each step is **Goal / Why / Do / Proof**. The **Do** is what you type on air.
The C# video is [`SCRIPT-csharp.md`](SCRIPT-csharp.md). Steps 0, 1 and 5 are shared word for
word, so a C# viewer never has to watch this one.

Before you hit record: `ollama serve` is running, `gemma4:e2b` is **warm**, `code/ts/` holds
the chapter 02 file unchanged, `code/prompts/` is there, and `code/web/` does **not** exist
yet — you copy the page in on camera, you do not type it.

| Step | Minutes |
|---|---|
| 0 — the hatch and the counter | 0:00–1:30 |
| 1 — two servers, two ports | 1:30–3:00 |
| 2 — serve the page | 3:00–5:00 |
| 3 — POST /chat, and the `send` parameter | 5:00–8:30 |
| 4 — stream it into the browser | 8:30–10:30 |
| 5 — kill Ollama on purpose | 10:30–12:00 |

If the video runs long, **cut after step 3** and make steps 4 and 5 chapter `03.A`. Step 3 ends
on a working page with an answer in it, which is a fair place to stop.

---

## Step 0 — The hatch and the counter

**Goal.** The viewer wants a page before you write a line of it.

**Why.** "A chat UI" sounds like a side quest. Two seconds of the terminal makes the case for
you, so make it with the terminal, not with words.

**Do.** Run chapter 02's code and ask one question with thinking on:

```bash
cd chapters/02-the-prompt/code/ts
THINK=on node src/main.ts "Which file starts the HTTP server?"
```

Let the grey wall scroll. Try to scroll back to the question. You cannot, not without losing
your place. Say: from the next chapter there will be tool calls in here too.

Then the Everyday Picture from `README.md` — the coffee shop counter. Two shops, both four
minutes. One closes a hatch. One lets you watch the cup.

**Proof.** A screen of grey, and a viewer who wants somewhere better to put it.

---

## Step 1 — Two servers, two ports

**Goal.** Nobody ends this video thinking the browser talks to the model.

**Why.** This is the one idea that, if missed, makes every later chapter confusing. Draw it
before you write it.

**Do.** No editor yet. On a whiteboard, or just in the terminal, name the three programs:

```text
browser        ->  our server   ->  Ollama
(no port)          port 3000        port 11434
```

Say the sentence out loud: **our server is a client of Ollama's.** Then prove the two ports are
already separate things, with curl, before any of your code exists:

```bash
curl -s -o /dev/null -w "ollama: %{http_code}\n" http://localhost:11434/api/tags
curl -s -o /dev/null -w "ours:   %{http_code}\n" http://localhost:3000/
# ollama: 200
# ours:   000
```

Port 11434 answers. Port 3000 is nobody. Your job this video is to be somebody on 3000.

**Proof.** The viewer can point at which box the browser reaches, and which one it never does.

---

## Step 2 — Serve the page

**Goal.** A browser shows your page, served by your code.

**Why.** Get a win on screen fast, before the hard part. Serving a file is five lines and it
makes the rest feel small.

**Do.** Copy the page in. Say out loud that you are not typing it, and why:

```bash
cp -R ../../../03-the-chat-page/code/web code/web   # or however your recording tree is laid out
```

Open `code/web/index.html` for ten seconds. Point at the three blocks — `<style>`, `<body>`,
`<script>` — and say: the script does a `fetch` and reads the body as a stream, which is the
loop from chapter 01.A one layer up. Then close it and do not open it again.

Now in `src/main.ts`:

1. `import { createServer } from "node:http";`
2. `const PORT = Number(process.env.PORT ?? 3000);`
3. replace the whole `else` branch — the readline loop — with:

```ts
const page = await readFile("../web/index.html", "utf8");

const server = createServer(async (req, res) => {
  res.writeHead(200, { "content-type": "text/html; charset=utf-8" });
  res.end(page);
});

server.listen(PORT, () => console.log(`Repo Librarian on http://localhost:${PORT}`));
```

Deleting the readline loop on camera is the point. Say it: the page replaces it, and keeping
both would mean three modes in one file.

```bash
cd chapters/03-the-chat-page/code/ts
node src/main.ts
```

Open `http://localhost:3000`. Type a question, press the button. **Nothing happens.** Good —
say so before the viewer thinks it is broken. There is no `/chat` yet, so the page is a shape
with nothing behind it.

**Proof.** Your page, in a browser, from your server, with a dead button.

---

## Step 3 — POST /chat, and the `send` parameter

**Goal.** The button works, and the viewer sees exactly what made it possible.

**Why.** This is the chapter. Not the HTML, not the port — the one line that let `chat` feed
something other than a terminal.

**Do.** Start in `chat`, not in the server. Show the problem first: scroll to the
`process.stdout.write` calls inside it and say — this function has a terminal baked into it.

Change the signature, and pull the printing out into a named function:

```ts
async function chat(messages: Message[], send: (part: Message) => void): Promise<Message>
```

Inside the line loop, the three `if` blocks collapse to one call:

```ts
const part: Message = JSON.parse(line).message;
send(part);
reply.thinking += part.thinking ?? "";
reply.content += part.content ?? "";
```

Then paste the printing back as `toTerminal`, and point out it is chapter 02's code with a
name. Fix the one-shot call to `await chat(messages, toTerminal)`.

Pause here for twenty seconds. This is the beat: **`chat` no longer decides where the words
go.** Four lines. It is also why chapter 05's loop will not have to touch this function.

Now the endpoint. Add the `if` at the top of the handler:

```ts
if (req.method !== "POST" || req.url !== "/chat") {
  res.writeHead(200, { "content-type": "text/html; charset=utf-8" });
  res.end(page);
  return;
}

let body = "";
for await (const chunk of req) body += chunk;

messages.push({ role: "user", content: body });

res.writeHead(200, { "content-type": "application/x-ndjson; charset=utf-8" });
messages.push(await chat(messages, (part) => res.write(JSON.stringify(part) + "\n")));
res.end();
```

Say the `if` out loud: one page, one endpoint, so this is not a router. And name the second
`send` as you type it — same function, different window.

Restart, reload, ask. Then ask a second question and let the history prove itself:

```text
Which file starts the HTTP server?
What did I just ask you?
```

**Proof.** The refusal appears in the browser, and the second answer names the first question.

---

## Step 4 — Stream it into the browser

**Goal.** The words appear one at a time, and the viewer knows which line did it.

**Why.** It already streams by this point, which is the surprise. Spend the time on *why*,
because the viewer will not get another look at chunked encoding in this course.

**Do.** It is already working — so prove it with curl first, where the pieces are visible:

```bash
curl -s -N -X POST http://localhost:3000/chat -d "Which file starts the HTTP server?"
# {"role":"assistant","content":"I"}
# {"role":"assistant","content":" cannot"}
# ...
```

Then show *why* it works. Two things, thirty seconds each.

**One: no `content-length`.** Point at `res.writeHead` and say what is missing from it. You
cannot send a length you do not know. Prove what HTTP does instead:

```bash
curl -s -D - -o /dev/null -X POST http://localhost:3000/chat -d "say ok"
# Transfer-Encoding: chunked
```

**Two: one JSON object per line, not plain words.** Turn thinking on and reload the page:

```bash
THINK=on node src/main.ts
```

Grey thinking fills the top box, then the answer appears below it. Say: plain words could not
have done that, because the page would not know which was which.

If you want the number, it is in `README.md` — first words at about 200 ms, answer finished
around 4 to 6 seconds. The model did not get faster.

**Proof.** Grey thinking streaming into a browser, and a viewer who can say what
`Transfer-Encoding: chunked` bought them.

---

## Step 5 — Kill Ollama on purpose

**Goal.** The viewer learns that an uncaught throw in a request handler ends the server.

**Why.** Every chapter from here lives in this process. If it dies on one bad request, the rest
of the course is built on sand. Two minutes now.

**Do.** With the server running and no `try` yet, stop Ollama in another terminal and ask a
question in the page.

The page goes dead — no answer, no message. Switch to the server terminal and show the corpse:

```text
[TypeError: fetch failed] {
  [cause]: AggregateError [ECONNREFUSED]
```

The process is gone. Say why: the throw was inside an `async` handler, nothing caught it, and
the headers had already gone out, so there was no way left to tell the browser anything.

Fix it on camera:

```ts
try {
  messages.push(await chat(messages, (part) => res.write(JSON.stringify(part) + "\n")));
} catch (error) {
  res.write(JSON.stringify({ error: String(error) }) + "\n");
  messages.pop();
}
res.end();
```

Point at `messages.pop()` and say why it is there: a question with no answer after it confuses
the next call. Restart, ask again with Ollama still down:

```text
TypeError: fetch failed
```

Four useless words in the page — admit it. Node hides the real reason in `error.cause`, and the
C# version says `Connection refused (localhost:11434)` instead. Both mean `ollama serve`.

Start Ollama, ask again without restarting your server. It answers.

**Proof.** The server survived a failed request, said so in the page, and recovered on its own.

**The handoff.** "It has a page, it streams, and it still says 'I cannot see the files yet'. It
is telling the truth — it has no tools. Next video we give it three, by hand, and every one of
them will show up in this page."
