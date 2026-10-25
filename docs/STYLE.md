# Style

How we write, so twelve chapters read like one person wrote them. Borrowed from
[`agent-ready-payments`](https://github.com/BaiGanio/agent-ready-payments) and cut down.

## Words

- Short sentences. One idea per sentence.
- Small words. If a big word is needed, explain it right after, in the same sentence.
- Active voice. "The host runs the tool", not "the tool is run".
- No hype. No "simply", no "just", no "obviously". If it were obvious there would be no video.
- Never say "AI-powered". Say what it does.
- The reader may be a junior. They can use a terminal and have written a small program. They
  may not know HTTP or JSON — [`BASICS.md`](BASICS.md) covers that. They do not know what a
  tool call is.
- Every new word links to [`GLOSSARY.md`](GLOSSARY.md) the first time a chapter uses it. If the
  word is not there, add it first.

## Every chapter README has these sections, in this order

1. `# Chapter NN — Title`, then video length, the git tag, links to the files, and a
   **New words** line that links each new word to the glossary
2. `## What we solve` — the problem, in 3-5 sentences
3. `## Simple example` — a command and its real output. Copied from a real run, never invented.
4. `## Everyday picture` — one picture from normal life. This is what the viewer remembers.
5. `## What this chapter adds` — one `flowchart TD`
6. `## TypeScript`, then `## C#` — separate sections, always. Each is its own video.
7. `## What differs between the two` — a table. "Nothing worth a sentence" is a valid answer.
8. `## When it breaks` — a table of the 2-3 errors a viewer will really hit, and the fix.
   Each error is copied from a real run.
9. `## Try it yourself` — one small task, 10-20 minutes, that changes the code. The answer is
   not in the repo. A junior learns more from one task than from ten questions.
10. `## Done when` — a checklist the viewer can tick

## Videos

Chapters 01-08 are two videos each: one TypeScript, one C#. Each video stands alone. It opens
with the same short "what we solve" and "everyday picture", so a C# viewer never has to watch
the TypeScript video. Chapters 00, 09, 10 are one video. Chapter 11 is text only.

## Diagrams

- `flowchart TD`. Down the page, never `LR`. On GitHub a wide diagram shrinks to nothing.
- Never a `;` inside a diagram line. Mermaid reads it as the end of the line and refuses.
- One diagram per chapter. Two means the chapter is too big.

## Code

- No dependency unless the chapter is about that dependency.
- Each stack stays natural for its language. We do not make C# look like TypeScript.
- Comment the **why**, never the what. `// loop over the files` is noise.
  `// the model never sees the token` is the lesson.
- A comment that explains a surprise is worth five lines. A comment that repeats the code is a
  cost. If you cannot say why, there may be nothing there.
- Max 100 columns. On camera, long lines wrap and the viewer loses the thread.
- The code in `main` reads top to bottom in the order the video types it.

## Numbers and claims

Any number in the docs came from a real run on a real machine, and `docs/PROGRESS.md` says
which machine and when. If you cannot point at the run, delete the number.
