using Smart_Water_Consumption_Service.Models;
using Smart_Water_Consumption_Service.Services.DbServices;
using System;
using System.Collections.Generic;
using System.Net.Http.Json;
using System.Text;

namespace Smart_Water_Consumption_Service.Services.AiService.MachineLearningModel
{
    public class ModelPredictionsConnector
    {
        private HttpClient _httpClient;
        public ModelPredictionsConnector(ApiBaseUrl apiBaseUrl)
        {
            _httpClient = new HttpClient();
            if(!string.IsNullOrEmpty(apiBaseUrl.ApiUrl))
                _httpClient.BaseAddress = new Uri(apiBaseUrl.ApiUrl);
        }

        public async Task<int> PredictStatus(ConsumptionStatusRequest consumptionStatusRequest)
        {
            var response = await _httpClient.PostAsJsonAsync("api/Predictor/status", consumptionStatusRequest);
            if (response.IsSuccessStatusCode)
                return await response.Content.ReadFromJsonAsync<int>();
            return -1;
        }

        public async Task<double> PredictNextHourUsage(FuturePredictionRequest futurePredictionRequest)
        {
            var response = await _httpClient.PostAsJsonAsync("api/Predictor/hour", futurePredictionRequest);
            if (response.IsSuccessStatusCode)
                return await response.Content.ReadFromJsonAsync<double>();
            return -1;
        }

        public async Task<double> PredictNextWeekUsage(FuturePredictionRequest futurePredictionRequest)
        {
            var response = await _httpClient.PostAsJsonAsync("api/Predictor/week", futurePredictionRequest);
            if (response.IsSuccessStatusCode)
                return await response.Content.ReadFromJsonAsync<double>();
            return -1;
        }

        public async Task<double> PredictNextMonthUsage(FuturePredictionRequest futurePredictionRequest)
        {
            var response = await _httpClient.PostAsJsonAsync("api/Predictor/month", futurePredictionRequest);
            if (response.IsSuccessStatusCode)
                return await response.Content.ReadFromJsonAsync<double>();
            return -1;
        }
    }
}
