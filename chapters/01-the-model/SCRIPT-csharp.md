# Chapter 01 — camera script, C#

Target 12 minutes. Read this file top to bottom while you record. Nothing else is open.

Every step is **Say / Type / Proof**, and some add a **Note** — the one sentence you must not
forget. **Type** is exactly what goes on air. Code steps give a line range in the finished file
[`code/csharp/Program.cs`](code/csharp/Program.cs), so a bad take is a paste away.

The TypeScript video is [`SCRIPT-ts.md`](SCRIPT-ts.md). Steps 1, 2, 3, 4, 15 and 16 are shared
with it word for word, so a TypeScript viewer never has to watch this one.

| Act | Steps | Minutes |
|---|---|---|
| 0 — the promise | 1 | 0:00–1:00 |
| 1 — the endpoint, by curl | 2–4 | 1:00–3:30 |
| 2 — the code | 5–12 | 3:30–9:30 |
| 3 — break it, and hand off | 13–16 | 9:30–12:00 |

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
# P5. warm the SDK too, or the first `dotnet run` costs you 20 more seconds of silence
cd /Users/lk/Projects/BaiGanio/ai-agents-1on1/chapters/01-the-model/code/csharp
dotnet build
```

```bash
# P6. keep the finished code, then clear the folder so you can type it on air
cd /Users/lk/Projects/BaiGanio/ai-agents-1on1
mkdir -p /tmp/take-01 && cp -R chapters/01-the-model/code/csharp /tmp/take-01/
rm -rf chapters/01-the-model/code/csharp
```

```bash
# P7. after the take, put the finished code back
cd /Users/lk/Projects/BaiGanio/ai-agents-1on1
git checkout -- chapters/01-the-model/code/csharp
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

**Say.** "One command, and no NuGet package. `HttpClient` and `System.Text.Json` are in the box."

**Type.**

```bash
cd /Users/lk/Projects/BaiGanio/ai-agents-1on1/chapters/01-the-model/code
mkdir -p csharp && cd csharp
dotnet new console -n RepoLibrarian -o . -f net10.0
```

**Note.** `-o .` puts the files here instead of in a new nested folder.

**Proof.** `RepoLibrarian.csproj` and `Program.cs` exist.

### 6. The header, the usings, the two settings

**Say.** "Where the model lives, and which model. Both come from the environment, with a
default, so nobody has to edit code to switch model."

**Type.** Clear `Program.cs`, then type lines **1–11**.

**Note.** This is a top-level `Program.cs`. No classes, no interfaces, no dependency injection.
Say so, and say when they arrive: chapter 07, when there is something real to inject.

**Proof.** The file exists.

### 7. One HttpClient, and a long timeout

**Say.** "One client for the whole process. A new one per call leaks sockets."

**Type.** Lines **13–19**.

**Note.** The default timeout is 100 seconds. A local model can take minutes on the first
answer while it loads into RAM. Say what that failure looks like: a network error that is
really a slow disk.

**Proof.** —

### 8. The JSON options

**Say.** "Ollama's field names are lower case. C# properties are not. We say so once, here."

**Type.** Lines **21–27**.

**Note.** Two options, and both matter. `PropertyNamingPolicy = CamelCase` maps the names.
`DefaultIgnoreCondition = WhenWritingNull` keeps empty fields out of the request body.
We come back and delete the first one at step 13.

**Proof.** —

### 9. The three lines that do the work

**Say.** "Read the question, build the list, print the answer."

**Type.** Lines **29–36**.

**Note.** Stop at line 33. Say: **"a list, not a string. The model gets the whole story every
time, because it remembers nothing."** That line is the seed of chapter 06.

**Proof.** —

### 10. The request

**Say.** "This is the curl. Same URL, same body, same `Stream: false`."

**Type.** Lines **38–58**.

**Note.** `Stream: false` means one JSON object at the end, not a drip of words. Streaming is
nicer for a user and noisier for a lesson, so it waits until chapter 06. And the status check
exists because Ollama puts the real reason in the body — without it you get a bare 500.

**Proof.** —

### 11. The three records

**Say.** "Three lines for the whole protocol. No classes, no properties, no boilerplate."

**Type.** Lines **60–62**.

**Note.** Records go at the bottom. Top-level statements must come before any type.

**Proof.** The file is 62 lines and it builds.

### 12. Run it

**Say.** "The two dashes matter. Without them, `dotnet run` eats your words as its own flags."

**Type.**

```bash
dotnet run -- "In five words, what is an AI agent?"
```

**Proof.** An answer in the terminal, from your own code.

---

## Act 3 — Break it, and hand off · 9:30–12:00

### 13. Delete the camelCase option

**Say.** "This is the one that bites every C# viewer. Watch."

**Type.** Delete line **25** in the editor, then:

```bash
dotnet run -- "hello"
```

Put the line back and run again.

**Note.** The code is right and the names are wrong, so the reply parses into nothing. Say
that out loud — it is not an HTTP problem.

**Proof.** A `NullReferenceException` on screen, then a working answer.

### 14. Kill the server

**Say.** "This is the error you will hit first. Here is what it looks like."

**Type.** In the `ollama serve` window, press `Ctrl+C`. Then:

```bash
dotnet run -- "hello"
```

Then start the server again.

**Proof.** A connection-refused error on screen.

### 15. Let it make something up

**Say.** "It has never seen this folder. Watch it answer anyway."

**Type.**

```bash
dotnet run -- "Which file starts the HTTP server?"
```

**Note.** This is the reason the rest of the course exists. Do not fix it here.

**Proof.** The model invented a filename, on camera.

### 16. Hand off

**Say.** "Five words took six seconds, and you saw why in the `thinking` field. Next video you
get a flag for that, and you watch the answer arrive word by word."

**Proof.** Stop recording.

---

## After the take

1. Run **P7** above to restore the code.
2. Write the real run numbers and the machine into [`docs/PROGRESS.md`](../../docs/PROGRESS.md).
3. Tick the boxes in the chapter [`README.md`](README.md) "Done when" list.
