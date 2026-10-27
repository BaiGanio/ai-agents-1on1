# Chapter 01 — camera script, TypeScript

Target 12 minutes. Read this file top to bottom while you record. Nothing else is open.

Every step is **Say / Type / Proof**, and some add a **Note** — the one sentence you must not
forget. **Type** is exactly what goes on air. Code steps give a line range in the finished file
[`code/ts/src/main.ts`](code/ts/src/main.ts), so a bad take is a paste away.

The C# video is [`SCRIPT-csharp.md`](SCRIPT-csharp.md). Steps 1, 2, 3, 4, 15 and 16 are shared
with it word for word, so a C# viewer never has to watch this one.

| Act | Steps | Minutes |
|---|---|---|
| 0 — the promise | 1 | 0:00–1:00 |
| 1 — the endpoint, by curl | 2–4 | 1:00–3:30 |
| 2 — the code | 5–13 | 3:30–9:30 |
| 3 — break it, and hand off | 14–16 | 9:30–12:00 |

---

## Before you record — off camera

Run these in order. Every one is a copy-paste.

```bash
# P1. the model server, in its own terminal window. Leave it running.
ollama serve
```

```bash
# P2. the model is pulled
ollama pull gemma4:e2b
```

```bash
# P3. the model is warm. Without this, your first on-air answer costs 20 seconds of silence.
ollama run gemma4:e2b "hi" --verbose
```

```bash
# P4. everything else is installed
cd /Users/lk/Projects/BaiGanio/ai-agents-1on1
./scripts/check.sh
```

```bash
# P5. keep the finished code, then clear the folder so you can type it on air
cd /Users/lk/Projects/BaiGanio/ai-agents-1on1
mkdir -p /tmp/take-01 && cp -R chapters/01-the-model/code/ts /tmp/take-01/
rm -rf chapters/01-the-model/code/ts
```

```bash
# P6. after the take, put the finished code back
cd /Users/lk/Projects/BaiGanio/ai-agents-1on1
git checkout -- chapters/01-the-model/code/ts
git status
```

Last checks, by eye: terminal font is sized up, the repo is at the end of chapter 00, the editor
has no other tab open, and notifications are off.

---

## Act 0 — The promise · 0:00–1:00

### 1. Say the everyday picture

**Say.** The Everyday Picture from [`README.md`](README.md) — the stranger behind the door.
No hands. Amnesia. A guesser. Thirty seconds, then move.

**Note.** Say the small true thing early: a model is an HTTP endpoint. Most people think there
is more, and that belief is what makes agents feel hard.

**Proof.** You have not opened an editor yet.

---

## Act 1 — The endpoint, by curl · 1:00–3:30

### 2. Ask the model with curl

**Say.** "No library yet. One command. This is the whole lesson."

**Type.**

```bash
curl -s http://localhost:11434/api/chat -d '{
  "model": "gemma4:e2b",
  "stream": false,
  "messages": [{"role":"user","content":"In five words, what is an AI agent?"}]
}' | python3 -m json.tool
```

**Proof.** A JSON object on screen with an answer inside it.

### 3. Point at four fields

**Say.** Point at these and nothing else: `message`, `done`, `eval_count`, then `thinking`.

**Note.** `thinking` is the model writing notes to itself before it answers. That is where the
wait goes. Chapter 01.A turns it off.

**Proof.** The viewer has seen the shape of the reply.

### 4. Run it again, same question

**Say.** "Same question. Different words. It guesses, every time."

**Type.** Press up-arrow, enter. Twice.

**Proof.** Two different answers to one question, on screen.

---

## Act 2 — The code · 3:30–9:30

### 5. Make the project

**Say.** "Node 24 and up runs a `.ts` file directly. No `tsc`. No build. No `dist`."

**Type.**

```bash
cd /Users/lk/Projects/BaiGanio/ai-agents-1on1/chapters/01-the-model/code
mkdir -p ts/src && cd ts
npm init -y
npm pkg set type=module name=repo-librarian private=true
npm pkg set scripts.start="node src/main.ts"
npm install -D @types/node
```

**Note.** There are **no runtime dependencies**, on purpose. `fetch` is built into Node.
`@types/node` is only so the editor knows what `process` is.

**Proof.** `package.json` and `node_modules/` exist.

### 6. Add tsconfig.json

**Say.** "Nothing compiles this file. It is for the editor, so red squiggles mean something."

**Type.** Create `tsconfig.json`. Paste the whole of
[`code/ts/tsconfig.json`](code/ts/tsconfig.json) — 15 lines. Do not read it out loud.

**Proof.** The editor stops complaining about `process`.

### 7. The header and the two settings

**Say.** "Where the model lives, and which model. Both come from the environment, with a
default, so nobody has to edit code to switch model."

**Type.** `src/main.ts`, lines **1–7**.

**Proof.** The file exists.

### 8. The Message type

**Say.** "Three roles. That is the entire protocol."

**Type.** Line **9**.

**Proof.** —

### 9. The request

**Say.** "This is the curl. Same URL, same body, same `stream: false`."

**Type.** Lines **11–19**.

**Note.** `stream: false` means one JSON object at the end, not a drip of words. Streaming is
nicer for a user and noisier for a lesson, so it waits until chapter 06.

**Proof.** —

### 10. The error check, and break it on purpose

**Say.** "Ollama puts the real reason in the body. Without this line you get a bare 500."

**Type.** Lines **21–25**.

**Then, on camera:** delete those five lines, finish the file, run it with a bad model name,
and show the useless result. Put the lines back.

```bash
MODEL=nope:1b node src/main.ts "hello"
```

**Note.** Run this beat **after** step 12, or the file does not run yet. Say so on air:
"park that, we come back to it."

**Proof.** The viewer saw the bad error and the good error, side by side.

### 11. Read the reply

**Say.** "One message in the body. We return it and throw the rest away."

**Type.** Lines **27–29**.

**Proof.** —

### 12. The three lines at the bottom

**Say.** "Read the question, build the list, print the answer."

**Type.** Lines **31–38**.

**Note.** Stop at line 35. Say: **"a list, not a string. The model gets the whole story every
time, because it remembers nothing."** That line is the seed of chapter 06.

**Proof.** The file is 38 lines.

### 13. Run it

**Say.** "No build. Just run the TypeScript."

**Type.**

```bash
node src/main.ts "In five words, what is an AI agent?"
```

**Proof.** An answer in the terminal, from your own code.

---

## Act 3 — Break it, and hand off · 9:30–12:00

### 14. Kill the server

**Say.** "This is the error you will hit first. Here is what it looks like."

**Type.** In the `ollama serve` window, press `Ctrl+C`. Then:

```bash
node src/main.ts "hello"
```

Then start the server again.

**Proof.** `ECONNREFUSED` on screen.

### 15. Let it make something up

**Say.** "It has never seen this folder. Watch it answer anyway."

**Type.**

```bash
node src/main.ts "Which file starts the HTTP server?"
```

**Note.** This is the reason the rest of the course exists. Do not fix it here.

**Proof.** The model invented a filename, on camera.

### 16. Hand off

**Say.** "Five words took six seconds, and you saw why in the `thinking` field. Next video you
get a flag for that, and you watch the answer arrive word by word."

**Proof.** Stop recording.

---

## After the take

1. Run **P6** above to restore the code.
2. Write the real run numbers and the machine into [`docs/PROGRESS.md`](../../docs/PROGRESS.md).
3. Tick the boxes in the chapter [`README.md`](README.md) "Done when" list.
