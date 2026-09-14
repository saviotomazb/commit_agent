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

    try
    {
        var diff = await gitService.GetDiffAsync();

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