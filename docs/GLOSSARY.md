# Glossary

Every new word in the course, in one line. A chapter links here the first time it uses a word.
If a word is missing, add it here before you use it in a chapter.

| Word | What it means |
|---|---|
| <a id="agent"></a>**agent** | a model, a prompt, some tools and a loop. The model picks the next step. |
| <a id="agentic-workflow"></a>**agentic workflow** | several model calls wired together by your code. See [workflow](#workflow). |
| <a id="context-window"></a>**context window** | how much text the model can read in one call. Past it, the oldest text falls out. |
| <a id="endpoint"></a>**endpoint** | one URL on a server that does one job, like `POST /api/chat` |
| <a id="gguf"></a>**GGUF** | a file format for a model, so one file holds the whole model. llama.cpp reads it. |
| <a id="guardrail"></a>**guardrail** | a rule in *your* code that stops the agent: a step cap, a timeout, an allow-list |
| <a id="hallucination"></a>**hallucination** | the model says something false, and sounds sure. It guesses instead of looks. |
| <a id="json"></a>**JSON** | text that holds data: `{"name": "value"}`. See [`BASICS.md`](BASICS.md). |
| <a id="llama-cpp"></a>**llama.cpp** | a runner with more knobs than Ollama. Chapter 09. |
| <a id="local-model"></a>**local model** | a model that runs on your computer, not in the cloud. No key, no bill. |
| <a id="loop"></a>**the loop** | your code: send, run the tool the model asked for, send the result back, repeat |
| <a id="memory"></a>**memory** | short: the message list in this run. Long: notes saved to a file. |
| <a id="message"></a>**message** | one turn of talk: a `role` and some `content` |
| <a id="model"></a>**model** | a program that reads text and writes the text most likely to come next |
| <a id="ollama"></a>**Ollama** | a runner. It loads a model and gives it an HTTP endpoint on port 11434. |
| <a id="prompt"></a>**prompt** | the text you send the model |
| <a id="quantisation"></a>**quantisation** | making a model smaller by storing its numbers with less detail. Faster, a bit less clever. |
| <a id="role"></a>**role** | who wrote a message: `system`, `user`, `assistant`, or `tool` |
| <a id="runner"></a>**runner** | the program that loads a model and answers HTTP calls. Ollama, llama.cpp. |
| <a id="streaming"></a>**streaming** | the reply comes word by word, not all at the end |
| <a id="system-prompt"></a>**system prompt** | the first message. It says who the agent is and what rules it follows. |
| <a id="token"></a>**token** | a piece of a word. Models read and write tokens, not letters. |
| <a id="tool"></a>**tool** | a function in *your* code that the model may ask you to run |
| <a id="tool-call"></a>**tool call** | the model writes "run `list_files` with `path: .`". Your code runs it. The model runs nothing. |
| <a id="workflow"></a>**workflow** | model calls in an order your code decides. In an [agent](#agent), the model decides. Chapter 08. |
