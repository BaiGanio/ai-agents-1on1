# Chapter 01 — camera script

Target 12 minutes. Each step is **Goal / Why / Do / Proof**. The **Do** is what you type on air.

Before you hit record: `ollama serve` is running, the model is pulled and **already warm**
(run one throwaway question), both terminals are open and font-sized up, and the repo is at
the end of chapter 00.

| Step | Minutes |
|---|---|
| 0 — the promise | 0:00–1:00 |
| 1 — the endpoint, by curl | 1:00–3:30 |
| 2 — TypeScript | 3:30–7:00 |
| 3 — C# | 7:00–10:30 |
| 4 — break it, and the handoff | 10:30–12:00 |

---

## Step 0 — The promise

**Goal.** The viewer knows what will exist in 12 minutes.

**Why.** Say the small true thing early: a model is an HTTP endpoint. Most people think there
is more, and that belief is what makes agents feel hard.

**Do.** On camera, say the Everyday Picture from `README.md` — the stranger behind the door.
No hands. Amnesia. A guesser. Thirty seconds, then move.

**Proof.** You have not opened an editor yet.

---

## Step 1 — The endpoint, by curl

**Goal.** One answer out of the model, with no code in the way.

**Why.** curl first is the whole lesson in one command. Nobody can claim a library did
something clever, because there is no library yet.

**Do.**

```bash
curl -s http://localhost:11434/api/chat -d '{
  "model": "gemma4:e4b",
  "stream": false,
  "messages": [{"role":"user","content":"In five words, what is an AI agent?"}]
}' | python3 -m json.tool
```

Point at three things in the output and nothing else: `message`, `done`, `eval_count`.
Then run the same command twice and let the viewer see the wording change.

**Proof.** Two different answers to the same question, on screen.

---

## Step 2 — TypeScript

**Goal.** `ts/src/main.ts` exists and prints an answer.

**Why.** Typing it yourself, after the curl, shows the code is only a wrapper around that one
request. Keep saying "this is the curl" as you type.

**Do.** Write it in this order, and run after each piece:

1. the two `const`s for URL and model
2. the `Message` type
3. `chat()` — the `fetch`, then the `if (!response.ok)`, then the return
4. the three lines at the bottom: read `argv`, build the list, print

Say out loud at the `messages` array: **"a list, not a string. The model gets the whole story
every time, because it remembers nothing."** That line is the seed of chapter 06.

**Proof.**

```bash
node src/main.ts "In five words, what is an AI agent?"
```

---

## Step 3 — C#

**Goal.** `csharp/Program.cs` does the same, and the viewer sees how little differs.

**Why.** Two stacks is the selling point of the course. Do not apologise for the repetition —
name each difference as it appears, because that naming *is* the content.

**Do.**

```bash
cd csharp
dotnet new console -n RepoLibrarian -o . -f net10.0
```

Then the same four pieces, same order. Stop at exactly two places:

- the 5-minute `Timeout` — show the 100-second default failing if you have the patience, or
  just say what it looks like: a network error that is really a slow disk
- `PropertyNamingPolicy` — delete it on camera, run, get a `NullReferenceException`, put it back

**Proof.**

```bash
dotnet run -- "In five words, what is an AI agent?"
```

---

## Step 4 — Break it, and the handoff

**Goal.** The viewer has seen the two failures they are most likely to hit.

**Why.** A viewer whose first error was never shown on screen assumes they broke something.
Two minutes here saves them an hour.

**Do.** Kill `ollama serve`. Run both stacks. Show both errors. Start it again.
Then ask the model `"Which file starts the HTTP server?"` and let it answer confidently about
a folder it has never seen.

**Proof.** The model made something up, on camera.

**The handoff.** "It cannot look. Next video we tell it who it is. Then we give it a
page, and then hands."
