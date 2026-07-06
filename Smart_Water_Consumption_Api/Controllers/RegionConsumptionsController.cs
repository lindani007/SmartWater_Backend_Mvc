using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Smart_Water_Consumption_Api.Models;
using Smart_Water_Consumption_Api.Models.Db;

[Route("api/[controller]")]
[ApiController]
public class RegionConsumptionsController : ControllerBase
{
    private readonly SmartWaterCnsumptionDbContentext _context;
    public RegionConsumptionsController(SmartWaterCnsumptionDbContentext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<RegionConsumption>>> GetAllRegionsConsumptions()
    {
        return await _context.RegionConsumptions.ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<RegionConsumption>> GetRegionConsumption(int id)
    {
        var regionconsumption = await _context.RegionConsumptions.FindAsync(id);

        if (regionconsumption == null)
        {
            return NotFound();
        }

        return regionconsumption;
    }
    [HttpPut]
    public async Task<ActionResult<RegionConsumption>> UpdateRegionConsumption(List<RegionConsumption> regionconsumption)
    {
        if (!regionconsumption.Any())
            return BadRequest();
        foreach (var item in regionconsumption)
        {
            bool exists = await _context.RegionConsumptions.AnyAsync(x => x.Id == item.Id);
            if (exists)
                _context.RegionConsumptions.Update(item);
            else
               await _context.RegionConsumptions.AddAsync(item);
        }
        await _context.SaveChangesAsync();

        return Ok(regionconsumption);
    }
    [HttpPost]
    public async Task<ActionResult<RegionConsumption>> PostRegionConsumption(RegionConsumption regionconsumption)
    {
        if(regionconsumption == null)
            return BadRequest();
        _context.RegionConsumptions.Add(regionconsumption);
        await _context.SaveChangesAsync();

        return Ok(regionconsumption);
    }

    
}
