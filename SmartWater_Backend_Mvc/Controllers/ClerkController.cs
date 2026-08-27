using Microsoft.AspNetCore.Mvc;
using Smart_Water_Consumption_Service.Services.DbServices;
using SmartWater_Backend_Mvc.Models;

namespace SmartWater_Backend_Mvc.Controllers
{
    public class ClerkController : Controller
    {
        private readonly OwnerDbService _ownerDbService;
        private readonly RegionDbService _regionDbService;
        private readonly Sub_RegionDbService _subRegionDbService;
        private readonly ApplianceDbService _applianceDbService;
        public ClerkController(OwnerDbService ownerDbService, RegionDbService regionDbService1, Sub_RegionDbService sub_RegionDbService1, ApplianceDbService applianceDbService1) 
        { 
            _ownerDbService = ownerDbService;
            _regionDbService = regionDbService1;
            _subRegionDbService = sub_RegionDbService1;
            _applianceDbService = applianceDbService1;
        }
        [HttpGet]
        public async Task<IActionResult> Citezens()
        {
            try
            {
                return View(await GetCitezens());

            }
            catch (Exception ex)
            {
                ViewData["Message"] = ex.Message;
                return View(await GetCitezens());
            }
        }

        [HttpGet]
        public async Task<IActionResult> AddCitezens(int id)
        {
            try
            {
                var subreagions = await GetAllSubRegions();
                var subr = subreagions.FirstOrDefault(x => x.Id == id);
                Owner owner = new Owner();
                owner.SubRegionId = subr.Id;
                owner.SubRegionName = subr.SubRegionName;
                return View(owner);
            }
            catch (Exception ex)
            {
                ViewData["Message"] = ex.Message;
                return RedirectToAction("Citezens");
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddCitezens(Owner owner)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    owner.Account_Number = await CreateCitezenAccountNumber();
                    string results = await AddCitezen(owner);
                    if (results == "Citezen added")
                    {
                        List<Owner> owners = await GetCitezens();
                        return View("Citezens", owners);
                    }
                    ViewData["Message"] = results;
                    return View(owner);
                }
                return View(owner);
            }
            catch (Exception ex)
            {
                ViewData["Message"] = ex.Message;
                return View(owner);
            }
        }

        [HttpGet]
        public async Task<IActionResult> EditCitezens(string id)
        {
            try
            {
                List<Owner> owners = await GetCitezens();
                Owner citezen = owners.Where(x => x.OwnerId_Number == id).FirstOrDefault() ?? new Owner();

                return View(citezen);
            }
            catch (Exception ex)
            {
                ViewData["Message"] = ex.Message;
                return RedirectToAction("Citezens");
            }
        }

        [HttpPost]
        public async Task<IActionResult> EditCitezens(Owner owner)
        {
            try
            {
                Smart_Water_Consumption_Service.Models.Owner citezen = new()
                {
                    Owner_AccountNumber = owner.Account_Number ?? string.Empty,
                    Household_ID = owner.HouseId.ToString(),
                    Owner_Name = owner.OwnerName ?? string.Empty,
                    Owner_MeterNumber = owner.Meter_Number ?? string.Empty,
                    Owner_Email = owner.Email ?? string.Empty,
                    Owner_IdNumber = owner.OwnerId_Number ?? string.Empty,
                    Owner_PhoneNumber = owner.Phone ?? string.Empty,
                    Sub_RegionName = owner.SubRegionName ?? string.Empty,
                };

                var response = await _ownerDbService.UpdateOwner(citezen);
                if (response.IsSuccess)
                {
                    List<Owner> owners = await GetCitezens();
                    return View("Citezens", owners);
                }
                ViewData["Message"] = response.Message ?? string.Empty;
                return View(owner);
            }
            catch (Exception ex)
            {
                ViewData["Message"] = ex.Message;
                return View(owner);
            }
            
        }

        [HttpGet]
        public async Task<IActionResult> Regions()
        {
            try
            {
                return View(await GetAllRegions());
            }
            catch (Exception ex)
            {
                ViewData["Message"] = ex.Message;
                return RedirectToAction("Login", "Account");
            }
        }

