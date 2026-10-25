# CLAUDE.md

A video course that builds an AI agent from nothing, in TypeScript and C#, against a model on
the viewer's own machine. No framework, no cloud, no API key. The agent is the Repo Librarian.

## Read first, every session

1. `docs/PROGRESS.md` — the board, the decisions, what is open. Start here.
2. `docs/PLAN.md` — the chapters, and the line and minute budget of each.
3. `docs/STYLE.md` — how docs and code are written. Follow it exactly.

The audience includes juniors. `docs/BASICS.md` (HTTP and JSON) and `docs/GLOSSARY.md` (every
new word) exist for them. A new word goes in the glossary before a chapter uses it.

If a request goes against `PLAN.md` or a decision in `PROGRESS.md`, say so before you start.
Change the file only when the user agrees.

## Run it

```bash
./scripts/check.sh                             # is everything installed and working?
ollama serve                                   # the model server, port 11434
cd ts && node src/main.ts "your question"      # Node 24+, no build step
cd csharp && dotnet run -- "your question"     # .NET 10
```

`OLLAMA_URL` and `MODEL` override the defaults (`http://localhost:11434`, `gemma4:e4b`).
More in `docs/SETUP.md`.

## Rules that are easy to break

- **No dependencies.** No npm package, no NuGet package, unless the chapter is about it.
- **Both stacks, same behavior.** A change in `ts/` needs the same change in `csharp/`.
  Each side stays natural for its language. Where they differ, the chapter README says why.
- **The code grows.** One project per language. No copy per chapter. Each chapter ends on a
  git tag, `chNN-name`. Never move a tag that is already on `origin`.
- **The budget is real.** "New code" in `PLAN.md` is what the viewer types on camera.
  Past it, the chapter splits. Say so — do not squeeze.
- **Code in `main` reads top to bottom** in the order the video types it.
- **Comments say why, never what.** Max 100 columns.
- **No invented numbers or output.** Every number and every "Simple example" comes from a real
  run. Write the machine and date in `docs/PROGRESS.md`. If you did not run it, do not write it.
- **Diagrams:** `flowchart TD`, one per chapter, never a `;` in a line.
- **No tests in this repo.** Decision 6. Prove it works by running it.
- **Plain words** in docs: short sentences, active voice, no "simply", "just", "obviously".
- **Every chapter has "When it breaks" and "Try it yourself".** Errors are copied from real
  runs. The task changes the code, and its answer is not in the repo.
- **Two videos per chapter (01-08), one per language.** Each script stands alone.

## When a chapter changes

1. Update the code in both stacks and run both.
2. Fill the chapter's `README.md`, `QA.md`, and one script per language, in the shape
   `STYLE.md` gives. Add new words to `docs/GLOSSARY.md`.
3. Update its row in `docs/PROGRESS.md`. Add a row to Decisions for any new choice.
4. Renumbering a chapter touches the folder, its headings, its tag, and every "chapter NN" in
   docs and code comments. Grep for all of them.

## Git

Conventional commits (`feat:`, `docs:`, `fix:`). `bin/` and `obj/` stay out.

**After every change — code, docs, anything — end your reply with a ready commit message** in a
code block: a subject line under 72 characters, a blank line, then a short body that says what
changed and why. The user commits it. Commit yourself only when asked.
