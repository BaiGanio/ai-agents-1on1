# scripts

| Script | What it does |
|---|---|
| `check.sh` | checks Node, .NET, Ollama, the model, and tool calling. Says how to fix each failure. |
| `measure.ts` | `node scripts/measure.ts <model>` — RAM at 16K, 32K, 64K, 128K on your machine |
| `check-script.py` | `python3 scripts/check-script.py chapters/NN/SCRIPT-csharp.md` — every code block in a camera script is verbatim in the file it names, at the lines it claims, in order (decision 38) |

**Planned:**

| Planned | What it will do | Needed by |
|---|---|---|
| `ollama.sh` | start the server, pull the model if missing, then run `check.sh` | chapter 01 |
| `llama.sh` | start `llama-server` with the model and flags from `.env` | chapter 09 |

`llama.sh` is a trimmed copy of `scripts/llm.sh` in
[`agent-ready-payments`](https://github.com/BaiGanio/agent-ready-payments).
Do not drop `--jinja`: without the model's own chat template the model prints tool calls as
prose instead of returning them as data.

`check-script.py` is not a test of behaviour — decision 6 rules those out. It checks the docs
against the code, which is the thing a human cannot do reliably: notice that a code block in a
script moved three lines when the file above it grew. Run it after any change to a chapter's
`code/`, and after any edit to a script.
