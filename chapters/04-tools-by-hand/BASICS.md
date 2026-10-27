# Chapter 04 — basics: turning text into data

Two things this chapter needs. Nothing else.

New words are in [`GLOSSARY.md`](../../docs/GLOSSARY.md).

---

## 1. Parsing JSON

The model sends you a **string**. Always a string. Even when that string looks like this:

```text
{"tool": "list_files", "path": "."}
```

Your code cannot ask it `.tool` yet. It is 35 characters of text. To
[parse](../../docs/GLOSSARY.md#parse) is to turn that text into data your code can reach into.

**TypeScript:**

```ts
const call = JSON.parse(text);
console.log(call.tool);        // "list_files"
console.log(call.path);        // "."
```

`JSON.parse` gives you a plain object. There is no type checking. If the model sent
`{"banana": 1}`, then `call.tool` is `undefined` and your code carries on happily until
something later falls over.

**C#:**

```csharp
record Call(string Tool, string? Path, string? Query);

var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
Call call = JsonSerializer.Deserialize<Call>(text, options)!;
Console.WriteLine(call.Tool);   // "list_files"
```

Two things to notice.

`PropertyNamingPolicy = JsonNamingPolicy.CamelCase` is the same line from chapter 01. The
model writes `"tool"` in lower case. Your property is `Tool`. This line connects them, so you
never have to write `[JsonPropertyName]` on anything.

`string?` on `Path` and `Query` means "may be null". It must be. `list_files` sends a `path`
and no `query`. `search_text` sends a `query` and no `path`. One record holds all three tools,
and the key that was not sent arrives as `null`.

Keys the model invented that you never declared are **ignored**. C# does not complain. That is
usually what you want, and it does mean a typo in the model's output can look like success.

---

## 2. `try` / `catch`, for the day the text is not JSON

You asked the model for JSON in a sentence of English. Sentences are not contracts. Sooner or
later it sends you this, and it is not even wrong:

```text
I am the Repo Librarian. I can answer questions about a folder of code using my tools.
```

There is no tool for *"hello, who are you?"*. So it answered. Parsing that throws.

### The real errors, from real runs on this machine

**TypeScript**, parsing that sentence:

```text
SyntaxError: Unexpected token 'I', "I am the R"... is not valid JSON
    at JSON.parse (<anonymous>)
```

**C#**, parsing the same sentence:

```text
System.Text.Json.JsonException: 'I' is an invalid start of a value.
Path: $ | LineNumber: 0 | BytePositionInLine: 0.
```

Both errors quote the first character that was not JSON. `'I'`. That is the single most useful
thing in them: it tells you the model wrote a sentence, not that your parser is broken.

### The shape to write

**The `catch` is not the error path.** Read that twice. In this chapter, a parse failure means
*the model gave you a plain answer*, which is a perfectly good outcome. So:

**TypeScript:**

```ts
let call;
try {
  call = JSON.parse(text);
} catch {
  console.log(text);     // it was prose. That was the answer. Done.
  return;
}
// we have a tool call
```

`catch` with no `(e)` is legal, and it says "I do not care which error". Use it when you really
do not.

**C#:**

```csharp
Call? call = null;
try
{
    call = JsonSerializer.Deserialize<Call>(text, options);
}
catch (JsonException)
{
    Console.WriteLine(text);   // it was prose. That was the answer.
}
```

### Catch the narrow type, never `Exception`

`catch (JsonException)`, not `catch (Exception)`. In TypeScript there is only one `catch`, so
be glad and move on — but in C# you get the choice, and the wrong choice hides real bugs.

| You catch | What happens the day your tool has a bug |
|---|---|
| `JsonException` | the bug crashes the program and you see it |
| `Exception` | the bug is swallowed, the model's text is printed, and you spend an afternoon on it |

A `catch` that is too wide is the most expensive two characters in this chapter.

### One more real one: the markdown fence

The other thing a model does is wrap perfectly good JSON in a code fence, because its training
data is full of them:

````text
```json
{"tool": "list_files", "path": "."}
```
````

**TypeScript:**

```text
SyntaxError: Unexpected token '`', "```json {""... is not valid JSON
```

**C#:**

```text
System.Text.Json.JsonException: '`' is an invalid start of a value.
Path: $ | LineNumber: 0 | BytePositionInLine: 0.
```

Same error, different first character: a backtick. The JSON inside is fine. Your `catch` will
treat it as prose and print the fence to the user, which looks broken.

You can strip the fence before you parse. On `gemma4:e2b` this did not happen once in the runs
for this chapter, so the chapter does not add the code for it — but you now know the error on
sight, which is the point.
