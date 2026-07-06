using Smart_Water_Consumption_Service.Models;
using System;
using System.Collections.Generic;
using System.Net.Http.Json;
using System.Text;

namespace Smart_Water_Consumption_Service.Services.DbServices
{
    public class AppliancePredictionDbService
    {
        private readonly HttpClient _httpClient;

        public AppliancePredictionDbService(ApiBaseUrl apiBaseUrl)
        {
            _httpClient = new HttpClient();
            if (!string.IsNullOrEmpty(apiBaseUrl.ApiUrl))
                _httpClient.BaseAddress = new Uri(apiBaseUrl.ApiUrl);
        }

        public async Task<MyGenericValidation<IEnumerable<AppliancePrediction>>> GetAllAppliancePredictions()
        {
            var response = await _httpClient.GetAsync("api/AppliancePredictions");
            MyGenericValidation<IEnumerable<AppliancePrediction>> validation = new MyGenericValidation<IEnumerable<AppliancePrediction>>();
            if (response.IsSuccessStatusCode)
            {
                validation.IsSuccess = true;
                validation.Message = await response.Content.ReadAsStringAsync();
                validation.Data = await response.Content.ReadFromJsonAsync<IEnumerable<AppliancePrediction>>() ?? new List<AppliancePrediction>();
            }
            else
            {
                validation.IsSuccess = false;
                validation.Message = await response.Content.ReadAsStringAsync();
                validation.Data = null;
            }
            return validation;
        }

        public async Task<MyGenericValidation<AppliancePrediction>> AddAppliancePrediction(AppliancePrediction applianceprediction)
        {
            var response = await _httpClient.PostAsJsonAsync("api/AppliancePredictions", applianceprediction);
            MyGenericValidation<AppliancePrediction> validation = new MyGenericValidation<AppliancePrediction>();
            if (response.IsSuccessStatusCode)
            {
                validation.IsSuccess = true;
                validation.Message = await response.Content.ReadAsStringAsync();
                validation.Data = await response.Content.ReadFromJsonAsync<AppliancePrediction>();
            }
            else
            {
                validation.IsSuccess = false;
                validation.Message = await response.Content.ReadAsStringAsync();
                validation.Data = null;
            }
            return validation;
        }
    }
}
