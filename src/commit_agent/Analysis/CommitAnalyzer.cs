using commit_agent.Git;

namespace commit_agent.Analysis;

public class CommitAnalyzer
{
    private readonly GitService _gitService;

    public CommitAnalyzer(GitService gitService)
    {
        _gitService = gitService;
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