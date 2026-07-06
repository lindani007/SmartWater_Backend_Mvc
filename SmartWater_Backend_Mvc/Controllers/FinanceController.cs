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
            List<RegionConsumption> regions = await GetRegionConsumptions();
            return View(regions);
        }

        [HttpGet]
        public async Task<IActionResult> SubRegion(int id)
        {
            List<SubRegion> subregions = await GetAllSubRegionsConsumption();
            subregions = subregions.Where(x => x.Region_Id == id).ToList();
            return View(subregions);
        }

        [HttpGet]
        public async Task<IActionResult> HouseHolds(int id)
        {
            List<HouseHold> houses = await GetAllHouseHoldConsumption(id);
            List<HouseHold> subregions = new List<HouseHold>();
            subregions = houses.Where(x => x.SubRegion_Id == id).ToList();
            return View(subregions);
        }

        [HttpPost]
        public IActionResult SummarizeFinanceReport()
        {
            var regions = new List<RegionConsumption>()
            {
                new RegionConsumption {Region_Id = 1, Region_Name = "Durban Central", Water_Sent = 100000, Water_Used = 70000, Expected_Cost = 350000, Actual_Cost= 290000},
                new RegionConsumption {Region_Id = 2, Region_Name = "Umngeni", Water_Sent = 50000, Water_Used = 45000, Expected_Cost = 150000, Actual_Cost = 125000},
                new RegionConsumption {Region_Id = 3, Region_Name = "Brea", Water_Sent = 150000, Water_Used = 120000, Expected_Cost = 450000, Actual_Cost = 378000},
                new RegionConsumption {Region_Id = 4, Region_Name = "Morning Side", Water_Sent = 60000, Water_Used = 60000, Expected_Cost = 200000, Actual_Cost = 200000},
            };

            var subRegions = new List<SubRegion>()
            {
                new SubRegion {SubRegion_Id = 1, Region_Id = 1, SubRegion_Name = "West Street", Expected_Cost = 350000, Actual_Cost= 290000},
                new SubRegion {SubRegion_Id = 2, Region_Id = 1, SubRegion_Name = "Anton Lembede", Expected_Cost = 150000, Actual_Cost = 125000},
                new SubRegion {SubRegion_Id = 3, Region_Id = 1, SubRegion_Name = "Naidoo", Expected_Cost = 450000, Actual_Cost = 378000},
                new SubRegion {SubRegion_Id = 4, Region_Id = 2, SubRegion_Name = "Adrian", Expected_Cost = 200000, Actual_Cost = 200000},
                new SubRegion {SubRegion_Id = 5, Region_Id = 2, SubRegion_Name = "S B Xuma", Expected_Cost = 350000, Actual_Cost= 290000},
                new SubRegion {SubRegion_Id = 6, Region_Id = 3, SubRegion_Name = "Edward", Expected_Cost = 150000, Actual_Cost = 125000},
                new SubRegion {SubRegion_Id = 7, Region_Id = 4, SubRegion_Name = "Grey", Expected_Cost = 450000, Actual_Cost = 378000},
                new SubRegion {SubRegion_Id = 8, Region_Id = 2, SubRegion_Name = "Smith", Expected_Cost = 200000, Actual_Cost = 200000},
            };

            var houses = new List<HouseHold>()
            {
                new HouseHold {SubRegion_Id = 1, Household_Id = 1, Account_Number = "1234567890", Other_OutStandingCost = 349, Status = "OutStanding"},
                new HouseHold {SubRegion_Id = 1, Household_Id = 2, Account_Number = "1234567810", Other_OutStandingCost = 0, Status = "Paid"},
                new HouseHold {SubRegion_Id = 3, Household_Id = 3, Account_Number = "1234567811", Other_OutStandingCost = 49, Status = "OutStanding"},
                new HouseHold {SubRegion_Id = 2, Household_Id = 4, Account_Number = "1234567890", Other_OutStandingCost = 0, Status = "Paid"},
                new HouseHold {SubRegion_Id = 2, Household_Id = 5, Account_Number = "1234567890", Other_OutStandingCost = 0, Status = "Paid"},
                new HouseHold {SubRegion_Id = 3, Household_Id = 6, Account_Number = "1234567890", Other_OutStandingCost = 109, Status = "OutStanding"},
                new HouseHold {SubRegion_Id = 3, Household_Id = 7, Account_Number = "1234567890", Other_OutStandingCost = 0, Status = "Paid"},
                new HouseHold {SubRegion_Id = 2, Household_Id = 8, Account_Number = "1234567890", Other_OutStandingCost = 0, Status = "Paid"},
            };

            var sb = new StringBuilder();
            sb.AppendLine("### Municipal Full-Tier Hierarchical Financial Analysis\n");
            sb.AppendLine("#### 📈 Tiers Performing Well");

            var goodHouses = houses.Where(h => h.Status == "Paid").Take(2);
            foreach (var h in goodHouses)
            {
                sb.AppendLine($"* **Household Account {h.Account_Number}**: Cleared 100% of water balances. Maintenance score high.");
            }
            sb.AppendLine("* **Morning Side Region**: Efficiency target metrics balanced cleanly across collections loops.\n");

            sb.AppendLine("#### ⚠️ Critical Deficiency Tiers (Doing Bad)");
            foreach (var r in regions.Where(x => (x.Expected_Cost - x.Actual_Cost) > 0))
            {
                var diff = r.Expected_Cost - r.Actual_Cost;
                sb.AppendLine($"* **Region: {r.Region_Name}**: Accumulating deficit patterns totaling R{diff:N2}.");
            }

            var badHouses = houses.Where(h => h.Status == "OutStanding").Take(2);
            foreach (var h in badHouses)
            {
                sb.AppendLine($"  |_ **Household Account {h.Account_Number}**: Overdue billing trace identified at R{h.Other_OutStandingCost:N2}.");
            }

            sb.AppendLine("\n#### 🛠️ Core Tactical Recommendations");
            sb.AppendLine("1. Establish high-frequency automated SMS collection loops inside Durban Central zones.");
            sb.AppendLine("2. Deploy regional maintenance checking teams to monitor structural grid locations with persistent outstanding debt profiles.");

            ViewBag.AnalysisType = "Full Structural Matrix Audit";
            return PartialView("_AiReportViewer", sb.ToString());
        }

        [HttpPost]
        public IActionResult SummarizeRegionReport()
        {
            return PartialView("_AiReportViewer");
        }
        [HttpPost]
        public IActionResult SummarizeSubRegionReport()
        {
            return PartialView("_AiReportViewer");
        }
        [HttpPost]
        public IActionResult SummarizeHouseholdReport()
        {
            return PartialView("_AiReportViewer");
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


