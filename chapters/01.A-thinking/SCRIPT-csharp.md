# Chapter 01.A — camera script, C#

Target 12 minutes. Each step is **Goal / Why / Do / Proof**. The **Do** is what you type on air.
The TypeScript video is [`SCRIPT-ts.md`](SCRIPT-ts.md). Steps 0, 1 and 4 are shared word for
word, so a C# viewer never has to watch the other one.

Before you hit record: `ollama serve` is running, `gemma4:e2b` is **warm**, `phi4-mini:3.8b` is
pulled (you need it in step 4), `code/csharp/` holds the chapter 01 project, and you have run
`dotnet build` once off camera.

| Step | Minutes |
|---|---|
| 0 — the six seconds | 0:00–1:30 |
| 1 — see the thinking, by curl | 1:30–4:00 |
| 2 — the THINK flag | 4:00–6:30 |
| 3 — streaming | 6:30–10:30 |
| 4 — break it, and the handoff | 10:30–12:00 |

---

## Step 0 — The six seconds

**Goal.** The viewer feels the problem before you name it.

**Why.** Everyone watched chapter 01 and waited. Nobody was told why. Name the wait and you
have their attention for twelve minutes.

**Do.** Run the chapter 01 code, unchanged, and **do not speak** while it runs. Let the silence
sit. Then say the Everyday Picture from `README.md` — the scrap paper. The model is doing the
work before the answer, and the door is in the way.

**Proof.** Six seconds of dead air, on camera, on purpose.

---

## Step 1 — See the thinking, by curl

**Goal.** The notes are real text, and they are already in the reply.

**Why.** Nothing is added yet. The field was always there — chapter 01 printed `content` and
threw `thinking` away.

**Do.**

```bash
curl -s http://localhost:11434/api/chat -d '{
  "model": "gemma4:e2b",
  "stream": false,
  "messages": [{"role":"user","content":"Say hi in two words."}]
}' | python3 -m json.tool
```

Scroll through the `thinking` field. It is long. Point at `eval_count` — **379 tokens** for a
two-word answer on this machine. Say the trade out loud: you paid for 379, you printed 2.

Then run it again with `"think": false` added, and show that `thinking` is gone and it is fast.

**Proof.** The same question, two shapes of reply, side by side in the scrollback.

---

## Step 2 — The THINK flag

**Goal.** `THINK=on|off` works, with `stream` still `false`.

**Why.** One change at a time. The flag is small and the stream is not, so land the flag first
and keep the code running.

**Do.** Copy chapter 01's project into `chapters/01.A-thinking/code/csharp/`. Then add three
things:

1. the `bool? think` switch expression at the top — `"on"`, `"off"`, `_ => null`
2. `bool? Think` on the `ChatRequest` record, and `string? Thinking` on `Message`
3. `DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull` in the options

Stop at number 3 and make it the lesson. Delete it on camera. Run with `THINK` unset. Show the
request now carries `"think": null`, and show Ollama taking it as "no". Put the line back.
`null` must mean **not on the wire**, not `false`. Open the three-state table in `README.md`
if they need to see it.

Then print `Thinking` as well as `Content`, run `THINK=on`, and show the notes.

**Proof.**

```bash
THINK=on  dotnet run -- "In five words, what is an AI agent?"
THINK=off dotnet run -- "In five words, what is an AI agent?"
```

Say the measured numbers: 5.4 s against 0.4 s for the TypeScript side on this machine. The C#
side adds about half a second of `dotnet run` on top, every run. Say that, so nobody thinks
C# is slower at HTTP.

---

## Step 3 — Streaming

**Goal.** The grey notes appear word by word, then the answer.

**Why.** This is the part the viewer came for, and it has the one C# trap in it.

**Do.** Flip `Stream` to `true` and run it. `ReadFromJsonAsync` chokes on 330 JSON objects
glued together. Show the error. Do not fix it yet.

Now build the reader, in this order, running after each piece:

1. swap `PostAsJsonAsync` for an `HttpRequestMessage` plus `http.SendAsync`
2. `using var reader = new StreamReader(await response.Content.ReadAsStreamAsync())`,
   then `while (await reader.ReadLineAsync() is { } line)` and deserialize each line
3. run it

It works — and **it is not streaming**. Everything appears in one burst at the end. The code
is right, the output is right, and the feature is gone. Let that sit for a beat, then add:

```csharp
HttpCompletionOption.ResponseHeadersRead
```

Run again. Now it drips. That one flag is the whole difference, and it fails silently, which
is why it gets its own minute.

Say the free thing C# gets here: `ReadLineAsync` waits for a whole line, so a network chunk
that stops mid-line is not your problem. The TypeScript video spends three lines on that.

Last, split the printing: `part.Thinking` goes out in grey, `part.Content` goes out plain, and
one `\n\n` between them the first time content shows up.

**Proof.**

```bash
THINK=on dotnet run -- "In five words, what is an AI agent?"
```

Grey text scrolling, then a blank line, then the answer.

---

## Step 4 — Break it, and the handoff

**Goal.** The viewer has seen the error they will hit on another model.

**Why.** The first thing anyone does is swap the model. If `THINK=on` 400s and they have never
seen it, they think they broke the streaming.

**Do.**

```bash
MODEL=phi4-mini:3.8b THINK=on dotnet run -- "hi"
```

Read the error out loud: `400 Bad Request: "phi4-mini:3.8b" does not support thinking`.
Then run it again with `THINK` unset and show it answering. Say it once: **this is why unset
is a state.**

**Proof.** The 400, then the answer, from the same model.

**The handoff.** "It streams, and you choose whether it thinks. But it still has no idea who
it is or what it is for. Next video we tell it, in one block of text."
