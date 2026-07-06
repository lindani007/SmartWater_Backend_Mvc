using Microsoft.AspNetCore.Mvc;
using Smart_Water_Consumption_Service.Services.DbServices;
using SmartWater_Backend_Mvc.Models;

namespace SmartWater_Backend_Mvc.Controllers
{
    public class EngineerController : Controller
    {
        private readonly RegionConsumptionDbService _regionConsumptionDbService;
        private readonly Sub_RegionConsumptionDbService _subregionConsumptionDbService;
        private readonly HouseConsumptionDbService _houseConsumptionDbService;
        public EngineerController(RegionConsumptionDbService regionConsumptionDbService1, Sub_RegionConsumptionDbService sub_RegionConsumptionDbService, HouseConsumptionDbService houseConsumptionDbService)
        {
            _houseConsumptionDbService = houseConsumptionDbService;
            _regionConsumptionDbService = regionConsumptionDbService1;
            _subregionConsumptionDbService = sub_RegionConsumptionDbService;
        }
        [HttpGet]
        public async Task<IActionResult> Regions()
        {
            List<RegionConsumption> regions = await GetRegionConsumptions();
            return View(regions);
        }

        [HttpGet]
        public async Task<IActionResult> SubRegionsEngineer(int id)
        {
            List<SubRegion> regions = await GetAllSubRegionsConsumption();
            List<SubRegion> subregions = new List<SubRegion>();
            subregions = regions.Where(x => x.Region_Id == id).ToList();
            return View(subregions);
        }

        [HttpGet]
        public async Task<IActionResult> EnginnerHouseHolds(int id)
        {
            List<HouseHold> houses = await GetAllHouseHoldConsumption(id);
            List<HouseHold> subregions = new List<HouseHold>();
            subregions = houses.Where(x => x.SubRegion_Id == id).ToList();
            return View(subregions);
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
    }
}
