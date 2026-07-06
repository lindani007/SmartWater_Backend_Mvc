using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Smart_Water_Consumption_Api.Models;
using Smart_Water_Consumption_Api.Models.Db;

[Route("api/[controller]")]
[ApiController]
public class RegionPredictionsController : ControllerBase
{
    private readonly SmartWaterCnsumptionDbContentext _context;
    public RegionPredictionsController(SmartWaterCnsumptionDbContentext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<RegionPrediction>>> GetAllRegionPrediction()
    {
        return await _context.RegionPredictions.ToListAsync();
    }

   
    [HttpPost]
    public async Task<ActionResult<RegionPrediction>> PostRegionPrediction(RegionPrediction regionprediction)
    {
        if (regionprediction == null)
            return BadRequest();
        _context.RegionPredictions.Add(regionprediction);
        await _context.SaveChangesAsync();

        return Ok( regionprediction);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteRegionPrediction(int? id)
    {
        var regionprediction = await _context.RegionPredictions.FindAsync(id);
        if (regionprediction == null)
        {
            return NotFound();
        }

        _context.RegionPredictions.Remove(regionprediction);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
