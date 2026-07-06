using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System;
using System.Linq;

namespace SmartWater_Backend_Mvc.Controllers
{
    public class RegionalController : Controller
    {
        public IActionResult Analysis()
        {
            return View("~/Clients/Views/Regional/Analysis.cshtml");
        }

        [HttpGet]
        public Task<IActionResult> GetRegions(string? search, string? status)
        {
            var regions = new[]
            {
                new { regionId = 1, regionName = "Central District", waterDispatched = "12,500 KL", expectedCost = "$12,500", costPaid = "$11,200", outstandingCost = "$1,300", status = "OK", outstandingCostValue = 1300 },
                new { regionId = 2, regionName = "North Subregion", waterDispatched = "8,500 KL", expectedCost = "$8,500", costPaid = "$7,000", outstandingCost = "$1,500", status = "Alert", outstandingCostValue = 1500 },
                new { regionId = 3, regionName = "South Valley", waterDispatched = "6,200 KL", expectedCost = "$6,200", costPaid = "$6,200", outstandingCost = "$0", status = "OK", outstandingCostValue = 0 }
            };

            var query = regions.AsEnumerable();
            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(r => r.regionName.Contains(search, StringComparison.OrdinalIgnoreCase));
            }
            if (!string.IsNullOrWhiteSpace(status))
            {
                query = query.Where(r => string.Equals(r.status, status, StringComparison.OrdinalIgnoreCase));
            }

            return Task.FromResult<IActionResult>(Json(query));
        }

        [HttpGet]
        public Task<IActionResult> GetSubRegions(int regionId)
        {
            var subs = regionId switch
            {
                1 => new[] { new { subRegionName = "Central A", waterDispatched = "5,000 KL", variance = "-1%", alertStatus = "OK" }, new { subRegionName = "Central B", waterDispatched = "7,500 KL", variance = "+3%", alertStatus = "Alert" } },
                2 => new[] { new { subRegionName = "North 1", waterDispatched = "4,500 KL", variance = "+10%", alertStatus = "Alert" } },
                _ => Array.Empty<object>()
            };

            return Task.FromResult<IActionResult>(Json(subs));
        }
    }
}
