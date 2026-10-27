# Chapter 03 — camera script, C#

Target 12 minutes. Each step is **Goal / Why / Do / Proof**. The **Do** is what you type on air.
The TypeScript video is [`SCRIPT-ts.md`](SCRIPT-ts.md). Steps 0, 1 and 5 are shared word for
word, so a TypeScript viewer never has to watch this one.

Before you hit record: `ollama serve` is running, `gemma4:e2b` is **warm**, `code/csharp/` holds
the chapter 02 file unchanged, `code/prompts/` is there, and `code/web/` does **not** exist
yet — you copy the page in on camera, you do not type it.

| Step | Minutes |
|---|---|
| 0 — the hatch and the counter | 0:00–1:30 |
| 1 — two servers, two ports | 1:30–3:00 |
| 2 — serve the page | 3:00–5:30 |
| 3 — POST /chat, and the `send` parameter | 5:30–9:00 |
| 4 — stream it into the browser | 9:00–10:30 |
| 5 — kill Ollama on purpose | 10:30–12:00 |

This video types more than the TypeScript one — about 65 new lines against 41. `HttpListener`
is the difference, and it is worth it (see step 2). If the video runs long, **cut after step 3**
and make steps 4 and 5 chapter `03.A`. Step 3 ends on a working page with an answer in it,
which is a fair place to stop.

---

## Step 0 — The hatch and the counter

**Goal.** The viewer wants a page before you write a line of it.

**Why.** "A chat UI" sounds like a side quest. Two seconds of the terminal makes the case for
you, so make it with the terminal, not with words.

**Do.** Run chapter 02's code and ask one question with thinking on:

```bash
cd chapters/02-the-prompt/code/csharp
THINK=on dotnet run -- "Which file starts the HTTP server?"
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

**Goal.** A browser shows your page, served by your code, with no new package.

**Why.** Get a win on screen fast, and settle the "where is ASP.NET?" question in one breath
rather than letting it hang over the video.

**Do.** Copy the page in. Say out loud that you are not typing it, and why:

```bash
cp -R ../../../03-the-chat-page/code/web code/web   # or however your recording tree is laid out
```

Open `code/web/index.html` for ten seconds. Point at the three blocks — `<style>`, `<body>`,
`<script>` — and say: the script does a `fetch` and reads the body as a stream, which is the
loop from chapter 01.A one layer up. Then close it and do not open it again.

Now say the thing the viewer is already thinking. Show the `.csproj`:

```bash
cat RepoLibrarian.csproj
```

It is the same console app as chapter 01. No NuGet package, and no `Microsoft.NET.Sdk.Web`. We
use `HttpListener` from the base library instead. Give the trade honestly: a minimal API would
be about twenty lines shorter, and it would also boot a whole web host and print its own
startup logging into the terminal where the model output goes. This chapter is about a request
and a response, so we use the thing that is a request and a response.

In `Program.cs`:

1. `using System.Net;`
2. `var port = Environment.GetEnvironmentVariable("PORT") ?? "3000";`
3. turn the one-shot branch into an early `return`, so the server is not nested in an `else`:

```csharp
if (args.Length > 0)
{
    messages.Add(new("user", string.Join(' ', args)));
    await ChatAsync(messages, ToTerminal);
    Console.WriteLine();
    return;
}
```

4. then delete the whole readline loop and put the server in its place:

```csharp
var page = Encoding.UTF8.GetBytes(await File.ReadAllTextAsync("../web/index.html"));

using var listener = new HttpListener();
listener.Prefixes.Add($"http://localhost:{port}/");
listener.Start();
Console.WriteLine($"Repo Librarian on http://localhost:{port}");

while (true)
{
    var context = await listener.GetContextAsync();
    var response = context.Response;

    response.ContentType = "text/html; charset=utf-8";
    await response.OutputStream.WriteAsync(page);
    response.Close();
}
```

Deleting the readline loop on camera is the point. Say it: the page replaces it, and keeping
both would mean three modes in one file.

One trap worth twenty seconds, and do it live. Drop the trailing slash off the prefix:

```csharp
listener.Prefixes.Add($"http://localhost:{port}");    // no slash
```

```text
Unhandled exception. System.ArgumentException: Only Uri prefixes ending in '/' are allowed.
(Parameter 'uriPrefix')
   at System.Net.HttpListener.AddPrefix(String uriPrefix)
```

Put the slash back. This is the kind message — it throws on `Prefixes.Add`, before `Start()`,
and it tells you exactly what is wrong. Show it anyway, because the next one does not.

```bash
cd chapters/03-the-chat-page/code/csharp
dotnet run
```

Open `http://localhost:3000`. Type a question, press the button. **Nothing happens.** Good —
say so before the viewer thinks it is broken. There is no `/chat` yet, so the page is a shape
with nothing behind it.

**Proof.** Your page, in a browser, from a plain console app, with a dead button.

---

## Step 3 — POST /chat, and the `send` parameter

**Goal.** The button works, and the viewer sees exactly what made it possible.

**Why.** This is the chapter. Not the HTML, not the port — the one line that let `ChatAsync`
feed something other than a terminal.

**Do.** Start in `ChatAsync`, not in the server. Show the problem first: scroll to the
`Console.Write` calls inside it and say — this method has a terminal baked into it.

Change the signature, and pull the printing out into a named method:

```csharp
async Task<Message> ChatAsync(List<Message> history, Action<Message> send)
```

Inside the line loop, the two `if` blocks collapse to one call:

```csharp
var part = JsonSerializer.Deserialize<ChatResponse>(line, json)!.Message;

send(part);
thinking.Append(part.Thinking);
content.Append(part.Content);
```

