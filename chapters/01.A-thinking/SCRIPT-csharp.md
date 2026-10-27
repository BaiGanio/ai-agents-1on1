# Chapter 01.A — camera script, C#

Target 12 minutes. Read this file top to bottom while you record. Nothing else needs to be open:
every command and every line of code is here.

Every step is **Say / Type / Proof**, and some add a **Note** — the one sentence you must not
forget. **Type** is exactly what goes on air. The finished files are
[`code/csharp/Helper.cs`](code/csharp/Helper.cs) and
[`code/csharp/Program.cs`](code/csharp/Program.cs), and each code step says which lines of them
you are typing, so a bad take is one paste away.

This chapter starts as a copy of chapter 01's code. Some blocks below are already on screen
when you start — they are marked **Already there**. Do not type those. They are printed so the
two files read whole, in order, on this page.

Both files grow. `Helper.cs` gets the thinking switch and two colour codes. `Program.cs` swaps
one call for a stream and reads it a line at a time.

The TypeScript video is [`SCRIPT-ts.md`](SCRIPT-ts.md). Steps 1, 2, 3 and 4 are shared with it
word for word, so a TypeScript viewer never has to watch this one.

| Act | Steps | Minutes |
|---|---|---|
| 0 — the six seconds | 1 | 0:00–1:30 |
| 1 — the thinking is already there, by curl | 2–4 | 1:30–4:00 |
| 2 — Helper.cs, the switch and the colours | 5–9 | 4:00–6:00 |
| 3 — Program.cs, read the stream | 10–15 | 6:00–10:00 |
| 4 — break it, and hand off | 16–19 | 10:00–12:00 |

---

## Before you record — off camera

Run these in order. Every one is a copy-paste.

```bash
# P1. the model server, in its own terminal window. Leave it running.
ollama serve
```

```bash
# P2. both models are pulled. You need phi4-mini for step 17.
ollama pull gemma4:e2b
ollama pull phi4-mini:3.8b
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
cd /Users/lk/Projects/BaiGanio/ai-agents-1on1/chapters/01.A-thinking/code/csharp
dotnet build
```

```bash
# P6. keep the finished code, then put chapter 01's code in its place so you can type the
#     difference on air
cd /Users/lk/Projects/BaiGanio/ai-agents-1on1
mkdir -p /tmp/take-01A && cp -R chapters/01.A-thinking/code/csharp /tmp/take-01A/
rm -rf chapters/01.A-thinking/code/csharp
cp -R chapters/01-the-model/code/csharp chapters/01.A-thinking/code/csharp
rm -rf chapters/01.A-thinking/code/csharp/bin chapters/01.A-thinking/code/csharp/obj
```

```bash
# P7. after the take, put the finished code back
cd /Users/lk/Projects/BaiGanio/ai-agents-1on1
rm -rf chapters/01.A-thinking/code/csharp
git checkout -- chapters/01.A-thinking/code/csharp
git status
```

Last checks, by eye: terminal font is sized up, the terminal is wide enough that grey text does
not wrap mid-word, the editor has no other tab open, and notifications are off.

---

## Act 0 — The six seconds · 0:00–1:30

### 1. Let the silence sit

**Say.** Nothing. Run chapter 01's code, ask one question, and **do not speak** while it runs.

**Type.**

```bash
cd /Users/lk/Projects/BaiGanio/ai-agents-1on1/chapters/01.A-thinking/code/csharp
dotnet run
```

At the `You:` prompt:

```text
In five words, what is an AI agent?
```

Then say the Everyday Picture from [`README.md`](README.md) — the scrap paper. Thirty seconds.

**Note.** Name the wait before you fix it. Everyone watched chapter 01 and waited, and nobody
told them why. That silence is the whole reason this chapter exists.

**Proof.** Six seconds of dead air, on camera, on purpose.

---

## Act 1 — The thinking is already there, by curl · 1:30–4:00

### 2. Ask for the whole reply and look inside it

**Say.** "We are not adding thinking. It was always there. Chapter 01 printed `content` and
threw the rest away."

**Type.**

```bash
curl -s http://localhost:11434/api/chat -d '{
  "model": "gemma4:e2b",
  "stream": false,
  "messages": [{"role":"user","content":"Say hi in two words."}]
}' | python3 -m json.tool
```

**Proof.** A long `thinking` field on screen, above a two-word `content`.

### 3. Point at the price

**Say.** Scroll to `eval_count`. Say the trade out loud: you paid for those tokens and you
printed two words.

