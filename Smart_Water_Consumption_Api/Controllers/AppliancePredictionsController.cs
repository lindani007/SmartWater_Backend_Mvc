using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Smart_Water_Consumption_Api.Models;
using Smart_Water_Consumption_Api.Models.Db;

[Route("api/[controller]")]
[ApiController]
public class AppliancePredictionsController : ControllerBase
{
    private readonly SmartWaterCnsumptionDbContentext _context;
    public AppliancePredictionsController(SmartWaterCnsumptionDbContentext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<AppliancePrediction>>> GetAllAppliancePrediction()
    {
        return await _context.AppliancePredictions.ToListAsync();
    }


    [HttpPost]
    public async Task<ActionResult<AppliancePrediction>> PostAppliancePrediction(AppliancePrediction applianceprediction)
    {
        _context.AppliancePredictions.Add(applianceprediction);
        await _context.SaveChangesAsync();
        return Ok(applianceprediction);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteAppliancePrediction(int? id)
    {
        var applianceprediction = await _context.AppliancePredictions.FindAsync(id);
        if (applianceprediction == null)
        {
            return NotFound();
        }

        _context.AppliancePredictions.Remove(applianceprediction);
        await _context.SaveChangesAsync();

        return NoContent();
    }

}