        [HttpGet]
        public IActionResult AddRegion()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddRegion(Region region)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    string results = await AddRegionToDb(region);
                    if (results == "region added")
                    {
                        return View("Regions", await GetAllRegions());
                    }
                    return View(region);
                }
                return View(region);
            }
            catch (Exception ex)
            {
                ViewData["Message"] = ex.Message;
                return View(region);
            }
            
        }

        [HttpGet]
        public async Task<IActionResult> EditRegion(int id)
        {
            try
            {
                List<Region> regions = await GetAllRegions();
                var region = regions.Where(x => x.Region_Id == id).FirstOrDefault();
                return View(region);
            }
            catch (Exception ex)
            {
                ViewData["Message"] = ex.Message;
                return RedirectToAction("Regions");
            }
        }

        [HttpPost]
        public async Task<IActionResult> EditRegion(Region region)
        {
            try
            {
                Smart_Water_Consumption_Service.Models.Region regionsToUpdate = new Smart_Water_Consumption_Service.Models.Region()
                {
                    Id = region.Region_Id,
                    Region_Name = region.Region_Name ?? string.Empty,
                    Region_Meter = region.Meter_Number ?? string.Empty,
                };
                var response = await _regionDbService.UpdateRegion(regionsToUpdate);
                if (response.IsSuccess)
                {
                    List<Region> regions = await GetAllRegions();
                    return View("Regions", regions);
                }
                ViewData["Message"] = response.Message ?? string.Empty;
                return View(region);
            }
            catch (Exception ex)
            {
                ViewData["Message"] = ex.Message;
                return View(region);
            }
        }

        [HttpGet]
        public async Task<IActionResult> DeleteRegion(int id)
        {
            try
            {
                var response = await _regionDbService.DeleteRegion(id);
                if (response.IsSuccess)
                {
                    List<Region> NEWregions = await GetAllRegions();
                    return View("Regions", NEWregions);
                }
                List<Region> regions = await GetAllRegions();
                return View("Regions", regions);
            }
            catch (Exception ex)
            {
                ViewData["Message"] = ex.Message;
                return RedirectToAction("Regions");
            }
        }

        [HttpGet]
        public async Task<IActionResult> SubRegions()
        {
            try
            {
                return View(await GetAllSubRegions());
            }
            catch (Exception ex)
            {
                ViewData["Message"] = ex.Message;
                return RedirectToAction("Regions");
            }
        }

        [HttpGet]
        public async Task<IActionResult> AddSubRegion(int id)
        {
            try
            {
                var regions = await GetAllRegions();
                Region region = regions.FirstOrDefault(x => x.Region_Id == id) ?? new Region();
                Sub_Region sub_Region = new Sub_Region();
                sub_Region.RegionId = id;
                sub_Region.Region_Name = region.Region_Name ?? string.Empty;
                return View(sub_Region);
            }
            catch (Exception ex)
            {
                ViewData["Message"] = ex.Message;
                return RedirectToAction("SubRegions");
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddSubRegion(Sub_Region subRegion)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    string results = await AddSubReginToDb(subRegion);
                    if (results == "Sub Region added")
                    {
                        List<Sub_Region> subregions = await GetAllSubRegions();
                        return View("SubRegions", subregions);
                    }
                    ViewData["Message"] = results;
                    return View(subRegion);
                }
                return View(subRegion);
            }
            catch (Exception ex)
            {
                ViewData["Message"] = ex.Message;
                return View(subRegion);
            }
        }

        [HttpGet]
        public async Task<IActionResult> EditSubRegion(int id)
        {
            try
            {
                List<Sub_Region> subregions = await GetAllSubRegions();
                var region = subregions.Where(x => x.Id == id).ToList().FirstOrDefault();
                return View(region);
            }
            catch (Exception ex)
            {
                ViewData["Message"] = ex.Message;
                return RedirectToAction("SubRegions");
            }
        }

        [HttpPost]
        public async Task<IActionResult> EditSubRegion(Sub_Region region)
        {
            try
            {
                Smart_Water_Consumption_Service.Models.Sub_Region sub_Region = new()
                {
                    Id = region.Id,
                    RegionId = region.RegionId,
                    RegionName = region.Region_Name ?? string.Empty,
                    SubRegionName = region.SubRegionName ?? string.Empty,
                    SubRegion_Meter = region.Meter_Number ?? string.Empty
                };
                var response = await _subRegionDbService.PutSub_Region(sub_Region);
                if (response.IsSuccess)
                {
                    List<Sub_Region> subregions = await GetAllSubRegions();
                    return View("SubRegions", subregions);
                }
                ViewData["Message"] = response.Message ?? string.Empty;
                return View(region);
            }
            catch (Exception ex)
            {
                ViewData["Message"] = ex.Message;
                return View(region);
            }
        }

        [HttpGet]
        public async Task<IActionResult> DeleteSubRegion(int id)
        {
            try
            {
                var reponse = await _subRegionDbService.DeleteSub_Region(id);
                if (reponse.IsSuccess)
                {
                    List<Sub_Region> NEWsubregions = await GetAllSubRegions();
                    return View("SubRegions", NEWsubregions);
                }
                List<Sub_Region> subregions = await GetAllSubRegions();
                return View("SubRegions", subregions);
            }
            catch (Exception ex)
            {
                ViewData["Message"] = ex.Message;
                return RedirectToAction("SubRegions");
            }
           
        }

       
        
        private async Task<List<Models.Owner>> GetCitezens()
        {
            var response = await _ownerDbService.GetAllOwners();
            var citezens = response.Data.Select(x => new Models.Owner
            {
                Meter_Number = x.Owner_MeterNumber ?? string.Empty,
                OwnerName = x.Owner_Name ?? string.Empty,
                Account_Number = x.Owner_AccountNumber ?? string.Empty,
                Email = x.Owner_Email ?? string.Empty,
                Phone = x.Owner_PhoneNumber ?? string.Empty,
                SubRegionName = x.Sub_RegionName ?? string.Empty,
                OwnerId_Number = x.Owner_IdNumber ?? string.Empty,
                HouseId = int.Parse(x.Household_ID),
                PropertyValue = x.PropertyValue
            }).ToList();

            return citezens;
        }
        private async Task<string> AddCitezen(Models.Owner owner)
        {            var citezenToAdd = new Smart_Water_Consumption_Service.Models.Owner
            {
                Owner_AccountNumber = owner.Account_Number ?? string.Empty,
                Household_ID = owner.HouseId.ToString(),
                Owner_Name = owner.OwnerName ?? string.Empty,
                Owner_MeterNumber = owner.Meter_Number ?? string.Empty,
                Owner_Email = owner.Email ?? string.Empty,
                Owner_IdNumber = owner.OwnerId_Number ?? string.Empty,
                Owner_PhoneNumber = owner.Phone ?? string.Empty,
                Sub_RegionName = owner.SubRegionName ?? string.Empty,
                PropertyValue = owner.PropertyValue,
                Sub_RegionId = owner.SubRegionId
            };
            var response = await _ownerDbService.AddOwner(citezenToAdd);

            if (response.IsSuccess)
                return "Citezen added";
            return response.Message ?? string.Empty;
        }
        private async Task<string> CreateCitezenAccountNumber()
        {
            var citezens = await GetCitezens();
            int citezensCount = citezens.Count;
            citezensCount += 1;
            return citezensCount.ToString("D10");
        }

        private async Task<List<Region>> GetAllRegions()
        {
            var response = await _regionDbService.GetAllRegions();
            List<Region> regions = response.Data.Select(x => new Region
            {
                Region_Id = x.Id,
                Region_Name = x.Region_Name ?? string.Empty,
                Meter_Number = x.Region_Meter ?? string.Empty
            }).ToList();
            return regions;
        }
        private async Task<string> AddRegionToDb(Region region)
        {
            Smart_Water_Consumption_Service.Models.Region regionToAdd = new()
            {
                Region_Name = region.Region_Name ?? string.Empty,
                Region_Meter = region.Meter_Number ?? string.Empty
            };
            var response = await _regionDbService.PostRegion(regionToAdd);
            if (response.IsSuccess)
                return "region added";

            return response.Message ?? string.Empty;
        }

        private async Task<List<Sub_Region>> GetAllSubRegions()
        {
            var response =  await _subRegionDbService.GetAllSub_Regions();
            List<Sub_Region> sub_Regions = response.Data.Select(x => new Sub_Region
            {
                Id = x.Id,
                Meter_Number = x.SubRegion_Meter ?? string.Empty,
                Region_Name = x.RegionName ?? string.Empty,
                SubRegionName = x.SubRegionName ?? string.Empty,
                RegionId = x.RegionId.Value,
            }).ToList();

            return sub_Regions;
        }
        private async Task<string> AddSubReginToDb(Sub_Region subRegion)
        {
            var regions = await GetAllRegions();
            var region = regions.FirstOrDefault(x => x.Region_Name == subRegion.Region_Name);
            Smart_Water_Consumption_Service.Models.Sub_Region subRegionToAdd = new()
            {
                RegionName = subRegion.Region_Name ?? string.Empty,
                SubRegionName = subRegion.SubRegionName ?? string.Empty,
                SubRegion_Meter = subRegion.Meter_Number ?? string.Empty,
                RegionId = region.Region_Id
            };
            var response = await _subRegionDbService.PutSub_Region(subRegionToAdd);
            if (response.IsSuccess)
                return "Sub Region added";

            return response.Message ?? string.Empty;
        }

        private async Task<IEnumerable<Smart_Water_Consumption_Service.Models.Appliance>> GetOwnerAppliences(string account)
        {
            var rawData = await _applianceDbService.GetAllAppliance();
            return rawData.Data ?? new List<Smart_Water_Consumption_Service.Models.Appliance>();
        }
    }
}
