# Chapter 03 — basics: a web page and a web server

Five things this chapter needs. Nothing else.

You already met HTTP in [chapter 01's basics](../01-the-model/BASICS.md), on the asking side.
This chapter is the same HTTP from the answering side.

New words are in [`GLOSSARY.md`](../../docs/GLOSSARY.md).

---

## 1. A port is a door number

One machine, one address. Many programs that all want to listen. A
[port](../../docs/GLOSSARY.md#port) is how they stay apart: a number from 1 to 65535 that says
which program a message is for.

`localhost` means *this machine*. You have been writing it since chapter 01:

```text
http://localhost:11434/api/chat
         ^            ^
         |            the path: which job on that program
         the port: which program
```

11434 is Ollama's door. In this chapter you open your own, number 3000. Both are running at the
same time, on the same machine, and they do not collide — different doors.

Two rules about ports:

- **One listener per port.** Start a second program on 3000 and it fails. There is one door and
  somebody is standing in it. The error is `EADDRINUSE` or `Address already in use`.
- **Ports under 1024 need special permission.** That is why examples use 3000, 5000, 8080 and
  never 80.

---

## 2. A web server is a function

Strip away every framework you have seen and a web server is this:

> wait for a request, look at it, write a response, repeat.

A request is text with three parts you care about here:

```http
POST /chat HTTP/1.1
Host: localhost:3000
Content-Type: text/plain

Which file starts the HTTP server?
```

- the **method**: `POST`
- the **path**: `/chat`
- the **body**: the question

A response is text with two parts you care about:

```http
HTTP/1.1 200 OK
Content-Type: application/x-ndjson

{"role":"assistant","content":"I"}
```

- the **status**: `200` means fine. `404` means no such path. `500` means we broke.
- the **body**: what you sent back.

That is all a server is. Our whole server is one function that reads the method and the path,
and picks one of two things to do.

---

## 3. GET against POST

Two methods, and the difference matters once.

| | `GET` | `POST` |
|---|---|---|
| means | give me something | here, take this |
| has a body | no | yes |
| the browser does it | by typing an address, or clicking a link | by `fetch`, or submitting a form |
| safe to repeat | yes | no, it may do something twice |

Typing `http://localhost:3000` in the address bar is a `GET`. There is no body, so there is no
question in it — which is why the page comes back and nothing is asked. Asking a question is a
`POST`, because a question is a body.

Our server checks for exactly one case: **`POST` to `/chat`**. Everything else gets the page.
That is not a router. It is an `if`.

---

## 4. `fetch` from the browser

`fetch` in a browser is the same `fetch` you used in chapter 01 from Node. Same function, same
shape:

```js
const response = await fetch("/chat", { method: "POST", body: "a question" });
```

`"/chat"` has no host on the front. A path on its own means *the same server this page came
from*, which is ours. That is the one reason the page must be opened at
`http://localhost:3000` and not by double-clicking the file: a file on your disk did not come
from a server, so `/chat` points nowhere.

### Reading a response that is still arriving

`await response.text()` waits for the whole body. That is fine for a small answer and wrong
here, because we want the first word now.

So we read the body as a stream instead:

```js
const reader = response.body.pipeThrough(new TextDecoderStream()).getReader();

for (;;) {
  const { value, done } = await reader.read();
  if (done) break;
  console.log(value);          // whatever has arrived so far, as text
}
```

`reader.read()` waits for the next piece and tells you when there are no more. `value` is
**not** a whole line. It is however many bytes turned up together. It may be three lines, or
half of one.

That is why the page keeps a `buffer`:

```js
buffer += value;
const lines = buffer.split("\n");
buffer = lines.pop();          // the last piece has no newline yet, so it is unfinished
```

`split` on a newline gives you the finished lines plus one unfinished tail. `pop` takes the
tail off and puts it back in the buffer to wait. You will recognise this — it is the same loop
chapter 01.A wrote to read Ollama's stream. Same problem, same answer, one layer up.

---

## 5. One HTML file, no build step

[`code/web/index.html`](code/web/index.html) is given to you. You do not type it on camera. It
has no framework, no npm install, no build, and no dependency — the same rule as the rest of
this repo.

It is three things in one file:

| Tag | What it holds |
|---|---|
| `<style>` | how it looks |
| `<body>` | the boxes: a log, a text area, a button |
| `<script>` | the `fetch` and the reader loop from part 4 |

If you have never written a page, read the `<script>` block and ignore the rest. The other two
are decoration. The script is the only part that talks to your server.
