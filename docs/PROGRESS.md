# Progress

One row per chapter. Update it when a state changes. This is the first file to open each session.

**Legend.** `—` not started · `docs` lesson written · `code` code written and running ·
`ready` docs + code + both scripts agree with each other and with the chapters after it
(decision 19) · `shot` video recorded · `live` published

| # | Chapter | Docs | Code TS | Code C# | Script | Video | Tag |
|---|---|---|---|---|---|---|---|
| 00 | the agent we build | — | n/a | n/a | — | — | — |
| 01 | the model | ✅ | ✅ | ✅ | ✅ | — | — *(old tag `ch01-model` is out of date)* |
| 01.A | thinking | ✅ | ✅ | ✅ | ✅ | — | — |
| 02 | the prompt | — | — | — | — | — | — |
| 03 | the chat page | — | — | — | — | — | — |
| 04 | tools by hand | — | — | — | — | — | — |
| 05 | the loop | — | — | — | — | — | — |
| 06 | memory | — | — | — | — | — | — |
| 07 | guardrails | — | — | — | — | — | — |
| 08 | workflows | — | — | — | — | — | — |
| 09 | llama.cpp | — | — | — | — | — | — |
| 10 | choosing a model | — | n/a | n/a | — | — | — |
| 11 | side by side | — | — | — | — | — | — |
| 12 | frameworks *(optional)* | — | — | — | — | — | — |

## Verified on this machine

**2026-10-25, M1 Pro, 32 GB, macOS 27.**

- Ollama 0.40.6. `gemma4:e4b` returns a correct `list_files` tool call in 1.2 s once warm,
  5.6 s cold. 15 output tokens.
- `gemma4:e2b` (the default from decision 14) returns a correct `list_files` tool call in
  2.8 s warm, 6.6 s cold. 197 and 348 output tokens, because it thinks first. Both stacks
  answer with it. Chapter 01's examples were rerun on it.
- Chapter 01.A code, both stacks, on `gemma4:e2b`: `THINK=on` streams grey notes, then the
  answer, in 6.1 s. `THINK=off` answers in 0.5 s. `THINK` unset behaves like on for e2b, and
  works on `phi4-mini:3.8b`, which cannot think. `THINK=on` on phi4-mini returns a 400.
- RAM per model and context: see `docs/MODELS.md`. Measured with `scripts/measure.ts` and, for
  Ornith, llama.cpp 0.7.0. The `gemma4:26b` run was stopped before 64K.
- Node 26.12.0 runs `src/main.ts` directly. No build step, no dependencies.
- .NET 10.0.401. `dotnet run` in `code/csharp/` answers against the same endpoint.
- Both stacks answered "In five words, what is an AI agent?" correctly and differently.
- `scripts/check.sh` passed all six checks in 8.9 s, Ollama warm. With `MODEL=nope:1b` it
  failed on the model line and exited 1.

- Chapter 01 code back from `17db27d` into `chapters/01-the-model/code/`. Rerun on `gemma4:e2b`:
  TS answered "Perceives, reasons, acts autonomously." in 7.0 s. C# answered "Perceives,
  reasons, acts to achieve." in 7.3 s.
- Chapter 01.A code from `42f8ac5` in `chapters/01.A-thinking/code/`. `THINK=off` answers in
  both stacks.

**2026-10-26, same machine. For the chapter 01.A docs.**

- `gemma4:e2b`, "In five words, what is an AI agent?", warm:
  TS `THINK=off` 0.4 s, "Perceives, reasons, acts autonomously."
  TS `THINK=on` 5.4 s, "Perceives, reasons, acts, achieves goals."
  C# `THINK=off` 1.1 s, "Perceives, reasons, acts, learns." C# `THINK=on`, "Autonomous agent
  perceiving acting." The C# times include about 0.5 s of `dotnet run`.
- Streamed "Say hi in two words." on `gemma4:e2b`: **330 lines**, 328 of thinking and 2 of
  answer. `eval_count` 379. The last line is the only one with the counts.
- The `think` field has three behaviours, all checked by curl on `gemma4:e2b`:
  no field → 714 characters of thinking, `eval_count` 204.
  `"think": null` → no thinking at all. `"think": true` → thinking.
  So `null` is **not** the same as absent. That is what the C# `WhenWritingNull` option buys.
- `MODEL=phi4-mini:3.8b THINK=on` → `400 Bad Request: {"error":"\"phi4-mini:3.8b\" does not
  support thinking"}`. The same model with `THINK` unset answered "AI agent automates tasks."

## Decisions