**Note.** On this machine that question cost **379 tokens** and came back as **330 lines** when
streamed — 328 of thinking, 2 of answer. Say the real number from your own screen, not this one.

**Proof.** The viewer has seen what the wait buys.

### 4. Turn it off, by curl

**Say.** "One field. Watch."

**Type.**

```bash
curl -s http://localhost:11434/api/chat -d '{
  "model": "gemma4:e2b",
  "stream": false,
  "think": false,
  "messages": [{"role":"user","content":"Say hi in two words."}]
}' | python3 -m json.tool
```

**Note.** Three states, not two. `true` thinks. `false` does not. **Not sent at all** means the
model does what it ships with, and that is the only safe setting for a model you have not tested.
That third state is why the code below is a `bool?` and not a `bool`.

**Proof.** No `thinking` field, and it came back fast.

---

## Act 2 — Helper.cs, the switch and the colours · 4:00–6:00

### 5. Already there — where the model lives · Helper.cs lines 1-13

**Say.** "This is chapter 01's plumbing, untouched. Ten seconds, then we add to it."

**Already there.** Do not type it.

```csharp
using System.Text.Json;
using System.Text.Json.Serialization;

// Everything needed to reach the model, in one place, so Program.cs is only the conversation.
public static class Helper
{
    // Both read the environment first. Nobody has to edit code to point at another machine
    // or swap the model.
    public static readonly string Model =
        Environment.GetEnvironmentVariable("MODEL") ?? "gemma4:e2b";

    public static readonly string OllamaUrl =
        Environment.GetEnvironmentVariable("OLLAMA_URL") ?? "http://localhost:11434";
```

**Proof.** The file is on screen, unchanged from last chapter.

### 6. The thinking switch, and two colours · Helper.cs lines 15-21

**Say.** "One line for the switch. It is `bool?`, not `bool`, because of the three states we
just saw."

**Type.**

```csharp
    // Flip this to false to get the answer without the wait. A model that cannot think answers
    // Think: true with a 400, not with an answer, so leave it off on those.
    public static readonly bool? Think = true;

    // The thinking is grey so your eye skips it. These are ANSI colour codes, not a library.
    public static readonly string Grey = "\x1b[90m";
    public static readonly string Reset = "\x1b[0m";
```

**Note.** Say both things. One: a `bool` has two states and the wire has three, so the question
mark is the lesson, not a style choice. Two: those two strings are not a package. `\x1b[90m`
tells the terminal "grey from here", `\x1b[0m` tells it "back to normal". Every terminal has
understood them for forty years.

**Proof.** The file builds, and nothing has changed in the output yet.

### 7. Already there — one HttpClient · Helper.cs lines 23-30

**Say.** "Untouched. One client, five-minute timeout."

**Already there.** Do not type it.

```csharp
    // One client for the whole process. A new one per call leaks sockets, and a local model can
    // take minutes on the first answer while it loads into RAM, so 100 seconds is too short.
    public static readonly HttpClient OllamaHttpClient =
        new()
        {
            BaseAddress = new Uri(OllamaUrl),
            Timeout = TimeSpan.FromMinutes(5),
        };
```

**Proof.** —

### 8. The JSON options get a second reason to exist · Helper.cs lines 32-41

**Say.** "This block was already here. One line in it now carries the whole three-state idea,
so add the comment and say why."

**Type.** The two comment lines above `DefaultIgnoreCondition` are new. The rest is already
there.

```csharp
    public static readonly JsonSerializerOptions AgentJsonOptions =
        new()
        {
            // Ollama's field names are lower case. C# properties are not, so we say so once, here.
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            // This is what makes Think: null mean "field not sent". Sent as null, it means
            // "no thinking", which is a different answer.
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        };
}
```

**Note.** This is the step to slow down on. **Delete `DefaultIgnoreCondition` on camera.** Set
`Think = null`, run, and show the request going out as `"think": null` — which Ollama reads as
"no thinking", not as "I did not ask". Put the line back. `null` must mean **not on the wire**.
Measured by curl on this machine: no field gives 714 characters of thinking, `"think": null`
gives none at all.

**Proof.** The class closes. `Helper.cs` is 41 lines so far.

### 9. The records grow by one field each · Helper.cs lines 43-45

**Say.** "Two of the three records change. `Message` can carry thinking. `ChatRequest` can ask
for it."

**Type.** Replace chapter 01's three records with these.

