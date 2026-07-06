using Smart_Water_Consumption_Service.Models;
using Smart_Water_Consumption_Service.Services.DbServices;
using System;
using System.Collections.Generic;
using System.Text;

namespace SmartWater_Citizen_Maui.ViewModels
{
    public class ConnectToSimulation
    {
        RegionConsumptionDbService regionConsumptionDbService;
        Sub_RegionConsumptionDbService sub_RegionConsumptionDbService;
        OwnerDbService ownerDbService;
        ApplianceConsumptionDbService applianceConsumptionDbService;
        ApplianceDbService applianceDbService;
        public ConnectToSimulation() 
        {
            ApiBaseUrl apiBaseUrl = new ApiBaseUrl { ApiUrl = "https://localhost:7203" };
            regionConsumptionDbService = new RegionConsumptionDbService(apiBaseUrl);
            sub_RegionConsumptionDbService = new Sub_RegionConsumptionDbService(apiBaseUrl);
            ownerDbService = new OwnerDbService(apiBaseUrl);
            applianceConsumptionDbService = new ApplianceConsumptionDbService(apiBaseUrl);
            applianceDbService = new ApplianceDbService(apiBaseUrl);
        }
        private async Task<IEnumerable<Smart_Water_Consumption_Service.Models.Appliance>> GetOwnerAppliences(string account)
        {
            var rawData = await applianceDbService.GetAllAppliance();
            return rawData.Data ?? new List<Smart_Water_Consumption_Service.Models.Appliance>();
        }
    }
}
