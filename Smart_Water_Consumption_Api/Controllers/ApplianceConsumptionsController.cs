using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Smart_Water_Consumption_Api.Models;
using Smart_Water_Consumption_Api.Models.Db;

[Route("api/[controller]")]
[ApiController]
public class ApplianceConsumptionsController : ControllerBase
{
    private readonly SmartWaterCnsumptionDbContentext _context;
    public ApplianceConsumptionsController(SmartWaterCnsumptionDbContentext context)
    {
        _context = context;
    }
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ApplianceConsumption>>> GetAllApplianceConsumptions()
    {
        return await _context.ApplianceConsumptions.ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ApplianceConsumption>> GetApplianceConsumption(int id)
    {
        var applianceconsumption = await _context.ApplianceConsumptions.FindAsync(id);

        if (applianceconsumption == null)
        {
            return NotFound();
        }

        return applianceconsumption;
    }

    [HttpPost]
    public async Task<ActionResult<ApplianceConsumption>> PostApplianceConsumption(List<ApplianceConsumption> applianceconsumption)
    {
        _context.ApplianceConsumptions.AddRange(applianceconsumption);
        await _context.SaveChangesAsync();

        return Ok(applianceconsumption);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteApplianceConsumption(int? id)
    {
        var applianceconsumption = await _context.ApplianceConsumptions.FindAsync(id);
        if (applianceconsumption == null)
        {
            return NotFound();
        }

        _context.ApplianceConsumptions.Remove(applianceconsumption);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