```csharp
record Message(string Role, string Content, string? Thinking = null);
record ChatRequest(string Model, List<Message> Messages, bool Stream, bool? Think);
record ChatResponse(Message Message);
```

**Note.** `string? Thinking = null` has a default, so every `new Message("user", prompt)` in
`Program.cs` keeps working with no edit. Point at that — it is why the file still builds.

**Proof.** `Helper.cs` is 45 lines and the build is clean.

---

## Act 3 — Program.cs, read the stream · 6:00–10:00

### 10. The header and the set-up · Program.cs lines 1-15

**Say.** "New title, and two new `using` lines. Everything under them is chapter 01's."

**Type.** The top comment and the two new `using` lines are new. The four lines after them are
already there.

```csharp
// Chapter 01.A — thinking.
//
// Chapter 01 showed a model is an HTTP endpoint.
// This chapter shows it thinks before it answers, and that the reply arrives word by word.
// The thinking is grey. The answer is plain.

using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

List<Message> messages = [];

HttpClient http = Helper.OllamaHttpClient;

Console.WriteLine("Type a question. An empty line stops the app.");
```

**Note.** `System.Text` is for `StringBuilder`, `System.Text.Json` for `JsonSerializer`. Name
them now so they are not a mystery in four minutes.

**Proof.** It still builds.

### 11. Already there — the loop and the question · Program.cs lines 17-25

**Say.** "Untouched. You talk to it, it reads a line, an empty line stops it."

**Already there.** Do not type it.

```csharp
while (true)
{
    Console.Write("You: ");

    // ReadLine gives null when input ends, so this one check covers both quit and end of input.
    string? prompt = Console.ReadLine();
    if (string.IsNullOrWhiteSpace(prompt)) break;

    messages.Add(new("user", prompt));
```

**Proof.** —

### 12. Two words on the request · Program.cs line 27

**Say.** "`Stream: false` becomes `Stream: true`, and we pass the switch."

**Type.**

```csharp
    ChatRequest request = new(Helper.Model, messages, Stream: true, Think: Helper.Think);
```

**Note.** Run it now, before you touch anything else. `PostAsJsonAsync` and
`ReadFromJsonAsync` choke: the body is 330 JSON objects glued together, and
`ReadFromJsonAsync` wants one. **Show the error and do not fix it.** The next two steps are the
fix, and the viewer should want them.

**Proof.** A parse error on screen, caused by one word.

### 13. Send it by hand so you can read the body yourself · Program.cs lines 29-38

**Say.** "`PostAsJsonAsync` is a convenience that reads the whole body for you. We cannot use a
convenience that waits, so we build the request ourselves."

**Type.** Delete chapter 01's `PostAsJsonAsync` line and type this in its place.

```csharp
    using HttpRequestMessage message =
        new(HttpMethod.Post, "/api/chat")
        {
            Content = JsonContent.Create(request, options: Helper.AgentJsonOptions),
        };

    // ResponseHeadersRead hands us the stream as it arrives. Without it HttpClient waits for the
    // last byte, and the stream stops being a stream.
    using var response =
        await http.SendAsync(message, HttpCompletionOption.ResponseHeadersRead);
```

**Note.** This is the one C# trap in the chapter, and it fails **silently**. Type
`HttpCompletionOption.ResponseDefault` first, run it, and watch the whole answer land in one
burst at the end. The code is right. The output is right. The feature is gone. Then change the
one word to `ResponseHeadersRead` and run again. Now it drips. Give that its own minute.

**Proof.** Nothing visible yet — it still cannot parse. Say so, so nobody thinks it is broken.

### 14. Already there — the status check · Program.cs lines 40-47

**Say.** "Untouched, but the reason it exists just got better. Watch step 17."

**Already there.** Do not type it. The comment is reworded, nothing else.

```csharp
    if (!response.IsSuccessStatusCode)
    {
        // Ollama puts the real reason in the body — an unknown model, a model that cannot think.
        // Without this line you get a bare 400 and no idea which of the two it was.
        var body = await response.Content.ReadAsStringAsync();
        throw new HttpRequestException(
            $"{(int)response.StatusCode} {response.ReasonPhrase}: {body}");
    }
```

**Proof.** —

### 15. Read it a line at a time, and colour it · Program.cs lines 49-81

**Say.** "This replaces the two lines that read the reply in chapter 01. It is the body of the
chapter."

**Type.** Delete chapter 01's `ReadFromJsonAsync`, `Console.WriteLine` and `messages.Add` lines.
Type this in their place, and run after each of the four pieces.

