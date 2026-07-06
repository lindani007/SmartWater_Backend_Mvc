using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Smart_Water_Consumption_Api.Models;
using Smart_Water_Consumption_Api.Models.Db;

[Route("api/[controller]")]
[ApiController]
public class HouseholdPredictionsController : ControllerBase
{
    private readonly SmartWaterCnsumptionDbContentext _context;
    public HouseholdPredictionsController(SmartWaterCnsumptionDbContentext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<HouseholdPrediction>>> GetAllHouseholdPrediction()
    {
        return await _context.HouseholdPredictions.ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<HouseholdPrediction>> GetHouseholdPrediction(int id)
    {
        var householdprediction = await _context.HouseholdPredictions.FindAsync(id);

        if (householdprediction == null)
        {
            return NotFound();
        }

        return householdprediction;
    }

    
    [HttpPost]
    public async Task<ActionResult<HouseholdPrediction>> AddHouseholdPrediction(HouseholdPrediction householdprediction)
    {
        if(householdprediction == null)
            return BadRequest("House hold details are required");
        _context.HouseholdPredictions.Add(householdprediction);
        await _context.SaveChangesAsync();
        return Ok(householdprediction);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteHouseholdPrediction(int? id)
    {
        var householdprediction = await _context.HouseholdPredictions.FindAsync(id);
        if (householdprediction == null)
        {
            return NotFound();
        }

        _context.HouseholdPredictions.Remove(householdprediction);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
