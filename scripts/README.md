# scripts

**Empty on purpose, for now.**

| Planned | What it will do | Needed by |
|---|---|---|
| `ollama.sh` | start the server, pull the model if missing, check tool calling works | chapter 01 |
| `llama.sh` | start `llama-server` with the model and flags from `.env` | chapter 07 |

`llama.sh` is a trimmed copy of `scripts/llm.sh` in
[`agent-ready-payments`](https://github.com/BaiGanio/agent-ready-payments).
Do not drop `--jinja`: without the model's own chat template the model prints tool calls as
prose instead of returning them as data.
