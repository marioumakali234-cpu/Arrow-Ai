using AIProject.Models;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace AIProject.Services
{
    public class ArrowAI : IAiService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public ArrowAI(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
        }

        public async Task<string> GetResponseAsync(List<ChatMessage> messages)
        {
            var apiKey = _configuration["OpenRouter:ApiKey"];
            var model = _configuration["OpenRouter:Model"];

            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", apiKey);

            var requestBody = new
            {
                model = model,
                messages = messages
            };

            var json = JsonSerializer.Serialize(
                requestBody,
                new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                }
            );
            var content = new StringContent(
                json,
                Encoding.UTF8,
                "application/json"
            );

            var response = await _httpClient.PostAsync(
                "https://openrouter.ai/api/v1/chat/completions",
                content
            );

            var responseText = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception(
                    $"OpenRouter Error: {response.StatusCode} - {responseText}"
                );
            }

            using var document = JsonDocument.Parse(responseText);

            var answer = document
                .RootElement
                .GetProperty("choices")[0]
                .GetProperty("message")
                .GetProperty("content")
                .GetString();

            return answer ?? "No response from AI.";
        }
    }
}