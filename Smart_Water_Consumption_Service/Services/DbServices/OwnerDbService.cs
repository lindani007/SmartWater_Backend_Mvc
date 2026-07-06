using Smart_Water_Consumption_Service.Models;
using System;
using System.Collections.Generic;
using System.Net.Http.Json;
using System.Text;

namespace Smart_Water_Consumption_Service.Services.DbServices
{
    public class OwnerDbService
    {
        private readonly HttpClient _httpClient;

        public OwnerDbService(ApiBaseUrl apiBaseUrl)
        {
            _httpClient = new HttpClient();
            if (!string.IsNullOrEmpty(apiBaseUrl.ApiUrl))
                _httpClient.BaseAddress = new Uri(apiBaseUrl.ApiUrl);
        }

        public async Task<MyGenericValidation<IEnumerable<Owner>>> GetAllOwners()
        {
            var response = await _httpClient.GetAsync("api/Owners");
            MyGenericValidation<IEnumerable<Owner>> validation = new MyGenericValidation<IEnumerable<Owner>>();
            if (response.IsSuccessStatusCode)
            {
                validation.IsSuccess = true;
                validation.Message = await response.Content.ReadAsStringAsync();
                validation.Data = await response.Content.ReadFromJsonAsync<IEnumerable<Owner>>() ?? new List<Owner>();
            }
            else
            {
                validation.IsSuccess = false;
                validation.Message = await response.Content.ReadAsStringAsync();
                validation.Data = null;
            }
            return validation;
        }

        public async Task<MyGenericValidation<Owner>> GetOwner(string accountNumber)
        {
            var response = await _httpClient.GetAsync($"api/Owners/{accountNumber}");
            MyGenericValidation<Owner> validation = new MyGenericValidation<Owner>();
            if (response.IsSuccessStatusCode)
            {
                validation.IsSuccess = true;
                validation.Message = await response.Content.ReadAsStringAsync();
                validation.Data = await response.Content.ReadFromJsonAsync<Owner>();
            }
            else
            {
                validation.IsSuccess = false;
                validation.Message = await response.Content.ReadAsStringAsync();
                validation.Data = null;
            }
            return validation;
        }

        public async Task<MyGenericValidation<Owner>> UpdateOwner(Owner owner)
        {
            var response = await _httpClient.PutAsJsonAsync("api/Owners", owner);
            MyGenericValidation<Owner> validation = new MyGenericValidation<Owner>();
            if (response.IsSuccessStatusCode)
            {
                validation.IsSuccess = true;
                validation.Message = await response.Content.ReadAsStringAsync();
                validation.Data = await response.Content.ReadFromJsonAsync<Owner>();
            }
            else
            {
                validation.IsSuccess = false;
                validation.Message = await response.Content.ReadAsStringAsync();
                validation.Data = null;
            }
            return validation;
        }

        public async Task<MyGenericValidation<Owner>> AddOwner(Owner owner)
        {
            var response = await _httpClient.PostAsJsonAsync("api/Owners", owner);
            MyGenericValidation<Owner> validation = new MyGenericValidation<Owner>();
            if (response.IsSuccessStatusCode)
            {
                validation.IsSuccess = true;
                validation.Message = await response.Content.ReadAsStringAsync();
                validation.Data = await response.Content.ReadFromJsonAsync<Owner>();
            }
            else
            {
                validation.IsSuccess = false;
                validation.Message = await response.Content.ReadAsStringAsync();
                validation.Data = null;
            }
            return validation;
        }

        public async Task<MyGenericValidation<string>> DeleteOwner(string? owner_idnumber)
        {
            var response = await _httpClient.DeleteAsync($"api/Owners/{owner_idnumber}");
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
