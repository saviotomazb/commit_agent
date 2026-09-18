using System.Text.Json;

namespace commit_agent.Analysis;

public class CommitSuggestionParser
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    private static readonly HashSet<string> AllowedTypes =
    [
        "feat",
        "fix",
        "refactor",
        "docs",
        "test",
        "chore"
    ];

    public CommitSuggestion Parse(string response)
    {
        if (string.IsNullOrWhiteSpace(response))
        {
            throw new InvalidOperationException(
                "The LLM returned an empty response.");
        }

        var json = ExtractJson(response);

        var suggestion = JsonSerializer.Deserialize<CommitSuggestion>(
            json,
            JsonOptions);

        if (suggestion is null)
        {
            throw new InvalidOperationException(
                "The LLM response could not be converted into a commit suggestion.");
        }

        Validate(suggestion);

        return suggestion;
    }

    private static string ExtractJson(string response)
    {
        var content = response.Trim();

        if (content.StartsWith("```"))
        {
            var firstLineEnd = content.IndexOf('\n');

            if (firstLineEnd >= 0)
            {
                content = content[(firstLineEnd + 1)..];
            }

            var closingFence = content.LastIndexOf("```");

            if (closingFence >= 0)
            {
                content = content[..closingFence];
            }

            content = content.Trim();
        }

        var jsonStart = content.IndexOf('{');
        var jsonEnd = content.LastIndexOf('}');

        if (jsonStart < 0 || jsonEnd < jsonStart)
        {
            throw new InvalidOperationException(
                "The LLM response does not contain a valid JSON object.");
        }

        return content[jsonStart..(jsonEnd + 1)];
    }

    private static void Validate(CommitSuggestion suggestion)
    {
        if (string.IsNullOrWhiteSpace(suggestion.Type))
        {
            throw new InvalidOperationException(
                "The commit type returned by the LLM is empty.");
        }

        if (!AllowedTypes.Contains(suggestion.Type.ToLowerInvariant()))
        {
            throw new InvalidOperationException(
                $"The commit type '{suggestion.Type}' is not a valid Conventional Commit type.");
        }

        if (string.IsNullOrWhiteSpace(suggestion.Description))
        {
            throw new InvalidOperationException(
                "The commit description returned by the LLM is empty.");
        }
    }
}