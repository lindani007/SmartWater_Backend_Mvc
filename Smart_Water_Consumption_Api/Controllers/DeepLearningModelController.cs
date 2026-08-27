using Azure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Dynamic;
using System.Text;
using System.Text.Json;

namespace Smart_Water_Consumption_Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DeepLearningModelController : ControllerBase
    {
        private readonly HttpClient _httpClient;
        private const string LmStudioEndpoint = "http://127.0.0.1:1234/v1/chat/completions";

        public DeepLearningModelController()
        {
            _httpClient = new HttpClient();
            _httpClient.Timeout = TimeSpan.FromMinutes(10);
        }

        [HttpPost]
        public async Task<string> GetLLMResponse([FromBody] string prompt)
        {
            try
            {
                var requestPayload = new
                {
                    model = "qwen2.5-3b-instruct",
                                    messages = new[]
                    {
                        new
                        {
                            role = "user",
                            content = prompt
                        }
                    },
                    temperature = 0.2,
                    max_tokens = 384,
                    stream = false
                };

                var jsonOptions = new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                };

                var content = new StringContent(
                    JsonSerializer.Serialize(requestPayload, jsonOptions),
                    Encoding.UTF8,
                    "application/json"
                );

                var response = await _httpClient.PostAsync(LmStudioEndpoint, content);
                response.EnsureSuccessStatusCode();

                var responseJson = await response.Content.ReadAsStringAsync();
                using var doc = JsonDocument.Parse(responseJson);

                string resultText = doc.RootElement
                    .GetProperty("choices")[0]
                    .GetProperty("message")
                    .GetProperty("content")
                    .GetString() ?? string.Empty;

                return resultText;
            }
            catch (Exception ex)
            {
                // 	Error occurred while calling LLM: The JSON value could not be converted to System.String.Path: $ | LineNumber: 0 | BytePositionInLine: 1.
                Console.WriteLine($"Error occurred while calling LLM: {ex.Message}");
                return $"Error occurred while calling LLM: {ex.Message}";
            }
        }
    }
}
