// Measures how much RAM a model needs at each context size, on *your* machine.
// Usage: node scripts/measure.ts gemma4:e2b [more models...]
//
// For each context size it loads the model and asks Ollama how much memory it holds.
// The llama.cpp runner reserves the whole context at load. The MLX runner grows as text comes
// in, so for MLX we fill the context to about 85% first. That is what an agent near the end of
// a long run costs. A 128K fill takes minutes on a laptop.

const OLLAMA_URL = process.env.OLLAMA_URL ?? "http://localhost:11434";
const CONTEXTS = (process.env.CONTEXTS ?? "16384 32768 65536 131072").split(" ").map(Number);

type Loaded = { name: string; size: number; runner?: string };

async function call(path: string, body?: object) {
  const response = await fetch(`${OLLAMA_URL}${path}`, body && {
    method: "POST",
    body: JSON.stringify(body),
  });
  return response.json();
}

// A long fill can take more than 5 minutes before the first byte, and Node's fetch gives up at
// 5 minutes. Streaming sends bytes from the start. The last line holds the final counts.
async function fill(model: string, ctx: number) {
  const response = await fetch(`${OLLAMA_URL}/api/generate`, {
    method: "POST",
    body: JSON.stringify({
      model,
      prompt: filler(ctx * 0.85) + "\nSay ok.",
      think: false,
      keep_alive: "5m",
      options: { num_ctx: ctx, num_predict: 1 },
    }),
  });
  const lines = (await response.text()).trim().split("\n");
  return JSON.parse(lines[lines.length - 1]);
}

// About 3 characters per token for this text. The real count comes back as prompt_eval_count.
function filler(tokens: number): string {
  const words = ["river", "table", "green", "seven", "window", "quiet", "paper", "engine"];
  const parts: string[] = [];
  for (let i = 0, length = 0; length < tokens * 3; i++) {
    const word = `${words[(i * 5) % words.length]}${i % 100} `;
    parts.push(word);
    length += word.length;
  }
  return parts.join("");
}

const gb = (bytes: number) => (bytes / 1e9).toFixed(1).padStart(6);

const pulled: { name: string; size: number }[] = (await call("/api/tags")).models;
console.log("model          context  tokens   file  total  (GB)");

for (const model of process.argv.slice(2)) {
  const file = pulled.find((m) => m.name === model || m.name === `${model}:latest`)?.size;
  if (!file) {
    console.log(`${model} is not pulled. Run: ollama pull ${model}`);
    continue;
  }

  for (const ctx of CONTEXTS) {
    // An empty prompt loads the model and returns at once.
    await call("/api/generate", { model, prompt: "", keep_alive: "5m", options: { num_ctx: ctx } });
    const find = async (): Promise<Loaded> =>
      (await call("/api/ps")).models.find((m: Loaded) => m.name === model);
    let loaded = await find();

    let tokens = "-";
    if (loaded.runner !== "llamacpp") {
      const result = await fill(model, ctx);
      if (result.error) {
        console.log(`${model} at ${ctx / 1024}K: ${result.error}`);
        break;
      }
      tokens = String(result.prompt_eval_count);
      loaded = await find();
    }

    // The llama.cpp runner maps the file into memory, and `size` counts only what it adds on
    // top. The MLX runner loads the file, and `size` already counts it. Same field, two meanings.
    const total = loaded.runner === "llamacpp" ? file + loaded.size : loaded.size;

    console.log(
      `${model.padEnd(14)} ${`${ctx / 1024}K`.padStart(7)} ${tokens.padStart(7)} ${gb(file)} ${gb(total)}   ${loaded.runner}`,
    );
    await call("/api/generate", { model, keep_alive: 0 }); // unload, so each size starts clean
  }
}
