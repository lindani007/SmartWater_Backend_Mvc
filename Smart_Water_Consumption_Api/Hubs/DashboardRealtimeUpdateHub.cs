using Microsoft.AspNetCore.SignalR;

namespace Smart_Water_Consumption_Api.Hubs
{
    public class DashboardRealtimeUpdateHub: Hub
    {
        public async Task SendDataToDashboards(bool anyUpdate)
        {
            await Clients.All.SendAsync("ReceiveData", anyUpdate);
        }
    }
}
