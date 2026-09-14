using System.Diagnostics;

namespace commit_agent.Git;

public class GitService
{
    public async Task<string> GetDiffAsync()
    {
        var processStartInfo = new ProcessStartInfo
        {
            FileName = "git",
            Arguments = "diff",
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        using var process = new Process
        {
            StartInfo = processStartInfo
        };

        process.Start();

        var output = await process.StandardOutput.ReadToEndAsync();
        var error = await process.StandardError.ReadToEndAsync();

        await process.WaitForExitAsync();

        if (process.ExitCode != 0)
        {
            throw new InvalidOperationException(
                $"Falha ao executar git diff: {error}");
        }

        return output;
    }
}