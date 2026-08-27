using Smart_Water_Consumption_Service.Models;
using System;
using System.Collections.Generic;
using System.Net.Http.Json;
using System.Text;

namespace Smart_Water_Consumption_Service.Services.DbServices
{
    public class ChatMessagesDbService
    {
        private readonly HttpClient _httpClient;

        public ChatMessagesDbService(ApiBaseUrl apiBaseUrl)
        {
            _httpClient = new HttpClient();
            if (!string.IsNullOrEmpty(apiBaseUrl.ApiUrl))
                _httpClient.BaseAddress = new Uri(apiBaseUrl.ApiUrl);
        }

        public async Task<MyGenericValidation<IEnumerable<ChatMessage>>> GetAllChatMessages()
        {
            var response = await _httpClient.GetAsync("api/ChatMessages");
            MyGenericValidation<IEnumerable<ChatMessage>> validation = new MyGenericValidation<IEnumerable<ChatMessage>>();
            if (response.IsSuccessStatusCode)
            {
                validation.IsSuccess = true;
                validation.Message = await response.Content.ReadAsStringAsync();
                validation.Data = await response.Content.ReadFromJsonAsync<IEnumerable<ChatMessage>>() ?? new List<ChatMessage>();
            }
            else
            {
                validation.IsSuccess = false;
                validation.Message = await response.Content.ReadAsStringAsync();
                validation.Data = null;
            }
            return validation;
        }

        public async Task<MyGenericValidation<List<ChatMessage>>> GetAllUseChatMessage(string id)
        {
            var response = await _httpClient.GetAsync($"api/ChatMessages/{id}");
            MyGenericValidation<List<ChatMessage>> validation = new MyGenericValidation<List<ChatMessage>>();
            if (response.IsSuccessStatusCode)
            {
                validation.IsSuccess = true;
                validation.Message = await response.Content.ReadAsStringAsync();
                validation.Data = await response.Content.ReadFromJsonAsync<List<ChatMessage>>() ?? new List<ChatMessage>();
            }
            else
            {
                validation.IsSuccess = false;
                validation.Message = await response.Content.ReadAsStringAsync();
                validation.Data = null;
            }
            return validation;
        }
        public async Task<MyGenericValidation<ChatMessage>> AddChatMessage(ChatMessage chatmessages)
        {
            var response = await _httpClient.PostAsJsonAsync("api/ChatMessages", chatmessages);
            MyGenericValidation<ChatMessage> validation = new MyGenericValidation<ChatMessage>();
            if (response.IsSuccessStatusCode)
            {
                validation.IsSuccess = true;
                validation.Message = await response.Content.ReadAsStringAsync();
                validation.Data = await response.Content.ReadFromJsonAsync<ChatMessage>();
            }
            else
            {
                validation.IsSuccess = false;
                validation.Message = await response.Content.ReadAsStringAsync();
                validation.Data = null;
            }
            return validation;
        }

        public async Task<MyGenericValidation<ChatMessage>> DeleteChatMessage(int? chatid)
        {
            var response = await _httpClient.DeleteAsync($"api/ChatMessages/{chatid}");
            MyGenericValidation<ChatMessage> validation = new MyGenericValidation<ChatMessage>();
            if (response.IsSuccessStatusCode)
            {
                validation.IsSuccess = true;
                validation.Message = await response.Content.ReadAsStringAsync();
                validation.Data = await response.Content.ReadFromJsonAsync<ChatMessage>();
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
