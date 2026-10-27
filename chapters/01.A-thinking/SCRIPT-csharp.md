# Chapter 01.A — camera script, C#

Target 12 minutes. Read this file top to bottom while you record. Nothing else needs to be open:
every command and every line of code is here.

Every step is **Say / Type / Proof**, and some add a **Note** — the one sentence you must not
forget. **Type** is exactly what goes on air. The finished files are
[`code/csharp/ChatProtocol.cs`](code/csharp/ChatProtocol.cs),
[`code/csharp/Helper.cs`](code/csharp/Helper.cs) and
[`code/csharp/Program.cs`](code/csharp/Program.cs), and each code step says which lines of them
you are typing, so a bad take is one paste away.

This chapter starts as a copy of chapter 01's code: two files, `Helper.cs` and `Program.cs`.
Some blocks below are already on screen when you start — they are marked **Already there**. Do
not type those. They are printed so all three files read whole, in order, on this page.

The shape of the chapter: **the conversation barely changes, the plumbing does the new work.**
A third file appears, `ChatProtocol.cs`, and the records move into it. `Helper.cs` gets the
thinking switch, two colour codes, and two methods — one that reads the stream, one that reads
an error. `Program.cs` swaps one call for a stream and then calls those two methods. That is
why `Program.cs` ends at 53 lines and still reads top to bottom.

The TypeScript video is [`SCRIPT-ts.md`](SCRIPT-ts.md). Steps 1, 2, 3 and 4 are shared with it
word for word, so a TypeScript viewer never has to watch this one.

| Act | Steps | Minutes |
|---|---|---|
| 0 — the six seconds | 1 | 0:00–1:30 |
| 1 — the thinking is already there, by curl | 2–4 | 1:30–4:00 |
| 2 — ChatProtocol.cs, the wire gets its own file | 5–6 | 4:00–5:00 |
| 3 — Helper.cs, the switch, the colours, the reader | 7–13 | 5:00–9:00 |
| 4 — Program.cs, four lines of difference | 14–21 | 9:00–10:30 |
| 5 — break it, and hand off | 22–25 | 10:30–12:00 |

---

## Before you record — off camera

Run these in order. Every one is a copy-paste.

```bash
# P1. the model server, in its own terminal window. Leave it running.
ollama serve
```

```bash
# P2. both models are pulled. You need phi4-mini for step 23.
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
#     difference on air. Chapter 01 has no ChatProtocol.cs — you create it on camera.
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

## Act 2 — ChatProtocol.cs, the wire gets its own file · 4:00–5:00

### 5. Delete the records off the bottom of Helper.cs · Helper.cs, old lines 33-35

**Say.** "Three records sat at the bottom of `Helper.cs` in chapter 01. Two of them change this
chapter, and both of them are about the wire, not about plumbing. So they get their own file."

**Type.** In `Helper.cs`, select these three lines and delete them.

```csharp
record Message(string Role, string Content);
record ChatRequest(string Model, List<Message> Messages, bool Stream);
record ChatResponse(Message Message);
```

**Note.** Say why out loud, once, so it is not housekeeping: `Helper.cs` is **how** we reach the
model and `ChatProtocol.cs` is **what** goes down the wire. Every chapter from here changes the
wire, and this is the file you will open each time.

**Proof.** The build breaks. Say that it will, and that the next step fixes it.

### 6. The new file · ChatProtocol.cs lines 1-13

**Say.** "New file. Three records, and two of them grow by one field."

**Type.** Create `ChatProtocol.cs` and type all of it.

```csharp
// The wire format, and nothing else. Helper.cs is the plumbing. Program.cs is the conversation.

// Thinking has a default, so every `new Message("user", prompt)` from chapter 01 still compiles.
public record Message(string Role, string Content, string? Thinking = null);

public record ChatRequest(
    string Model,
    List<Message> Messages,
    bool Stream = false,
    bool? Think = false
);

public record ChatResponse(Message Message);
```

**Note.** Two things, and say both.

One: `string? Thinking = null` has a default, so the `new Message("user", prompt)` already in
`Program.cs` keeps working with no edit. Point at that line in the other file — it is why the
build goes green again without you touching it.

Two: `bool? Think` is the three states from step 4, written down. A `bool` has two states and
the wire has three, so the question mark is the lesson, not a style choice.

**Proof.** The build is clean again. `ChatProtocol.cs` is 13 lines.

---

## Act 3 — Helper.cs, the switch, the colours, the reader · 5:00–9:00

### 7. One new `using` · Helper.cs lines 1-3

**Say.** "One new line at the top. `System.Text` is for `StringBuilder`, which arrives in four
minutes."

**Type.** Line 1 is new. The two under it are already there.

```csharp
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
```

**Proof.** Still builds.

### 8. Already there — where the model lives · Helper.cs lines 5-14

**Say.** "This is chapter 01's plumbing, untouched. Ten seconds, then we add to it."

**Already there.** Do not type it.

```csharp
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

