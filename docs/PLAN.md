# The plan

One decision per line. We follow this file. When something changes, we change the file.

## What the course is

A YouTube series plus this repo. Nine videos. Each video builds **one component** of an AI agent.
Two stacks, same behaviour: **TypeScript** and **C#**. The model runs **on your machine**.

Audience: working developers who have never built an agent. Self-study.

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

## The nine videos

Target: 12-15 minutes. Two of them may run to 20.
"New code" is the amount you type on camera. If a chapter grows past it, we split the chapter.

| # | Chapter | What the viewer learns | New code | Minutes |
|---|---|---|---|---|
| 00 | `the-agent-we-build` | the finished demo, and the six parts of an agent | none | 8 |
| 01 | `the-model` | raw HTTP to a local model. It only reads text and writes text. | ~40 lines | 12 |
| 02 | `the-prompt` | the system prompt. The cheapest way to change behaviour. | ~20 lines | 12 |
| 03 | `tools-by-hand` | ask for JSON, parse it, run a function. Tool calling is just text. | ~70 lines | 15 |
| 04 | `the-loop` | the native `tools` field, and the while-loop. Now it is an agent. | ~60 lines | 15 |
| 05 | `memory` | short memory (the message list) and long memory (a notes file) | ~50 lines | 15 |
| 06 | `guardrails` | step cap, timeout, path allow-list, ask before you write | ~60 lines | 15 |
| 07 | `llama-cpp` | swap the runner. Flags, quantisation, context size. | ~10 lines | 20 |
| 08 | `side-by-side` | TypeScript against C#: what differs, what does not | none | 20 |
| 09 | `frameworks` *(optional, text only)* | the same agent in a framework, and what it hid | — | — |

Chapter 03 before chapter 04 is on purpose. By hand first, so the viewer sees that a "tool call"
is a model writing some JSON and nothing more. Then the native field, which is the same trick
with a nicer wrapper — and the fallback when a small model has no tool support.

## Models

| Chapter | Runner | Model | Why |
|---|---|---|---|
| 01-06 | Ollama | `gemma4:e4b` | one install, tool calling works, 9.6 GB |
| 01-06 | Ollama | `qwen3.5:4b` | the fallback for a smaller machine |
| 07 | llama.cpp | `Ornith-1.0-9B-MTP` (GGUF) | not in the Ollama library, so it earns the hard runner |

Floor: 16 GB RAM. Verified on an M1 Pro / 32 GB, 2026-10-25: `gemma4:e4b` returned a correct
`list_files` tool call in 1.2 s after load.

## Code layout

**One project per language, and it grows.** Not a copy per chapter.

Each chapter ends on a **git tag**: `ch01-model`, `ch02-prompt`, and so on. A viewer who lands on
video 5 runs `git checkout ch05-memory` and has working code. On camera you can run
`git diff ch03-tools-by-hand..ch04-loop` to show exactly what changed.

```
ai-agents-1on1/
  README.md                   the front door
  docs/
    PLAN.md                   this file
    PROGRESS.md               the board: one row per chapter
    SETUP.md                  install Ollama, pull the model, run both stacks
    STYLE.md                  how we write the docs and the code
  chapters/NN-name/
    README.md                 the lesson: what we solve, example, why it matters
    SCRIPT.md                 the camera script: Goal / Why / Do / Proof, per step
    QA.md                     questions a viewer should be able to answer
  ts/                         one TypeScript project, grows per chapter
  csharp/                     one C# project, grows per chapter
  sample-repo/                the folder the Librarian reads
  scripts/                    ollama.sh, llama.sh
```

`ts` and `csharp` do the same thing in every chapter: same tools, same prompt, same output.
Each side stays natural for its language. Where they must differ, `chapters/NN/README.md` says why.

## Still open

Two choices we make later, when we see the code. Nothing in this plan depends on them.

1. **Tags or folders.** We start with tags. If viewers trip on `git checkout`, we export each tag
   to a folder. The layout above survives either way.
2. **One video or two per chapter.** Each chapter README keeps TypeScript and C# in separate
   sections, so splitting a video later costs nothing.
3. **Chapter 09, frameworks.** Simple wins. If it adds noise, it stays a text appendix.

## Order of work

1. `docs/` and the chapter skeletons. **Done.**
2. Chapter 01 code, both stacks, running. **Done.**
3. `sample-repo` with something worth asking questions about.
4. Chapters 02-04. That is the heart of the course — get these right before anything else.
5. Record 00 only after 04 works. The demo in video 00 must be the real thing.
