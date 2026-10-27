// Chapter 02 — the prompt.
//
// The model now gets a system message first, read from a text file. Swap the file and the agent
// is somebody else, with no code change. A loop keeps that message in front of every turn.

import { readFile } from "node:fs/promises";
import * as readline from "node:readline/promises";

const OLLAMA_URL = process.env.OLLAMA_URL ?? "http://localhost:11434";
const MODEL = process.env.MODEL ?? "gemma4:e2b";

// on, off, or unset. Unset sends nothing, so the model does what it ships with. We do not send
// `true` by default: a model that cannot think answers that with an error, not with an answer.
const THINK = process.env.THINK === "on" ? true : process.env.THINK === "off" ? false : undefined;

// Names a file in ../prompts. That file is the whole personality of the agent.
const PROMPT = process.env.PROMPT ?? "librarian";

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

// Read once, before anything else happens. Edit the file mid-session and nothing changes until
// you restart. The path is relative to the folder you ran `node` from, not to this file.
const system = await readFile(`../prompts/${PROMPT}.md`, "utf8");

// The model remembers nothing between calls. Every turn sends the whole history back, so the
// system message at index 0 is re-sent every single time. Drop it and the agent forgets itself.
const messages: Message[] = [{ role: "system", content: system }];

const question = process.argv.slice(2).join(" ");

if (question) {
  messages.push({ role: "user", content: question });
  await chat(messages);
} else {
  const rl = readline.createInterface({ input: process.stdin, output: process.stdout });
  // Not rl.prompt(). That one throws once stdin has closed, and stdin closes on the last
  // line when you pipe input in. A plain write cannot throw.
  process.stdout.write("You: ");

  // Ends when stdin closes, which is Ctrl-D. No `while (true)` and no word to quit on.
  for await (const line of rl) {
    if (line.trim()) {
      messages.push({ role: "user", content: line });
      // The list only ever grows. It has no limit until chapter 06 gives it one.
      messages.push(await chat(messages));
    }
    process.stdout.write("You: ");
  }

  process.stdout.write("\n");
}
