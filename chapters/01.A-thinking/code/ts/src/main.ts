// Chapter 01.A — thinking.
//
// Chapter 01 showed a model is an HTTP endpoint. This chapter shows it thinks before it answers,
// and lets you choose: watch the thinking, or turn it off. The reply now streams, word by word.

const OLLAMA_URL = process.env.OLLAMA_URL ?? "http://localhost:11434";
const MODEL = process.env.MODEL ?? "gemma4:e2b";

// on, off, or unset. Unset sends nothing, so the model does what it ships with. We do not send
// `true` by default: a model that cannot think answers that with an error, not with an answer.
const THINK = process.env.THINK === "on" ? true : process.env.THINK === "off" ? false : undefined;

const GREY = "\x1b[90m";
const RESET = "\x1b[0m";

type Message = { role: "system" | "user" | "assistant"; content: string; thinking?: string };

/** Sends the conversation so far. Prints the reply as it streams. Returns the whole message. */
async function chat(messages: Message[]): Promise<Message> {
  const response = await fetch(`${OLLAMA_URL}/api/chat`, {
    method: "POST",
    headers: { "content-type": "application/json" },
    // JSON.stringify drops a field whose value is undefined, so THINK unset means no `think`.
    body: JSON.stringify({ model: MODEL, messages, stream: true, think: THINK }),
  });

  if (!response.ok) {
    // Ollama puts the real reason in the body — an unknown model, a bad field. Without this
    // line you get a bare "500" and no idea which of the two it was.
    throw new Error(`${response.status} ${response.statusText}: ${await response.text()}`);
  }

  const reply: Message = { role: "assistant", content: "", thinking: "" };
  let buffer = "";

  // A stream is one JSON object per line. A network chunk can stop in the middle of a line,
  // so we keep the unfinished tail in `buffer` until the rest of it arrives.
  for await (const chunk of response.body!.pipeThrough(new TextDecoderStream())) {
    buffer += chunk;
    const lines = buffer.split("\n");
    buffer = lines.pop()!;

    for (const line of lines.filter(Boolean)) {
      const part: Message = JSON.parse(line).message;
      if (part.thinking) {
        process.stdout.write(GREY + part.thinking + RESET);
        reply.thinking += part.thinking;
      }
      if (part.content) {
        if (!reply.content && reply.thinking) process.stdout.write("\n\n");
        process.stdout.write(part.content);
        reply.content += part.content;
      }
    }
  }

  process.stdout.write("\n");
  return reply;
}

const prompt = process.argv.slice(2).join(" ") || "Say hello in five words.";

// The model remembers nothing between calls. Every turn sends the whole history back.
// That list is the agent's short memory, and it is the only memory it has until chapter 06.
const messages: Message[] = [{ role: "user", content: prompt }];

await chat(messages);
