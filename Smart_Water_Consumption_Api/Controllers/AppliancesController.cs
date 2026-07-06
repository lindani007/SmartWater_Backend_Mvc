using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Smart_Water_Consumption_Api.Models;
using Smart_Water_Consumption_Api.Models.Db;

[Route("api/[controller]")]
[ApiController]
public class AppliancesController : ControllerBase
{
    private readonly SmartWaterCnsumptionDbContentext _context;
    public AppliancesController(SmartWaterCnsumptionDbContentext context)
    {
        _context = context;
    }
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Appliance>>> GetAllAppliance()
    {
        return await _context.Appliances.ToListAsync();
    }
    [HttpGet("{appliance_id}")]
    public async Task<ActionResult<Appliance>> GetAppliance(int appliance_id)
    {
        var appliance = await _context.Appliances.FindAsync(appliance_id);

        if (appliance == null)
        {
            return NotFound($"Applience with id{appliance_id} was not found");
        }

        return appliance;
    }

    [HttpPut]
    public async Task<IActionResult> PutAppliance(Appliance appliance)
    {
        if (appliance == null)
        {
            return BadRequest("Appliance is required");
        }

        bool exist = _context.Appliances.Any(x => x.Appliance_Id == appliance.Appliance_Id);
        if(exist)
        {
            _context.Appliances.Update(appliance);
            await _context.SaveChangesAsync();
            return Ok(appliance);
        }
        else
        {
            return NotFound("Appliance does not exist");
        }
    }
    
    [HttpPost]
    public async Task<ActionResult<Appliance>> PostAppliance(Appliance appliance)
    {
        _context.Appliances.Add(appliance);
        await _context.SaveChangesAsync();

        return Ok( appliance);
    }

    [HttpDelete("{appliance_id}")]
    public async Task<IActionResult> DeleteAppliance(int? appliance_id)
    {
        var appliance = await _context.Appliances.FindAsync(appliance_id);
        if (appliance == null)
        {
            return NotFound("Appliance do not exists");
        }

        _context.Appliances.Remove(appliance);
        await _context.SaveChangesAsync();

        return Ok($"Appliance deleted succeseful");
    }
}
