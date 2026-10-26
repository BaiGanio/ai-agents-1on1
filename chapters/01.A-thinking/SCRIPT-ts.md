# Chapter 01.A — camera script, TypeScript

Target 12 minutes. Each step is **Goal / Why / Do / Proof**. The **Do** is what you type on air.
The C# video is [`SCRIPT-csharp.md`](SCRIPT-csharp.md). Steps 0, 1 and 4 are shared word for
word, so a C# viewer never has to watch this one.

Before you hit record: `ollama serve` is running, `gemma4:e2b` is **warm**, `phi4-mini:3.8b` is
pulled (you need it in step 4), and `code/ts/` holds the chapter 01 file, unchanged.

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

**Do.** Copy chapter 01's file into `chapters/01.A-thinking/code/ts/`. Then add two things:

1. the `THINK` const at the top, with the three-way `?:`
2. `think: THINK` in the `JSON.stringify` body

Stop here and say the important thing: **`JSON.stringify` drops a key whose value is
`undefined`.** So `THINK` unset puts no `think` field on the wire at all. No `if`, no branch.
Open the three-state table in `README.md` if they need to see it.

Then print `answer.thinking` as well as `answer.content`, run `THINK=on`, and show the notes.

**Proof.**

```bash
THINK=on  node src/main.ts "In five words, what is an AI agent?"
THINK=off node src/main.ts "In five words, what is an AI agent?"
```

Say the measured numbers: 5.4 s against 0.4 s on this machine.

---

## Step 3 — Streaming

**Goal.** The grey notes appear word by word, then the answer.

**Why.** This is the part the viewer came for, and it is the part with the real bug in it.

**Do.** Flip `stream` to `true` and run it. It breaks — `response.json()` cannot parse 330 JSON
objects glued together. Show the error. Do not fix it yet.

Now build the reader, in this order, running after each piece:

1. `for await (const chunk of response.body!.pipeThrough(new TextDecoderStream()))`
2. inside it, `JSON.parse(chunk)` — naive, and it works for about ten tokens, then throws
3. **now** add the `buffer`: `buffer += chunk`, `split("\n")`, `lines.pop()`

That order matters. The viewer must see the half-line bug happen before you fix it, or the
three buffer lines look like paranoia.

Last, split the printing: `part.thinking` goes out in grey, `part.content` goes out plain, and
one `\n\n` between them the first time content shows up.

**Proof.**

```bash
THINK=on node src/main.ts "In five words, what is an AI agent?"
```

Grey text scrolling, then a blank line, then the answer.

---

## Step 4 — Break it, and the handoff

**Goal.** The viewer has seen the error they will hit on another model.

**Why.** The first thing anyone does is swap the model. If `THINK=on` 400s and they have never
seen it, they think they broke the streaming.

**Do.**

```bash
MODEL=phi4-mini:3.8b THINK=on node src/main.ts "hi"
```

Read the error out loud: `400 Bad Request: "phi4-mini:3.8b" does not support thinking`.
Then run it again with `THINK` unset and show it answering. Say it once: **this is why unset
is a state.**

**Proof.** The 400, then the answer, from the same model.

**The handoff.** "It streams, and you choose whether it thinks. But it still has no idea who
it is or what it is for. Next video we tell it, in one block of text."
