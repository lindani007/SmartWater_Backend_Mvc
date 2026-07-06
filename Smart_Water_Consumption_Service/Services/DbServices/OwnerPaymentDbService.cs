using Smart_Water_Consumption_Service.Models;
using System;
using System.Collections.Generic;
using System.Net.Http.Json;
using System.Text;

namespace Smart_Water_Consumption_Service.Services.DbServices
{
    public class OwnerPaymentDbService
    {
        private readonly HttpClient _httpClient;

        public OwnerPaymentDbService(ApiBaseUrl apiBaseUrl)
        {
            _httpClient = new HttpClient();
            if (!string.IsNullOrEmpty(apiBaseUrl.ApiUrl))
                _httpClient.BaseAddress = new Uri(apiBaseUrl.ApiUrl);
        }

        public async Task<MyGenericValidation<IEnumerable<OwnerPayment>>> GetAllOwnersPayments()
        {
            var response = await _httpClient.GetAsync("api/OwnerPayments");
            MyGenericValidation<IEnumerable<OwnerPayment>> validation = new MyGenericValidation<IEnumerable<OwnerPayment>>();
            if (response.IsSuccessStatusCode)
            {
                validation.IsSuccess = true;
                validation.Message = await response.Content.ReadAsStringAsync();
                validation.Data = await response.Content.ReadFromJsonAsync<IEnumerable<OwnerPayment>>() ?? new List<OwnerPayment>();
            }
            else
            {
                validation.IsSuccess = false;
                validation.Message = await response.Content.ReadAsStringAsync();
                validation.Data = null;
            }
            return validation;
        }

        public async Task<MyGenericValidation<OwnerPayment>> GetOwnerPayment(int id)
        {
            var response = await _httpClient.GetAsync($"api/OwnerPayments/{id}");
            MyGenericValidation<OwnerPayment> validation = new MyGenericValidation<OwnerPayment>();
            if (response.IsSuccessStatusCode)
            {
                validation.IsSuccess = true;
                validation.Message = await response.Content.ReadAsStringAsync();
                validation.Data = await response.Content.ReadFromJsonAsync<OwnerPayment>();
            }
            else
            {
                validation.IsSuccess = false;
                validation.Message = await response.Content.ReadAsStringAsync();
                validation.Data = null;
            }
            return validation;
        }

        public async Task<MyGenericValidation<OwnerPayment>> PostOwnerPayment(OwnerPayment ownerpayment)
        {
            var response = await _httpClient.PostAsJsonAsync("api/OwnerPayments", ownerpayment);
            MyGenericValidation<OwnerPayment> validation = new MyGenericValidation<OwnerPayment>();
            if (response.IsSuccessStatusCode)
            {
                validation.IsSuccess = true;
                validation.Message = await response.Content.ReadAsStringAsync();
                validation.Data = await response.Content.ReadFromJsonAsync<OwnerPayment>();
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
