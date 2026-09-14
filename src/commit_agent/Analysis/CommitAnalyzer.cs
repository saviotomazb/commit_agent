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

        return diff;
    }
}