# Chapter 01 — camera script, C#

Target 12 minutes. Each step is **Goal / Why / Do / Proof**. The **Do** is what you type on air.
The TypeScript video is [`SCRIPT-ts.md`](SCRIPT-ts.md). The two share steps 0, 1 and 3, word for
word, so a C# viewer never has to watch the other one.

Before you hit record: `ollama serve` is running, the model is pulled and **already warm**
(run one throwaway question), the terminal font is sized up, and the repo is at the end of
chapter 00. Run `dotnet build` once off camera, or the first `dotnet run` costs you 20 seconds
of silence.

| Step | Minutes |
|---|---|
| 0 — the promise | 0:00–1:00 |
| 1 — the endpoint, by curl | 1:00–3:30 |
| 2 — the code | 3:30–9:30 |
| 3 — break it, and the handoff | 9:30–12:00 |

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
  "model": "gemma4:e2b",
  "stream": false,
  "messages": [{"role":"user","content":"In five words, what is an AI agent?"}]
}' | python3 -m json.tool
```

Point at three things in the output and nothing else: `message`, `done`, `eval_count`.
Then point at `thinking`: the model wrote notes to itself first, and that is the wait.
Then run the same command twice and let the viewer see the wording change.

**Proof.** Two different answers to the same question, on screen.

---

## Step 2 — The code

**Goal.** `code/csharp/Program.cs` exists and prints an answer.

**Why.** Typing it yourself, after the curl, shows the code is only a wrapper around that one
request. Keep saying "this is the curl" as you type.

**Do.** Start the project on camera. It takes one command and no NuGet package:

```bash
cd chapters/01-the-model/code/csharp
dotnet new console -n RepoLibrarian -o . -f net10.0
```

Say out loud: `HttpClient` and `System.Text.Json` are in the box. Nothing to install.
Then write the file in this order, and run after each piece:

1. the two environment-variable reads for URL and model
2. the `record Message`, `record ChatRequest`, `record ChatResponse` at the bottom
3. `ChatAsync()` — the `PostAsJsonAsync`, then the status check, then the return
4. the three lines at the top level: read `args`, build the list, print

This is a top-level `Program.cs`. No classes, no interfaces, no dependency injection. Say so,
and say when they arrive: chapter 07, when there is something real to inject.

Stop at exactly three places:

- the `messages` list. Say: **"a list, not a string. The model gets the whole story every
  time, because it remembers nothing."** That line is the seed of chapter 06.
- the 5-minute `Timeout`. The default is 100 seconds. Say what the failure looks like: a
  network error that is really a slow disk loading gigabytes into RAM.
- `PropertyNamingPolicy = CamelCase`. Delete it on camera, run, get a
  `NullReferenceException`, put it back. Ollama's field names are lower case. C# properties
  are not.

**Proof.**

```bash
dotnet run -- "In five words, what is an AI agent?"
```

---

## Step 3 — Break it, and the handoff

**Goal.** The viewer has seen the two failures they are most likely to hit.

**Why.** A viewer whose first error was never shown on screen assumes they broke something.
Two minutes here saves them an hour.

**Do.** Kill `ollama serve`. Run it. Show the connection error. Start it again.
Then ask the model `"Which file starts the HTTP server?"` and let it answer confidently about
a folder it has never seen.

**Proof.** The model made something up, on camera.

**The handoff.** "Five words took six seconds, and you saw why in the `thinking` field. Next
video you get a flag for that, and you watch the answer arrive word by word."
