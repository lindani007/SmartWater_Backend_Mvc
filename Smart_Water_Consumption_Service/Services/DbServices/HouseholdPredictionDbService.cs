using Smart_Water_Consumption_Service.Models;
using System;
using System.Collections.Generic;
using System.Net.Http.Json;
using System.Text;

namespace Smart_Water_Consumption_Service.Services.DbServices
{
    public class HouseholdPredictionDbService
    {
        private readonly HttpClient _httpClient;

        public HouseholdPredictionDbService(ApiBaseUrl apiBaseUrl)
        {
            _httpClient = new HttpClient();
            if (!string.IsNullOrEmpty(apiBaseUrl.ApiUrl))
                _httpClient.BaseAddress = new Uri(apiBaseUrl.ApiUrl);
        }

        public async Task<MyGenericValidation<IEnumerable<HouseholdPrediction>>> GetAllHouseholdPredictions()
        {
            var response = await _httpClient.GetAsync("api/HouseholdPredictions");
            MyGenericValidation<IEnumerable<HouseholdPrediction>> validation = new MyGenericValidation<IEnumerable<HouseholdPrediction>>();
            if (response.IsSuccessStatusCode)
            {
                validation.IsSuccess = true;
                validation.Message = await response.Content.ReadAsStringAsync();
                validation.Data = await response.Content.ReadFromJsonAsync<IEnumerable<HouseholdPrediction>>() ?? new List<HouseholdPrediction>();
            }
            else
            {
                validation.IsSuccess = false;
                validation.Message = await response.Content.ReadAsStringAsync();
                validation.Data = null;
            }
            return validation;
        }

        public async Task<MyGenericValidation<HouseholdPrediction>> AddHouseholdPrediction(HouseholdPrediction householdprediction)
        {
            var response = await _httpClient.PostAsJsonAsync("api/HouseholdPredictions", householdprediction);
            MyGenericValidation<HouseholdPrediction> validation = new MyGenericValidation<HouseholdPrediction>();
            if (response.IsSuccessStatusCode)
            {
                validation.IsSuccess = true;
                validation.Message = await response.Content.ReadAsStringAsync();
                validation.Data = await response.Content.ReadFromJsonAsync<HouseholdPrediction>();
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
