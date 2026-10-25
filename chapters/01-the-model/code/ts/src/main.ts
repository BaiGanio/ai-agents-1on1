// Chapter 01 — the model.
//
// The whole point of this file: a model is an HTTP endpoint. You send it a list of messages.
// It sends back one more message. There is no magic, no SDK, and no state on the server.

const OLLAMA_URL = process.env.OLLAMA_URL ?? "http://localhost:11434";
const MODEL = process.env.MODEL ?? "gemma4:e2b";

type Message = { role: "system" | "user" | "assistant"; content: string };

/** Sends the conversation so far. Returns the one message the model wrote. */
async function chat(messages: Message[]): Promise<Message> {
  const response = await fetch(`${OLLAMA_URL}/api/chat`, {
    method: "POST",
    headers: { "content-type": "application/json" },
    // stream:false means "one JSON object when you are done", not a drip of tokens.
    // Streaming is nicer for a user and noisier for a lesson, so it stays off until chapter 06.
    body: JSON.stringify({ model: MODEL, messages, stream: false }),
  });

  if (!response.ok) {
    // Ollama puts the real reason in the body — an unknown model, a bad field. Without this
    // line you get a bare "500" and no idea which of the two it was.
    throw new Error(`${response.status} ${response.statusText}: ${await response.text()}`);
  }

  const body = (await response.json()) as { message: Message };
  return body.message;
}

const prompt = process.argv.slice(2).join(" ") || "Say hello in five words.";

// The model remembers nothing between calls. Every turn sends the whole history back.
// That list is the agent's short memory, and it is the only memory it has until chapter 06.
const messages: Message[] = [{ role: "user", content: prompt }];

const answer = await chat(messages);
console.log(answer.content.trim());
