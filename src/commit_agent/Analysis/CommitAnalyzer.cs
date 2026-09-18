using commit_agent.AI;
using commit_agent.Git;

namespace commit_agent.Analysis;

public class CommitAnalyzer
{
    private readonly GitService _gitService;
    private readonly ILLMProvider _llmProvider;
    private readonly CommitSuggestionParser _parser;

    public CommitAnalyzer(
        GitService gitService,
        ILLMProvider llmProvider,
        CommitSuggestionParser parser)
    {
        _gitService = gitService;
        _llmProvider = llmProvider;
        _parser = parser;
    }

    public async Task<CommitSuggestion?> AnalyzeAsync()
    {
        var diff = await _gitService.GetDiffAsync();

        if (string.IsNullOrWhiteSpace(diff))
        {
            return null;
        }

        var prompt = $"""
            Analyze the following Git diff and generate a Conventional Commit suggestion.

            Your response MUST be valid JSON.
            Do not use Markdown.
            Do not use code fences.
            Do not include any text before or after the JSON.

            The JSON must contain exactly these properties:
            "type": the Conventional Commit type
            "description": the commit description

            Rules:
            - Use one of these types: feat, fix, refactor, docs, test, chore.
            - Keep the description concise.

            Git diff:
            {diff}
            """;

        var response = await _llmProvider.GenerateAsync(prompt);

        return _parser.Parse(response);
    }
}