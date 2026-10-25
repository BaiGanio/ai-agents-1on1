# Progress

One row per chapter. Update it when a state changes. This is the first file to open each session.

**Legend.** `—` not started · `docs` lesson written · `code` code written and running ·
`ready` docs + code + script, recordable · `shot` video recorded · `live` published

| # | Chapter | Docs | Code TS | Code C# | Script | Video | Tag |
|---|---|---|---|---|---|---|---|
| 00 | the agent we build | — | n/a | n/a | — | — | — |
| 01 | the model | ✅ | ✅ | ✅ | ✅ | — | `ch01-model` *(not final)* |
| 01.A | thinking | — | ✅ | ✅ | — | — | — |
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
- Node 26.12.0 runs `ts/src/main.ts` directly. No build step, no dependencies.
- .NET 10.0.401. `dotnet run` in `csharp/` answers against the same endpoint.
- Both stacks answered "In five words, what is an AI agent?" correctly and differently.
- `scripts/check.sh` passed all six checks in 8.9 s, Ollama warm. With `MODEL=nope:1b` it
  failed on the model line and exited 1.

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

## To do

- Finish the `gemma4:26b` row in `docs/MODELS.md` (64K, 128K) and test tools on 26b, qwen3.5:4b
  and Ornith. Needed by chapter 10.
- `chapters/01-the-model/SCRIPT.md` is one script for both languages. Split it into a
  TypeScript script (steps 0, 1, 2, 4) and a C# script (steps 0, 1, 3, 4).

## Open

1. Code layout: a folder per chapter, or one project with tags. Decided while recording.
2. Whether chapter 12 (frameworks) exists at all.
3. C# server for chapter 03: `HttpListener` (no project change) or a minimal API
   (`Microsoft.NET.Sdk.Web`, still no NuGet). Decided when we write the chapter.