First, somewhere to collect the two halves:

```csharp
    var content = new StringBuilder();
    var thinking = new StringBuilder();
```

Then the reader:

```csharp
    // A stream is one JSON object per line. ReadLineAsync waits for a whole line, so a network
    // chunk that stops mid-line is not our problem here.
    using var reader = new StreamReader(await response.Content.ReadAsStreamAsync());
    while (await reader.ReadLineAsync() is { } line)
    {
        if (line.Length == 0) continue;

        Message part =
            JsonSerializer.Deserialize<ChatResponse>(line, Helper.AgentJsonOptions)!.Message;

        if (!string.IsNullOrEmpty(part.Thinking))
        {
            Console.Write(Helper.Grey + part.Thinking + Helper.Reset);
            thinking.Append(part.Thinking);
        }

        if (!string.IsNullOrEmpty(part.Content))
        {
            // The first word of the answer needs a gap after the grey, or the two run together.
            if (content.Length == 0 && thinking.Length > 0) Console.Write("\n\n");
            Console.Write(part.Content);
            content.Append(part.Content);
        }
    }
```

Last, close the turn:

```csharp
    Console.WriteLine();

    // The reply goes back into the list. The model remembers nothing, so the list is the memory.
    messages.Add(new Message("assistant", content.ToString(), thinking.ToString()));
}
```

**Note.** Say the free thing C# gets here, because the TypeScript video spends three lines on
it: `ReadLineAsync` waits for a whole line, so a network chunk that stops mid-line is not your
problem. In TypeScript it is.

Then say why there are two `StringBuilder`s and not one. You print each piece the moment it
arrives, but the **list** needs the finished message. Printing and remembering are two jobs.

**Proof.** `Program.cs` is 81 lines and the build is clean.

### 16. Run it, two turns

**Say.** "Grey first, then the answer. And turn two still sees turn one."

**Type.**

```bash
cd /Users/lk/Projects/BaiGanio/ai-agents-1on1/chapters/01.A-thinking/code/csharp
dotnet run
```

Then type two questions that lean on each other, and an empty line to stop:

```text
In five words, what is an AI agent?
And in three?
```

**Note.** Measured on this machine, `gemma4:e2b`: those two turns took **8.3 s** including
`dotnet run`, and the answers were `Perceives, reasons, takes actions.` then
`Sense, think, act.` Point at the second one — the thinking out loud quotes the first answer,
on screen, in grey. The list is the memory, and you can read it happening.

**Proof.** Grey text scrolling, a blank line, an answer, twice.

---

## Act 4 — Break it, and hand off · 10:00–12:00

### 17. Flip the switch off

**Say.** "One line. That is the whole flag."

**Type.** In `Helper.cs`, change one word:

```csharp
    public static readonly bool? Think = false;
```

Then run and ask the same question:

```bash
dotnet run
```

**Note.** Measured on this machine: **1.45 s** with thinking off against **8.3 s** for two
thinking turns, and the answer was `Autonomous program interacting with environment.` — no
worse than the slow one. Say that plainly. Thinking is not free and it is not always better.
You now own the choice.

Put the line back to `true` before the next step.

**Proof.** No grey. An answer, fast.

### 18. Break it on a model that cannot think

**Say.** "The first thing anyone does is swap the model. Here is the error you will get."

**Type.**

```bash
MODEL=phi4-mini:3.8b dotnet run
```

Ask anything at the `You:` prompt. Then set `Think = null` in `Helper.cs` and run it again.

**Note.** The real error, from a real run on this machine:

```text
Unhandled exception. System.Net.Http.HttpRequestException: 400 Bad Request:
{"error":"\"phi4-mini:3.8b\" does not support thinking"}
```

Say it once: **this is why `null` is a state.** With `Think = null` the field never goes on the
wire, and the same model answers. That is the payoff for step 8.

**Proof.** The 400, then the same model answering.

### 19. Hand off

**Say.** "It streams, and you choose whether it thinks. But it still has no idea who it is or
what it is for. Next video we tell it, in one block of text."

**Proof.** Stop recording.

---

## After the take

1. Run **P7** above to restore the code.
2. Check `Helper.cs` really is back to `Think = true` — steps 8, 17 and 18 all change that line.
3. Write the real run numbers and the machine into [`docs/PROGRESS.md`](../../docs/PROGRESS.md).
4. Tick the boxes in the chapter [`README.md`](README.md) "Done when" list.
