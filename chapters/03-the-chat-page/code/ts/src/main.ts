// Chapter 03 — the chat page.
//
// The agent gets a window. One HTML page, one POST /chat endpoint, served by the same process
// that calls the model. The stream from chapter 01.A now runs all the way to the browser.

import { readFile } from "node:fs/promises";
import { createServer } from "node:http";

const OLLAMA_URL = process.env.OLLAMA_URL ?? "http://localhost:11434";
const MODEL = process.env.MODEL ?? "gemma4:e2b";

// on, off, or unset. Unset sends nothing, so the model does what it ships with. We do not send
// `true` by default: a model that cannot think answers that with an error, not with an answer.
const THINK = process.env.THINK === "on" ? true : process.env.THINK === "off" ? false : undefined;

// Names a file in ../prompts. That file is the whole personality of the agent.
const PROMPT = process.env.PROMPT ?? "librarian";

// 11434 is Ollama. 3000 is us. Two servers, two ports, one machine.
const PORT = Number(process.env.PORT ?? 3000);

const GREY = "\x1b[90m";
const RESET = "\x1b[0m";

type Message = { role: "system" | "user" | "assistant"; content: string; thinking?: string };

/** Sends the conversation so far. Hands each streamed piece to `send`. Returns the whole reply. */
async function chat(messages: Message[], send: (part: Message) => void): Promise<Message> {
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
      // `chat` no longer decides where the words go. The caller does. That one change is what
      // lets the same function feed a terminal and a browser.
      send(part);
      reply.thinking += part.thinking ?? "";
      reply.content += part.content ?? "";
    }
  }

  return reply;
}

let grey = false;

/** A `send` for the terminal: thinking in grey, a blank line, then the answer. */
function toTerminal(part: Message): void {
  if (part.thinking) {
    process.stdout.write(GREY + part.thinking + RESET);
    grey = true;
  }
  if (part.content) {
    if (grey) {
      process.stdout.write("\n\n");
      grey = false;
    }
    process.stdout.write(part.content);
  }
}

const system = await readFile(`../prompts/${PROMPT}.md`, "utf8");

// The model remembers nothing between calls. Every turn sends the whole history back, so the
// system message at index 0 is re-sent every single time. Drop it and the agent forgets itself.
const messages: Message[] = [{ role: "system", content: system }];

const question = process.argv.slice(2).join(" ");

if (question) {
  messages.push({ role: "user", content: question });
  await chat(messages, toTerminal);
  process.stdout.write("\n");
} else {
  // Read once, at start. The page is not the lesson, so we do not reload it per request.
  const page = await readFile("../web/index.html", "utf8");

  const server = createServer(async (req, res) => {
    if (req.method !== "POST" || req.url !== "/chat") {
      // There is one page and one endpoint, so there is no router. Everything else is the page.
      res.writeHead(200, { "content-type": "text/html; charset=utf-8" });
      res.end(page);
      return;
    }

    // A request body arrives in pieces, exactly like the model's answer does.
    let body = "";
    for await (const chunk of req) body += chunk;

    // One history for the whole process, shared by every browser tab. Chapter 06 gives each
    // conversation its own.
    messages.push({ role: "user", content: body });

    // No content-length, because we do not know it yet. Node then sends the body in chunks,
    // and that is what lets the browser show the first word before the last one is written.
    res.writeHead(200, { "content-type": "application/x-ndjson; charset=utf-8" });

    try {
      messages.push(await chat(messages, (part) => res.write(JSON.stringify(part) + "\n")));
    } catch (error) {
      // One bad request must not take the server down with it, and an uncaught throw in here
      // would. The headers are already sent, so the only place left to say so is the stream.
      res.write(JSON.stringify({ error: String(error) }) + "\n");
      // Drop the question too. A user turn with no answer after it confuses the next call.
      messages.pop();
    }

    res.end();
  });

  server.listen(PORT, () => console.log(`Repo Librarian on http://localhost:${PORT}`));
}
