using commit_agent.AI;
using commit_agent.Analysis;
using commit_agent.Git;

Console.WriteLine("Commit Agent");

if (args.Length == 0)
{
    Console.WriteLine("Nenhum comando foi especificado.");
    return;
}

var command = args[0];

if (command == "analyze")
{
    var gitService = new GitService();

    using var httpClient = new HttpClient
    {
        BaseAddress = new Uri("http://localhost:11434/")
    };

    ILLMProvider llmProvider = new OllamaProvider(httpClient);

    var parser = new CommitSuggestionParser();

    var validator = new CommitSuggestionValidator();

    var analyzer = new CommitAnalyzer(
        gitService,
        llmProvider,
        parser,
        validator);

    try
    {
        var suggestion = await analyzer.AnalyzeAsync();

        if (suggestion is null)
        {
            Console.WriteLine("Nenhuma alteração encontrada.");
            return;
        }

        Console.WriteLine($"{suggestion.Type}: {suggestion.Description}");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Erro: {ex.Message}");
    }
}
else
{
    Console.WriteLine($"Comando não reconhecido: {command}");
}