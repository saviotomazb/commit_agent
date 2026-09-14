using System.Net.Http.Json;

namespace commit_agent.AI;

public class OllamaProvider : ILLMProvider
{
    private readonly HttpClient _httpClient;

    public OllamaProvider(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<string> GenerateAsync(string prompt)
    {
        var request = new
        {
            model = "llama3.2",
            prompt,
            stream = false
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