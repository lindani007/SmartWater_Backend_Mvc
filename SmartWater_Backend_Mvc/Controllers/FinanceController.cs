using Microsoft.AspNetCore.Mvc;
using Smart_Water_Consumption_Service.Services.DbServices;
using SmartWater_Backend_Mvc.Models;
using System.Text;

namespace SmartWater_Backend_Mvc.Controllers
{
    public class FinanceController : Controller
    {
        private readonly RegionConsumptionDbService _regionConsumptionDbService;
        private readonly Sub_RegionConsumptionDbService _subregionConsumptionDbService;
        private readonly HouseConsumptionDbService _houseConsumptionDbService;
        public FinanceController(RegionConsumptionDbService regionConsumptionDbService1, Sub_RegionConsumptionDbService sub_RegionConsumptionDbService, HouseConsumptionDbService houseConsumptionDbService)
        {
            _houseConsumptionDbService = houseConsumptionDbService;
            _regionConsumptionDbService = regionConsumptionDbService1;
            _subregionConsumptionDbService = sub_RegionConsumptionDbService;
        }

        [HttpGet]
        public async Task<IActionResult> FinanceView()
        {
            try
            {
                return View(await GetRegionConsumptions());
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in FinanceView: {ex.Message}");
                return RedirectToAction("Login", "Account");
            }
        }

        [HttpGet]
        public async Task<IActionResult> SubRegion(int id)
        {
            try
            {
                List<SubRegion> subregions = await GetAllSubRegionsConsumption();
                subregions = subregions.Where(x => x.Region_Id == id).ToList();
                return View(subregions);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in SubRegion: {ex.Message}");
                return RedirectToAction("FinanceView");
            }
            
        }

        [HttpGet]
        public async Task<IActionResult> HouseHolds(int id)
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
                Console.WriteLine($"Error in HouseHolds: {ex.Message}");
                return RedirectToAction("SubRegion");
            }
            
        }

        private async Task<List<RegionConsumption>> GetRegionConsumptions()
        {
            var response = await _regionConsumptionDbService.GetAllRegionsConsumptions();
            var regions = response.Data.Select(x => new RegionConsumption
            {
                Id = x.Id,
                Region_Id = x.Region_Id,
                Region_Name = x.Region_Name,
                Water_Sent = x.Water_Sent.Value,
                Water_Used = x.Water_Payed.Value,
                Water_Thats_OutStanding = x.OutstandingWater.Value,
                Expected_Cost = x.Extepected_Amount.Value,
                Actual_Cost = x.Actual_Amount.Value,
                FreeWater = x.FreeWater,
                Consumption_Month = x.Date.Value
            }).ToList();
            return regions;
        }
        private async Task<List<SubRegion>> GetAllSubRegionsConsumption()
        {
            var response = await _subregionConsumptionDbService.GetAllSub_RegionConsumption();
            var subregions = response.Data.Select(x => new SubRegion
            {
                SubRegion_Id = x.Sub_RegionId,
                Region_Id = x.RegionId,
                SubRegion_Name = x.Sub_RegionName,
                Water_Sent = x.Water_Sent.Value,
                Water_Used = x.Water_Payed.Value,
                Water_Thats_OutStanding = x.OutstandingWater.Value,
                Expected_Cost = x.Extepected_Amount.Value,
                Actual_Cost = x.Actual_Amount.Value,
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
                SubRegion_Id = x.Sub_RegionId.Value,
                Household_Id = x.Household_ID.Length,
                SubRegion_Name = subregion.SubRegion_Name,
                Water_Received = x.Water_Received.Value,
                Cost = x.Actual_Amount.Value,
                Consumption_Month = x.Date.Value,
                Account_Number = x.Owner_AccountNumber,
                Other_OutStandingCost = x.OutStanding_Amount.Value,
                Status = x.Payment_Status,
                FreeWater = x.FreeWater
            }).ToList();
            return houses;
        }
    }
}


