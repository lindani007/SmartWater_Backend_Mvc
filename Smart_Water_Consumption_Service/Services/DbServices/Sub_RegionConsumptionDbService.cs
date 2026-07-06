using Smart_Water_Consumption_Service.Models;
using System;
using System.Collections.Generic;
using System.Net.Http.Json;
using System.Text;

namespace Smart_Water_Consumption_Service.Services.DbServices
{
    public class Sub_RegionConsumptionDbService
    {
        private readonly HttpClient _httpClient;

        public Sub_RegionConsumptionDbService(ApiBaseUrl apiBaseUrl)
        {
            _httpClient = new HttpClient();
            if (!string.IsNullOrEmpty(apiBaseUrl.ApiUrl))
                _httpClient.BaseAddress = new Uri(apiBaseUrl.ApiUrl);
        }

        public async Task<MyGenericValidation<IEnumerable<Sub_RegionConsumption>>> GetAllSub_RegionConsumption()
        {
            var response = await _httpClient.GetAsync("api/Sub_RegionConsumption");
            MyGenericValidation<IEnumerable<Sub_RegionConsumption>> validation = new MyGenericValidation<IEnumerable<Sub_RegionConsumption>>();
            if (response.IsSuccessStatusCode)
            {
                validation.IsSuccess = true;
                validation.Message = await response.Content.ReadAsStringAsync();
                validation.Data = await response.Content.ReadFromJsonAsync<IEnumerable<Sub_RegionConsumption>>() ?? new List<Sub_RegionConsumption>();
            }
            else
            {
                validation.IsSuccess = false;
                validation.Message = await response.Content.ReadAsStringAsync();
                validation.Data = null;
            }
            return validation;
        }

        public async Task<MyGenericValidation<Sub_RegionConsumption>> GetSub_RegionConsumption(int id)
        {
            var response = await _httpClient.GetAsync($"api/Sub_RegionConsumption/{id}");
            MyGenericValidation<Sub_RegionConsumption> validation = new MyGenericValidation<Sub_RegionConsumption>();
            if (response.IsSuccessStatusCode)
            {
                validation.IsSuccess = true;
                validation.Message = await response.Content.ReadAsStringAsync();
                validation.Data = await response.Content.ReadFromJsonAsync<Sub_RegionConsumption>();
            }
            else
            {
                validation.IsSuccess = false;
                validation.Message = await response.Content.ReadAsStringAsync();
                validation.Data = null;
            }
            return validation;
        }
        public async Task<MyGenericValidation<List<Sub_RegionConsumption>>> UpdateSub_RegionConsumption(List<Sub_RegionConsumption> sub_regionconsumption)
        {
            var response = await _httpClient.PutAsJsonAsync("api/Sub_RegionConsumption", sub_regionconsumption);
            MyGenericValidation<List<Sub_RegionConsumption>> validation = new MyGenericValidation<List<Sub_RegionConsumption>>();
            if (response.IsSuccessStatusCode)
            {
                validation.IsSuccess = true;
                validation.Message = await response.Content.ReadAsStringAsync();
                validation.Data = await response.Content.ReadFromJsonAsync<List<Sub_RegionConsumption>>();
            }
            else
            {
                validation.IsSuccess = false;
                validation.Message = await response.Content.ReadAsStringAsync();
                validation.Data = new List<Sub_RegionConsumption>();
            }
            return validation;
        }
        public async Task<MyGenericValidation<Sub_RegionConsumption>> PostSub_RegionConsumption(Sub_RegionConsumption sub_regionconsumption)
        {
            var response = await _httpClient.PostAsJsonAsync("api/Sub_RegionConsumption", sub_regionconsumption);
            MyGenericValidation<Sub_RegionConsumption> validation = new MyGenericValidation<Sub_RegionConsumption>();
            if (response.IsSuccessStatusCode)
            {
                validation.IsSuccess = true;
                validation.Message = await response.Content.ReadAsStringAsync();
                validation.Data = await response.Content.ReadFromJsonAsync<Sub_RegionConsumption>();
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