**Proof.** The block is on screen, unchanged from last chapter.

### 9. The thinking switch, and two colours · Helper.cs lines 16-22

**Say.** "One line for the switch. It is a line you flip, not a setting you pass in. You decide
this once for your project, not once per run."

**Type.**

```csharp
    // Flip this to false to get the answer without the wait. A model that cannot think answers
    // Think: true with a 400, not with an answer, so leave it off on those.
    public static readonly bool? Think = true;

    // The thinking is grey so your eye skips it. These are ANSI colour codes, not a library.
    public static readonly string Grey = "\x1b[90m";
    public static readonly string Reset = "\x1b[0m";
```

**Note.** Say that those two strings are not a package. `\x1b[90m` tells the terminal "grey from
here", `\x1b[0m` tells it "back to normal". Every terminal has understood them for forty years.

**Proof.** The file builds, and nothing has changed in the output yet.

### 10. Already there — one HttpClient · Helper.cs lines 24-31

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

### 11. The JSON options get a second reason to exist · Helper.cs lines 33-41

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
```

**Note.** This is the step to slow down on. **Delete `DefaultIgnoreCondition` on camera.** Set
`Think = null`, run, and show the request going out as `"think": null` — which Ollama reads as
"no thinking", not as "I did not ask". Put the line back. `null` must mean **not on the wire**.
Measured by curl on this machine: no field gives 714 characters of thinking, `"think": null`
gives none at all.

**Proof.** Still builds.

### 12. Move the error throw out of Program.cs · Helper.cs lines 43-50

**Say.** "Chapter 01 threw this error inline, in the middle of the conversation. It is plumbing.
Cut it from `Program.cs` and paste it here, as a method."

**Type.** Cut the four lines out of `Program.cs`'s `if` block, then type this in `Helper.cs`.

```csharp
    // Ollama puts the real reason in the body — an unknown model, a model that cannot think.
    // Without this you get a bare 400 and no idea which of the two it was.
    public static async Task HandleResponseError(HttpResponseMessage response)
    {
        var body = await response.Content.ReadAsStringAsync();
        throw new HttpRequestException(
            $"{(int)response.StatusCode} {response.ReasonPhrase}: {body}");
    }
```

**Note.** The comment changed by one word — chapter 01 said "a bad field", this says "a model
that cannot think". That is step 23 arriving early. Say it, then move on.

**Proof.** `Program.cs` is shorter and still builds.

### 13. The stream reader · Helper.cs lines 52-84

**Say.** "This is the body of the chapter, and it lives in the helper, because reading a stream
is plumbing. Four small pieces. Run after each one."

**Type.** First the signature and the reader:

```csharp
    // Prints each piece the moment it arrives, and fills the caller's two builders. Printing and
    // remembering are two jobs, so the caller owns what it wants to keep.
    public static async Task HandleStream(
        HttpResponseMessage response,
        StringBuilder content,
        StringBuilder thinking)
    {
        // A stream is one JSON object per line. ReadLineAsync waits for a whole line, so a
        // network chunk that stops mid-line is not our problem here.
        using var reader = new StreamReader(await response.Content.ReadAsStreamAsync());
        while (await reader.ReadLineAsync() is { } line)
        {
            if (line.Length == 0) continue;

            Message part =
                JsonSerializer.Deserialize<ChatResponse>(line, AgentJsonOptions)!.Message;

            if (!string.IsNullOrEmpty(part.Thinking))
            {
                Console.Write(Grey + part.Thinking + Reset);
                thinking.Append(part.Thinking);
            }

            if (!string.IsNullOrEmpty(part.Content))
            {
                // The first word of the answer needs a gap after the grey, or they run together.
                if (content.Length == 0 && thinking.Length > 0) Console.Write("\n\n");
                Console.Write(part.Content);
                content.Append(part.Content);
            }
        }
    }
}
```

**Note.** Three things to say here, in this order.

One: the free thing C# gets, because the TypeScript video spends three lines on it.
`ReadLineAsync` waits for a whole line, so a network chunk that stops mid-line is not your
problem. In TypeScript it is.

Two: why the method takes two `StringBuilder`s instead of returning a string. You print each
piece the moment it arrives, but the caller's **list** needs the finished message. Printing and
remembering are two jobs, and the caller is the one that remembers.

Three: `is { } line` means "not null, and call it `line`". `ReadLineAsync` returns null at the
end of the stream, so that one pattern is also how the loop ends.

**Proof.** `Helper.cs` is 84 lines and the build is clean.

---

## Act 4 — Program.cs, four lines of difference · 9:00–10:30

### 14. The header and two `using` lines · Program.cs lines 1-8

**Say.** "New title. And `System.Text.Json` leaves — `JsonSerializer` lives in the helper now.
`System.Text` arrives, for the two builders."

**Type.** The top comment is new. Replace chapter 01's one `using` line with these two.

```csharp
// Chapter 01.A — thinking.
//
// Chapter 01 showed a model is an HTTP endpoint.
// This chapter shows it thinks before it answers, and that the reply arrives word by word.
// The thinking is grey. The answer is plain.

