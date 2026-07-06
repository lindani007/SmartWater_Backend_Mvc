using Microsoft.AspNetCore.SignalR.Client;
using Smart_Water_Consumption_Service.Services.SimulationService;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace Smart_Water_Consumption_Service.Services.SignalRService
{
    public class RealtimeDashboard
    {
        public event Action<bool>? OnDataReceived;
        private HubConnection? _connection;
        public async Task<string> StartConnection()
        {
            try
            {
                _connection = new HubConnectionBuilder()
                .WithUrl("https://localhost:7203/dashboardHub")
                .Build();
                _connection.On<bool>("ReceiveData", (anyUpdate) =>
                {
                    OnDataReceived?.Invoke(anyUpdate);
                });
                await _connection.StartAsync();
                Console.WriteLine("Connection started.");
                return "Connection started.";
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error occurred: {ex.Message}");
                return $"Error occurred: {ex.Message}";
            }
            
        }

        public async Task SendData(bool anyUpdate)
        {
            try
            {
                if (_connection != null && _connection.State == HubConnectionState.Connected)
                {
                    await _connection.InvokeAsync("SendDataToDashboards", anyUpdate);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error occurred while sending data: {ex.ToString()}");
            }
        }
    }
}
