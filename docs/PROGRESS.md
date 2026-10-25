# Progress

One row per chapter. Update it when a state changes. This is the first file to open each session.

**Legend.** `—` not started · `docs` lesson written · `code` code written and running ·
`ready` docs + code + script, recordable · `shot` video recorded · `live` published

| # | Chapter | Docs | Code TS | Code C# | Script | Video | Tag |
|---|---|---|---|---|---|---|---|
| 00 | the agent we build | — | n/a | n/a | — | — | — |
| 01 | the model | ✅ | ✅ | ✅ | ✅ | — | `ch01-model` *(not cut)* |
| 02 | the prompt | — | — | — | — | — | — |
| 03 | the chat page | — | — | — | — | — | — |
| 04 | tools by hand | — | — | — | — | — | — |
| 05 | the loop | — | — | — | — | — | — |
| 06 | memory | — | — | — | — | — | — |
| 07 | guardrails | — | — | — | — | — | — |
| 08 | workflows | — | — | — | — | — | — |
| 09 | llama.cpp | — | — | — | — | — | — |
| 10 | side by side | — | — | — | — | — | — |
| 11 | frameworks *(optional)* | — | — | — | — | — | — |

## Verified on this machine

**2026-10-25, M1 Pro, 32 GB, macOS 27.**

- Ollama 0.40.6. `gemma4:e4b` returns a correct `list_files` tool call in 1.2 s once warm,
  5.6 s cold. 15 output tokens.
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
| 3 | One project per language that grows, git tag per chapter | 2026-10-25 | no copy-paste across 16 folders; `git diff` works on camera |
| 4 | `gemma4:e4b` default, `qwen3.5:4b` fallback | 2026-10-25 | tool calling works and is already proven on this machine |
| 5 | Ornith-1.0-9B-MTP only in chapter 09 | 2026-10-25 | not in the Ollama library — it belongs with the hard runner |
| 6 | No tests, no TDD in this repo | 2026-10-25 | the payments repo does TDD. A 15-minute video cannot. |
| 7 | This repo is the simple sibling of `agent-ready-payments` | 2026-10-25 | audience is developers new to agents. No DB, no MCP, no domain. |
| 8 | A chat page as chapter 03, right after the prompt. Old 03-09 move to 04-10. | 2026-10-25 | viewers see a page early, and every later tool call shows in it |
| 9 | One shared `web/index.html`. Both stacks serve it, same `/chat` endpoint. No dependencies. | 2026-10-25 | keeps "same behaviour in both". The HTML is not the lesson. |
| 10 | `CLAUDE.md` is committed | 2026-10-25 | a course about agents can show how its author works with one |
| 11 | Agentic workflows as chapter 08, after guardrails. Old 08-10 move to 09-11. | 2026-10-25 | the job market asks for it. It needs the loop and the guardrails first. |
| 12 | Juniors are in the audience. Added `BASICS.md`, `GLOSSARY.md`, `scripts/check.sh`, and two chapter sections: "When it breaks", "Try it yourself". | 2026-10-25 | juniors stop at setup, at new words, and at the first error they cannot read |
| 13 | Chapters 01-08 are two videos each, one per language | 2026-10-25 | two languages in one video is too much for a junior. Closes old open item 2. |

## To do

- `chapters/01-the-model/SCRIPT.md` is one script for both languages. Split it into a
  TypeScript script (steps 0, 1, 2, 4) and a C# script (steps 0, 1, 3, 4).

## Open

1. Tags or exported folders per chapter — decided after we see a viewer trip on `git checkout`.
2. Whether chapter 11 (frameworks) exists at all.
3. C# server for chapter 03: `HttpListener` (no project change) or a minimal API
   (`Microsoft.NET.Sdk.Web`, still no NuGet). Decided when we write the chapter.
