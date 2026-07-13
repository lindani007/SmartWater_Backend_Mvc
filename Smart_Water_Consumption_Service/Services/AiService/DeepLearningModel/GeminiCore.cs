using GenerativeAI;
using System;
using System.Collections.Generic;
using System.Text;

namespace Smart_Water_Consumption_Service.Services.AiService.DeepLearningModel
{
    public class GeminiCore
    {
        private readonly GenerativeModel _model;
        public GeminiCore()
        {
            string apiKey = "AQ.Ab8RN6Jw85TPmTWENDzzh9f3FOXD_oNpWwEfi05-ahdFEmHKkA";
            _model = new GenerativeModel(apiKey, "gemini-3.5-flash");
        }

        public async Task<string> GetGeminiResponse(string prompt)
        {
            var response = await _model.GenerateContentAsync(prompt);
            return response.Text;
        }
    }
}
