using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

public static class Helper
{
    public static readonly string Model = "phi4-mini:3.8b";
    public static readonly bool? Think = false;

    public static readonly string Grey = "\x1b[90m";
    public static readonly string Reset = "\x1b[0m";

    public static readonly HttpClient OllamaHttpClient =
        new()
        {
          BaseAddress = new Uri("http://localhost:11434"),
          Timeout = TimeSpan.FromMinutes(5)  
        };

    public static readonly JsonSerializerOptions AgentJsonOptions =
        new()
        {
          PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
          DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull  
        };

    public static async Task HandleResponseError(HttpResponseMessage response)
    {
        var body = await response.Content.ReadAsStringAsync();
        throw new HttpRequestException(
            $"{(int)response.StatusCode} {response.ReasonPhrase}: {body}");
    }

    public static async Task HandleStream(HttpResponseMessage response, StringBuilder content, StringBuilder thinking)
    {
        using var reader = new StreamReader(await response.Content.ReadAsStreamAsync());
        while (await reader.ReadLineAsync() is { } line)
        {
            if (line.Length == 0) continue;

            Message part =
                JsonSerializer.Deserialize<ChatResponse>(line, AgentJsonOptions)!.Message;

            if (!string.IsNullOrEmpty(part.Thinking))
            {
                Console.Write(Grey + part.Thinking + Reset);
                thinking.Append(part.Thinking);
            }

            if (!string.IsNullOrEmpty(part.Content))
            {
                // The first word of the answer needs a gap after the grey, or the two run together.
                if (content.Length == 0 && thinking.Length > 0) Console.Write("\n\n");
                Console.Write(part.Content);
                content.Append(part.Content);
            }
        }
    }
}