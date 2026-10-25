# AI Agents, one on one

Build an AI agent from nothing. No framework. No cloud. No API key.
The model runs on your laptop. You write about 300 lines of code, in **TypeScript** and in **C#**.

Twelve chapters. Most build one part of the agent, in two short videos: one TypeScript, one
C#. Watch the language you use. One chapter gives the agent a chat page. One shows agentic
workflows: when your code should pick the path, and when the model should.

The code in this repo is the finished code of each chapter.

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

An **agentic workflow** is several of these calls wired together by your code. In a workflow,
your code picks the path. In an agent, the model picks it. Chapter 08 builds both and says
when to use which.

## What we build: the Repo Librarian

An agent that reads a folder of code and answers questions about it.
Three tools: `list_files`, `read_file`, `search_text`.

```
you  > Which file starts the HTTP server?
agent> [list_files "."]  [search_text "listen"]  [read_file "src/main.ts"]
agent> src/main.ts, line 14. It calls server.listen on the port from PORT.
```

## Start here

1. New to HTTP or JSON? Read [`docs/BASICS.md`](docs/BASICS.md) first. Ten minutes.
2. [`docs/SETUP.md`](docs/SETUP.md) — install Ollama, pull the model, run both stacks
3. Run `./scripts/check.sh`. It says what is missing, and how to fix it.
4. [`chapters/`](chapters/) — the lessons, in order
5. [`docs/PLAN.md`](docs/PLAN.md) — what the course is and why it is shaped this way
6. A word you do not know? [`docs/GLOSSARY.md`](docs/GLOSSARY.md)

Each chapter ends on a git tag. To get the code as it stood at the end of chapter 05:

```bash
git checkout ch05-loop
```

## The chapters

| # | Chapter | What you learn |
|---|---|---|
| 00 | [the agent we build](chapters/00-the-agent-we-build/) | the demo, and the six parts |
| 01 | [the model](chapters/01-the-model/) | raw HTTP to a model on your machine |
| 02 | [the prompt](chapters/02-the-prompt/) | the system prompt, and why it is cheap |
| 03 | [the chat page](chapters/03-the-chat-page/) | one HTML page to talk to the agent, served by both stacks |
| 04 | [tools by hand](chapters/04-tools-by-hand/) | a tool call is just the model writing JSON |
| 05 | [the loop](chapters/05-the-loop/) | the while-loop that makes it an agent |
| 06 | [memory](chapters/06-memory/) | what to keep, what to drop, what to save |
| 07 | [guardrails](chapters/07-guardrails/) | stop it looping forever or doing damage |
| 08 | [workflows](chapters/08-workflows/) | agentic workflows: chain, router, checker. Workflow or agent? |
| 09 | [llama.cpp](chapters/09-llama-cpp/) | swap the runner and tune it |
| 10 | [side by side](chapters/10-side-by-side/) | TypeScript against C# |
| 11 | [frameworks](chapters/11-frameworks/) | the same agent in a framework *(optional)* |

## Going deeper

This repo is small on purpose. For the production shape of the same ideas — a database, MCP,
idempotency, human approval of a refund — see
[**BaiGanio/agent-ready-payments**](https://github.com/BaiGanio/agent-ready-payments).
Its lab 09 is this course's chapter 05, with everything a payment platform adds.

## You will need

- **Node 24+** and the **.NET 10 SDK**
- **Ollama**, and about 10 GB of disk for the model
- **16 GB of RAM.** Less works with a smaller model — `docs/SETUP.md` says which.
