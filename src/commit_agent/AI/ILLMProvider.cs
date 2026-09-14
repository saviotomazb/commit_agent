namespace commit_agent.AI;

public interface ILLMProvider
{
    Task<string> GenerateAsync(string prompt);
}