using System.Net.Http.Json;
using System.Text;
```

**Proof.** It still builds.

### 15. Already there — the set-up · Program.cs lines 10-14

**Say.** "Untouched."

**Already there.** Do not type it.

```csharp
List<Message> messages = [];

HttpClient http = Helper.OllamaHttpClient;

Console.WriteLine("Type a question. An empty line stops the app.");
```

**Proof.** —

### 16. Already there — the loop and the question · Program.cs lines 16-24

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

### 17. Two words on the request · Program.cs line 26

**Say.** "`Stream: false` becomes `Stream: true`, and we pass the switch."

**Type.**

```csharp
    ChatRequest request = new(Helper.Model, messages, Stream: true, Think: Helper.Think);
```

**Note.** Run it now, before you touch anything else. `PostAsJsonAsync` and
`ReadFromJsonAsync` choke: the body is 330 JSON objects glued together, and
`ReadFromJsonAsync` wants one. **Show the error and do not fix it.** The next steps are the
fix, and the viewer should want them.

**Proof.** A parse error on screen, caused by one word.

### 18. Send it by hand so you can read the body yourself · Program.cs lines 28-37

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

### 19. The error check is two lines now · Program.cs lines 39-40

**Say.** "Same check. The four lines that did the work moved into the helper in step 12."

**Type.**

```csharp
    if (!response.IsSuccessStatusCode)
        await Helper.HandleResponseError(response);
```

**Note.** Point at what `Program.cs` reads like now: send, check, stream, remember. Four verbs,
no plumbing. That is what moving the two methods out bought.

**Proof.** The `if` block is one line long.

### 20. Two builders, a label, and one call · Program.cs lines 42-47

**Say.** "Two `StringBuilder`s, because printing and remembering are two jobs. Then one call."

**Type.** Delete chapter 01's `ReadFromJsonAsync` and `Console.WriteLine` lines. Type this in
their place.

```csharp
    StringBuilder content = new();
    StringBuilder thinking = new();

    Console.Write($"{Helper.Model}: ");

    await Helper.HandleStream(response, content, thinking);
```

**Note.** Say what the label buys. On screen you now get `You:` and then `gemma4:e2b:`, so the
viewer can always tell who is talking — and the model name is right there when you swap it in
step 23.

**Proof.** Grey text, then an answer. The chapter works from here.

### 21. Close the turn · Program.cs lines 49-53

**Say.** "The reply goes back into the list. Both halves of it."

**Type.** Replace chapter 01's `messages.Add(reply.Message)` with these.

```csharp
    Console.WriteLine();

    // The reply goes back into the list. The model remembers nothing, so the list is the memory.
    messages.Add(new Message("assistant", content.ToString(), thinking.ToString()));
}
```

**Proof.** `Program.cs` is 53 lines and the build is clean.

---

## Act 5 — Break it, and hand off · 10:30–12:00

### 22. Run it, two turns

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

**Note.** Measured on this machine, `gemma4:e2b`: those two turns took **11.8 s** including
`dotnet run`, and the answers were `Perceives, reasons, acts, achieves goals.` then
`Senses, plans, acts.` Point at the grey on turn two — it says, out loud, *"The previous answer
was 'Perceives, reasons, acts, achieves goals'."* The list is the memory, and you can read it
happening on screen.

**Proof.** Grey text scrolling, a blank line, an answer, twice.

### 23. Flip the switch off

**Say.** "One line. That is the whole flag."

**Type.** In `Helper.cs`, change one word:

```csharp
    public static readonly bool? Think = false;
```

Then run and ask the same question:

```bash
dotnet run
```

**Note.** Measured on this machine, same question: **6.2 s** with thinking on, **1.5 s** with it
off, and the fast answer was `Autonomous decision-making intelligent software.` — no worse than
the slow one. Say that plainly. Thinking is not free and it is not always better. You now own
the choice.

Put the line back to `true` before the next step.

**Proof.** No grey. An answer, four times faster.

### 24. Break it on a model that cannot think

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
wire, and the same model answers. That is the payoff for step 11.

If you have ten spare seconds, run `MODEL=nope:1b dotnet run` too. It gives
`404 Not Found: {"error":"model 'nope:1b' not found"}` — the same helper, a different mistake.

**Proof.** The 400, then the same model answering.

### 25. Hand off

**Say.** "It streams, and you choose whether it thinks. But it still has no idea who it is or
what it is for. Next video we tell it, in one block of text."

**Proof.** Stop recording.

---

## After the take

1. Run **P7** above to restore the code.
2. Check `Helper.cs` really is back to `Think = true` — steps 11, 23 and 24 all change that line.
3. Check `ChatProtocol.cs` exists and the three files build clean.
4. Write the real run numbers and the machine into [`docs/PROGRESS.md`](../../docs/PROGRESS.md).
5. Tick the boxes in the chapter [`README.md`](README.md) "Done when" list.
