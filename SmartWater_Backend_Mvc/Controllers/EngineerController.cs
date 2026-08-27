using Microsoft.AspNetCore.Mvc;
using Smart_Water_Consumption_Service.Services.AiService.DeepLearningModel;
using Smart_Water_Consumption_Service.Services.DbServices;
using SmartWater_Backend_Mvc.Models;

namespace SmartWater_Backend_Mvc.Controllers
{
    public class EngineerController : Controller
    {
        private readonly RegionConsumptionDbService _regionConsumptionDbService;
        private readonly Sub_RegionConsumptionDbService _subregionConsumptionDbService;
        private readonly HouseConsumptionDbService _houseConsumptionDbService;
        private readonly HouseholdPredictionDbService _householdPrediction;
        private readonly Sub_RegionPredictionDbService _subregionPrediction;
        private readonly RegionPredictionDbService _regionPrediction;
        private readonly RegionDbService _regionDbService;
        private readonly Sub_RegionDbService _subregionDbService;
        private readonly OwnerDbService _ownerDbService;
        public EngineerController(RegionConsumptionDbService regionConsumptionDbService1, Sub_RegionConsumptionDbService sub_RegionConsumptionDbService, HouseConsumptionDbService houseConsumptionDbService,
            HouseholdPredictionDbService householdPrediction, Sub_RegionPredictionDbService subRegionPrediction, RegionPredictionDbService regionPrediction,
            RegionDbService regionDbService1, Sub_RegionDbService sub_RegionDbService1, OwnerDbService ownerDbService1)
        {
            _houseConsumptionDbService = houseConsumptionDbService;
            _regionConsumptionDbService = regionConsumptionDbService1;
            _subregionConsumptionDbService = sub_RegionConsumptionDbService;
            _householdPrediction = householdPrediction;
            _subregionPrediction = subRegionPrediction;
            _regionPrediction = regionPrediction;
            _regionDbService = regionDbService1;
            _subregionDbService = sub_RegionDbService1;
            _ownerDbService = ownerDbService1;
        }
        [HttpGet]
        public async Task<IActionResult> Regions()
        {
            try
            {
                return View(await GetRegionConsumptions());
            }
            catch (Exception ex)
            {
                return RedirectToAction("Login", "Account");
            }
        }

        [HttpGet]
        public async Task<IActionResult> SubRegionsEngineer(int id)
        {
            try
            {
                List<SubRegion> regions = await GetAllSubRegionsConsumption();
                List<SubRegion> subregions = new List<SubRegion>();
                subregions = regions.Where(x => x.Region_Id == id).ToList();
                return View(subregions);
            }
            catch (Exception ex)
            {
                return RedirectToAction("Regions");
            }
        }

        [HttpGet]
        public async Task<IActionResult> EnginnerHouseHolds(int id)
        {
            try
            {
                List<HouseHold> houses = await GetAllHouseHoldConsumption(id);
                List<HouseHold> subregions = new List<HouseHold>();
                subregions = houses.Where(x => x.SubRegion_Id == id).ToList();
                return View(subregions);
            }
            catch (Exception ex)
            {
                return RedirectToAction("SubRegionsEngineer");
            }
        }

        [HttpGet]
        public async Task<IActionResult> RegionsPredictions()
        {
            try
            {
                return View(await GetAllRegionsPredictions());
            }
            catch (Exception ex)
            {
                return RedirectToAction("Regions");
            }
        }

        [HttpGet]
        public async Task<IActionResult> SubRegionsPredictions()
        {
            try
            {
                return View(await GetAllSubRegionsPredictions());

            }
            catch (Exception ex)
            {
                return RedirectToAction("SubRegionsEngineer");
            }
        }

        [HttpGet]
        public async Task<IActionResult> HouseholdPredictions()
        {
            try
            {
                return View(await GetAllHousholdPredictions());
            }
            catch (Exception ex)
            {
                return RedirectToAction("EnginnerHouseHolds");
            }
        }

