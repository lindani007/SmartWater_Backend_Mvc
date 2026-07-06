using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System;
namespace SmartWater_Backend_Mvc.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Overview()
        {
            // Return sample local data rather than calling an external API
            var model = new[]
            {
                new {
                    regionName = "Central District",
                    totalKiloliters = 12500,
                    expectedRevenue = "$12,500",
                    actualRevenue = "$11,200",
                    outstandingBalance = "$1,300",
                    waterDispatched = "12,500 KL",
                    expectedCost = "$12,500",
                    costPaid = "$11,200",
                    outstandingCost = "$1,300",
                    status = "OK"
                },
                new {
                    regionName = "North Subregion",
                    totalKiloliters = 8500,
                    expectedRevenue = "$8,500",
                    actualRevenue = "$7,000",
                    outstandingBalance = "$1,500",
                    waterDispatched = "8,500 KL",
                    expectedCost = "$8,500",
                    costPaid = "$7,000",
                    outstandingCost = "$1,500",
                    status = "Alert"
                }
            };

            return View(model);
        }
    }
}
