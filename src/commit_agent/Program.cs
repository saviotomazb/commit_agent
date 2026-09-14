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
    var analyzer = new CommitAnalyzer(gitService);

    try
    {
        var diff = await analyzer.AnalyzeAsync();

        if (string.IsNullOrWhiteSpace(diff))
        {
            Console.WriteLine("Nenhuma alteração encontrada.");
            return;
        }

        Console.WriteLine("Alterações encontradas:");
        Console.WriteLine(diff);
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