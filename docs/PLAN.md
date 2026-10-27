# The plan

One decision per line. We follow this file. When something changes, we change the file.

## What the course is

A YouTube series plus this repo. Thirteen chapters. Most build **one component** of an AI agent.
One (08) shows agentic workflows — the topic most job ads now name.
One (10) puts the models side by side, so a viewer can pick one for their machine.
Two stacks, same behaviour: **TypeScript** and **C#**. The model runs **on your machine**.

Audience: junior and working developers who have never built an agent. Self-study. They can
use a terminal and have written a small program. Each chapter has a `BASICS.md` for what it
needs first, new words are in `docs/GLOSSARY.md`, and `scripts/check.sh` tells them what is
missing.

This repo is the **simple sibling** of [`BaiGanio/agent-ready-payments`](https://github.com/BaiGanio/agent-ready-payments).
That one goes deep: Postgres, SQL Server, MCP, EF Core, refunds, human approval. This one has
no database and no domain. Just the agent.

## The agent we build: Repo Librarian

It reads a folder of code and answers questions about it.

Three tools, and that is all:

| Tool | What it does |
|---|---|
| `list_files(path)` | names the files in a folder |
| `read_file(path)` | gives back the text of one file |
| `search_text(query)` | finds the lines that contain some text |

Why this one. No API key. No network. No fake server. It works offline, like the model does.
Every viewer has a folder of files. The tools are 10 lines each in both languages.

## The thirteen chapters

Target: 12-15 minutes per chapter. Two of them may run to 20.
Chapters 01-08 (and 01.A) are two videos each, one per language (see `STYLE.md`).
Minutes below are per video.
"New code" is the amount you type on camera. It is a guide, not a cap: a chapter may go well
past it. Two real limits sit on top of it. **No source file goes over 200 lines** — past that,
pull a part out into its own file and keep the chapter whole. And a chapter splits on **minutes**,
never on lines.

| # | Chapter | What the viewer learns | New code | Minutes |
|---|---|---|---|---|
| 00 | `the-agent-we-build` | the finished demo, and the six parts of an agent | none | 8 |
| 01 | `the-model` | raw HTTP to a local model. It only reads text and writes text. | ~40 lines | 12 |
| 01.A | `thinking` | the model thinks before it answers. One switch in your own code, and streaming so you watch it live. | ~30 lines | 12 |
| 02 | `the-prompt` | the system prompt. The cheapest way to change behaviour. | ~20 lines | 12 |
| 04 | `tools-by-hand` | ask for JSON, parse it, run a function. Tool calling is just text. | ~70 lines | 15 |
| 05 | `the-loop` | the native `tools` field, and the while-loop. Now it is an agent. | ~60 lines | 15 |
| 06 | `memory` | short memory (the message list) and long memory (a notes file) | ~50 lines | 15 |
| 07 | `guardrails` | step cap, timeout, path allow-list, ask before you write | ~60 lines | 15 |
| 08 | `workflows` | agentic workflows: chain, router, checker. Code picks the path, or the model does. | ~60 lines | 15 |
| 09 | `llama-cpp` | swap the runner. Flags, quantisation, context size. | ~10 lines | 20 |
| 10 | `choosing-a-model` | five models: file size, RAM per context, speed, vision. Pick one for your machine. | none | 15 |
| 11 | `side-by-side` | TypeScript against C#: what differs, what does not | none | 20 |
| 12 | `frameworks` *(optional, text only)* | the same agent in a framework, and what it hid | — | — |

Chapter 01.A comes right after the model on purpose. The default model thinks before it
answers, and the viewer waits for it from the first call. So they meet the switch first, and
decide for themselves whether to see the thinking, stream it, or turn it off. It is called
01.A, not 02, so the chapters after it keep their numbers.

**There is no chapter 03.** It was a chat page, and it is dropped (decision 35). The console is
the only client in this course. The numbers after it keep their place — 02 is followed by 04 —
because a renumber touches five folder names, every heading, every tag and every "chapter NN" in
the docs and the code. The gap is cheaper than the churn.

So the window onto the agent is the terminal. Every chapter from 04 on prints its tool calls
there, in the same `You:` loop the viewer already knows from chapter 01.

Chapter 08 comes after guardrails on purpose. A workflow runs several model calls, so each one
needs the step cap and timeout from 07. It reuses the Librarian's tools — no new domain.

Chapter 04 before chapter 05 is on purpose. By hand first, so the viewer sees that a "tool call"
is a model writing some JSON and nothing more. Then the native field, which is the same trick
with a nicer wrapper — and the fallback when a small model has no tool support.

## Models

| Chapter | Runner | Model | Why |
|---|---|---|---|
| 01-08 | Ollama | `gemma4:e2b` | **the default.** Tool calling works, vision, about 8 GB of RAM at any context |
| 01-08 | Ollama | `gemma4:e4b` | the step up when e2b trips on a hard question |
| 09 | llama.cpp | `Ornith-1.0-9B-MTP` (GGUF) | not in the Ollama library, so it earns the hard runner |
| 10 | both | the above, plus `gemma4:26b` and `qwen3.5:4b` | the comparison |

Floor: 16 GB RAM. The full table — file, RAM per context, tools, vision — is in
[`MODELS.md`](MODELS.md). Verified on an M1 Pro / 32 GB, 2026-10-25: `gemma4:e2b` returned a
correct `list_files` tool call in 2.8 s warm.

## Code layout

One folder per chapter (decision 20): `chapters/NN-name/code/ts/` and `code/csharp/`.

**How a chapter is made.** At recording, each chapter starts as a copy of the code from the
chapter before it. Only the new code is typed on camera. That way each chapter shows clearly
what it adds, and in what order — first, middle, last.

When recording is done, we pick one of two shapes: a folder of code per chapter, or one project
that grows with a git tag per chapter. Git tags made before then are not final and may be
deleted.

```
ai-agents-1on1/
  README.md                   the front door
  docs/
    PLAN.md                   this file
    PROGRESS.md               the board: one row per chapter
    SETUP.md                  install Ollama, pull the model, run both stacks
    STYLE.md                  how we write the docs and the code
    GLOSSARY.md               every new word, in one line
    MODELS.md                 which model fits which machine: file, RAM per context, vision
  chapters/NN-name/
    README.md                 the lesson: what we solve, example, why it matters
    SCRIPT-ts.md              the TypeScript camera script: Goal / Why / Do / Proof
    SCRIPT-csharp.md          the C# camera script. Shared steps are word for word.
    QA.md                     questions a viewer should be able to answer
    BASICS.md                 what a junior must know before this chapter
    code/ts/                  the TypeScript code at the end of this chapter
    code/csharp/              the C# code at the end of this chapter
    code/prompts/             the system prompts. Both stacks read this one folder.
  sample-repo/                the folder the Librarian reads
  scripts/                    check.sh, measure.ts, ollama.sh, llama.sh
```

`ts` and `csharp` do the same thing in every chapter: same tools, same prompt, same output.
Each side stays natural for its language. Where they must differ, `chapters/NN/README.md` says why.

## Still open

Two choices we make later, when we see the code. Nothing in this plan depends on them.

1. **Code layout.** A folder per chapter, or one project with tags. See "Code layout" above.
2. **Chapter 12, frameworks.** Simple wins. If it adds noise, it stays a text appendix.

## Order of work

1. `docs/` and the chapter skeletons. **Done.**
2. Chapter 01 code, both stacks, running. **Done.**
3. `sample-repo` with something worth asking questions about.
4. Chapters 02, 04 and 05. That is the heart of the course — get these right before anything
   else.
5. Record 00 only after 05 works. The demo in video 00 must be the real thing.