| # | Decision | Date | Why |
|---|---|---|---|
| 1 | Raw HTTP, no SDK | 2026-10-25 | the lines an SDK saves are the lines the course teaches |
| 2 | Tools by hand (04) before native tools (05) | 2026-10-25 | shows a tool call is just text, and is the fallback for weak models |
| 3 | ~~One project per language that grows, git tag per chapter~~ *(replaced by 18)* | 2026-10-25 | no copy-paste across 16 folders; `git diff` works on camera |
| 4 | `gemma4:e4b` default, `qwen3.5:4b` fallback | 2026-10-25 | tool calling works and is already proven on this machine |
| 5 | Ornith-1.0-9B-MTP only in chapter 09 | 2026-10-25 | not in the Ollama library — it belongs with the hard runner |
| 6 | No tests, no TDD in this repo | 2026-10-25 | the payments repo does TDD. A 15-minute video cannot. |
| 7 | This repo is the simple sibling of `agent-ready-payments` | 2026-10-25 | audience is developers new to agents. No DB, no MCP, no domain. |
| 8 | A chat page as chapter 03, right after the prompt. Old 03-09 move to 04-10. | 2026-10-25 | viewers see a page early, and every later tool call shows in it |
| 9 | One shared `web/index.html`. Both stacks serve it, same `/chat` endpoint. No dependencies. | 2026-10-25 | keeps "same behaviour in both". The HTML is not the lesson. |
| 10 | `CLAUDE.md` is committed | 2026-10-25 | a course about agents can show how its author works with one |
| 11 | Agentic workflows as chapter 08, after guardrails. Old 08-10 move to 09-11. | 2026-10-25 | the job market asks for it. It needs the loop and the guardrails first. |
| 12 | Juniors are in the audience. Added a `BASICS.md` per chapter, `GLOSSARY.md`, `scripts/check.sh`, and two chapter sections: "When it breaks", "Try it yourself". | 2026-10-25 | juniors stop at setup, at new words, and at the first error they cannot read |
| 13 | Chapters 01-08 are two videos each, one per language | 2026-10-25 | two languages in one video is too much for a junior. Closes old open item 2. |
| 14 | `gemma4:e2b` is the default. `gemma4:e4b` is the step up. `qwen3.5:4b` is no longer the fallback. | 2026-10-25 | e2b calls tools, has vision, and needs about 8 GB at any context. qwen3.5:4b has a smaller file but needs more RAM. |
| 15 | `docs/MODELS.md` and `scripts/measure.ts` | 2026-10-25 | a viewer picks a model by their RAM, and can measure their own machine |
| 16 | Chapter 10 is "choosing a model", after llama.cpp. Old 10-11 move to 11-12. One video. | 2026-10-25 | Ornith runs only in llama.cpp, so the comparison comes after it |
| 17 | Thinking is chapter 01.A, right after the model: a `THINK` on/off flag, and streaming. Lettered so nothing renumbers. | 2026-10-25 | the default model thinks before every answer. Viewers should meet it, and choose, first. |
| 18 | Code layout is not final. Replaces decision 3. Each chapter starts as a copy of the one before at recording. Folders or tags decided later. Tags made so far are not final. | 2026-10-25 | the author tracks what each chapter adds, in order, while recording |
| 19 | No chapter is recorded until every chapter is `ready`. `ready` now means: docs, code and both scripts agree with each other and with the chapters after it. | 2026-10-25 | a chapter's code must lead into the next one. Recording early locks in code that a later chapter may change. |
| 21 | One camera script per language: `SCRIPT-ts.md` and `SCRIPT-csharp.md`. Chapter 01's `SCRIPT.md` is split and deleted. Shared steps are copied word for word, not linked. | 2026-10-26 | decision 13 makes each video stand alone. A reader of one script must never jump to the other. Closes the third To-do item. |
| 22 | `chapters/01.A-thinking/code/lk-csharp/` is the author's own copy. It stays, and nothing in the course links to it. | 2026-10-26 | the author uses it while recording. It is not part of the layout in `PLAN.md`, so it must not be deleted as a stray. |
| 23 | Chapter 02 loads the system prompt from a file. `prompts/librarian.md` and a second persona, picked by a `PROMPT` environment variable. It also adds a terminal read-loop, so the prompt holds across turns. | 2026-10-26 | a prompt you edit without touching code is the lesson. The read-loop shows the system prompt surviving many turns, which one-shot cannot. |
| 20 | One code folder per chapter: `chapters/NN-name/code/ts/` and `code/csharp/`. Replaces the shared `ts/` and `csharp/`. Chapter 01's code comes from `17db27d`, 01.A's from `42f8ac5`. Closes open item 1. | 2026-10-25 | 01 and 01.A sit side by side, so `diff -r` shows what a chapter adds. Decision 19 needs that check for every pair. |

## To do

- Chapter 01 is not `ready` under decision 19. Its docs and scripts match its own code now,
  but nothing after chapter 02 exists to check them against.
- Chapter 01.A is not `ready` under decision 19, for the same reason. Docs, code and both
  scripts agree with each other and were run on 2026-10-26.
- Chapter 02 is over its ~20-line budget in `PLAN.md`. Decision 23 adds a prompt file **and**
  a terminal read-loop, which is nearer 40 lines. If the video runs long, split it: `02` for
  the prompt file, `02.A` for the read-loop.
- Finish the `gemma4:26b` row in `docs/MODELS.md` (64K, 128K) and test tools on 26b, qwen3.5:4b
  and Ornith. Needed by chapter 10.

## Open

1. ~~Code layout~~ — closed by decision 20.
2. Whether chapter 12 (frameworks) exists at all.
3. C# server for chapter 03: `HttpListener` (no project change) or a minimal API
   (`Microsoft.NET.Sdk.Web`, still no NuGet). Decided when we write the chapter.
