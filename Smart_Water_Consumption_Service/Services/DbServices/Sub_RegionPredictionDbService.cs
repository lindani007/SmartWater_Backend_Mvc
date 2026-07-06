using Smart_Water_Consumption_Service.Models;
using System;
using System.Collections.Generic;
using System.Net.Http.Json;
using System.Text;

namespace Smart_Water_Consumption_Service.Services.DbServices
{
    public class Sub_RegionPredictionDbService
    {
        private readonly HttpClient _httpClient;

        public Sub_RegionPredictionDbService(ApiBaseUrl apiBaseUrl)
        {
            _httpClient = new HttpClient();
            if (!string.IsNullOrEmpty(apiBaseUrl.ApiUrl))
                _httpClient.BaseAddress = new Uri(apiBaseUrl.ApiUrl);
        }

        public async Task<MyGenericValidation<IEnumerable<SubRegionPrediction>>> GetAllSub_RegionPredictions()
        {
            var response = await _httpClient.GetAsync("api/Sub_RegionPredictions");
            MyGenericValidation<IEnumerable<SubRegionPrediction>> validation = new MyGenericValidation<IEnumerable<SubRegionPrediction>>();
            if (response.IsSuccessStatusCode)
            {
                validation.IsSuccess = true;
                validation.Message = await response.Content.ReadAsStringAsync();
                validation.Data = await response.Content.ReadFromJsonAsync<IEnumerable<SubRegionPrediction>>() ?? new List<SubRegionPrediction>();
            }
            else
            {
                validation.IsSuccess = false;
                validation.Message = await response.Content.ReadAsStringAsync();
                validation.Data = null;
            }
            return validation;
        }

        public async Task<MyGenericValidation<SubRegionPrediction>> AddSub_RegionPrediction(SubRegionPrediction sub_regionprediction)
        {
            var response = await _httpClient.PostAsJsonAsync("api/Sub_RegionPredictions", sub_regionprediction);
            MyGenericValidation<SubRegionPrediction> validation = new MyGenericValidation<SubRegionPrediction>();
            if (response.IsSuccessStatusCode)
            {
                validation.IsSuccess = true;
                validation.Message = await response.Content.ReadAsStringAsync();
                validation.Data = await response.Content.ReadFromJsonAsync<SubRegionPrediction>();
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
