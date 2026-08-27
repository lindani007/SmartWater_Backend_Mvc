using Smart_Water_Consumption_Service.Models;
using System;
using System.Collections.Generic;
using System.Net.Http.Json;
using System.Text;

namespace Smart_Water_Consumption_Service.Services.AiService.DeepLearningModel
{
    public class MyLLMAiService
    {
        HttpClient _httpClient;
        public MyLLMAiService(ApiBaseUrl apiBaseUrl)
        {
            _httpClient = new HttpClient();
            if (!string.IsNullOrEmpty(apiBaseUrl.ApiUrl))
            {
                _httpClient.BaseAddress = new Uri(apiBaseUrl.ApiUrl);
                _httpClient.Timeout = TimeSpan.FromMinutes(10);
            }
        }

        public async Task<string> GetLLMResponse(string prompt)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("api/DeepLearningModel", prompt);
                return await response.Content.ReadAsStringAsync() ?? string.Empty;
            }
            catch (Exception ex)
            {
                return $"Error: {ex.Message}";
            }
        }
    }
}
