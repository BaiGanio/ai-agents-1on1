# Chapter 08 — basics: functions that call functions

Two things this chapter needs. Nothing else.

New words are in [`GLOSSARY.md`](../../docs/GLOSSARY.md).

---

## 1. One function feeds the next

A function takes something in and gives something back. When one function's result goes into
the next, you have a chain:

```csharp
string word = await Route(question);          // "look"
List<Tool> tools = Schemas.For(word);         // three tools
Outcome outcome = await Agent.Run(messages, tools);
```

Read it from the top. `word` is made first. `tools` needs `word`. `outcome` needs `tools`. You
can see the order because each line uses the one above. That is all a
[workflow](../../docs/GLOSSARY.md#workflow) is: your code calls the model, takes what came
back, and decides what to call next.

## 2. A record holds two answers

`Agent.Run` has two things to give back: the answer, and what the tools said. A `record` is a
small named bundle, so one function can return both:

```csharp
public record Outcome(string Answer, List<string> Evidence);

Outcome outcome = new("done", ["list_files . -> GLOSSARY.md"]);
Console.WriteLine(outcome.Answer);     // done
```

TypeScript does the same with an object: `{ answer, evidence }`.
