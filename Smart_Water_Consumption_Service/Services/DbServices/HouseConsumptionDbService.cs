using Smart_Water_Consumption_Service.Models;
using System;
using System.Collections.Generic;
using System.Net.Http.Json;
using System.Text;

namespace Smart_Water_Consumption_Service.Services.DbServices
{
    public class HouseConsumptionDbService
    {
        private readonly HttpClient _httpClient;

        public HouseConsumptionDbService(ApiBaseUrl apiBaseUrl)
        {
            _httpClient = new HttpClient();
            if (!string.IsNullOrEmpty(apiBaseUrl.ApiUrl))
                _httpClient.BaseAddress = new Uri(apiBaseUrl.ApiUrl);
        }

        public async Task<MyGenericValidation<IEnumerable<HouseConsumption>>> GetAllHouseConsumption()
        {
            var response = await _httpClient.GetAsync("api/HouseConsumptions");
            MyGenericValidation<IEnumerable<HouseConsumption>> validation = new MyGenericValidation<IEnumerable<HouseConsumption>>();
            if (response.IsSuccessStatusCode)
            {
                validation.IsSuccess = true;
                validation.Message = await response.Content.ReadAsStringAsync();
                validation.Data = await response.Content.ReadFromJsonAsync<IEnumerable<HouseConsumption>>() ?? new List<HouseConsumption>();
            }
            else
            {
                validation.IsSuccess = false;
                validation.Message = await response.Content.ReadAsStringAsync();
                validation.Data = null;
            }
            return validation;
        }

        public async Task<MyGenericValidation<HouseConsumption>> GetHouseConsumption(string household_id)
        {
            var response = await _httpClient.GetAsync($"api/HouseConsumptions/{household_id}");
            MyGenericValidation<HouseConsumption> validation = new MyGenericValidation<HouseConsumption>();
            if (response.IsSuccessStatusCode)
            {
                validation.IsSuccess = true;
                validation.Message = await response.Content.ReadAsStringAsync();
                validation.Data = await response.Content.ReadFromJsonAsync<HouseConsumption>();
            }
            else
            {
                validation.IsSuccess = false;
                validation.Message = await response.Content.ReadAsStringAsync();
                validation.Data = null;
            }
            return validation;
        }

        public async Task<MyGenericValidation<List<HouseConsumption>>> UpdateHouseConsumption(List<HouseConsumption> houseconsumptions)
        {
            var response = await _httpClient.PutAsJsonAsync("api/HouseConsumptions", houseconsumptions);
            MyGenericValidation<List<HouseConsumption>> validation = new MyGenericValidation<List<HouseConsumption>>();
            if (response.IsSuccessStatusCode)
            {
                validation.IsSuccess = true;
                validation.Message = await response.Content.ReadAsStringAsync();
                validation.Data = await response.Content.ReadFromJsonAsync<List<HouseConsumption>>();
            }
            else
            {
                validation.IsSuccess = false;
                validation.Message = await response.Content.ReadAsStringAsync();
                validation.Data = new List<HouseConsumption>();
            }
            return validation;
        }
        public async Task<MyGenericValidation<HouseConsumption>> AddHouseConsumption(HouseConsumption houseconsumption)
        {
            var response = await _httpClient.PostAsJsonAsync("api/HouseConsumptions", houseconsumption);
            MyGenericValidation<HouseConsumption> validation = new MyGenericValidation<HouseConsumption>();
            if (response.IsSuccessStatusCode)
            {
                validation.IsSuccess = true;
                validation.Message = await response.Content.ReadAsStringAsync();
                validation.Data = await response.Content.ReadFromJsonAsync<HouseConsumption>();
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
