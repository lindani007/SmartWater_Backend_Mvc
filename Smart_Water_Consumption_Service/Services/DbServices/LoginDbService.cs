using Smart_Water_Consumption_Service.Models;
using System;
using System.Collections.Generic;
using System.Net.Http.Json;
using System.Text;

namespace Smart_Water_Consumption_Service.Services.DbServices
{
    public class LoginDbService
    {
        private readonly HttpClient _httpClient;

        public  LoginDbService(ApiBaseUrl apiBaseUrl)
        {
            _httpClient = new HttpClient();
            if(!string.IsNullOrEmpty(apiBaseUrl.ApiUrl))
                _httpClient.BaseAddress = new Uri(apiBaseUrl.ApiUrl);
        }

        public async Task<IEnumerable<Login>> ReturnUsers()
        {
            var response = await _httpClient.GetAsync("api/Logins");
            return await response.Content.ReadFromJsonAsync<IEnumerable<Login>>() ?? new List<Login>();
        }

        public async Task<MyGenericValidation<Login>> GetCitezen(string accountNumber)
        {
            var response = await _httpClient.GetAsync($"api/Logins/account/{accountNumber}");
            MyGenericValidation<Login> validation = new MyGenericValidation<Login>();
            if (response.IsSuccessStatusCode)
            {
                validation.IsSuccess = true;
                validation.Message = await response.Content.ReadAsStringAsync();
                validation.Data = await response.Content.ReadFromJsonAsync<Login>();
            }
            else
            {
                validation.IsSuccess = false;
                validation.Message = await response.Content.ReadAsStringAsync();
                validation.Data = null;
            }
            return validation;
        }

        public async Task<MyGenericValidation<Login>> GetEmployee(string employeeNumber)
        {
            var response = await _httpClient.GetAsync($"api/Logins/employeenumber/{employeeNumber}");
            MyGenericValidation<Login> validation = new MyGenericValidation<Login>();
            if (response.IsSuccessStatusCode)
            {
                validation.IsSuccess = true;
                validation.Message = await response.Content.ReadAsStringAsync();
                validation.Data = await response.Content.ReadFromJsonAsync<Login>();
            }
            else
            {
                validation.IsSuccess = false;
                validation.Message = await response.Content.ReadAsStringAsync();
                validation.Data = null;
            }
            return validation;
        }
        public async Task<MyGenericValidation<Login>> UpdatePassword(Login login)
        {
            var response = await _httpClient.PutAsJsonAsync($"api/Logins", login);
            MyGenericValidation<Login> validation = new MyGenericValidation<Login>();
            if (response.IsSuccessStatusCode)
            {
                validation.IsSuccess = true;
                validation.Message = await response.Content.ReadAsStringAsync();
                validation.Data = await response.Content.ReadFromJsonAsync<Login>();
            }
            else
            {
                validation.IsSuccess = false;
                validation.Message = await response.Content.ReadAsStringAsync();
                validation.Data = null;
            }
            return validation;
        }

        public async Task<MyGenericValidation<Login>> AddUser(Login login)
        {
            var response = await _httpClient.PostAsJsonAsync($"api/Logins", login);
            MyGenericValidation<Login> validation = new MyGenericValidation<Login>();
            if (response.IsSuccessStatusCode)
            {
                validation.IsSuccess = true;
                validation.Message = await response.Content.ReadAsStringAsync();
                validation.Data = await response.Content.ReadFromJsonAsync<Login>();
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
