using System.Net.Http.Json;

namespace commit_agent.AI;

public class OllamaProvider : ILLMProvider
{
    private readonly HttpClient _httpClient;
    private readonly OllamaOptions _options;

    public OllamaProvider(
        HttpClient httpClient,
        OllamaOptions options)
    {
        _httpClient = httpClient;
        _options = options;
    }

    public async Task<string> GenerateAsync(string prompt)
    {
        var request = new
        {
            model = _options.Model,
            prompt,
            stream = false,
            format = new
            {
                type = "object",
                properties = new
                {
                    type = new
                    {
                        type = "string",
                        @enum = new[]
                        {
                            "feat",
                            "fix",
                            "refactor",
                            "docs",
                            "test",
                            "chore"
                        }
                    },
                    description = new
                    {
                        type = "string"
                    }
                },
                required = new[]
                {
                    "type",
                    "description"
                }
            }
        };

        var response = await _httpClient.PostAsJsonAsync(
            "api/generate",
            request);

        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<OllamaResponse>();

        return result?.Response ?? string.Empty;
    }

    private class OllamaResponse
    {
        public string Response { get; set; } = string.Empty;
    }
}