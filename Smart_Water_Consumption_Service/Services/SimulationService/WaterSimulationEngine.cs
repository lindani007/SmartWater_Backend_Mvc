using Microsoft.AspNetCore.SignalR;
using Smart_Water_Consumption_Service.Models;
using Smart_Water_Consumption_Service.Services.DbServices;
using Smart_Water_Consumption_Service.Services.SignalRService;
using Smart_Water_Consumption_Service.Services.TarrifService;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Net.NetworkInformation;
using System.Runtime.Intrinsics.X86;
using System.Text;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Smart_Water_Consumption_Service.Services.SimulationService
{
    public class WaterSimulationEngine
    {
        private ApplianceConsumptionDbService _applianceConsumptionDbService;
        private HouseConsumptionDbService _houseConsumptionDbService;
        private Sub_RegionConsumptionDbService _sub_RegionConsumptionDbService;
        private RegionConsumptionDbService _regionConsumptionDbService;
        private ApplianceDbService _applianceDbService;
        private OwnerDbService _ownerDbService;
        private RegionDbService _regiondbservice;
        private Sub_RegionDbService _sub_RegionDbService;
        public WaterSimulationEngine()
        {
            ApiBaseUrl apiBaseUrl = new ApiBaseUrl { ApiUrl = "https://localhost:7203" };
            _applianceConsumptionDbService = new ApplianceConsumptionDbService(apiBaseUrl);
            _houseConsumptionDbService = new HouseConsumptionDbService(apiBaseUrl);
            _applianceDbService = new ApplianceDbService(apiBaseUrl);
            _sub_RegionConsumptionDbService = new Sub_RegionConsumptionDbService(apiBaseUrl);
            _regionConsumptionDbService = new RegionConsumptionDbService(apiBaseUrl);
            _ownerDbService = new OwnerDbService(apiBaseUrl);
            _regiondbservice = new RegionDbService(apiBaseUrl);
            _sub_RegionDbService = new Sub_RegionDbService(apiBaseUrl);
        }
        Random _random = new Random();
        public async Task RunSimulation()
        {
            try
            {
                RealtimeDashboard realtimeDashboard = new RealtimeDashboard();
                
                await AddAplianceConsumptionsToDb(await GetApplianceConsumptions());
                await UpdateHouseConsumptionsToDb(await GetHouseConsumptions());
                await UpdateSubRegionConsumptionsToDb(await GetSubRegionConsumption());
                await UpdateRegionConsumptionsToDb(await GetRegionConsumption());

                await realtimeDashboard.StartConnection();
                bool updatesAvailable = true;
                await realtimeDashboard.SendData(updatesAvailable);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in WaterSimulationEngine.RunSimulation: {ex.Message}");
            }
        }
        private async Task<string> AddAplianceConsumptionsToDb(List<ApplianceConsumption> applianceConsumptions)
        {
            var result = await _applianceConsumptionDbService.AddApplianceConsumption(applianceConsumptions);
            if (result.IsSuccess)
                return "ok";
            return result.Message;
        }
        private async Task<string> UpdateHouseConsumptionsToDb(List<HouseConsumption> houseConsumptions)
        {
            var result = await _houseConsumptionDbService.UpdateHouseConsumption(houseConsumptions);
            if (result.IsSuccess)
                return "ok";
            return result.Message;
        }
        private async Task<string> UpdateSubRegionConsumptionsToDb(List<Sub_RegionConsumption> subRegionConsumptions)
        {
            var result = await _sub_RegionConsumptionDbService.UpdateSub_RegionConsumption(subRegionConsumptions);
            if (result.IsSuccess)
                return "ok";
            return result.Message;
        }
        private async Task<string> UpdateRegionConsumptionsToDb(List<RegionConsumption> RegionConsumptions)
        {
            var result = await _regionConsumptionDbService.PutRegionConsumption(RegionConsumptions);
            if (result.IsSuccess)
                return "ok";
            return result.Message;
        }

        public async Task<List<ApplianceConsumption>> GetApplianceConsumptions()
        {
            var apps = await _applianceDbService.GetAllAppliance();
            var list = apps.Data.Select(x => new Appliance
            {
                Appliance_Id = x.Appliance_Id,
                Appliance_MeterNumber = x.Appliance_MeterNumber,
                Appliance_Name = x.Appliance_Name,
                NumberOfPeopleUsingIt = x.NumberOfPeopleUsingIt,
                Owner_AccountNumber = x.Owner_AccountNumber,
            }).ToList();

            return await SimulateApplianceConsumption(list);
        }
        public async Task<List<HouseConsumption>> GetHouseConsumptions()
        {

            var applianceResult = await _applianceConsumptionDbService.GetAllApplianceConsumptions();

            List<ApplianceConsumption> applianceConsumptions = applianceResult.Data
                .Where(x => x.Date.Value.Month == DateTime.Now.Month && x.Date.Value.Year == DateTime.Now.Year)
                .ToList();

            var houseResult = await _houseConsumptionDbService.GetAllHouseConsumption();
            if (!houseResult.Data.Any())
            {
                List<HouseConsumption> newhouses = new();

                var groups = applianceConsumptions
                    .GroupBy(x => x.Account);

                foreach (var group in groups)
                {
                    double daily = Math.Round(group.Sum(x => x.Daily_Usage / 1000 ?? 0), 2);
                    double night = Math.Round(group.Sum(x => x.Night_Usage / 1000 ?? 0), 2);
                    double cost = await CalculateExpectedAmount((daily + night), 6, group.Key);
                     
                    newhouses.Add(new HouseConsumption
                    {
                        Household_ID = await GetOwnersHousId(group.Key),
                        Owner_AccountNumber = group.Key,
                        Daily_Usage = daily,
                        Night_Usage = night,
                        Total_WaterUsed = Math.Round((daily + night), 2),
                        Leakage_Percent = 0.00,
                        Actual_Amount = cost,
                        Extepected_Amount = cost,
                        OutStanding_Amount = Math.Round(group.Sum(x => x.Cost ?? 0), 2) - Math.Round(group.Sum(x => x.Cost ?? 0), 2),
                        Spike_Usage = 0,
                        Water_Received = Math.Round((daily + night), 2),
                        FreeWater = 6,
                        Payment_Status = "Free",
                        Date = DateTime.Now,
                        Sub_RegionId = await GetOwnersSubRegionId(group.Key),

                    });
                }
                return newhouses;
            }

            List<HouseConsumption> houses = houseResult.Data
                .Select(x => new HouseConsumption
                {
                    Id = x.Id,
                    Household_ID = x.Household_ID,
                    Owner_AccountNumber = x.Owner_AccountNumber,
                    Water_Received = x.Water_Received,
                    Total_WaterUsed = x.Total_WaterUsed,
                    Leakage_Percent = x.Leakage_Percent,
                    Daily_Usage = x.Daily_Usage,
                    Night_Usage = x.Night_Usage,
                    Spike_Usage = x.Spike_Usage,
                    Date = x.Date,
                    Sub_RegionId = x.Sub_RegionId,
                    Extepected_Amount = x.Extepected_Amount,
                    Actual_Amount = x.Actual_Amount,
                    OutStanding_Amount = x.OutStanding_Amount,
                    FreeWater = x.FreeWater,
                    Payment_Status = x.Payment_Status
                }).ToList();

            foreach (var house in houses)
            {
                if (house.Date?.Month != DateTime.Now.Month || house.Date?.Year != DateTime.Now.Year)
                {
                    house.Daily_Usage = 0;
                    house.Night_Usage = 0;
                    house.Total_WaterUsed = 0;
                    house.Actual_Amount = 0;
                    house.Extepected_Amount = 0;
                    house.OutStanding_Amount = 0;
                    house.Spike_Usage = 0;
                    house.Leakage_Percent = 0;
                    house.Water_Received = 0;
                }

                var data = applianceConsumptions.Where(x => x.Account == house.Owner_AccountNumber).ToList();

                double daily = Math.Round(data.Sum(x => x.Daily_Usage / 1000 ?? 0), 2);
                double night = Math.Round(data.Sum(x => x.Night_Usage / 1000 ?? 0), 2);
                double cost = Math.Round(data.Sum(x => x.Cost ?? 0), 2);

                house.Daily_Usage = daily;
                house.Night_Usage = night;
                house.Total_WaterUsed = Math.Round((daily + night), 2);
                house.Actual_Amount = cost;
                house.Extepected_Amount = cost;
                house.Leakage_Percent = 0.00;
                house.Spike_Usage = Math.Max(0, (house.Water_Received ?? 0) - (daily + night));
                house.OutStanding_Amount = (house.Extepected_Amount ?? 0) - (house.Actual_Amount ?? 0);
                house.Payment_Status = house.OutStanding_Amount > 0 ? "Outstanding" : "Paid";
                house.Date = DateTime.Now;
                house.FreeWater = 6;
                house.Water_Received = Math.Round((daily + night), 2);
            }

            return houses;
        }
        public async Task<List<Sub_RegionConsumption>> GetSubRegionConsumption()
        {

            var houseResult = await _houseConsumptionDbService.GetAllHouseConsumption();

            List<HouseConsumption> houses = houseResult.Data
                .Where(x => x.Date?.Month == DateTime.Now.Month &&
                            x.Date?.Year == DateTime.Now.Year)
                .ToList();


            var subResult = await _sub_RegionConsumptionDbService.GetAllSub_RegionConsumption();

            if (!subResult.Data.Any())
            {
                List<Sub_RegionConsumption> newsubs = new();

                var groups = houses.GroupBy(x => x.Sub_RegionId);

                foreach (var group in groups)
                {
                    double dayUmbush = 0.00;
                    double nightUmbush = 0.00;
                    if (_random.Next(0, 101) < 60)
                    {
                        (dayUmbush, nightUmbush) = SimulateWaterAmbush(_random.Next(0, 25));
                    }

                    double leakagePipe = 0.00;
                    if (_random.Next(0, 101) < 40)
                    {
                        leakagePipe = SimulateLeakingPipe();
                    }

                    double leakPipeKl = Math.Round(leakagePipe / 1000, 2);
                    double dayambushKl = Math.Round(dayUmbush / 1000, 2);
                    double nightambushKl = Math.Round(nightUmbush / 1000, 2);


                    double watersent = Math.Round((group.Sum(x => x.Total_WaterUsed ?? 0) + dayambushKl + nightambushKl + leakPipeKl), 2);
                    double waterUsed = Math.Round((group.Sum(x => x.Total_WaterUsed ?? 0)), 2);
                    double leakagePercent = Math.Round(((watersent - waterUsed) / watersent) * 100, 2);
                    newsubs.Add(new Sub_RegionConsumption
                    {
                        Sub_RegionId = group.Key ?? 0,
                        Daily_Usage = Math.Round((group.Sum(x => x.Daily_Usage ?? 0) + dayambushKl + leakPipeKl), 2),
                        Night_Usage = Math.Round((group.Sum(x => x.Night_Usage ?? 0) + nightambushKl + leakPipeKl), 2),
                        Water_Payed = Math.Round(waterUsed, 2),
                        Leakage_Percent = leakagePercent,
                        Spike_Usage = group.Sum(x => x.Spike_Usage ?? 0),
                        Extepected_Amount = group.Sum(x => x.Extepected_Amount ?? 0),
                        Actual_Amount = group.Sum(x => x.Actual_Amount ?? 0),
                        OutStanding_Amount = group.Sum(x => x.OutStanding_Amount ?? 0),
                        FreeWater = group.Sum(x => x.FreeWater ?? 0),
                        OutstandingWater = Math.Round((watersent - waterUsed), 2),
                        Water_Sent = Math.Round(watersent, 2),
                        Date = DateTime.Now,
                        Sub_RegionName = await GetOwnersSubRegionName(group.Key.Value),
                        RegionId = await GetRegionId(group.Key.Value)
                    });
                }
                return newsubs;
            }

            List<Sub_RegionConsumption> subs = subResult.Data.ToList();

            foreach (var sub in subs)
            {
                if (sub.Date?.Month != DateTime.Now.Month || sub.Date?.Year != DateTime.Now.Year)
                {
                    sub.Daily_Usage = 0;
                    sub.Night_Usage = 0;
                    sub.Leakage_Percent = 0;
                    sub.Spike_Usage = 0;
                    sub.Extepected_Amount = 0;
                    sub.Actual_Amount = 0;
                    sub.OutStanding_Amount = 0;
                    sub.Water_Payed = 0;
                    sub.OutstandingWater = 0;
                    sub.FreeWater = 0;
                }

                double dayUmbush = 0.00;
                double nightUmbush = 0.00;
                if (_random.Next(0, 101) < 60)
                {
                    (dayUmbush, nightUmbush) = SimulateWaterAmbush(_random.Next(0, 25));
                }

                double leakagePipe = 0.00;
                if (_random.Next(0, 101) < 40)
                {
                    leakagePipe = SimulateLeakingPipe();
                }

                double leakPipeKl = Math.Round(leakagePipe / 1000, 2);
                double dayambushKl = Math.Round(dayUmbush / 1000, 2);
                double nightambushKl = Math.Round(nightUmbush / 1000, 2);


                var data = houses.Where(x => x.Sub_RegionId == sub.Sub_RegionId).ToList();

                double watersent = Math.Round((data.Sum(x => x.Daily_Usage ?? 0) + data.Sum(x => x.Night_Usage ?? 0) + dayambushKl + nightambushKl + leakPipeKl), 2);
                double waterused = Math.Round(data.Sum(x => x.Total_WaterUsed ?? 0), 2);
                double leakagePercent = Math.Round(((watersent - waterused) / watersent) * 100, 2);
                sub.Id = sub.Id;
                sub.Daily_Usage = Math.Round((data.Sum(x => x.Daily_Usage ?? 0) + dayambushKl + leakPipeKl), 2);
                sub.Night_Usage = Math.Round((data.Sum(x => x.Night_Usage ?? 0) + nightambushKl + leakPipeKl), 2);
                sub.Leakage_Percent = leakagePercent;
                sub.Spike_Usage = data.Sum(x => x.Spike_Usage ?? 0);
                sub.Extepected_Amount = Math.Round(data.Sum(x => x.Extepected_Amount ?? 0), 2);
                sub.Actual_Amount = Math.Round(data.Sum(x => x.Actual_Amount ?? 0), 2);
                sub.OutStanding_Amount = (sub.Extepected_Amount ?? 0) - (sub.Actual_Amount ?? 0);
                sub.FreeWater = data.Sum(x => x.FreeWater ?? 0);
                sub.Water_Payed = Math.Round(waterused, 2);
                sub.OutstandingWater = Math.Round((watersent - waterused), 2);
                sub.Date = DateTime.Now;
                sub.Water_Sent = Math.Round(watersent, 2);
            }

            return subs;
        }
        public async Task<List<RegionConsumption>> GetRegionConsumption()
        {

            var subResult = await _sub_RegionConsumptionDbService.GetAllSub_RegionConsumption();

            List<Sub_RegionConsumption> subs = subResult.Data
                .Where(x => x.Date?.Month == DateTime.Now.Month &&
                            x.Date?.Year == DateTime.Now.Year)
                .ToList();



            var regionResult = await _regionConsumptionDbService.GetAllRegionsConsumptions();

            if (!regionResult.Data.Any())
            {
                List<RegionConsumption> newregions = new();

                var groups = subs.GroupBy(x => x.RegionId);

                foreach (var group in groups)
                {
                    double dayUmbush = 0.00;
                    double nightUmbush = 0.00;
                    if (_random.Next(0, 101) < 60)
                    {
                        (dayUmbush, nightUmbush) = SimulateWaterAmbush(_random.Next(0, 25));
                    }

                    double leakagePipe = 0.00;
                    if (_random.Next(0, 101) < 40)
                    {
                        leakagePipe = SimulateLeakingPipe();
                    }

                    double leakPipeKl = Math.Round(leakagePipe / 1000, 2);
                    double dayambushKl = Math.Round(dayUmbush / 1000, 2);
                    double nightambushKl = Math.Round(nightUmbush / 1000, 2);
                    double leaks = leakPipeKl + dayambushKl + nightambushKl;
                    double watersent = Math.Round((group.Sum(x => x.Water_Sent.Value) + leaks), 2);
                    double waterUsed = Math.Round(group.Sum(x => x.Water_Payed ?? 0), 2);
                    double leakagePercent = Math.Round(((watersent - waterUsed) / watersent) * 100, 2);
                    newregions.Add(new RegionConsumption
                    {
                        Region_Id = group.Key,
                        Daily_Usage = Math.Round((group.Sum(x => x.Daily_Usage ?? 0) + dayambushKl + leakPipeKl), 2),
                        Night_Usage = Math.Round((group.Sum(x => x.Night_Usage ?? 0) + nightambushKl + leakPipeKl), 2),
                        Water_Payed = Math.Round(waterUsed, 2),
                        Leakage_Percent = leakagePercent,
                        Spike_Usage = group.Sum(x => x.Spike_Usage ?? 0),
                        Extepected_Amount = group.Sum(x => x.Extepected_Amount ?? 0),
                        Actual_Amount = group.Sum(x => x.Actual_Amount ?? 0),
                        OutStanding_Amount = Math.Round(group.Sum(x => x.OutStanding_Amount ?? 0), 2),
                        FreeWater = group.Sum(x => x.FreeWater ?? 0),
                        OutstandingWater = Math.Round((watersent - waterUsed), 2),
                        Water_Sent = Math.Round(watersent, 2),
                        Date = DateTime.Now,
                        Region_Name = await GetRegionName(group.Key)
                    });
                }
                return newregions;
            }

            List<RegionConsumption> regions = regionResult.Data.ToList();

            foreach (var region in regions)
            {
                if (region.Date?.Month != DateTime.Now.Month || region.Date?.Year != DateTime.Now.Year)
                {
                    region.Daily_Usage = 0;
                    region.Night_Usage = 0;
                    region.Leakage_Percent = 0;
                    region.Spike_Usage = 0;
                    region.Extepected_Amount = 0;
                    region.Actual_Amount = 0;
                    region.OutStanding_Amount = 0;
                    region.Water_Payed = 0;
                    region.OutstandingWater = 0;
                    region.FreeWater = 0;
                }

                double dayUmbush = 0.00;
                double nightUmbush = 0.00;
                if (_random.Next(0, 101) < 60)
                {
                    (dayUmbush, nightUmbush) = SimulateWaterAmbush(_random.Next(0, 25));
                }

                double leakagePipe = 0.00;
                if (_random.Next(0, 101) < 40)
                {
                    leakagePipe = SimulateLeakingPipe();
                }

                double leakPipeKl = Math.Round((leakagePipe / 1000), 2);
                double dayambushKl = Math.Round((dayUmbush / 1000), 2);
                double nightambushKl = Math.Round((nightUmbush / 1000), 2);
                double leaks = leakPipeKl + dayambushKl + nightambushKl;
                var data = subs.Where(x => x.RegionId == region.Region_Id).ToList();
                double waterused = Math.Round(data.Sum(x => x.Water_Payed ?? 0), 2);
                double watersent = Math.Round((data.Sum(x => x.Water_Sent.Value) + leaks), 2);
                region.Daily_Usage = Math.Round(data.Sum(x => x.Daily_Usage ?? 0), 2) + dayambushKl;
                region.Night_Usage = Math.Round(data.Sum(x => x.Night_Usage ?? 0), 2) + nightambushKl;
                region.Leakage_Percent = Math.Round(((watersent - waterused) / watersent) * 100, 2);
                region.Spike_Usage = data.Sum(x => x.Spike_Usage ?? 0);
                region.Extepected_Amount = data.Sum(x => x.Extepected_Amount ?? 0);
                region.Actual_Amount = data.Sum(x => x.Actual_Amount ?? 0);
                region.OutStanding_Amount = (region.Extepected_Amount ?? 0) - (region.Actual_Amount ?? 0);
                region.FreeWater = data.Sum(x => x.FreeWater ?? 0);
                region.Water_Payed = Math.Round(waterused, 2);
                region.OutstandingWater = Math.Round((watersent - waterused), 2);
                region.Date = DateTime.Now;
                region.Region_Name = await GetRegionName(region.Region_Id);
                region.Water_Sent = Math.Round(watersent, 2);
            }

            return regions;
        }

        public async Task<List<ApplianceConsumption>> SimulateApplianceConsumption(List<Appliance> appliances)
        {
            List<ApplianceConsumption> simulatedData = new();

            int currentHour = DateTime.Now.Hour;
            bool isNight = currentHour >= 22 || currentHour <= 4;

            foreach (var appliance in appliances)
            {
                double flowRate = SimulateWaterFlowRatePerMinute(appliance.Appliance_Name);

                var behaviour = SimulateHumanBehavior(appliance.Appliance_Name);

                string[] events =
                {
                    "Monday",
                    "Tuesday",
                    "Wednesday",
                    "Thursday",
                    "Friday",
                    "Weekend",
                    "Guests at Home",
                    "Water Restriction Level 2"
                };
                int eventToChoose = _random.Next(0, events.Length);
                int frequency = DetermineUsageFrequency(currentHour, appliance.NumberOfPeopleUsingIt, events[eventToChoose]);
                if (frequency <= 0)
                    frequency = 1;
                double Usage = flowRate * behaviour.DurationMinutes * frequency;
                double leakageSim = SimulateBackgroundLeaks(appliance.Appliance_Name, isNight);
                double dailyUsage;
                double nightUsage;
                Usage += leakageSim;
                if (isNight)
                {
                    dailyUsage = 0;
                    nightUsage = Usage;
                }

                else
                {
                    dailyUsage = Usage;
                    nightUsage = 0;
                }

                double usageKL = Usage / 1000.0;

                string propValue = await GetOwnersPropetyvalue(appliance.Owner_AccountNumber);

                double cost = await CalculateApplianceConsumptionCost(appliance.Owner_AccountNumber, propValue, usageKL);
                if (cost <= 0)
                    cost = 0.00;
                // 12. Leakage %
                double leakagePercent = 0.00;

                // 13. Status
                string status = "Normal";
                int aiResponse = _random.Next(0, 2);
                if(aiResponse == 0)
                    status = "Normal";
                else
                    status = "OverConsumption";

                simulatedData.Add(new ApplianceConsumption
                {
                    Appliance_Id = appliance.Appliance_Id,
                    Appliance_Name = appliance.Appliance_Name,

                    Daily_Usage = Math.Round(dailyUsage, 2),
                    Night_Usage = Math.Round(nightUsage, 2),

                    Cost = Math.Round(cost, 2),

                    Date = DateTime.Now,

                    Leakage_Percent = Math.Round(leakagePercent, 2),

                    Daily_Status = status,
                    Night_Status = status,


                    Account = appliance.Owner_AccountNumber
                });
            }

            return simulatedData;
        }
        private async Task<string> GetOwnersPropetyvalue(string acc)
        {
            var owners = await _ownerDbService.GetAllOwners();
            var owner = owners.Data.FirstOrDefault(x => x.Owner_AccountNumber == acc);
            return owner.PropertyValue;
        }
        private async Task<string> GetOwnersHousId(string acc)
        {
            var owners = await _ownerDbService.GetAllOwners();
            var owner = owners.Data.FirstOrDefault(x => x.Owner_AccountNumber == acc);
            return owner.Household_ID ?? string.Empty;
        }
        private async Task<int> GetOwnersSubRegionId(string acc)
        {
            var owners = await _ownerDbService.GetAllOwners();
            var owner = owners.Data.FirstOrDefault(x => x.Owner_AccountNumber == acc);
            return owner.Sub_RegionId.Value;
        }
        private async Task<string> GetOwnersSubRegionName(int id)
        {
            var owners = await _ownerDbService.GetAllOwners();
            var owner = owners.Data.FirstOrDefault(x => x.Sub_RegionId == id);
            return owner.Sub_RegionName;
        }
        private async Task<string> GetRegionName(int regionId)
        {
            var region = await _regiondbservice.GetRegion(regionId);
            return region.Data.Region_Name;
        }
        private async Task<int> GetRegionId(int subId)
        {
            var subs = await _sub_RegionDbService.GetAllSub_Regions();
            var sub = subs.Data.FirstOrDefault(x => x.Id == subId);
            return sub.RegionId.Value;
        }

        private double SimulateWaterFlowRatePerMinute(string applianceName)
        {
            double baseFlow = 8.0;
            string name = applianceName.ToLower().Trim();

            if (name.Contains("shower") || name.Contains("shwr"))
            {
                baseFlow = 9.5;
            }
            else if (name.Contains("kitchen") || name.Contains("kitchn"))
            {
                baseFlow = 8.0;
            }
            else if (name.Contains("tap") || name.Contains("sink") || name.Contains("basin") || name.Contains("tp"))
            {
                if (name.Contains("outside") || name.Contains("irrigation") || name.Contains("garden"))
                {
                    baseFlow = 20.0;
                }
                else
                {
                    baseFlow = 6.0;
                }
            }
            else if (name.Contains("toilet") || name.Contains("flush") || name.Contains("toile") || name.Contains("tl"))
            {
                baseFlow = 9.0;
            }
            else if (name.Contains("bathtub") || name.Contains("bath") || name.Contains("bth") || name.Contains("bove"))
            {
                baseFlow = 110.0;
            }
            else if (name.Contains("geyser") || name.Contains("geazer") || name.Contains("gyser"))
            {
                baseFlow = 0.5;
            }
            else if (name.Contains("washing") || name.Contains("wash") || name.Contains("machin"))
            {
                baseFlow = 70.0;
            }
            else if (name.Contains("dishwasher") || name.Contains("dish"))
            {
                baseFlow = 15.0;
            }

            double pressureElevationMultiplier = _random.NextDouble() * (1.35 - 0.65) + 0.65;
            double valveEfficiencyMultiplier = _random.NextDouble() * (1.10 - 0.90) + 0.90;

            return baseFlow * pressureElevationMultiplier * valveEfficiencyMultiplier;
        }

        private (double DurationMinutes, double PauseTimeMinutes) SimulateHumanBehavior(string applianceName)
        {
            double duration = 5.0;
            double pauseTime = 0.0;
            string name = applianceName.ToLower();

            if (name.Contains("washing") || name.Contains("dishwasher") || name.Contains("toilet"))
            {
                return (1.0, 0.0);
            }

            int moodRoute = _random.Next(0, 5);
            switch (moodRoute)
            {
                case 0:
                    duration = _random.Next(3, 6);
                    pauseTime = _random.NextDouble() * 1.0;
                    break;
                case 2:
                    duration = _random.Next(12, 22);
                    pauseTime = _random.Next(1, 3);
                    break;
                case 3:
                    duration = _random.Next(6, 12);
                    pauseTime = _random.Next(2, 5);
                    break;
                default:
                    duration = _random.Next(5, 10);
                    break;
            }


            return (duration, pauseTime);
        }

        private int DetermineUsageFrequency(int hour, int totalPeople, string currentEvent)
        {
            int baseTriggers = 0;
            if (hour >= 6 && hour <= 8) baseTriggers = totalPeople * _random.Next(1, 3);
            else if (hour >= 17 && hour <= 20) baseTriggers = totalPeople * _random.Next(1, 2);
            else if (hour >= 9 && hour <= 16) baseTriggers = _random.Next(0, 2);

            if (currentEvent == "Weekend" || currentEvent == "Guests at Home")
                baseTriggers = (int)(baseTriggers * 1.4) + 1;
            return baseTriggers;
        }

        private double SimulateBackgroundLeaks(string applianceName, bool isNight)
        {
            string name = applianceName.ToLower();

            if (name.Contains("toilet"))
            {
                if (_random.Next(100) < 15)
                    return 200.0 / 24.0;
            }

            if (name.Contains("tap") || name.Contains("sink"))
            {
                if (_random.Next(100) < 8)
                    return 25.0 / 24.0;
            }

            if (name.Contains("shower"))
            {
                if (_random.Next(100) < 5)
                    return 40.0 / 24.0;
            }

            return 0;
        }

        private double SimulateLeakingPipe()
        {
            double baseLeakRatePerMin = 7.0;

            double pressureElevationMultiplier = _random.NextDouble() * (1.35 - 0.65) + 0.65;

            return baseLeakRatePerMin * pressureElevationMultiplier * 60.0;
        }

        private (double DayVolume, double NightVolume) SimulateWaterAmbush(int hour)
        {
            bool isNight = hour >= 22 || hour <= 5;

            double litresUsed = 0;

            if (hour >= 5 && hour <= 8)
            {
                litresUsed = _random.Next(40, 90);
            }
            else if (hour >= 9 && hour <= 16)
            {
                litresUsed = _random.Next(5, 25);
            }
            else if (hour >= 17 && hour <= 21)
            {
                litresUsed = _random.Next(50, 120);
            }
            else
            {
                litresUsed = _random.Next(2, 15);
            }

            if (isNight)
                return (0, litresUsed);

            return (litresUsed, 0);
        }

        private async Task<double> CalculateExpectedAmount(double waterreceived, double freewater, string acc)
        {
            string propValue = await GetOwnersPropetyvalue(acc);
            return WaterConsumptionCalculation.CalculateCostOfWater((waterreceived - freewater), propValue);
        }
        private async Task<double> CalculateApplianceConsumptionCost(string acc, string prop, double klToCalc)
        {
            var response = await _applianceConsumptionDbService.GetAllApplianceConsumptions();
            var ownerAppliances = response.Data.Select(x => new ApplianceConsumption
            {
                Appliance_Id = x.Appliance_Id,
                Appliance_Name = x.Appliance_Name,
                Daily_Usage = x.Daily_Usage,
                Night_Usage = x.Night_Usage,
                Cost = x.Cost,
                Date = x.Date,
                Leakage_Percent = x.Leakage_Percent,
                Daily_Status = x.Daily_Status,
                Night_Status = x.Night_Status,
                Account = x.Account,
                Id = x.Id
            }).Where(x => x.Account == acc && x.Date.Value.Date.Month == DateTime.Now.Month).ToList();
            double daily = ownerAppliances.Sum(x => x.Daily_Usage.Value);
            double night = ownerAppliances.Sum(x => x.Night_Usage.Value);
            double totalKl = (daily + night) / 1000;
            if (totalKl > 6)
            {
                double oldCost = WaterConsumptionCalculation.CalculateCostOfWater(totalKl, prop);
                totalKl += klToCalc;
                double newCost = WaterConsumptionCalculation.CalculateCostOfWater(totalKl, prop);
                return (newCost - oldCost);
            }
            else
            {
                return 0.00;
            }
        }

        private async Task<List<ApplianceConsumption>> GetDbAppliances()
        {
            var response = await _applianceConsumptionDbService.GetAllApplianceConsumptions();
            var ownerAppliances = response.Data.Select(x => new ApplianceConsumption
            {
                Appliance_Id = x.Appliance_Id,
                Appliance_Name = x.Appliance_Name,
                Daily_Usage = x.Daily_Usage,
                Night_Usage = x.Night_Usage,
                Cost = x.Cost,
                Date = x.Date,
                Leakage_Percent = x.Leakage_Percent,
                Daily_Status = x.Daily_Status,
                Night_Status = x.Night_Status,
                Account = x.Account,
                Id = x.Id,
            }).Where(x => x.Date.Value.Date.Month == DateTime.Now.Month && x.Date.Value.Year == DateTime.Now.Year).ToList();
            return ownerAppliances;
        }
        private async Task<List<HouseConsumption>> GetDbHouseConsumption()
        {
            var response = await _houseConsumptionDbService.GetAllHouseConsumption();
            var ownerHouseConsumption = response.Data.Select(x => new HouseConsumption
            {
                Id = x.Id,
                Household_ID = x.Household_ID,
                Owner_AccountNumber = x.Owner_AccountNumber,
                Water_Received = x.Water_Received,
                Total_WaterUsed = x.Total_WaterUsed,
                Leakage_Percent = x.Leakage_Percent,
                Daily_Usage = x.Daily_Usage,
                Night_Usage = x.Night_Usage,
                Spike_Usage = x.Spike_Usage,
                Date = x.Date,
                Sub_RegionId = x.Sub_RegionId,
                Extepected_Amount = x.Extepected_Amount,
                Actual_Amount = x.Actual_Amount,
                OutStanding_Amount = x.OutStanding_Amount,
                FreeWater = x.FreeWater,
                Payment_Status = x.Payment_Status,
            }).Where(x => x.Date.Value.Date.Month == DateTime.Now.Month && x.Date.Value.Year == DateTime.Now.Year).ToList();
            return ownerHouseConsumption;
        }
        private async Task<List<Sub_RegionConsumption>> GetDbSub_RegionConsumption()
        {
            var list = await _sub_RegionConsumptionDbService.GetAllSub_RegionConsumption();
            var subs = list.Data.Select(x => new Sub_RegionConsumption
            {
                Id = x.Id,
                Sub_RegionId = x.Sub_RegionId,
                Water_Payed = x.Water_Payed,
                Leakage_Percent = x.Leakage_Percent,
                Daily_Usage = x.Daily_Usage,
                Night_Usage = x.Night_Usage,
                Spike_Usage = x.Spike_Usage,
                Date = x.Date,
                Extepected_Amount = x.Extepected_Amount,
                Actual_Amount = x.Actual_Amount,
                OutStanding_Amount = x.OutStanding_Amount,
                FreeWater = x.FreeWater,
                Sub_RegionName = x.Sub_RegionName,
                OutstandingWater = x.OutstandingWater,
                RegionId = x.RegionId,
                Water_Sent = x.Water_Sent,
            }).Where(x => x.Date.Value.Date.Month == DateTime.Now.Month && x.Date.Value.Year == DateTime.Now.Year).ToList();
            return subs;
        }
        private async Task<List<RegionConsumption>> GetDbRegionConsumption()
        {
            var list = await _regionConsumptionDbService.GetAllRegionsConsumptions();
            var regions = list.Data.Select(x => new RegionConsumption
            {
                Id = x.Id,
                Region_Id = x.Region_Id,
                Water_Payed = x.Water_Payed,
                Leakage_Percent = x.Leakage_Percent,
                Daily_Usage = x.Daily_Usage,
                Night_Usage = x.Night_Usage,
                Spike_Usage = x.Spike_Usage,
                Date = x.Date,
                Extepected_Amount = x.Extepected_Amount,
                Actual_Amount = x.Actual_Amount,
                OutStanding_Amount = x.OutStanding_Amount,
                FreeWater = x.FreeWater,
                Region_Name = x.Region_Name,
                OutstandingWater = x.OutstandingWater,
                Water_Sent = x.Water_Sent,
            }).Where(x => x.Date.Value.Date.Month == DateTime.Now.Month && x.Date.Value.Year == DateTime.Now.Year).ToList();
            return regions;
        }
    }
}
