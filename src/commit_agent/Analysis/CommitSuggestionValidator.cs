namespace commit_agent.Analysis;

public class CommitSuggestionValidator
{
    private static readonly HashSet<string> AllowedTypes =
    [
        "feat",
        "fix",
        "refactor",
        "docs",
        "test",
        "chore"
    ];

    public void Validate(CommitSuggestion suggestion)
    {
        if (string.IsNullOrWhiteSpace(suggestion.Type))
        {
            throw new InvalidOperationException(
                "The commit type is empty.");
        }

        if (!AllowedTypes.Contains(suggestion.Type.ToLowerInvariant()))
        {
            throw new InvalidOperationException(
                $"The commit type '{suggestion.Type}' is not a valid Conventional Commit type.");
        }

        if (string.IsNullOrWhiteSpace(suggestion.Description))
        {
            throw new InvalidOperationException(
                "The commit description is empty.");
        }
    }
}