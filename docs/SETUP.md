# Setup

About 20 minutes, most of it the model download.

Every command here is copy-paste. If one fails, the table at the bottom says why.

---

## 1. Install the tools

```bash
brew install ollama node dotnet
```

Check all three:

```bash
node --version      # v24 or higher
dotnet --version    # 10.x
ollama --version    # 0.40 or higher
```

**Windows or Linux.** Ollama from https://ollama.com/download, Node from https://nodejs.org,
the .NET SDK from https://dot.net. Everything after this point is the same, except where a
box says otherwise.

---

## 2. Run the model server

Ollama is two things: a **server** that holds the model, and a **CLI** that talks to it.
Everything in this course talks to the server over HTTP on port `11434`.

### Start it

```bash
ollama serve
```

It prints log lines and stays in the foreground. **Leave this terminal alone** and open a new
one for everything else.

### Or start it in the background

Better for recording, because you do not burn a terminal:

```bash
brew services start ollama
```

> **Windows or Linux.** No `brew services`. On Windows the installer runs it as a background
> service already. On Linux: `systemctl --user start ollama`.

### Check it is up

```bash
curl http://localhost:11434/api/version
# {"version":"0.40.6"}
```

That one command is the only real test. If it answers, you are fine.

### Stop it

| How you started it | How you stop it |
|---|---|
| `ollama serve` in a terminal | `Ctrl+C` in that terminal |
| `brew services start ollama` | `brew services stop ollama` |
| you have no idea | `pkill ollama` |

### When the port is already taken

`ollama serve` says **`address already in use`**. That means a server is already running —
usually fine, just use it. If you want it gone, find it and kill it:

```bash
lsof -nP -iTCP:11434 -sTCP:LISTEN
# COMMAND   PID USER ...
# ollama  47376   lk  ... TCP 127.0.0.1:11434 (LISTEN)

kill 47376          # the PID from the line above
kill -9 47376       # only if a plain kill does not do it
```

One command for the impatient — it kills every Ollama process you own:

```bash
pkill ollama
```

> **Windows.** `netstat -ano | findstr 11434` to get the PID, then `taskkill /PID <pid> /F`.

### Free the RAM without stopping the server

A loaded model sits in RAM. See what is loaded:

```bash
ollama ps
# NAME         ID   SIZE   PROCESSOR   CONTEXT   UNTIL
# gemma4:e4b   ...  11 GB  100% GPU    8192      4 minutes from now
```

An empty table means nothing is loaded. Push one out now instead of waiting for the timeout:

```bash
ollama stop gemma4:e4b
```

`ollama stop` unloads a **model**. It does not stop the **server**. Different things.

### Read the logs

When the server misbehaves, the reason is here:

```bash
tail -f ~/.ollama/logs/server.log
```

---

## 3. Pull the model

```bash
ollama pull gemma4:e4b      # 9.6 GB — the default for this course
```

**Under 16 GB of RAM?** Use the small one:

```bash
ollama pull qwen3.5:4b      # 3.4 GB
```

Then tell the code to use it. Both stacks read this variable:

```bash
export MODEL=qwen3.5:4b
```

See what you have, and remove one you do not want:

```bash
ollama list
ollama rm qwen3.5:4b
```

---

## 4. Check that tool calling works

This is the one thing that breaks on small models, and chapters 04 onwards depend on it.
Test it **before** you start.

```bash
curl -s http://localhost:11434/api/chat -d '{
  "model": "gemma4:e4b",
  "stream": false,
  "messages": [{"role":"user","content":"List the files in the current folder."}],
  "tools": [{"type":"function","function":{
    "name":"list_files","description":"List files in a folder",
    "parameters":{"type":"object","properties":{"path":{"type":"string"}},"required":["path"]}}}]
}' | python3 -m json.tool
```

You want a `tool_calls` array:

```json
"message": {
  "role": "assistant",
  "content": "",
  "tool_calls": [{"function": {"name": "list_files", "arguments": {"path": "."}}}]
}
```

