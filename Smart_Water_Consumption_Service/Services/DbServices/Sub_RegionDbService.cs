using Smart_Water_Consumption_Service.Models;
using System;
using System.Collections.Generic;
using System.Net.Http.Json;
using System.Text;

namespace Smart_Water_Consumption_Service.Services.DbServices
{
    public class Sub_RegionDbService
    {
        private readonly HttpClient _httpClient;

        public Sub_RegionDbService(ApiBaseUrl apiBaseUrl)
        {
            _httpClient = new HttpClient();
            if (!string.IsNullOrEmpty(apiBaseUrl.ApiUrl))
                _httpClient.BaseAddress = new Uri(apiBaseUrl.ApiUrl);
        }

        public async Task<MyGenericValidation<IEnumerable<Sub_Region>>> GetAllSub_Regions()
        {
            var response = await _httpClient.GetAsync("api/Sub_Region");
            MyGenericValidation<IEnumerable<Sub_Region>> validation = new MyGenericValidation<IEnumerable<Sub_Region>>();
            if (response.IsSuccessStatusCode)
            {
                validation.IsSuccess = true;
                validation.Message = await response.Content.ReadAsStringAsync();
                validation.Data = await response.Content.ReadFromJsonAsync<IEnumerable<Sub_Region>>() ?? new List<Sub_Region>();
            }
            else
            {
                validation.IsSuccess = false;
                validation.Message = await response.Content.ReadAsStringAsync();
                validation.Data = null;
            }
            return validation;
        }

        public async Task<MyGenericValidation<Sub_Region>> GetSub_Region(int id)
        {
            var response = await _httpClient.GetAsync($"api/Sub_Region/{id}");
            MyGenericValidation<Sub_Region> validation = new MyGenericValidation<Sub_Region>();
            if (response.IsSuccessStatusCode)
            {
                validation.IsSuccess = true;
                validation.Message = await response.Content.ReadAsStringAsync();
                validation.Data = await response.Content.ReadFromJsonAsync<Sub_Region>();
            }
            else
            {
                validation.IsSuccess = false;
                validation.Message = await response.Content.ReadAsStringAsync();
                validation.Data = null;
            }
            return validation;
        }

        public async Task<MyGenericValidation<Sub_Region>> PutSub_Region(Sub_Region sub_region)
        {
            var response = await _httpClient.PutAsJsonAsync("api/Sub_Region", sub_region);
            MyGenericValidation<Sub_Region> validation = new MyGenericValidation<Sub_Region>();
            if (response.IsSuccessStatusCode)
            {
                validation.IsSuccess = true;
                validation.Message = await response.Content.ReadAsStringAsync();
                validation.Data = await response.Content.ReadFromJsonAsync<Sub_Region>();
            }
            else
            {
                validation.IsSuccess = false;
                validation.Message = await response.Content.ReadAsStringAsync();
                validation.Data = null;
            }
            return validation;
        }

        public async Task<MyGenericValidation<Sub_Region>> PostSub_Region(Sub_Region sub_region)
        {
            var response = await _httpClient.PostAsJsonAsync("api/Sub_Region", sub_region);
            MyGenericValidation<Sub_Region> validation = new MyGenericValidation<Sub_Region>();
            if (response.IsSuccessStatusCode)
            {
                validation.IsSuccess = true;
                validation.Message = await response.Content.ReadAsStringAsync();
                validation.Data = await response.Content.ReadFromJsonAsync<Sub_Region>();
            }
            else
            {
                validation.IsSuccess = false;
                validation.Message = await response.Content.ReadAsStringAsync();
                validation.Data = null;
            }
            return validation;
        }

        public async Task<MyGenericValidation<string>> DeleteSub_Region(int? id)
        {
            var response = await _httpClient.DeleteAsync($"api/Sub_Region/{id}");
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
