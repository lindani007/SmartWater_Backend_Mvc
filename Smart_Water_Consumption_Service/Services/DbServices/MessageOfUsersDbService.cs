using Smart_Water_Consumption_Service.Models;
using System;
using System.Collections.Generic;
using System.Net.Http.Json;
using System.Text;

namespace Smart_Water_Consumption_Service.Services.DbServices
{
    public class MessageOfUsersDbService
    {
        private readonly HttpClient _httpClient;

        public MessageOfUsersDbService(ApiBaseUrl apiBaseUrl)
        {
            _httpClient = new HttpClient();
            if (!string.IsNullOrEmpty(apiBaseUrl.ApiUrl))
                _httpClient.BaseAddress = new Uri(apiBaseUrl.ApiUrl);
        }

        public async Task<MyGenericValidation<IEnumerable<MessageOfUser>>> GetAllMessageOfUsers()
        {
            var response = await _httpClient.GetAsync("api/MessageOfUsers");
            MyGenericValidation<IEnumerable<MessageOfUser>> validation = new MyGenericValidation<IEnumerable<MessageOfUser>>();
            if (response.IsSuccessStatusCode)
            {
                validation.IsSuccess = true;
                validation.Message = await response.Content.ReadAsStringAsync();
                validation.Data = await response.Content.ReadFromJsonAsync<IEnumerable<MessageOfUser>>() ?? new List<MessageOfUser>();
            }
            else
            {
                validation.IsSuccess = false;
                validation.Message = await response.Content.ReadAsStringAsync();
                validation.Data = null;
            }
            return validation;
        }

        public async Task<MyGenericValidation<List<MessageOfUser>>> GetAllChatMessages(int id)
        {
            var response = await _httpClient.GetAsync($"api/MessageOfUsers/{id}");
            MyGenericValidation<List<MessageOfUser>> validation = new MyGenericValidation<List<MessageOfUser>>();
            if (response.IsSuccessStatusCode)
            {
                validation.IsSuccess = true;
                validation.Message = await response.Content.ReadAsStringAsync();
                validation.Data = await response.Content.ReadFromJsonAsync<List<MessageOfUser>>() ?? new List<MessageOfUser>();
            }
            else
            {
                validation.IsSuccess = false;
                validation.Message = await response.Content.ReadAsStringAsync();
                validation.Data = null;
            }
            return validation;
        }
        public async Task<MyGenericValidation<MessageOfUser>> AddMessageOfUser(MessageOfUser messageofusers)
        {
            var response = await _httpClient.PostAsJsonAsync("api/MessageOfUsers", messageofusers);
            MyGenericValidation<MessageOfUser> validation = new MyGenericValidation<MessageOfUser>();
            if (response.IsSuccessStatusCode)
            {
                validation.IsSuccess = true;
                validation.Message = await response.Content.ReadAsStringAsync();
                validation.Data = await response.Content.ReadFromJsonAsync<MessageOfUser>();
            }
            else
            {
                validation.IsSuccess = false;
                validation.Message = await response.Content.ReadAsStringAsync();
                validation.Data = null;
            }
            return validation;
        }

        public async Task<MyGenericValidation<List<MessageOfUser>>> DeleteMessageOfUser(int? messageid)
        {
            var response = await _httpClient.DeleteAsync($"api/MessageOfUsers/{messageid}");
            MyGenericValidation<List<MessageOfUser>> validation = new MyGenericValidation<List<MessageOfUser>>();
            if (response.IsSuccessStatusCode)
            {
                validation.IsSuccess = true;
                validation.Message = await response.Content.ReadAsStringAsync();
                validation.Data = await response.Content.ReadFromJsonAsync<List<MessageOfUser>>() ?? new List<MessageOfUser>();
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
