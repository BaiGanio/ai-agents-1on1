# Glossary

Every new word in the course, in one line. A chapter links here the first time it uses a word.
If a word is missing, add it here before you use it in a chapter.

| Word | What it means |
|---|---|
| <a id="agent"></a>**agent** | a model, a prompt, some tools and a loop. The model picks the next step. |
| <a id="agentic-workflow"></a>**agentic workflow** | several model calls wired together by your code. See [workflow](#workflow). |
| <a id="chunked"></a>**chunked** | a reply sent in pieces, with no length up front. Your code reads piece one before the last one exists. How Ollama streams. |
| <a id="context-window"></a>**context window** | how much text the model can read in one call. Past it, the oldest text falls out. |
| <a id="dispatch"></a>**dispatch** | picking which function to run from a name the model wrote. One `switch`. Chapter 04. |
| <a id="endpoint"></a>**endpoint** | one URL on a server that does one job, like `POST /api/chat` |
| <a id="environment-variable"></a>**environment variable** | a setting you put in front of a command: `MODEL=gemma4:e4b node src/main.ts`. It lives for that one run. |
| <a id="gguf"></a>**GGUF** | a file format for a model, so one file holds the whole model. llama.cpp reads it. |
| <a id="guardrail"></a>**guardrail** | a rule in *your* code that stops the agent: a step cap, a timeout, an allow-list |
| <a id="hallucination"></a>**hallucination** | the model says something false, and sounds sure. It guesses instead of looks. |
| <a id="http"></a>**HTTP** | the rules two programs use to ask and answer over a network. A method, a path, a body. |
| <a id="json"></a>**JSON** | text that holds data: `{"name": "value"}`. See [chapter 01 basics](../chapters/01-the-model/BASICS.md). |
| <a id="json-lines"></a>**JSON Lines** | many small JSON objects, one per line, instead of one big one. How Ollama streams. Also written NDJSON. |
| <a id="llama-cpp"></a>**llama.cpp** | a runner with more knobs than Ollama. Chapter 09. |
| <a id="local-model"></a>**local model** | a model that runs on your computer, not in the cloud. No key, no bill. |
| <a id="localhost"></a>**localhost** | this machine. `http://localhost:11434` means a program on your own computer. |
| <a id="loop"></a>**the loop** | your code: send, run the tool the model asked for, send the result back, repeat |
| <a id="memory"></a>**memory** | short: the message list in this run. Long: notes saved to a file. |
| <a id="message"></a>**message** | one turn of talk: a `role` and some `content` |
| <a id="mlx"></a>**MLX** | Apple's engine for running models on a Mac. Ollama uses it for some models. |
| <a id="model"></a>**model** | a program that reads text and writes the text most likely to come next |
| <a id="native-tool-calling"></a>**native tool calling** | you send your tool list as a `tools` field and the call comes back in a `tool_calls` field. No prose, no parse. Needs a model that supports it. Chapter 05. |
| <a id="ollama"></a>**Ollama** | a runner. It loads a model and gives it an HTTP endpoint on port 11434. |
| <a id="parameters"></a>**parameters** | the numbers a model learned. "4B" means 4 billion. More is smarter and slower. |
| <a id="parse"></a>**parse** | turn text into data your code can use. `JSON.parse` in TypeScript, `JsonSerializer` in C#. It throws when the text is not JSON. |
| <a id="persona"></a>**persona** | who the agent acts like. One [system prompt](#system-prompt) file per persona. Chapter 02. |
| <a id="port"></a>**port** | a door number on a machine, 1 to 65535. Ollama uses 11434. |
| <a id="prompt"></a>**prompt** | the text you send the model |
| <a id="quantisation"></a>**quantisation** | making a model smaller by storing its numbers with less detail. Faster, a bit less clever. |
| <a id="role"></a>**role** | who wrote a message: `system`, `user`, `assistant`, or `tool` |
| <a id="round-trip"></a>**round trip** | one send to the model and one reply back. A tool by hand costs two: one to get the call, one to give the result. Chapter 04. |
| <a id="runner"></a>**runner** | the program that loads a model and answers HTTP calls. Ollama, llama.cpp. |
| <a id="step"></a>**step** | one turn of [the loop](#loop): one send, and the tool that came back from it. The model decides how many a question takes. Chapter 05. |
| <a id="streaming"></a>**streaming** | the reply comes word by word, not all at the end |
| <a id="system-prompt"></a>**system prompt** | the first message. It says who the agent is and what rules it follows. |
| <a id="thinking"></a>**thinking** | notes the model writes to itself before it answers. Slower, often better. You can turn it off. Chapter 01.A. |
| <a id="token"></a>**token** | a piece of a word. Models read and write tokens, not letters. |
| <a id="tool"></a>**tool** | a function in *your* code that the model may ask you to run |
| <a id="tool-call"></a>**tool call** | the model writes "run `list_files` with `path: .`". Your code runs it. The model runs nothing. |
| <a id="tool-result"></a>**tool result** | the text your code sends back after it ran a tool. Just another [message](#message). Chapter 04. |
| <a id="tool-schema"></a>**tool schema** | the description of a tool that the model reads. Written in the [system prompt](#system-prompt) in chapter 04, sent as a `tools` field in chapter 05. |
| <a id="vision"></a>**vision** | the model can read an image you send it, not only text |
| <a id="workflow"></a>**workflow** | model calls in an order your code decides. In an [agent](#agent), the model decides. Chapter 08. |
