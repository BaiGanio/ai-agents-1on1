#!/usr/bin/env python3
"""Checks a camera script against the code it teaches (decision 38).

    python3 scripts/check-script.py chapters/05-the-loop/SCRIPT-csharp.md

A camera script carries only the code you type, and every code block names the lines it came
from. This checks the two have not drifted: each block must appear in one of the chapter's
finished files, byte for byte, at exactly the lines its heading claims, and the blocks for one
file must run down that file in order.

It is not a test of behaviour (decision 6 rules those out). It is the one thing a human cannot
do reliably: notice that a code block moved three lines when the file above it grew.
"""

import os
import re
import sys

# The files a camera script may quote. A chapter that adds one goes here.
CANDIDATES = [
    "code/csharp/ChatProtocol.cs",
    "code/csharp/Helper.cs",
    "code/csharp/Program.cs",
    "code/csharp/Tools.cs",
    "code/ts/src/helper.ts",
    "code/ts/src/main.ts",
    "code/ts/src/tools.ts",
    "code/prompts/librarian.md",
]

# A block's fence language says which files it may match, so a C# block cannot pass by
# accidentally matching a Markdown one.
SUFFIX = {"csharp": ".cs", "typescript": ".ts", "ts": ".ts", "markdown": ".md"}


def read_blocks(path):
    """Every fenced code block that follows a **Lines A-B** heading, in document order."""
    lines = open(path).read().split("\n")
    blocks = []
    step = None
    span = None
    i = 0
    while i < len(lines):
        line = lines[i]

        found = re.match(r"### (\d+)\. ", line)
        if found:
            step = found.group(1)

        found = re.search(r"\*\*[Ll]ines (\d+)-(\d+)\*\*", line)
        if found:
            span = (int(found.group(1)), int(found.group(2)))
        else:
            found = re.search(r"\*\*[Ll]ine (\d+)\*\*", line)
            if found:
                span = (int(found.group(1)), int(found.group(1)))

        if line.startswith("```") and line[3:].strip() in SUFFIX:
            lang = line[3:].strip()
            i += 1
            body = []
            while i < len(lines) and not lines[i].startswith("```"):
                body.append(lines[i])
                i += 1
            # A block with no line range is a shell command or a sample of output, not code
            # you type into a file. Those are not this script's business.
            if span:
                blocks.append((step, span, lang, body))
                span = None
        i += 1
    return blocks


def main():
    if len(sys.argv) != 2:
        print(__doc__)
        return 2

    script = sys.argv[1]
    chapter = os.path.dirname(script)

    files = {}
    for name in CANDIDATES:
        path = os.path.join(chapter, name)
        if os.path.exists(path):
            files[name] = open(path).read().split("\n")

    if not files:
        print("no code files found under %s — is that a chapter folder?" % chapter)
        return 2

    blocks = read_blocks(script)
    print("%s: %d code blocks with a line range" % (script, len(blocks)))

    failed = 0
    # Where the last block for each file started, so a block that jumps backwards is caught.
    last = {}

    for step, (start, end), lang, body in blocks:
        want = "\n".join(body)
        hits = [
            name
            for name, text in files.items()
            if name.endswith(SUFFIX[lang]) and "\n".join(text[start - 1 : end]) == want
        ]

        if not hits:
            failed += 1
            print("  FAIL  step %s, lines %d-%d: no %s file matches there"
                  % (step, start, end, lang))
            # The useful half of the message: say where the block really is.
            for name, text in files.items():
                whole = "\n".join(text)
                if want and want in whole:
                    at = whole[: whole.index(want)].count("\n") + 1
                    print("          it is in %s at lines %d-%d" % (name, at, at + len(body) - 1))
            continue

        name = hits[0]
        if start < last.get(name, 0):
            failed += 1
            print("  FAIL  step %s: %s lines %d-%d come after a block at line %d"
                  % (step, name, start, end, last[name]))
        last[name] = start
        print("  ok    step %-3s %-28s lines %d-%d" % (step, name, start, end))

    print()
    if failed:
        print("%d block(s) have drifted. Fix the script, or the line range, and run again."
              % failed)
        return 1

    print("every block is verbatim and in file order.")
    return 0


if __name__ == "__main__":
    sys.exit(main())
