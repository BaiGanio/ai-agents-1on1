# Basics: HTTP and JSON

Read this before chapter 01 if HTTP or JSON is new to you. It takes ten minutes.
The whole course talks to the model in these two things, and nothing else.

## HTTP: a note and a reply

Your program sends a **request**. A server sends back a **response**. That is all HTTP is.

A request has four parts:

| Part | Example | What it is |
|---|---|---|
| method | `GET` or `POST` | `GET` asks for something. `POST` sends something. |
| URL | `http://localhost:11434/api/chat` | where to send it |
| headers | `content-type: application/json` | notes about the body |
| body | `{"model": "gemma4:e4b", ...}` | the data. `GET` has none. |

A response has a **status code** and a **body**:

| Code | What it means |
|---|---|
| `200` | it worked |
| `404` | that URL does not exist |
| `500` | the server failed. Read the body — it says why. |

`localhost` means "this computer". `11434` is the **port** — the door number Ollama listens on.

## Try it

Start Ollama (`ollama serve`), then in another terminal:

```bash
curl http://localhost:11434/api/version
# {"version":"0.40.6"}
```

That was a `GET`. `curl` is a program that sends HTTP requests from the terminal.
Your version number may be different.

## JSON: data as text

JSON is how the request body and the response body hold data.

```json
{
  "model": "gemma4:e4b",
  "stream": false,
  "messages": [
    { "role": "user", "content": "Hello" }
  ]
}
```

| You see | It is | Example |
|---|---|---|
| `{ }` | an **object**: names and values | `{"role": "user"}` |
| `[ ]` | an **array**: a list | `[1, 2, 3]` |
| `"..."` | a string | `"gemma4:e4b"` |
| `true` / `false` | a boolean | `false` |
| `15` | a number | `15` |

Names are always in double quotes. No comma after the last item. Those two rules cause most
JSON errors.

## Put together

This is the request chapter 01 sends. Now you can read every part of it:

```bash
curl http://localhost:11434/api/chat -d '{
  "model": "gemma4:e4b",
  "stream": false,
  "messages": [{"role": "user", "content": "In five words, what is an AI agent?"}]
}'
```

`-d` means "send this body", and it makes the method `POST`. The reply is JSON too.
Chapter 01 takes it apart, field by field.

New words go in [`GLOSSARY.md`](GLOSSARY.md).
