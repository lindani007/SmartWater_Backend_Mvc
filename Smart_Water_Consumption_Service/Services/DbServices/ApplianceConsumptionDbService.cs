using Smart_Water_Consumption_Service.Models;
using System;
using System.Collections.Generic;
using System.Net.Http.Json;
using System.Text;

namespace Smart_Water_Consumption_Service.Services.DbServices
{
    public class ApplianceConsumptionDbService
    {
        private readonly HttpClient _httpClient;

        public ApplianceConsumptionDbService(ApiBaseUrl apiBaseUrl)
        {
            _httpClient = new HttpClient();
            if (!string.IsNullOrEmpty(apiBaseUrl.ApiUrl))
                _httpClient.BaseAddress = new Uri(apiBaseUrl.ApiUrl);
        }

        public async Task<MyGenericValidation<IEnumerable<ApplianceConsumption>>> GetAllApplianceConsumptions()
        {
            var response = await _httpClient.GetAsync("api/ApplianceConsumptions");
            MyGenericValidation<IEnumerable<ApplianceConsumption>> validation = new MyGenericValidation<IEnumerable<ApplianceConsumption>>();
            if (response.IsSuccessStatusCode)
            {
                validation.IsSuccess = true;
                validation.Message = await response.Content.ReadAsStringAsync();
                validation.Data = await response.Content.ReadFromJsonAsync<IEnumerable<ApplianceConsumption>>() ?? new List<ApplianceConsumption>();
            }
            else
            {
                validation.IsSuccess = false;
                validation.Message = await response.Content.ReadAsStringAsync();
                validation.Data = null;
            }
            return validation;
        }

        public async Task<MyGenericValidation<ApplianceConsumption>> GetApplianceConsumption(int id)
        {
            var response = await _httpClient.GetAsync($"api/ApplianceConsumptions/{id}");
            MyGenericValidation<ApplianceConsumption> validation = new MyGenericValidation<ApplianceConsumption>();
            if (response.IsSuccessStatusCode)
            {
                validation.IsSuccess = true;
                validation.Message = await response.Content.ReadAsStringAsync();
                validation.Data = await response.Content.ReadFromJsonAsync<ApplianceConsumption>();
            }
            else
            {
                validation.IsSuccess = false;
                validation.Message = await response.Content.ReadAsStringAsync();
                validation.Data = null;
            }
            return validation;
        }
        public async Task<MyGenericValidation<List<ApplianceConsumption>>> AddApplianceConsumption(List<ApplianceConsumption> applianceconsumption)
        {
            var response = await _httpClient.PostAsJsonAsync("api/ApplianceConsumptions", applianceconsumption);
            MyGenericValidation<List<ApplianceConsumption>> validation = new MyGenericValidation<List<ApplianceConsumption>>();
            if (response.IsSuccessStatusCode)
            {
                validation.IsSuccess = true;
                validation.Message = await response.Content.ReadAsStringAsync();
                validation.Data = await response.Content.ReadFromJsonAsync<List<ApplianceConsumption>>();
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
