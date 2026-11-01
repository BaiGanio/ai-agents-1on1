# Chapter 08 — Workflows

> **Not written yet.** This is the skeleton. See [`docs/PLAN.md`](../../docs/PLAN.md) for where
> this chapter sits, and [`docs/STYLE.md`](../../docs/STYLE.md) for how to fill it in.

**Video:** about 15 minutes.
**Code at the end of this chapter:** `git checkout ch08-workflows`
**New words:** TODO — link each to [`GLOSSARY.md`](../../docs/GLOSSARY.md)

## What we solve

Job ads ask for "agentic workflows". The term means several model calls wired together. We
build three with the Librarian's tools (decision 57):

- **Router.** One short call reads your question and says one word: `look`, `change` or
  `remember`. Your code gives the agent only that word's tools. A `look` question cannot delete
  a file.
- **Checker.** After the agent answers, one more call reads the tool results and the answer.
  `PASS`, or `FAIL` and a reason, and the agent gets one retry. Chapter 07 left a lie on the
  table: after a `n`, the model said "written". The checker is there to catch it.
- **Chain.** `/overview`. Code lists the files. One call per file writes one line. One last
  call joins them. The model picks no step.

Then the question that matters: should your code pick the path, or should the model?

## Simple example

TODO: one concrete thing the viewer sees happen on screen.

## Everyday picture

TODO: one picture from normal life that carries the idea.

## What this chapter adds

```mermaid
flowchart TD
    Q["Your question"] --> R["Router: one word, no tools"]
    R --> A["Agent loop from chapter 07, with that word's tools only"]
    A --> C["Checker: PASS or FAIL, no tools"]
    C -->|FAIL, once| A
    C -->|PASS| Out["Answer on screen"]
```

## TypeScript

TODO

## C#

TODO

## What differs between the two

TODO: or "nothing worth a sentence", which is also an answer.

## When it breaks

| What you see | What it means | What to do |
|---|---|---|
| TODO: copied from a real run | | |

## Try it yourself

TODO: one small task, 10-20 minutes, that changes the code.

## Done when

- [ ] TODO
