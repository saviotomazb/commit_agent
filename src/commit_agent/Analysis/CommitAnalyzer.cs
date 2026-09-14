using commit_agent.AI;
using commit_agent.Git;

namespace commit_agent.Analysis;

public class CommitAnalyzer
{
    private readonly GitService _gitService;
    private readonly ILLMProvider _llmProvider;

    public CommitAnalyzer(
        GitService gitService,
        ILLMProvider llmProvider)
    {
        _gitService = gitService;
        _llmProvider = llmProvider;
    }

    public async Task<string> AnalyzeAsync()
    {
        var diff = await _gitService.GetDiffAsync();

        if (string.IsNullOrWhiteSpace(diff))
        {
            return string.Empty;
        }

        var prompt = $"""
            Analyze the following Git diff and generate a Conventional Commit message.

            Rules:
            - Use the format: type: description
            - Use one of these types: feat, fix, refactor, docs, test, chore
            - Keep the description concise.
            - Return only the commit message.
            - Do not use Markdown.
            - Do not add explanations.

            Git diff:
            {diff}
            """;

        return await _llmProvider.GenerateAsync(prompt);
    }
}