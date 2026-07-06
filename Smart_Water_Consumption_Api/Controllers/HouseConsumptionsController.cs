using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Smart_Water_Consumption_Api.Models;
using Smart_Water_Consumption_Api.Models.Db;

[Route("api/[controller]")]
[ApiController]
public class HouseConsumptionsController : ControllerBase
{
    private readonly SmartWaterCnsumptionDbContentext _context;
    public HouseConsumptionsController(SmartWaterCnsumptionDbContentext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<HouseConsumption>>> GetAllHouseConsumption()
    {
        return await _context.HouseConsumptions.ToListAsync();
    }

    [HttpGet("{household_id}")]
    public async Task<ActionResult<HouseConsumption>> GetHouseConsumption(string household_id)
    {
        var houseconsumption = await _context.HouseConsumptions.FirstOrDefaultAsync( x => x.Owner_AccountNumber == household_id);

        if (houseconsumption == null)
        {
            return NotFound();
        }

        return houseconsumption;
    }
    
    [HttpPut]
    public async Task<ActionResult<HouseConsumption>> UpdateHouseConsumptions(List<HouseConsumption> houseconsumptions)
    {
        if (!houseconsumptions.Any())
            return BadRequest();

        _context.HouseConsumptions.UpdateRange(houseconsumptions);
        await _context.SaveChangesAsync();

        return Ok(houseconsumptions);
    }

    [HttpPost]
    public async Task<ActionResult<HouseConsumption>> AddHouseConsumption(HouseConsumption houseconsumption)
    {
        _context.HouseConsumptions.Add(houseconsumption);
        await _context.SaveChangesAsync();

        return Ok(houseconsumption);
    }

    // DELETE: api/HouseConsumption/5
    [HttpDelete("{household_id}")]
    public async Task<IActionResult> DeleteHouseConsumption(string? household_id)
    {
        var houseconsumption = await _context.HouseConsumptions.FindAsync(household_id);
        if (houseconsumption == null)
        {
            return NotFound();
        }

        _context.HouseConsumptions.Remove(houseconsumption);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
