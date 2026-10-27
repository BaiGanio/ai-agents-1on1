# Chapter 01 — camera script, C#

Target 12 minutes. Read this file top to bottom while you record. Nothing else needs to be open:
every command and every line of code is here.

Every step is **Say / Type / Proof**, and some add a **Note** — the one sentence you must not
forget. **Type** is exactly what goes on air. The finished files are
[`code/csharp/Helper.cs`](code/csharp/Helper.cs) and
[`code/csharp/Program.cs`](code/csharp/Program.cs), and each code step says which lines of them
you are typing, so a bad take is one paste away.

Two files, on purpose. `Helper.cs` is the plumbing: where the model lives, one HTTP client, one
set of JSON options, three records. `Program.cs` is only the conversation, and it reads top to
bottom. Say that out loud at step 6.

The TypeScript video is [`SCRIPT-ts.md`](SCRIPT-ts.md). Steps 1, 2, 3 and 4 are shared with it
word for word, so a TypeScript viewer never has to watch this one.

| Act | Steps | Minutes |
|---|---|---|
| 0 — the promise | 1 | 0:00–1:00 |
| 1 — the endpoint, by curl | 2–4 | 1:00–3:30 |
| 2 — Helper.cs, the plumbing | 5–9 | 3:30–6:30 |
| 3 — Program.cs, the conversation | 10–14 | 6:30–9:30 |
| 4 — break it, and hand off | 15–18 | 9:30–12:00 |

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

## Act 2 — Helper.cs, the plumbing · 3:30–6:30

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

### 6. Start Helper.cs — where the model lives · Helper.cs lines 1-13

**Say.** "Two files. This one is the plumbing, and you write it once and forget it. The other
one is the conversation, and it reads top to bottom."

**Type.** Make a new file, `Helper.cs`.

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

**Note.** Both read the environment first, with a default. Nobody has to edit code to point at
another machine or swap the model. Chapter 10 leans on that.

**Proof.** The file exists.

### 7. One HttpClient · Helper.cs lines 15-22

**Say.** "One client for the whole process. A new one per call leaks sockets."

**Type.**

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

**Note.** The default timeout is 100 seconds. A local model can take minutes on the first
answer while it loads into RAM. Say what that failure looks like: a network error that is
really a slow disk.

**Proof.** —

### 8. The JSON options · Helper.cs lines 24-31

**Say.** "Ollama's field names are lower case. C# properties are not. We say so once, here,
and never think about it again."

**Type.**

```csharp
    public static readonly JsonSerializerOptions AgentJsonOptions =
        new()
        {
            // Ollama's field names are lower case. C# properties are not, so we say so once, here.
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        };
}
```

**Note.** Both options matter. `PropertyNamingPolicy` maps the names.
`DefaultIgnoreCondition` keeps empty fields out of the request body.
We come back and delete the first one at step 15.

**Proof.** The class closes. `Helper.cs` is 31 lines so far.

### 9. The three records · Helper.cs lines 33-35

**Say.** "Three lines for the whole protocol. No classes, no properties, no boilerplate."

**Type.**

```csharp
record Message(string Role, string Content);
record ChatRequest(string Model, List<Message> Messages, bool Stream);
record ChatResponse(Message Message);
```

**Note.** They sit outside the class, at the bottom of the file. That is the whole API shape:
one message in a list, one message back.

**Proof.** `Helper.cs` is 35 lines and `Helper.cs` alone builds.

---

## Act 3 — Program.cs, the conversation · 6:30–9:30

### 10. Clear Program.cs and set up · Program.cs lines 1-14

**Say.** "Delete the hello world. Three lines of set-up, and all three come from `Helper`."

**Type.** Open `Program.cs`, delete everything, then type this.

```csharp
// Chapter 01 — the model.
//
// The whole point of this file: a model is an HTTP endpoint.
// You send it a list of messages.
// It sends back one more message.
// There is no magic, no SDK, and no state on the server.

using System.Net.Http.Json;

List<Message> messages = [];

HttpClient http = Helper.OllamaHttpClient;

Console.WriteLine("Type a question. An empty line stops the app.");
```

**Note.** Stop at `List<Message> messages = []`. Say: **"a list, not a string. The model gets
the whole story every time, because it remembers nothing."** That line is the seed of
chapter 06.

