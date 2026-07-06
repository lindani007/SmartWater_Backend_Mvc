using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Smart_Water_Consumption_Api.Models;
using Smart_Water_Consumption_Api.Models.Db;

[Route("api/[controller]")]
[ApiController]
public class SubRegionPredictionsController : ControllerBase
{
    private readonly SmartWaterCnsumptionDbContentext _context;
    public SubRegionPredictionsController(SmartWaterCnsumptionDbContentext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<SubRegionPrediction>>> GetSubRegionPrediction()
    {
        return await _context.SubRegionPredictions.ToListAsync();
    }

    
    [HttpPost]
    public async Task<ActionResult<SubRegionPrediction>> PostSubRegionPrediction(SubRegionPrediction subregionprediction)
    {
        _context.SubRegionPredictions.Add(subregionprediction);
        await _context.SaveChangesAsync();

        return Ok(subregionprediction);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteSubRegionPrediction(int? id)
    {
        var subregionprediction = await _context.SubRegionPredictions.FindAsync(id);
        if (subregionprediction == null)
        {
            return NotFound();
        }

        _context.SubRegionPredictions.Remove(subregionprediction);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