        private async Task<List<RegionConsumption>> GetRegionConsumptions()
        {
            var response = await _regionConsumptionDbService.GetAllRegionsConsumptions();
            var regions = response.Data.Select(x => new RegionConsumption
            {
                Region_Id = x.Region_Id,
                Water_Sent = x.Water_Sent.Value,
                Water_Used = x.Water_Payed.Value,
                Water_Thats_OutStanding = x.OutstandingWater.Value,
                Daily_Usage = x.Daily_Usage,
                Night_Usage = x.Night_Usage,
                Spike_Usage = x.Spike_Usage,
                Leakage_Propability = x.Leakage_Percent.Value,
                Consumption_Month = x.Date.Value,
                FreeWater = x.FreeWater,
                Region_Name = x.Region_Name,
            }).ToList();
            return regions;
        }
        private async Task<List<SubRegion>> GetAllSubRegionsConsumption()
        {
            var response = await _subregionConsumptionDbService.GetAllSub_RegionConsumption();
            var subregions = response.Data.Select(x => new SubRegion
            {
                Daily_Usage = x.Daily_Usage,
                Night_Usage = x.Night_Usage,
                Spike_Usage = x.Spike_Usage,
                Leakage_Propability = x.Leakage_Percent.Value,
                SubRegion_Id = x.Sub_RegionId,
                Region_Id = x.RegionId,
                SubRegion_Name = x.Sub_RegionName,
                Water_Sent = x.Water_Sent.Value,
                Water_Used = x.Water_Payed.Value,
                Water_Thats_OutStanding = x.OutstandingWater.Value,
                FreeWater = x.FreeWater,
                Consumption_Month = x.Date.Value
            }).ToList();
            return subregions;
        }
        private async Task<List<HouseHold>> GetAllHouseHoldConsumption(int subregionId)
        {
            var subregions = await GetAllSubRegionsConsumption();
            var subregion = subregions.FirstOrDefault(x => x.SubRegion_Id == subregionId);
            var response = await _houseConsumptionDbService.GetAllHouseConsumption();
            var houses = response.Data.Select(x => new HouseHold
            {
                Daily_Usage = x.Daily_Usage,
                Night_Usage = x.Night_Usage,
                Spike_Usage = x.Spike_Usage,
                Leakage_Propability = x.Leakage_Percent.Value,
                SubRegion_Id = x.Sub_RegionId.Value,
                Household_Id = x.Household_ID.Length,
                SubRegion_Name = subregion.SubRegion_Name,
                Water_Received = x.Water_Received.Value,
                Consumption_Month = x.Date.Value,
                Account_Number = x.Owner_AccountNumber,
                FreeWater = x.FreeWater
            }).ToList();
            return houses;
        }
        private async Task<List<HouseHold>> GetAllHouseHoldConsumption()
        {
            var response = await _houseConsumptionDbService.GetAllHouseConsumption();
            var houses = response.Data.Select(x => new HouseHold
            {
                Daily_Usage = x.Daily_Usage,
                Night_Usage = x.Night_Usage,
                Spike_Usage = x.Spike_Usage,
                Leakage_Propability = x.Leakage_Percent.Value,
                SubRegion_Id = x.Sub_RegionId.Value,
                Household_Id = x.Household_ID.Length,
                Water_Received = x.Water_Received.Value,
                Consumption_Month = x.Date.Value,
                Account_Number = x.Owner_AccountNumber,
                FreeWater = x.FreeWater
            }).ToList();
            return houses;
        }
        private async Task<List<RegionPrediction>> GetAllRegionsPredictions()
        {
            var regions = await _regionDbService.GetAllRegions();
            int count = regions.Data.Count();
            var response = await _regionPrediction.GetAllRegionPredictions();
            var list = response.Data?.OrderByDescending(x => x.MonthYear).Take(count).ToList();
            List<RegionPrediction> predictions = list.Select(x => new RegionPrediction
            {
                EstimatedHourCost = Math.Round(x.EstimatedHourCost, 2),
                EstimatedMonthCost = Math.Round(x.EstimatedMonthCost, 2),
                EstimatedWeekCost = Math.Round(x.EstimatedWeekCost, 2),
                PredictedHourConsumptionKl = Math.Round(x.PredictedHourConsumptionKl / 1000, 2),
                PredictedMonthConsumptionKl = Math.Round(x.PredictedMonthConsumptionKl / 1000, 2),
                PredictedWeekConsumptionKl = Math.Round(x.PredictedWeekConsumptionKl / 1000, 2),
                RegionName = x.RegionName
            }).ToList(); 
            return predictions;
        }
        private async Task<List<SubRegionPrediction>> GetAllSubRegionsPredictions()
        {
            var regions = await _subregionDbService.GetAllSub_Regions();
            int count = regions.Data.Count();
            var response = await _subregionPrediction.GetAllSub_RegionPredictions();
            var list = response.Data?.OrderByDescending(x => x.MonthYear).Take(count).ToList();
            List<SubRegionPrediction> predictions = list.Select(x => new SubRegionPrediction
            {
                EstimatedHourCost = Math.Round(x.EstimatedHourCost, 2),
                EstimatedMonthCost = Math.Round(x.EstimatedMonthCost, 2),
                EstimatedWeekCost = Math.Round(x.EstimatedWeekCost, 2),
                PredictedHourConsumptionKl = Math.Round(x.PredictedHourConsumptionKl / 1000, 2),
                PredictedMonthConsumptionKl = Math.Round(x.PredictedMonthConsumptionKl / 1000, 2),
                PredictedWeekConsumptionKl = Math.Round(x.PredictedWeekConsumptionKl / 1000, 2),
                SubRegionName = x.SubRegionName
            }).ToList();
            return predictions;
        }

        private async Task<List<HouseholdPrediction>> GetAllHousholdPredictions()
        {
            var regions = await _ownerDbService.GetAllOwners();
            int count = regions.Data.Count();
            var response = await _householdPrediction.GetAllHouseholdPredictions();
            var list = response.Data?.OrderByDescending(x => x.MonthYear).Take(count).ToList();

            var subregions = await GetAllSubRegionsConsumption();

            List<HouseholdPrediction> predictions = list.Select(x => new HouseholdPrediction
            {
                EstimatedHourCost = Math.Round(x.EstimatedHourCost, 2),
                EstimatedMonthCost = Math.Round(x.EstimatedMonthCost, 2),
                EstimatedWeekCost = Math.Round(x.EstimatedWeekCost, 2),
                PredictedHourConsumptionKl = Math.Round(x.PredictedHourConsumptionKl / 1000, 2),
                PredictedMonthConsumptionKl = Math.Round(x.PredictedMonthConsumptionKl / 1000, 2),
                PredictedWeekConsumptionKl = Math.Round(x.PredictedWeekConsumptionKl / 1000, 2),
                AccountNumber = x.AccountNumber,
                SubRegionName = subregions.Where(a => a.SubRegion_Id == x.SubRegionId).Select(x => x.SubRegion_Name).FirstOrDefault()
            }).ToList();
            return predictions;
        }
    }
}