If you get prose instead, that model cannot call tools. Pick another.
Chapter 04 builds the by-hand fallback for exactly this case.

---

## 5. The TypeScript project

Already in this repo at `ts/`. **Skip to "Run it"** unless you are following the video and
making it yourself.

### Create it from nothing

```bash
mkdir -p ts/src && cd ts
npm init -y
npm pkg set type=module name=repo-librarian private=true
npm pkg set scripts.start="node src/main.ts"
npm install -D @types/node
```

Then the two files: `src/main.ts` (the code) and `tsconfig.json` (for the editor only —
nothing compiles it).

There are **no runtime dependencies**, on purpose. `fetch` is built into Node.
`@types/node` is only so your editor knows what `process` is.

### Run it

```bash
cd ts
node src/main.ts "In five words, what is an AI agent?"
```

Or through the script, which is the same thing:

```bash
npm start -- "In five words, what is an AI agent?"
```

Node 24 and up runs `.ts` files directly. **No build step.** No `tsc`, no watcher, no `dist/`.
It strips the types and runs the JavaScript.

Type-check when you want to, which is a separate job from running:

```bash
npx tsc --noEmit
```

---

## 6. The C# project

Already in this repo at `csharp/`. **Skip to "Run it"** unless you are making it yourself.

### Create it from nothing

```bash
mkdir -p csharp && cd csharp
dotnet new console -n RepoLibrarian -o . -f net10.0
```

`-o .` puts the files in the folder you are in instead of a new nested one.
That gives you `RepoLibrarian.csproj` and `Program.cs`. Replace `Program.cs` with the code.

No NuGet packages. `HttpClient` and `System.Text.Json` are in the SDK.

### Run it

```bash
cd csharp
dotnet run -- "In five words, what is an AI agent?"
```

The `--` matters. Without it, `dotnet run` eats your words as its own flags.

From the repo root, without changing folder:

```bash
dotnet run --project csharp -- "In five words, what is an AI agent?"
```

The first run restores and compiles, so it is slow. Later runs are fast.
To skip the build check when you know nothing changed:

```bash
dotnet run --no-build -- "your question"
```

When a build goes strange, clear it out:

```bash
dotnet clean && rm -rf bin obj
```

`bin/` and `obj/` are in `.gitignore`. Never commit them.

---

## 7. Settings

Both stacks read the same two environment variables. Neither is required.

| Variable | Default | What it is |
|---|---|---|
| `OLLAMA_URL` | `http://localhost:11434` | where the model server listens |
| `MODEL` | `gemma4:e4b` | the model to use |

For one command only:

```bash
MODEL=qwen3.5:4b node src/main.ts "hello"
```

For the whole terminal:

```bash
export MODEL=qwen3.5:4b
```

---

## 8. One command to check everything

```bash
./scripts/check.sh
```

It checks Node, .NET, the server, the model, and that the model can call tools. Each failed
line says how to fix it. To check the two stacks by hand instead:

```bash
curl -s http://localhost:11434/api/version && \
  node ts/src/main.ts "Say ok." && \
  dotnet run --project csharp -- "Say ok."
```

Three answers means you are ready for chapter 01.

---

## When it does not work

| What you see | What it means | What to do |
|---|---|---|
| `ECONNREFUSED` / `Connection refused` | the server is not running | `ollama serve` |
| `address already in use` | it is already running | nothing. Or `pkill ollama` to restart it. |
| `model "..." not found` | you did not pull it | `ollama pull gemma4:e4b` |
| first answer takes 30 seconds | the model is loading into RAM | wait. The next one is fast. |
| the whole Mac goes slow | the model does not fit in RAM | `ollama stop <model>`, then use `qwen3.5:4b` |
| `Unknown option: --` in dotnet | you dropped the `--` | `dotnet run -- "your words"` |
| TypeScript error about `process` | `@types/node` is missing | `npm install -D @types/node` |
| C# answer is `null` | the JSON names did not match | keep `PropertyNamingPolicy = CamelCase` |
| prose where you expected `tool_calls` | that model cannot call tools | use `gemma4:e4b`, or see chapter 04 |