**Proof.** The hello world is gone.

### 11. The loop and the question · Program.cs lines 16-24

**Say.** "No command-line arguments. You talk to it, like a chat."

**Type.**

```csharp
while (true)
{
    Console.Write("You: ");

    // ReadLine gives null when input ends, so this one check covers both quit and end of input.
    string? prompt = Console.ReadLine();
    if (string.IsNullOrWhiteSpace(prompt)) break;

    messages.Add(new("user", prompt));
```

**Note.** `ReadLine` gives `null` when input ends, so one check covers both an empty line and
a piped file that ran out. Say that — it is why there is no second `if`.

**Proof.** —

### 12. The request · Program.cs lines 26-37

**Say.** "This is the curl. Same URL, same body, same `Stream: false`."

**Type.**

```csharp
    ChatRequest request = new(Helper.Model, messages, Stream: false);

    var response = await http.PostAsJsonAsync("/api/chat", request, Helper.AgentJsonOptions);

    if (!response.IsSuccessStatusCode)
    {
        // Ollama puts the real reason in the body — an unknown model, a bad field. Without this
        // line you get a bare 500 and no idea which of the two it was.
        var body = await response.Content.ReadAsStringAsync();
        throw new HttpRequestException(
            $"{(int)response.StatusCode} {response.ReasonPhrase}: {body}");
    }
```

**Note.** `Stream: false` means one JSON object at the end, not a drip of words. Streaming is
nicer for a user and noisier for a lesson, so it waits until chapter 01.A. And the status check
exists because Ollama puts the real reason in the body — without it you get a bare 500.

**Proof.** —

### 13. The reply, printed and remembered · Program.cs lines 39-46

**Say.** "Read the one message it wrote, print it, and put it back in the list."

**Type.**

```csharp
    var reply = await response.Content.ReadFromJsonAsync<ChatResponse>(Helper.AgentJsonOptions);

    Console.WriteLine(
        $"{Helper.Model} (role: {reply!.Message.Role}): {reply.Message.Content.Trim()}");

    // The reply goes back into the list. The model remembers nothing, so the list is the memory.
    messages.Add(reply.Message);
}
```

**Note.** The last line is the one to linger on. The reply goes into the same list the next
question goes into, so turn two sees turn one. That is the only memory an agent gets until
chapter 06.

**Proof.** `Program.cs` is 46 lines and the build is clean.

### 14. Run it

**Say.** "No arguments, no flags. It asks you."

**Type.**

```bash
dotnet run
```

Then type two questions that lean on each other, and an empty line to stop:

```text
In five words, what is an AI agent?
And in three?
```

**Note.** The second answer proves the list is the memory. Point at it.

**Proof.** Two answers in the terminal, the second one shorter, from your own code.

---

## Act 4 — Break it, and hand off · 9:30–12:00

### 15. Delete the camelCase option

**Say.** "This is the one that bites every C# viewer. Watch."

**Type.** In `Helper.cs`, delete this one line:

```csharp
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
```

Then run it and ask anything:

```bash
dotnet run
```

Put the line back and run again.

**Note.** The code is right and the names are wrong, so the reply parses into nothing. Say
that out loud — it is not an HTTP problem.

**Proof.** A `NullReferenceException` on screen, then a working answer.

### 16. Kill the server

**Say.** "This is the error you will hit first. Here is what it looks like."

**Type.** In the `ollama serve` window, press `Ctrl+C`. Then run and ask anything:

```bash
dotnet run
```

Then start the server again.

**Proof.** A connection-refused error on screen.

### 17. Let it make something up

**Say.** "It has never seen this folder. Watch it answer anyway."

**Type.** At the `You:` prompt:

```text
Which file starts the HTTP server?
```

**Note.** This is the reason the rest of the course exists. Do not fix it here.

**Proof.** The model invented a filename, on camera.

### 18. Hand off

**Say.** "Five words took six seconds, and you saw why in the `thinking` field. Next video you
get a flag for that, and you watch the answer arrive word by word."

**Proof.** Stop recording.

---

## After the take

1. Run **P7** above to restore the code.
2. Write the real run numbers and the machine into [`docs/PROGRESS.md`](../../docs/PROGRESS.md).
3. Tick the boxes in the chapter [`README.md`](README.md) "Done when" list.
