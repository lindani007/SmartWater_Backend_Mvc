using Smart_Water_Consumption_Service.Models;
using System;
using System.Collections.Generic;
using System.Net.Http.Json;
using System.Text;

namespace Smart_Water_Consumption_Service.Services.DbServices
{
    public class RegionConsumptionDbService
    {
        private readonly HttpClient _httpClient;

        public RegionConsumptionDbService(ApiBaseUrl apiBaseUrl)
        {
            _httpClient = new HttpClient();
            if (!string.IsNullOrEmpty(apiBaseUrl.ApiUrl))
                _httpClient.BaseAddress = new Uri(apiBaseUrl.ApiUrl);
        }

        public async Task<MyGenericValidation<IEnumerable<RegionConsumption>>> GetAllRegionsConsumptions()
        {
            var response = await _httpClient.GetAsync("api/RegionConsumptions");
            MyGenericValidation<IEnumerable<RegionConsumption>> validation = new MyGenericValidation<IEnumerable<RegionConsumption>>();
            if (response.IsSuccessStatusCode)
            {
                validation.IsSuccess = true;
                validation.Message = await response.Content.ReadAsStringAsync();
                validation.Data = await response.Content.ReadFromJsonAsync<IEnumerable<RegionConsumption>>() ?? new List<RegionConsumption>();
            }
            else
            {
                validation.IsSuccess = false;
                validation.Message = await response.Content.ReadAsStringAsync();
                validation.Data = null;
            }
            return validation;
        }

        public async Task<MyGenericValidation<RegionConsumption>> GetRegionConsumption(int id)
        {
            var response = await _httpClient.GetAsync($"api/RegionConsumptions/{id}");
            MyGenericValidation<RegionConsumption> validation = new MyGenericValidation<RegionConsumption>();
            if (response.IsSuccessStatusCode)
            {
                validation.IsSuccess = true;
                validation.Message = await response.Content.ReadAsStringAsync();
                validation.Data = await response.Content.ReadFromJsonAsync<RegionConsumption>();
            }
            else
            {
                validation.IsSuccess = false;
                validation.Message = await response.Content.ReadAsStringAsync();
                validation.Data = null;
            }
            return validation;
        }
        public async Task<MyGenericValidation<List<RegionConsumption>>> PutRegionConsumption(List<RegionConsumption> regionconsumption)
        {
            var response = await _httpClient.PutAsJsonAsync("api/RegionConsumptions", regionconsumption);
            MyGenericValidation<List<RegionConsumption>> validation = new MyGenericValidation<List<RegionConsumption>>();
            if (response.IsSuccessStatusCode)
            {
                validation.IsSuccess = true;
                validation.Message = await response.Content.ReadAsStringAsync();
                validation.Data = await response.Content.ReadFromJsonAsync<List<RegionConsumption>>();
            }
            else
            {
                validation.IsSuccess = false;
                validation.Message = await response.Content.ReadAsStringAsync();
                validation.Data = new List<RegionConsumption>();
            }
            return validation;
        }
        public async Task<MyGenericValidation<RegionConsumption>> PostRegionConsumption(RegionConsumption regionconsumption)
        {
            var response = await _httpClient.PostAsJsonAsync("api/RegionConsumptions", regionconsumption);
            MyGenericValidation<RegionConsumption> validation = new MyGenericValidation<RegionConsumption>();
            if (response.IsSuccessStatusCode)
            {
                validation.IsSuccess = true;
                validation.Message = await response.Content.ReadAsStringAsync();
                validation.Data = await response.Content.ReadFromJsonAsync<RegionConsumption>();
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
