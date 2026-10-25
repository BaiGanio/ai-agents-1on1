# Models

Which model fits your machine. Every number here came from one run on one Mac, listed at the
bottom. Your numbers will be close, not equal. To get your own, run:

```bash
node scripts/measure.ts gemma4:e2b
```

**Start with `gemma4:e2b`.** It is the default for this course. It calls tools, it sees images,
and it needs about 8 GB of RAM at any context size.

## The models

| Model | Runner | Size | File | Max context | Tools | Vision | Notes |
|---|---|---|---|---|---|---|---|
| `gemma4:e2b` | Ollama | 5.2B | 7.5 GB | 128K | ✅ tested | ✅ | **the default.** Also hears audio. |
| `gemma4:e4b` | Ollama | 8.0B | 9.6 GB | 128K | ✅ tested | ✅ | a bit smarter, a bit slower. Also hears audio. |
| `gemma4:26b` | Ollama | 26.2B | 16.1 GB | 256K | listed | ✅ | needs a 32 GB machine |
| `qwen3.5:4b` | Ollama | 4.7B | 3.4 GB | 256K | listed | ✅ | smallest file, but its cache grows fast |
| `Ornith-1.0-9B-MTP` | llama.cpp | 9B | 5.8 GB | 256K | not tested | ❌ | text only. Chapter 09. |

- **Size** is the number of [parameters](GLOSSARY.md#parameters): the numbers the model learned.
  More is usually smarter and always slower.
- **Tools: tested** means `scripts/check.sh` got a real tool call back. **Listed** means Ollama
  says the model supports tools, and we have not tried it yet.
- **Vision** means you can send it an image and ask about it. Ornith cannot. Chapter 10 shows
  the difference.

## RAM needed, by context size

The [context window](GLOSSARY.md#context-window) is how much text the model can read at once.
A longer one needs more memory for its cache. All numbers in GB.

| Model | 16K | 32K | 64K | 128K |
|---|---|---|---|---|
| `gemma4:e2b` | 7.7 | 7.6 | 7.8 | 8.2 |
| `gemma4:e4b` | 10.5 | 10.7 | 10.7 | 13.5 |
| `gemma4:26b` | 17.0 | 17.7 | not measured | not measured |
| `qwen3.5:4b` | 7.1 | 7.7 | 9.2 | 11.9 |
| `Ornith-1.0-9B-MTP` | 6.6 | 7.2 | 8.5 | 11.0 |

Two things the table shows:

- **The file size is not the RAM you need.** `qwen3.5:4b` has the smallest file and needs more
  RAM than `gemma4:e2b` from 32K up.
- **Gemma barely grows.** Most of its layers only look at the last part of the text, so its cache
  stays small. More of Qwen's and Ornith's layers look at all of it, so theirs grows faster.

## Which one for my machine

Leave room for macOS and your other apps. A model that does not fit makes the whole machine
slow, because it starts to swap — use the disk as RAM.

| Your RAM | Pick |
|---|---|
| 8 GB | none of these fit with room to spare. The course needs 16 GB. |
| 16 GB | `gemma4:e2b` at any context. Ornith or `qwen3.5:4b` up to 32K. |
| 24 GB | anything above, plus `gemma4:e4b` at any context |
| 32 GB+ | anything above, plus `gemma4:26b` up to 32K |

## How we measured

**2026-10-25, M1 Pro, 32 GB, macOS 27, Ollama 0.40.6, llama.cpp 0.7.0.**

RAM = the model file + the extra memory the runner takes for that context.

- **Ollama, llama.cpp runner** (`gemma4:e4b`, `qwen3.5:4b`). It reserves the whole cache when
  it loads. We loaded the model at each size and read `size` from `/api/ps`, which counts only
  the extra, and added the file.
- **Ollama, MLX runner** (`gemma4:e2b`, `gemma4:26b`). MLX is Apple's own engine. It grows the
  cache as text comes in, so we filled the context to 85% first. Here `size` already counts the
  file.
- **llama.cpp** (Ornith). We started `llama-server -c <size> -np 1 -lv 4` and added up the KV,
  recurrent and compute buffers it printed, plus the file.

`scripts/measure.ts` does the first two for you. The `gemma4:26b` run was stopped before 64K.
