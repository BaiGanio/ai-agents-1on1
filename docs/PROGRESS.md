# Progress

One row per chapter. Update it when a state changes. This is the first file to open each session.

**Legend.** `—` not started · `docs` lesson written · `code` code written and running ·
`ready` docs + code + script, recordable · `shot` video recorded · `live` published

| # | Chapter | Docs | Code TS | Code C# | Script | Video | Tag |
|---|---|---|---|---|---|---|---|
| 00 | the agent we build | — | n/a | n/a | — | — | — |
| 01 | the model | ✅ | ✅ | ✅ | ✅ | — | `ch01-model` *(not cut)* |
| 02 | the prompt | — | — | — | — | — | — |
| 03 | tools by hand | — | — | — | — | — | — |
| 04 | the loop | — | — | — | — | — | — |
| 05 | memory | — | — | — | — | — | — |
| 06 | guardrails | — | — | — | — | — | — |
| 07 | llama.cpp | — | — | — | — | — | — |
| 08 | side by side | — | — | — | — | — | — |
| 09 | frameworks *(optional)* | — | — | — | — | — | — |

## Verified on this machine

**2026-10-25, M1 Pro, 32 GB, macOS 27.**

- Ollama 0.40.6. `gemma4:e4b` returns a correct `list_files` tool call in 1.2 s once warm,
  5.6 s cold. 15 output tokens.
- Node 26.12.0 runs `ts/src/main.ts` directly. No build step, no dependencies.
- .NET 10.0.401. `dotnet run` in `csharp/` answers against the same endpoint.
- Both stacks answered "In five words, what is an AI agent?" correctly and differently.

## Decisions

| # | Decision | Date | Why |
|---|---|---|---|
| 1 | Raw HTTP, no SDK | 2026-10-25 | the lines an SDK saves are the lines the course teaches |
| 2 | Tools by hand (03) before native tools (04) | 2026-10-25 | shows a tool call is just text, and is the fallback for weak models |
| 3 | One project per language that grows, git tag per chapter | 2026-10-25 | no copy-paste across 16 folders; `git diff` works on camera |
| 4 | `gemma4:e4b` default, `qwen3.5:4b` fallback | 2026-10-25 | tool calling works and is already proven on this machine |
| 5 | Ornith-1.0-9B-MTP only in chapter 07 | 2026-10-25 | not in the Ollama library — it belongs with the hard runner |
| 6 | No tests, no TDD in this repo | 2026-10-25 | the payments repo does TDD. A 15-minute video cannot. |
| 7 | This repo is the simple sibling of `agent-ready-payments` | 2026-10-25 | audience is developers new to agents. No DB, no MCP, no domain. |

## Open

1. Tags or exported folders per chapter — decided after we see a viewer trip on `git checkout`.
2. One video per chapter, or one per language — decided when we see how long chapter 03 runs.
3. Whether chapter 09 (frameworks) exists at all.
