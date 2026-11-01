# Chapter 07 — basics: paths and timeouts

Three things this chapter needs. Nothing else.

New words are in [`GLOSSARY.md`](../../docs/GLOSSARY.md).

---

## 1. What `..` does to a path

A path names a place on disk. Three kinds of piece matter here:

- `docs/STYLE.md` is **relative**. It starts from some folder you already stand in.
- `/etc/hosts` starts with `/`, so it is **absolute**. It starts from the top of the disk,
  wherever you stand.
- `..` means "the folder above". `.` means "this folder".

So `docs/../../.ssh` reads like it is inside `docs/`, but it is not. Walk it: into `docs/`,
up one, up one more, then into `.ssh`. It ends two folders **above** where it started.

A [full path](../../docs/GLOSSARY.md#full-path) is the path after every `.` and `..` is
walked. Both languages make one for you:

```csharp
Path.GetFullPath(Path.Combine("/tmp/repo-07", "docs/../../etc/hosts"))   // "/tmp/etc/hosts"
Path.GetFullPath(Path.Combine("/tmp/repo-07", "/etc/hosts"))             // "/etc/hosts"
```

```ts
import { resolve } from "node:path";
resolve("/tmp/repo-07", "docs/../../etc/hosts");                          // "/tmp/etc/hosts"
resolve("/tmp/repo-07", "/etc/hosts");                                    // "/etc/hosts"
```

Look at the second line of each. **An absolute path wins.** `Combine` and `resolve` throw the
root away when the second path starts with `/`. Chapter 06.A's tools did exactly that, and
read `/etc/hosts` for the model.

---

## 2. An allow-list, not a block-list

There are two ways to keep the agent in its folder.

A [block-list](../../docs/GLOSSARY.md#block-list) names what is bad: no `..`, no `/etc`, no
`.ssh`. It fails on the spelling you did not think of: `docs/../..`, `./../`, a path
that starts with `/`, a link.

An [allow-list](../../docs/GLOSSARY.md#allow-list) names what is good, and refuses the rest.
Here, that is one rule: the full path must be the repo folder, or start with it.

```csharp
string root = "/tmp/repo-07";
string full = Path.GetFullPath(Path.Combine(root, path));
bool ok = full == root || full.StartsWith(root + "/");
```

The `+ "/"` matters. Without it, `/tmp/repo-07-old` starts with `/tmp/repo-07`, and a folder
next to yours gets in.

**What it does not catch.** A link inside the repo that points outside it. The full path is
inside, so the check says yes. This course has no links in its folders, so the code does not
check for them.

---

## 3. A timeout, and a token that carries it

A [timeout](../../docs/GLOSSARY.md#timeout) is a time limit. Past it, your code stops waiting.

The `HttpClient` in this course already has one: 5 minutes, for **one** call. That does not
stop a loop of many short calls: each one ends well inside its own 5 minutes. So this chapter
has one clock for the whole question.

In C#, the clock travels as a [cancellation
token](../../docs/GLOSSARY.md#cancellation-token). You make a `CancellationTokenSource` with a
time. It gives you a `Token`. Every slow call you pass it to stops when the time is up, and
throws `OperationCanceledException`:

```csharp
using CancellationTokenSource timer = new(TimeSpan.FromSeconds(60));
try
{
    await client.SendAsync(request, timer.Token);
}
catch (OperationCanceledException)
{
    Console.WriteLine("[stopped: 60 s]");
}
```

TypeScript has the same idea under another name. Run against Ollama with a 50 ms limit, the
error's name was `TimeoutError`:

```ts
try {
  await fetch(url, { signal: AbortSignal.timeout(60_000) });
} catch (e) {
  if (e.name === "TimeoutError") console.log("[stopped: 60 s]");
}
```

**A token does not undo anything.** It stops what is still running. A file deleted on step 2
stays deleted when the clock runs out on step 5. The same is true of the step cap.

---

## Check yourself

Before the video:

- [ ] you can walk `docs/../../.ssh` by hand and say where it ends
- [ ] you can say what `Path.Combine("/tmp/repo-07", "/etc/hosts")` gives, and why that is a
  hole
- [ ] you can say why an allow-list beats a block-list, in one sentence
- [ ] you can say why `+ "/"` is in the check
- [ ] you can say what a cancellation token stops, and what it does not undo