`Append` on a `null` string appends nothing, so the null checks go too. Then paste the printing
back as `ToTerminal`, and point out it is chapter 02's code with a name. One thing to say out
loud: `ToTerminal` captures the `grey` flag, so declare `var grey = false;` **above** it — a
local function can only capture a local that was declared before it.

Fix the one-shot call to `await ChatAsync(messages, ToTerminal)`.

Pause here for twenty seconds. This is the beat: **`ChatAsync` no longer decides where the
words go.** A handful of lines. It is also why chapter 05's loop will not have to touch this
method.

Now the endpoint. Add the `if` at the top of the `while` body, and the `/chat` path under it:

```csharp
var request = context.Request;

if (request.HttpMethod != "POST" || request.Url?.AbsolutePath != "/chat")
{
    response.ContentType = "text/html; charset=utf-8";
    await response.OutputStream.WriteAsync(page);
    response.Close();
    continue;
}

using var body = new StreamReader(request.InputStream, Encoding.UTF8);
messages.Add(new("user", await body.ReadToEndAsync()));

response.ContentType = "application/x-ndjson; charset=utf-8";
response.SendChunked = true;
messages.Add(await ChatAsync(messages, part => ToBrowser(part, response)));
response.Close();
```

And the second `send`, at the bottom next to `ToTerminal`:

```csharp
void ToBrowser(object part, HttpListenerResponse response)
{
    var line = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(part, json) + "\n");
    response.OutputStream.Write(line);
    response.OutputStream.Flush();
}
```

Say the `if` out loud: one page, one endpoint, so this is not a router. And name the second
`send` as you type it — same method, different window. `object` rather than `Message`, because
step 5 sends an error through the same hole.

Restart, reload, ask. Then ask a second question and let the history prove itself:

```text
Which file starts the HTTP server?
What did I just ask you?
```

**Proof.** The refusal appears in the browser, and the second answer names the first question.

---

## Step 4 — Stream it into the browser

**Goal.** The words appear one at a time, and the viewer knows which line did it.

**Why.** In C# streaming is one setting, and getting it wrong does not slow the server down —
it kills it. Break it on camera and the viewer will remember.

**Do.** Prove it works with curl first, where the pieces are visible:

```bash
curl -s -N -X POST http://localhost:3000/chat -d "Which file starts the HTTP server?"
# {"role":"assistant","content":"I"}
# {"role":"assistant","content":" cannot"}
# ...
```

Then show the header that makes it possible:

```bash
curl -s -D - -o /dev/null -X POST http://localhost:3000/chat -d "say ok"
# Transfer-Encoding: chunked
```

Say what it is for: we cannot send a `Content-Length` we do not know, because the model has not
finished writing. Chunked is the answer, and it is what lets the browser read the first word
before the last one exists.

Now break it once, and make it count. Set `SendChunked = false` and ask again:

```bash
curl -s -D - -o /dev/null -X POST http://localhost:3000/chat -d "say ok"
# Content-Length: 0
```

```text
Unhandled exception. System.Net.HttpListenerException (0x80131620): Unable to write data to the
transport connection: Broken pipe.
```

Read it out: C# took us at our word. We said there was no chunking, it did not know a length,
so it sent `Content-Length: 0` and closed the body. Then we wrote into a closed body, and the
server died. Not slow — dead. Put the line back.

One sentence of honesty while you are here. The `Flush()` in `ToBrowser` is belt and braces:
take it out and the stream still arrives in pieces, measured. It stays because it says what we
mean, and because a buffered stream is the next thing you would suspect.

Now turn thinking on and reload the page:

```bash
THINK=on dotnet run
```

Grey thinking fills the top box, then the answer appears below it. Say: plain words could not
have done that, because the page would not know which was which.

If you want the number, it is in `README.md` — first words at about 200 ms, answer finished
around 4 to 6 seconds. The model did not get faster.

**Proof.** Grey thinking streaming into a browser, and a viewer who can name the line that made
it a stream.

---

## Step 5 — Kill Ollama on purpose

**Goal.** The viewer learns that an uncaught exception in a request handler ends the server.

**Why.** Every chapter from here lives in this process. If it dies on one bad request, the rest
of the course is built on sand. Two minutes now.

**Do.** With the server running and no `try` yet, stop Ollama in another terminal and ask a
question in the page.

The page goes dead — no answer, no message. Switch to the server terminal and show the corpse:

```text
Unhandled exception. System.Net.Http.HttpRequestException: Connection refused (localhost:11434)
 ---> System.Net.Sockets.SocketException (61): Connection refused
```

The process is gone. Reload the page to prove it — the browser cannot even reach port 3000 any
more. Say why: nothing caught the exception, and the headers had already gone out, so there was
no way left to tell the browser anything. The browser got a `200` with an empty body.

Fix it on camera:

```csharp
try
{
    messages.Add(await ChatAsync(messages, part => ToBrowser(part, response)));
}
catch (Exception error)
{
    ToBrowser(new JsonError(error.Message), response);
    messages.RemoveAt(messages.Count - 1);
}

response.Close();
```

and one record at the bottom:

```csharp
record JsonError(string Error);
```

Point at `RemoveAt` and say why it is there: a question with no answer after it confuses the
next call. Restart, ask again with Ollama still down:

```text
Connection refused (localhost:11434)
```

In the page, naming the port. Worth one sentence: the TypeScript version says `fetch failed`
and hides the reason, so C# wins this one. Both mean `ollama serve`.

Start Ollama, ask again without restarting your server. It answers.

**Proof.** The server survived a failed request, said so in the page, and recovered on its own.

**The handoff.** "It has a page, it streams, and it still says 'I cannot see the files yet'. It
is telling the truth — it has no tools. Next video we give it three, by hand, and every one of
them will show up in this page."
