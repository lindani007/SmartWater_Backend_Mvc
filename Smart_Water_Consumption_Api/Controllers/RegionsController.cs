using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Smart_Water_Consumption_Api.Models;
using Smart_Water_Consumption_Api.Models.Db;

[Route("api/[controller]")]
[ApiController]
public class RegionsController : ControllerBase
{
    private readonly SmartWaterCnsumptionDbContentext _context;
    public RegionsController(SmartWaterCnsumptionDbContentext context)
    {
        _context = context;
    }
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Region>>> GetAllRegions()
    {
        return await _context.Regions.ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Region>> GetRegion(int id)
    {
        var region = await _context.Regions.FindAsync(id);

        if (region == null)
        {
            return NotFound();
        }

        return region;
    }

   
    [HttpPut]
    public async Task<IActionResult> UpdateRegion( Region region)
    {
        if (region == null)
        {
            return BadRequest("Regions is required");
        }
        _context.Regions.Update(region);
        await _context.SaveChangesAsync();
        
        return Ok(region);
    }

    
    [HttpPost]
    public async Task<ActionResult<Region>> PostRegion(Region region)
    {
        if (region == null)
            return BadRequest("Region is required");
        _context.Regions.Add(region);
        await _context.SaveChangesAsync();

        return Ok( region);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteRegion(int? id)
    {
        var region = await _context.Regions.FindAsync(id);
        if (region == null)
        {
            return NotFound($"Region with id {id} not found");
        }

        _context.Regions.Remove(region);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
