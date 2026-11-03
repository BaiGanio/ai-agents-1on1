// The inner loop, moved out of Program.cs in chapter 08 and not retyped. Chapter 07's code,
// word for word, except for what the workflows need: the tool list comes in, and what the
// tools said goes out. Router and checker both need one more call around this one.

// Evidence is one line per tool call: what was asked, what came back. The checker reads it.
public record Outcome(string Answer, List<string> Evidence);

public static class Agent
{
    public static async Task<Outcome> Run(List<Message> messages, List<Tool> tools)
    {
        List<string> evidence = [];
        HashSet<string> asked = [];

        while (true)
        {
            Guards.Steps++;
            int step = Guards.Steps;

            if (step > Guards.MaxSteps)
            {
                Console.WriteLine($"{Helper.Grey}[stopped: {Guards.MaxSteps} steps]{Helper.Reset}");
                return new("", evidence);
            }

            Console.Write($"{Helper.Model}: ");

            using CancellationTokenSource timer = new(Guards.TimeLeft());
            Message reply;
            try
            {
                reply = await Helper.Ask(messages, tools, timer.Token);
            }
            catch (OperationCanceledException)
            {
                double seconds = Guards.QuestionTime.TotalSeconds;
                Console.WriteLine($"\n{Helper.Grey}[stopped: {seconds} s]{Helper.Reset}");
                return new("", evidence);
            }
            messages.Add(reply);
            Console.WriteLine();

            if (reply.ToolCalls is null or { Count: 0 }) return new(reply.Content, evidence);

            foreach (ToolCall call in reply.ToolCalls)
            {
                ToolArguments? sent = call.Function?.Arguments;
                string shown = $"{call.Function?.Name} {sent?.Path ?? sent?.Query ?? sent?.Text}";
                Console.WriteLine($"{Helper.Grey}[step {step}] {shown}{Helper.Reset}");

                // The router chose this list. The model may still name a tool that is not on it,
                // and Tools.Run would run it. So the list is checked here, not trusted.
                string key = $"{call.Function?.Name} {sent?.Path} {sent?.Query} {sent?.Text}";
                string result =
                    !tools.Any(t => t.Function.Name == call.Function?.Name)
                        ? $"no such tool: {call.Function?.Name}"
                    : asked.Add(key) ? Tools.Run(call)
                    : "you already made this exact call. Use the result you have.";

                if (call.Function?.Name is "write_file" or "append_file" or "delete_file")
                    asked.Clear();

                Console.WriteLine(result);
                Console.WriteLine();

                // Short on purpose. Measured, 10 lies each: with 300 characters of a file read the
                // checker caught 0, with 60 it caught 10. A long result buries the one line it
                // needs, which is a refusal, and a refusal is short.
                string head = result.Replace('\n', ' ');
                evidence.Add($"{shown} -> {(head.Length > 60 ? head[..60] : head)}");
                messages.Add(new("tool", result));
            }
        }
    }
}
