using Microsoft.AspNetCore.SignalR;
using Org.BouncyCastle.Asn1.X509;
using Smart_Water_Consumption_Service.Models;
using Smart_Water_Consumption_Service.Services.AiService.MachineLearningModel;
using Smart_Water_Consumption_Service.Services.DbServices;
using Smart_Water_Consumption_Service.Services.SignalRService;
using Smart_Water_Consumption_Service.Services.TarrifService;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Net.NetworkInformation;
using System.Runtime.Intrinsics.X86;
using System.Text;
using System.Xml.Linq;
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
        private ModelPredictionsConnector _modelPredictionsConnector;
        private AppliancePredictionDbService _appliancePredictionDbService;
        private HouseholdPredictionDbService _householdPredictionDbService;
        private Sub_RegionPredictionDbService _sub_RegionPredictionDbService;
        private RegionPredictionDbService _regionPredictionDbService;
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
            _modelPredictionsConnector = new ModelPredictionsConnector(apiBaseUrl);
            _appliancePredictionDbService = new AppliancePredictionDbService(apiBaseUrl);
            _householdPredictionDbService = new HouseholdPredictionDbService(apiBaseUrl);
            _sub_RegionPredictionDbService = new Sub_RegionPredictionDbService(apiBaseUrl);
            _regionPredictionDbService = new RegionPredictionDbService(apiBaseUrl);
        }
        Random _random = new Random();
        public async Task RunSimulation()
        {


            await AddAplianceConsumptionsToDb(await GetApplianceConsumptions());

            var houses = await GetHouseConsumptions();
            await UpdateHouseConsumptionsToDb(houses.housesConsumption, houses.housesPredictions);

            var subs = await GetSubRegionConsumption();
            await UpdateSubRegionConsumptionsToDb(subs.subConsumptions, subs.subPredictions);

            var regions = await GetRegionConsumption();
            await UpdateRegionConsumptionsToDb(regions.regionCunsumptions, regions.regionPredictions);

            try
            {
                RealtimeDashboard realtimeDashboard = new RealtimeDashboard();
                
                
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
        private async Task<string> UpdateHouseConsumptionsToDb(List<HouseConsumption> houseConsumptions, List<HouseholdPrediction> householdPredictions)
        {
            var result = await _houseConsumptionDbService.UpdateHouseConsumption(houseConsumptions);
            foreach (var item in householdPredictions)
            {
                await _householdPredictionDbService.AddHouseholdPrediction(item);
            }
            
            if (result.IsSuccess)
                return "ok";
            return result.Message;
        }
        private async Task<string> UpdateSubRegionConsumptionsToDb(List<Sub_RegionConsumption> subRegionConsumptions, List<SubRegionPrediction> subRegionPredictions)
        {
            var result = await _sub_RegionConsumptionDbService.UpdateSub_RegionConsumption(subRegionConsumptions);
            foreach (var item in subRegionPredictions)
            {
                var response = await _sub_RegionPredictionDbService.AddSub_RegionPrediction(item);
            }
            if (result.IsSuccess)
                return "ok";
            return result.Message;
        }
        private async Task<string> UpdateRegionConsumptionsToDb(List<RegionConsumption> RegionConsumptions, List<RegionPrediction> regionPredictions)
        {
            var result = await _regionConsumptionDbService.PutRegionConsumption(RegionConsumptions);
            foreach (var item in regionPredictions)
            {
                await _regionPredictionDbService.AddRegionPrediction(item);
            }
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
        public async Task<(List<HouseConsumption> housesConsumption, List<HouseholdPrediction> housesPredictions)> GetHouseConsumptions()
        {

            var applianceResult = await _applianceConsumptionDbService.GetAllApplianceConsumptions();
            var appliancePredictionsResult = await _appliancePredictionDbService.GetAllAppliancePredictions();
            var apps = await _applianceDbService.GetAllAppliance();
            List<ApplianceConsumption> applianceConsumptions = applianceResult.Data
                .Where(x => x.Date.Value.Month == DateTime.Now.Month && x.Date.Value.Year == DateTime.Now.Year)
                .ToList();
            var owners = await _ownerDbService.GetAllOwners();
            List<HouseholdPrediction> householdPredictions = new List<HouseholdPrediction>();
            foreach (var owner in owners.Data)
            {
                int count = apps.Data.Where(x => x.Owner_AccountNumber == owner.Owner_AccountNumber).Count();
                List<AppliancePrediction> appliancePredictions = appliancePredictionsResult.Data.Where(x => x.Owner_AccountNumber == owner.Owner_AccountNumber).OrderByDescending(x => x.MonthYear).Take(count).ToList();
                string acc = owner.Owner_AccountNumber;
                string AccountNumber = acc;
                double EstimatedHourCost = Math.Round(appliancePredictions.Sum(x => x.EstimatedHourCost), 2);
                double EstimatedMonthCost = Math.Round(appliancePredictions.Sum(x => x.EstimatedMonthCost), 2);
                double EstimatedWeekCost = Math.Round(appliancePredictions.Sum(x => x.EstimatedWeekCost), 2);
                double PredictedHourConsumptionKl = Math.Round(appliancePredictions.Sum(x => x.PredictedHourConsumptionKl), 2);
                double PredictedMonthConsumptionKl = Math.Round(appliancePredictions.Sum(x => x.PredictedMonthConsumptionKl), 2);
                double PredictedWeekConsumptionKl = Math.Round(appliancePredictions.Sum(x => x.PredictedWeekConsumptionKl), 2);
                var MonthYear = appliancePredictions.Select(x => x.MonthYear).FirstOrDefault();
                int SubRegionId = await GetOwnersSubRegionId(acc);
                HouseholdPrediction householdPrediction = new HouseholdPrediction
                {
                    AccountNumber = acc,
                    EstimatedHourCost = Math.Round(appliancePredictions.Sum(x => x.EstimatedHourCost), 2),
                    EstimatedMonthCost = Math.Round(appliancePredictions.Sum(x => x.EstimatedMonthCost), 2),
                    EstimatedWeekCost = Math.Round(appliancePredictions.Sum(x => x.EstimatedWeekCost), 2),
                    PredictedHourConsumptionKl = Math.Round(appliancePredictions.Sum(x => x.PredictedHourConsumptionKl), 2),
                    PredictedMonthConsumptionKl = Math.Round(appliancePredictions.Sum(x => x.PredictedMonthConsumptionKl), 2),
                    PredictedWeekConsumptionKl = Math.Round(appliancePredictions.Sum(x => x.PredictedWeekConsumptionKl), 2),
                    MonthYear = appliancePredictions.Select(x => x.MonthYear).FirstOrDefault(),
                    SubRegionId = await GetOwnersSubRegionId(acc)
                };
                householdPredictions.Add(householdPrediction);
            }



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
                    if (cost < 0)
                        cost = 0;
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
                return (newhouses, householdPredictions);
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
                if (cost < 0)
                    cost = 0;
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

            return (houses, householdPredictions);
        }
        public async Task<(List<Sub_RegionConsumption> subConsumptions, List<SubRegionPrediction> subPredictions)> GetSubRegionConsumption()
        {

            var houseResult = await _houseConsumptionDbService.GetAllHouseConsumption();
            var housePredictionsResult = await _householdPredictionDbService.GetAllHouseholdPredictions();

            List<HouseConsumption> houses = houseResult.Data
                .Where(x => x.Date?.Month == DateTime.Now.Month &&
                            x.Date?.Year == DateTime.Now.Year)
                .ToList();
            var apps = await _ownerDbService.GetAllOwners();
            int count = apps.Data.Count();
            List<HouseholdPrediction> householdPredictions = housePredictionsResult.Data.Select(x => new HouseholdPrediction
            {
                AccountNumber = x.AccountNumber,
                EstimatedHourCost = x.EstimatedHourCost,
                MonthYear = x.MonthYear,
                EstimatedMonthCost = x.EstimatedMonthCost,
                EstimatedWeekCost = x.EstimatedWeekCost,
                PredictedHourConsumptionKl = x.PredictedHourConsumptionKl,
                PredictedMonthConsumptionKl = x.PredictedMonthConsumptionKl,
                PredictedWeekConsumptionKl = x.PredictedWeekConsumptionKl,
                SubRegionId = x.SubRegionId,
            }).OrderByDescending(x => x.MonthYear).Take(count).ToList();

            var houseGroup = householdPredictions.GroupBy(x => x.SubRegionId);
            List<SubRegionPrediction> subRegionPredictions = new List<SubRegionPrediction>();
            foreach (var grp in houseGroup)
            {
                int subId = grp.Key;
                SubRegionPrediction subRegionPrediction = new SubRegionPrediction
                {
                    SubRegionName = await GetOwnersSubRegionName(subId),
                    EstimatedHourCost = Math.Round(grp.Sum(x => x.EstimatedHourCost), 2),
                    EstimatedMonthCost = Math.Round(grp.Sum(x => x.EstimatedMonthCost), 2),
                    EstimatedWeekCost = Math.Round(grp.Sum(x => x.EstimatedWeekCost), 2),
                    PredictedHourConsumptionKl = Math.Round(grp.Sum(x => x.PredictedHourConsumptionKl),2 ),
                    PredictedMonthConsumptionKl = Math.Round(grp.Sum(x => x.PredictedMonthConsumptionKl),2),
                    PredictedWeekConsumptionKl = Math.Round(grp.Sum(x => x.PredictedWeekConsumptionKl), 2),
                    MonthYear = grp.Select(x => x.MonthYear).FirstOrDefault(),
                    SubRegionId = subId,
                    RegionId = await GetRegionId(subId),
                };
                subRegionPredictions.Add(subRegionPrediction);
            }
            var subResult = await _sub_RegionConsumptionDbService.GetAllSub_RegionConsumption();

            if (!subResult.Data.Any())
            {
                List<Sub_RegionConsumption> newsubs = new();

                var groups = houses.GroupBy(x => x.Sub_RegionId);

                foreach (var group in groups)
                {
                    double dayUmbush = 0.00;
                    double nightUmbush = 0.00;
                    if (_random.Next(0, 101) < 10)
                    {
                        (dayUmbush, nightUmbush) = SimulateWaterAmbush(_random.Next(0, 25));
                    }

                    double leakagePipe = 0.00;
                    if (_random.Next(0, 101) < 10)
                    {
                        leakagePipe = SimulateLeakingPipe();
                    }

                    double leakPipeKl = Math.Round(leakagePipe / 1000, 2);
                    double dayambushKl = Math.Round(dayUmbush / 1000, 2);
                    double nightambushKl = Math.Round(nightUmbush / 1000, 2);
                    double ambushCost = (leakPipeKl + dayambushKl + nightambushKl) * 18;
                    if (ambushCost < 0)
                        ambushCost = 0;

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
                        Extepected_Amount = group.Sum(x => (x.Extepected_Amount ?? 0) + ambushCost),
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
                return (newsubs, subRegionPredictions);
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
                if (_random.Next(0, 101) < 30)
                {
                    (dayUmbush, nightUmbush) = SimulateWaterAmbush(_random.Next(0, 25));
                }

                double leakagePipe = 0.00;
                if (_random.Next(0, 101) < 10)
                {
                    leakagePipe = SimulateLeakingPipe();
                }

                double leakPipeKl = Math.Round(leakagePipe / 1000, 2);
                double dayambushKl = Math.Round(dayUmbush / 1000, 2);
                double nightambushKl = Math.Round(nightUmbush / 1000, 2);
                double ambushCost = (leakPipeKl + dayambushKl + nightambushKl) * 18;
                if (ambushCost < 0)
                    ambushCost = 0;

                var data = houses.Where(x => x.Sub_RegionId == sub.Sub_RegionId).ToList();
                
                double watersent = Math.Round((data.Sum(x => x.Daily_Usage ?? 0) + data.Sum(x => x.Night_Usage ?? 0) + dayambushKl + nightambushKl + leakPipeKl), 2);
                double waterused = Math.Round(data.Sum(x => x.Total_WaterUsed ?? 0), 2);
                double freewater = data.Sum(x => x.FreeWater.Value);
                double cost = Math.Round(data.Sum(x => x.Actual_Amount ?? 0), 2);
                double loss = (cost + ambushCost ) - cost;
                if (loss < 0)
                    loss = 0.00;
                    

                double leakagePercent = Math.Round(((watersent - waterused) / watersent) * 100, 2);
                sub.Id = sub.Id;
                sub.Daily_Usage = Math.Round((data.Sum(x => x.Daily_Usage ?? 0) + dayambushKl + leakPipeKl), 2);
                sub.Night_Usage = Math.Round((data.Sum(x => x.Night_Usage ?? 0) + nightambushKl + leakPipeKl), 2);
                sub.Leakage_Percent = leakagePercent;
                sub.Spike_Usage = data.Sum(x => x.Spike_Usage ?? 0);
                sub.Extepected_Amount = Math.Round(data.Sum(x => (x.Extepected_Amount ?? 0 + ambushCost )), 2);
                sub.Actual_Amount = cost;
                sub.OutStanding_Amount = Math.Round(loss);
                sub.FreeWater = freewater;
                sub.Water_Payed = Math.Round(waterused, 2);
                sub.OutstandingWater = Math.Round((watersent - waterused), 2);
                sub.Date = DateTime.Now;
                sub.Water_Sent = Math.Round(watersent, 2);
            }

            return (subs, subRegionPredictions);
        }
        public async Task<(List<RegionConsumption> regionCunsumptions, List<RegionPrediction> regionPredictions)> GetRegionConsumption()
        {

            var subResult = await _sub_RegionConsumptionDbService.GetAllSub_RegionConsumption();
            var subPredictionsResults = await _sub_RegionPredictionDbService.GetAllSub_RegionPredictions();

            List<Sub_RegionConsumption> subs = subResult.Data
                .Where(x => x.Date?.Month == DateTime.Now.Month &&
                            x.Date?.Year == DateTime.Now.Year)
                .ToList();

            var apps = await _sub_RegionDbService.GetAllSub_Regions();
            int count = apps.Data.Count();
            List<SubRegionPrediction> subregionPredictions = subPredictionsResults.Data.Select(x => new SubRegionPrediction
            {
                RegionId = x.RegionId,
                EstimatedHourCost = x.EstimatedHourCost,
                MonthYear = x.MonthYear,
                EstimatedMonthCost = x.EstimatedMonthCost,
                EstimatedWeekCost = x.EstimatedWeekCost,
                PredictedHourConsumptionKl = x.PredictedHourConsumptionKl,
                PredictedMonthConsumptionKl = x.PredictedMonthConsumptionKl,
                PredictedWeekConsumptionKl = x.PredictedWeekConsumptionKl,
                SubRegionId = x.SubRegionId,
                SubRegionName = x.SubRegionName,
            }).OrderByDescending(x => x.MonthYear).Take(count).ToList();

            var subgrops = subregionPredictions.GroupBy(x => x.RegionId);

            List<RegionPrediction> regionPredictions = new List<RegionPrediction>();
            foreach (var grp in subgrops)
            {
                int regionId = grp.Key;
                RegionPrediction regionPrediction = new RegionPrediction
                {
                    EstimatedHourCost = Math.Round(grp.Sum(x => x.EstimatedHourCost), 2),
                    EstimatedMonthCost = Math.Round(grp.Sum(x => x.EstimatedMonthCost), 2),
                    EstimatedWeekCost = Math.Round(grp.Sum(x => x.EstimatedWeekCost), 2),
                    PredictedHourConsumptionKl = Math.Round(grp.Sum(x => x.PredictedHourConsumptionKl),2 ),
                    PredictedMonthConsumptionKl = Math.Round(grp.Sum(x => x.PredictedMonthConsumptionKl), 2),
                    PredictedWeekConsumptionKl = Math.Round(grp.Sum(x => x.PredictedWeekConsumptionKl), 2),
                    MonthYear = grp.Select(x => x.MonthYear).FirstOrDefault(),
                    RegionId = regionId,
                    RegionName = await GetRegionName(regionId),
                };
                regionPredictions.Add(regionPrediction);
            }

            var regionResult = await _regionConsumptionDbService.GetAllRegionsConsumptions();

            if (!regionResult.Data.Any())
            {
                List<RegionConsumption> newregions = new();

                var groups = subs.GroupBy(x => x.RegionId);

                foreach (var group in groups)
                {
                    double dayUmbush = 0.00;
                    double nightUmbush = 0.00;
                    if (_random.Next(0, 101) < 30)
                    {
                        (dayUmbush, nightUmbush) = SimulateWaterAmbush(_random.Next(0, 25));
                    }

                    double leakagePipe = 0.00;
                    if (_random.Next(0, 101) < 10)
                    {
                        leakagePipe = SimulateLeakingPipe();
                    }

                    double leakPipeKl = Math.Round(leakagePipe / 1000, 2);
                    double dayambushKl = Math.Round(dayUmbush / 1000, 2);
                    double nightambushKl = Math.Round(nightUmbush / 1000, 2);
                    double leaks = leakPipeKl + dayambushKl + nightambushKl;
                    double ambushCost = leaks * 18;
                    if (ambushCost < 0)
                        ambushCost = 0;
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
                        Extepected_Amount = group.Sum(x => (x.Extepected_Amount ?? 0) + ambushCost),
                        Actual_Amount = group.Sum(x => x.Actual_Amount ?? 0),
                        OutStanding_Amount = Math.Round(group.Sum(x => x.OutStanding_Amount ?? 0), 2),
                        FreeWater = group.Sum(x => x.FreeWater ?? 0),
                        OutstandingWater = Math.Round((watersent - waterUsed), 2),
                        Water_Sent = Math.Round(watersent, 2),
                        Date = DateTime.Now,
                        Region_Name = await GetRegionName(group.Key)
                    });
                }
                return (newregions, regionPredictions);
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
                if (_random.Next(0, 101) < 30)
                {
                    (dayUmbush, nightUmbush) = SimulateWaterAmbush(_random.Next(0, 25));
                }

                double leakagePipe = 0.00;
                if (_random.Next(0, 101) < 10)
                {
                    leakagePipe = SimulateLeakingPipe();
                }

                double leakPipeKl = Math.Round((leakagePipe / 1000), 2);
                double dayambushKl = Math.Round((dayUmbush / 1000), 2);
                double nightambushKl = Math.Round((nightUmbush / 1000), 2);
                double leaks = leakPipeKl + dayambushKl + nightambushKl;
                double ambushCost = leaks * 18;
                if (ambushCost < 0)
                    ambushCost = 0;
                var data = subs.Where(x => x.RegionId == region.Region_Id).ToList();
                double waterused = Math.Round(data.Sum(x => x.Water_Payed ?? 0), 2);
                double watersent = Math.Round((data.Sum(x => x.Water_Sent.Value) + leaks), 2);
                double freewater = data.Sum(x => x.FreeWater.Value);
                double cost = Math.Round(data.Sum(x => x.Actual_Amount ?? 0), 2);
                double Actualcost = await CalculateExpectedAmount(watersent, freewater, "0000000001");
                double loss = Actualcost - cost;
                if (loss <= 0)
                    loss = 0.00;
                region.Daily_Usage = Math.Round(data.Sum(x => x.Daily_Usage ?? 0), 2) + dayambushKl;
                region.Night_Usage = Math.Round(data.Sum(x => x.Night_Usage ?? 0), 2) + nightambushKl;
                region.Leakage_Percent = Math.Round(((watersent - waterused) / watersent) * 100, 2);
                region.Spike_Usage = data.Sum(x => x.Spike_Usage ?? 0);
                region.Extepected_Amount = data.Sum(x => (x.Extepected_Amount ?? 0) + ambushCost);
                region.Actual_Amount = cost;
                region.OutStanding_Amount = loss;
                region.FreeWater = freewater;
                region.Water_Payed = Math.Round(waterused, 2);
                region.OutstandingWater = Math.Round((watersent - waterused), 2);
                region.Date = DateTime.Now;
                region.Region_Name = await GetRegionName(region.Region_Id);
                region.Water_Sent = Math.Round(watersent, 2);
            }

            return (regions, regionPredictions);
        }

        public async Task<List<ApplianceConsumption>> SimulateApplianceConsumption(List<Appliance> appliances)
        {
            List<ApplianceConsumption> simulatedData = new();
            List<AppliancePrediction> appliancePredictions = new List<AppliancePrediction>();

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

                double leakageSim = 0;

                if (_random.Next(0,101) < 31)
                    leakageSim = SimulateBackgroundLeaks(appliance.Appliance_Name, isNight);
                double dailyUsage = leakageSim;
                double nightUsage = leakageSim;
                if (isNight)
                {
                    dailyUsage += 0;
                    nightUsage += Usage;
                }

                else
                {
                    dailyUsage += Usage;
                    nightUsage += 0;
                }

                double usageKL = Usage / 1000.0;

                string propValue = await GetOwnersPropetyvalue(appliance.Owner_AccountNumber);

                double cost = await CalculateApplianceConsumptionCost(appliance.Owner_AccountNumber, propValue, usageKL);
                if (cost <= 0)
                    cost = 0.00;

                double leakagePercent = 0;

                if (Usage > 0)
                {
                    double nightRatio = nightUsage / Usage;

                    if (nightRatio >= 0.60)
                        leakagePercent += 35;

                    double difference = Math.Abs(dailyUsage - nightUsage);

                    if (difference <= Usage * 0.10)
                        leakagePercent += 30;

                    if (behaviour.DurationMinutes > 120)
                        leakagePercent += 20;

                }

                leakagePercent = Math.Min(leakagePercent, 100);

                int holiday;

                if(_random.Next(0, 101) < 30)
                    holiday = 1;
                else
                    holiday = 0;

                if(eventToChoose > 7)
                    eventToChoose = 7;

                int timeofday;
                // Morning = 0, Afternoon = 1, Evening = 2, Night = 3

                if (currentHour >= 4 && currentHour < 12)
                    timeofday = 0;
                else if (currentHour >= 12 && currentHour < 17)
                    timeofday = 1;
                else if (currentHour >= 17 && currentHour < 20)
                    timeofday = 2;
                else
                    timeofday = 3;

                var applianceTodayUsage = await ReturnDailyAndNightUsgae(appliance.Appliance_Id);
                int overused = await HowManyOversudeItHad(appliance.Appliance_Id);
                var applianceAvg = await ReturnApplianceAveragesUsgae(appliance.Appliance_Id);

                int numberOfLeaks = 0;
                if (appliance.Appliance_Name.Contains("toilet"))
                    numberOfLeaks = _random.Next(0, 8);
                else if (appliance.Appliance_Name.Contains("sink") || appliance.Appliance_Name.Contains("tap"))
                    numberOfLeaks = _random.Next(0, 4);
                

                ConsumptionStatusRequest request = new ConsumptionStatusRequest
                {
                    ApplianceId = appliance.Appliance_Id,
                    Duration = behaviour.DurationMinutes,
                    FlowRate = flowRate,
                    WaterUsed = Usage,
                    DayUsage = applianceTodayUsage.Day,
                    NightUsage = applianceTodayUsage.Night,
                    Holiday = holiday,
                    PreviousLeak = numberOfLeaks,
                    Last7Average = applianceAvg.Last7days,
                    Last30Average = applianceAvg.Last30days,
                    NumberOfHighUsages = overused,
                    DayOfWeek = eventToChoose,
                    TimeOfTheDay = timeofday,
                    NumberOfPeople = appliance.NumberOfPeopleUsingIt
                };
                var lastUsgaes = await LastUsgages(appliance.Appliance_Id);
                int month = DateTime.Today.Month;

                int weekOfYear = CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(
                    DateTime.Today,
                    CalendarWeekRule.FirstFourDayWeek,
                    DayOfWeek.Monday);
                FuturePredictionRequest futurePredictionRequest = new FuturePredictionRequest
                {
                    ApplianceId = appliance.Appliance_Id,
                    YesterdayUsage = lastUsgaes.yesterday,
                    FlowRate = flowRate,
                    WeekOfYear = weekOfYear,
                    Month = month,
                    HourOfDay = currentHour,
                    Holiday = holiday,
                    PreviousHourUsage = lastUsgaes.lastHour,
                    Last7Average = applianceAvg.Last7days,
                    Last30Average = applianceAvg.Last30days,
                    CurrentWeekUsage = lastUsgaes.currentWeek,
                    DayOfWeek = eventToChoose,
                    CurrentMonthUsage = lastUsgaes.currentMonth,
                    NumberOfPeople = appliance.NumberOfPeopleUsingIt
                };

                int mlResponse = await _modelPredictionsConnector.PredictStatus(request);
                var futurePredictions = await GetFuturePredictions(futurePredictionRequest);
                double estimatedHourCost = await CalculateApplianceConsumptionCost(appliance.Owner_AccountNumber, propValue, (futurePredictions.nextHour / 1000));
                double estimatedWeekCost = await CalculateApplianceConsumptionCost(appliance.Owner_AccountNumber, propValue, (futurePredictions.nextWeek / 1000));
                double estimatedMonthCost = await CalculateApplianceConsumptionCost(appliance.Owner_AccountNumber, propValue, (futurePredictions.nextMonth / 1000));
                if (estimatedMonthCost < 0)
                    estimatedMonthCost = 0;
                AppliancePrediction appliancePrediction =  await AddFuturePrediction(appliance.Owner_AccountNumber, appliance.Appliance_Name, futurePredictions.nextHour, futurePredictions.nextWeek, futurePredictions.nextMonth,estimatedHourCost, estimatedWeekCost, estimatedMonthCost);
                appliancePredictions.Add(appliancePrediction);


                string status;
                
                if(mlResponse == 0)
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

            var appliancePredictionsToAdd = await CalculatePredictedApplianceConsumptionCost(appliancePredictions);
            foreach (var item in appliancePredictionsToAdd)
            {
                await _appliancePredictionDbService.AddAppliancePrediction(item);
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
                    return 200.0;
            }

            if (name.Contains("tap") || name.Contains("sink"))
            {
                if (_random.Next(100) < 8)
                    return 25.0;
            }

            if (name.Contains("shower"))
            {
                if (_random.Next(100) < 5)
                    return 40.0;
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
            }).Where(x => x.Account == acc && x.Date.Value.Date.Month == DateTime.Now.Month ).ToList();
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

        private async Task<List<AppliancePrediction>> CalculatePredictedApplianceConsumptionCost(List<AppliancePrediction> appliancePredictions)
        {
            var group = appliancePredictions.GroupBy(x => x.Owner_AccountNumber);
            foreach (var grp in group)
            {
                string acc = grp.Key;
                string prop = await GetOwnersPropetyvalue(acc);
                var ownerAppliances = appliancePredictions.Where(x => x.Owner_AccountNumber == acc);
                double totalKl = ownerAppliances.Sum(x => x.PredictedMonthConsumptionKl) / 1000;
                double cost = WaterConsumptionCalculation.CalculateCostOfWater(totalKl, prop);
                foreach (var item in appliancePredictions)
                {
                    if (totalKl > 6)
                    {
                        double contributedPercent = ((item.PredictedMonthConsumptionKl / 1000) / totalKl) * 100;
                        item.EstimatedMonthCost = Math.Round((cost * (contributedPercent / 100.0)), 2);
                    }
                }
            }

            return appliancePredictions;
        }

        private async Task<(double Day, double Night)> ReturnDailyAndNightUsgae(int applianceid)
        {
            var app = await GetDbAppliances();
            var list = app.Where(x => x.Date.Value.Date == DateTime.Today && x.Appliance_Id == applianceid).ToList();
            return (list.Sum(x => x.Daily_Usage.Value), list.Sum(x => x.Night_Usage.Value));
        }
        private async Task<(double Last7days, double Last30days)> ReturnApplianceAveragesUsgae(int applianceid)
        {
            var app = await GetDbAppliances();
            var list = app.Where(x => x.Appliance_Id == applianceid).ToList();
            double last7days = list.Where(x => x.Date.Value.Date >= DateTime.Today.AddDays(-7)).Sum(x => x.Daily_Usage.Value + x.Night_Usage.Value);
            double last30days = list.Where(x => x.Date.Value.Date >= DateTime.Today.AddDays(-30)).Sum(x => x.Daily_Usage.Value + x.Night_Usage.Value);
            return (last7days, last30days);
        }
        private async Task<int> HowManyOversudeItHad(int applianceid)
        {
            var app = await GetDbAppliances();
            var list = app.Where(x => x.Appliance_Id == applianceid).ToList();
            int leakageCount = list.Count(x => x.Night_Status != "Normal" || x.Daily_Status != "Normal");
            return leakageCount;
        }
        private async Task<(double nextHour, double nextWeek, double nextMonth)> GetFuturePredictions(FuturePredictionRequest futurePredictionRequest)
        {
            double nextHour = await _modelPredictionsConnector.PredictNextHourUsage(futurePredictionRequest);
            double nextWeek = await _modelPredictionsConnector.PredictNextWeekUsage(futurePredictionRequest);
            double nextMonth = await _modelPredictionsConnector.PredictNextMonthUsage(futurePredictionRequest);
            return (nextHour, nextWeek, nextMonth);
        }
        private async Task<(double lastHour, double yesterday, double currentWeek, double currentMonth)> LastUsgages(int applianceid)
        {
            var app = await GetDbAppliances();
            var list = app.Where(x => x.Appliance_Id == applianceid).ToList();
            double lasthour = list.Where(x => x.Date.Value.Date >= DateTime.Today.AddHours(-1)).Sum(x => x.Daily_Usage.Value + x.Night_Usage.Value);
            double yesterday = list.Where(x => x.Date.Value.Date >= DateTime.Today.AddDays(-1)).Sum(x => x.Daily_Usage.Value + x.Night_Usage.Value);
            DateTime today = DateTime.Today;

            int daysSinceMonday = ((int)today.DayOfWeek + 6) % 7;

            DateTime firstDayOfWeek = today.AddDays(-daysSinceMonday);

            double currentWeekUsage = list
                .Where(x => x.Date.HasValue &&
                            x.Date.Value.Date >= firstDayOfWeek)
                .Sum(x => x.Daily_Usage.Value + x.Night_Usage.Value);

            DateTime firstDayOfMonth = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);

            double currentMonthUsage = list
                .Where(x => x.Date.HasValue &&
                            x.Date.Value.Date >= firstDayOfMonth)
                .Sum(x => x.Daily_Usage.Value + x.Night_Usage.Value);
            return (lasthour, yesterday, currentWeekUsage, currentMonthUsage);
        }
        private async Task<AppliancePrediction> AddFuturePrediction(string acc, string appName, double hour, double week, double month, double hourcost, double weekCost, double monthcost)
        {
            AppliancePrediction appliancePrediction = new AppliancePrediction()
            {
                Appliance_Name = appName,
                EstimatedHourCost = Math.Round( hourcost, 2),
                EstimatedWeekCost = Math.Round(weekCost, 2),
                EstimatedMonthCost = Math.Round(monthcost, 2),
                PredictedHourConsumptionKl = Math.Round( hour, 2),
                PredictedMonthConsumptionKl = Math.Round( month, 2),
                PredictedWeekConsumptionKl = Math.Round( week, 2),
                Owner_AccountNumber = acc,
                MonthYear = DateTime.UtcNow
            };
            return appliancePrediction;
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
