using commit_agent.AI;
using commit_agent.Analysis;
using commit_agent.Git;
using Microsoft.Extensions.Configuration;

const string Version = "0.1.0";

if (args.Length == 0)
{
    ShowHelp();
    return;
}

var command = args[0].ToLowerInvariant();

switch (command)
{
    case "analyze":
        await AnalyzeAsync();
        break;

    case "help":
        ShowHelp();
        break;

    case "version":
        ShowVersion();
        break;

    default:
        Console.WriteLine($"Comando não reconhecido: {command}");
        Console.WriteLine();
        ShowHelp();
        break;
}

static async Task AnalyzeAsync()
{
    try
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: false)
            .Build();

        var ollamaOptions = configuration
            .GetSection("Ollama")
            .Get<OllamaOptions>()
            ?? throw new InvalidOperationException(
                "Ollama configuration was not found.");

        var gitService = new GitService();

        using var httpClient = new HttpClient
        {
            BaseAddress = new Uri("http://localhost:11434/")
        };

        ILLMProvider llmProvider = new OllamaProvider(
            httpClient,
            ollamaOptions);

        var parser = new CommitSuggestionParser();
        var validator = new CommitSuggestionValidator();

        var analyzer = new CommitAnalyzer(
            gitService,
            llmProvider,
            parser,
            validator);

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

static void ShowHelp()
{
    Console.WriteLine();
    Console.WriteLine("Usage:");
    Console.WriteLine("  commit-agent <command>");
    Console.WriteLine();
    Console.WriteLine("Commands:");
    Console.WriteLine("  analyze    Analyze Git changes and generate a commit suggestion");
    Console.WriteLine("  help       Show available commands");
    Console.WriteLine("  version    Show application version");
}

static void ShowVersion()
{
    Console.WriteLine($"Version: {Version}");
}