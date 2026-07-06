using Smart_Water_Consumption_Service.Models;
using System;
using System.Collections.Generic;
using System.Net.Http.Json;
using System.Text;

namespace Smart_Water_Consumption_Service.Services.DbServices
{
    public class ApplianceDbService
    {
        private readonly HttpClient _httpClient;

        public ApplianceDbService(ApiBaseUrl apiBaseUrl)
        {
            _httpClient = new HttpClient();
            if (!string.IsNullOrEmpty(apiBaseUrl.ApiUrl))
                _httpClient.BaseAddress = new Uri(apiBaseUrl.ApiUrl);
        }

        public async Task<MyGenericValidation<IEnumerable<Appliance>>> GetAllAppliance()
        {
            var response = await _httpClient.GetAsync("api/Appliances");
            MyGenericValidation<IEnumerable<Appliance>> validation = new MyGenericValidation<IEnumerable<Appliance>>();
            if (response.IsSuccessStatusCode)
            {
                validation.IsSuccess = true;
                validation.Message = await response.Content.ReadAsStringAsync();
                validation.Data = await response.Content.ReadFromJsonAsync<IEnumerable<Appliance>>() ?? new List<Models.Appliance>();
            }
            else
            {
                validation.IsSuccess = false;
                validation.Message = await response.Content.ReadAsStringAsync();
                validation.Data = new List<Appliance>() ;
            }
            return validation;
        }

        public async Task<MyGenericValidation<Appliance>> GetAppliance(int appliance_id)
        {
            var response = await _httpClient.GetAsync($"api/Appliances/{appliance_id}");
            MyGenericValidation<Appliance> validation = new MyGenericValidation<Appliance>();
            if (response.IsSuccessStatusCode)
            {
                validation.IsSuccess = true;
                validation.Message = await response.Content.ReadAsStringAsync();
                validation.Data = await response.Content.ReadFromJsonAsync<Appliance>();
            }
            else
            {
                validation.IsSuccess = false;
                validation.Message = await response.Content.ReadAsStringAsync();
                validation.Data = null;
            }
            return validation;
        }

        public async Task<MyGenericValidation<Appliance>> UpdateAppliance(Appliance appliance)
        {
            var response = await _httpClient.PutAsJsonAsync("api/Appliances", appliance);
            MyGenericValidation<Appliance> validation = new MyGenericValidation<Appliance>();
            if (response.IsSuccessStatusCode)
            {
                validation.IsSuccess = true;
                validation.Message = await response.Content.ReadAsStringAsync();
                validation.Data = await response.Content.ReadFromJsonAsync<Appliance>();
            }
            else
            {
                validation.IsSuccess = false;
                validation.Message = await response.Content.ReadAsStringAsync();
                validation.Data = null;
            }
            return validation;
        }

        public async Task<MyGenericValidation<Appliance>> AddAppliance(Appliance appliance)
        {
            var response = await _httpClient.PostAsJsonAsync("api/Appliances", appliance);
            MyGenericValidation<Appliance> validation = new MyGenericValidation<Appliance>();
            if (response.IsSuccessStatusCode)
            {
                validation.IsSuccess = true;
                validation.Message = await response.Content.ReadAsStringAsync();
                validation.Data = await response.Content.ReadFromJsonAsync<Appliance>();
            }
            else
            {
                validation.IsSuccess = false;
                validation.Message = await response.Content.ReadAsStringAsync();
                validation.Data = null;
            }
            return validation;
        }
        public async Task<MyGenericValidation<string>> DeleteAppliance(int? appliance_id)
        {
            var response = await _httpClient.DeleteAsync($"api/Appliances/{appliance_id}");
            MyGenericValidation<string> validation = new MyGenericValidation<string>();
            if (response.IsSuccessStatusCode)
            {
                validation.IsSuccess = true;
                validation.Message = await response.Content.ReadAsStringAsync();
                validation.Data = await response.Content.ReadAsStringAsync();
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
