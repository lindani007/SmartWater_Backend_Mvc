using Smart_Water_Consumption_Service.Models;
using System;
using System.Collections.Generic;
using System.Net.Http.Json;
using System.Text;

namespace Smart_Water_Consumption_Service.Services.DbServices
{
    public class RegionPredictionDbService
    {
        private readonly HttpClient _httpClient;

        public RegionPredictionDbService(ApiBaseUrl apiBaseUrl)
        {
            _httpClient = new HttpClient();
            if (!string.IsNullOrEmpty(apiBaseUrl.ApiUrl))
                _httpClient.BaseAddress = new Uri(apiBaseUrl.ApiUrl);
        }

        public async Task<MyGenericValidation<IEnumerable<RegionPrediction>>> GetAllRegionPredictions()
        {
            var response = await _httpClient.GetAsync("api/RegionPredictions");
            MyGenericValidation<IEnumerable<RegionPrediction>> validation = new MyGenericValidation<IEnumerable<RegionPrediction>>();
            if (response.IsSuccessStatusCode)
            {
                validation.IsSuccess = true;
                validation.Message = await response.Content.ReadAsStringAsync();
                validation.Data = await response.Content.ReadFromJsonAsync<IEnumerable<RegionPrediction>>() ?? new List<RegionPrediction>();
            }
            else
            {
                validation.IsSuccess = false;
                validation.Message = await response.Content.ReadAsStringAsync();
                validation.Data = null;
            }
            return validation;
        }

        public async Task<MyGenericValidation<RegionPrediction>> AddRegionPrediction(RegionPrediction regionprediction)
        {
            var response = await _httpClient.PostAsJsonAsync("api/RegionPredictions", regionprediction);
            MyGenericValidation<RegionPrediction> validation = new MyGenericValidation<RegionPrediction>();
            if (response.IsSuccessStatusCode)
            {
                validation.IsSuccess = true;
                validation.Message = await response.Content.ReadAsStringAsync();
                validation.Data = await response.Content.ReadFromJsonAsync<RegionPrediction>();
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
