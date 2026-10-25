# AI Agents, one on one

Build an AI agent from nothing. No framework. No cloud. No API key.
The model runs on your laptop. You write about 300 lines of code, in **TypeScript** and in **C#**.

Nine short videos, one per component. The code in this repo is the finished code of each video.

## An agent is six parts

| Part | What it is |
|---|---|
| **the model** | the brain. It only reads text and writes text. It can do nothing by itself. |
| **the prompt** | one block of text that says who the agent is and what rules it follows |
| **the tools** | small functions the model may ask for. *Your* code runs them, not the model. |
| **the loop** | your code: send, run the tool it asked for, send the result back, repeat |
| **the memory** | short — the message list in this run. Long — notes saved to a file. |
| **the guardrails** | a step cap, a timeout, an allow-list, and a yes/no before anything risky |

That is the whole thing. A framework does not add a seventh part. It hides the loop.

## What we build: the Repo Librarian

An agent that reads a folder of code and answers questions about it.
Three tools: `list_files`, `read_file`, `search_text`.

```
you  > Which file starts the HTTP server?
agent> [list_files "."]  [search_text "listen"]  [read_file "src/main.ts"]
agent> src/main.ts, line 14. It calls server.listen on the port from PORT.
```

## Start here

1. [`docs/SETUP.md`](docs/SETUP.md) — install Ollama, pull the model, run both stacks
2. [`chapters/`](chapters/) — the lessons, in order
3. [`docs/PLAN.md`](docs/PLAN.md) — what the course is and why it is shaped this way

Each chapter ends on a git tag. To get the code as it stood at the end of video 4:

```bash
git checkout ch04-loop
```

## The chapters

| # | Chapter | What you learn |
|---|---|---|
| 00 | [the agent we build](chapters/00-the-agent-we-build/) | the demo, and the six parts |
| 01 | [the model](chapters/01-the-model/) | raw HTTP to a model on your machine |
| 02 | [the prompt](chapters/02-the-prompt/) | the system prompt, and why it is cheap |
| 03 | [tools by hand](chapters/03-tools-by-hand/) | a tool call is just the model writing JSON |
| 04 | [the loop](chapters/04-the-loop/) | the while-loop that makes it an agent |
| 05 | [memory](chapters/05-memory/) | what to keep, what to drop, what to save |
| 06 | [guardrails](chapters/06-guardrails/) | stop it looping forever or doing damage |
| 07 | [llama.cpp](chapters/07-llama-cpp/) | swap the runner and tune it |
| 08 | [side by side](chapters/08-side-by-side/) | TypeScript against C# |
| 09 | [frameworks](chapters/09-frameworks/) | the same agent in a framework *(optional)* |

## Going deeper

This repo is small on purpose. For the production shape of the same ideas — a database, MCP,
idempotency, human approval of a refund — see
[**BaiGanio/agent-ready-payments**](https://github.com/BaiGanio/agent-ready-payments).
Its lab 09 is this course's chapter 04, with everything a payment platform adds.

## You will need

- **Node 24+** and the **.NET 10 SDK**
- **Ollama**, and about 10 GB of disk for the model
- **16 GB of RAM.** Less works with a smaller model — `docs/SETUP.md` says which.
