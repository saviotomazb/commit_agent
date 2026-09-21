using commit_agent.AI;
using commit_agent.Git;

namespace commit_agent.Analysis;

public class CommitAnalyzer
{
    private readonly GitService _gitService;
    private readonly ILLMProvider _llmProvider;
    private readonly CommitSuggestionParser _parser;
    private readonly CommitSuggestionValidator _validator;

    public CommitAnalyzer(
        GitService gitService,
        ILLMProvider llmProvider,
        CommitSuggestionParser parser,
        CommitSuggestionValidator validator)
    {
        _gitService = gitService;
        _llmProvider = llmProvider;
        _parser = parser;
        _validator = validator;
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

            Choose the commit type that best represents the primary change.

            Commit types:
            - feat: adds new functionality.
            - fix: corrects broken behavior.
            - refactor: restructures existing code without changing behavior.
            - docs: changes documentation.
            - test: changes tests.
            - chore: performs maintenance that does not affect application behavior.

            Description:
            - State the concrete change made by the diff.
            - Focus on the result of the change, not its implementation details.
            - Be specific about the affected component when useful.
            - Use imperative mood.
            - Keep it concise.
            - Do not repeat the commit type in the description.
            - Do not mention readability, maintainability, quality, or performance unless explicitly implemented.
            - Do not invent behavior or functionality.
            - Do not end with a period.
            - Start the description with a lowercase letter.

            Examples:
            feat: add commit suggestion parser
            fix: handle empty Git diff
            refactor: separate parsing from validation
            docs: update project roadmap
            test: add parser validation tests
            chore: update project dependencies

            Git diff:
            {diff}
            """;

        var response = await _llmProvider.GenerateAsync(prompt);

        var suggestion = _parser.Parse(response);

        _validator.Validate(suggestion);

        return suggestion;
    }
}