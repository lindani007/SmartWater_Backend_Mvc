using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Smart_Water_Consumption_Api.Models;
using Smart_Water_Consumption_Api.Models.Db;

[Route("api/[controller]")]
[ApiController]
public class Sub_RegionController : ControllerBase
{
    private readonly SmartWaterCnsumptionDbContentext _context;
    public Sub_RegionController(SmartWaterCnsumptionDbContentext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Sub_Region>>> GetAllSub_Regions()
    {
        return await _context.Sub_Regions.ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Sub_Region>> GetSub_Region(int id)
    {
        var sub_region = await _context.Sub_Regions.FindAsync(id);

        if (sub_region == null)
        {
            return NotFound();
        }

        return sub_region;
    }

   
    [HttpPut]
    public async Task<IActionResult> PutSub_Region( Sub_Region sub_region)
    {
        if (sub_region == null)
        {
            return BadRequest("SubRegion is required");
        }

        _context.Sub_Regions.Update(sub_region);
        await _context.SaveChangesAsync();
        return Ok(sub_region);
    }

    [HttpPost]
    public async Task<ActionResult<Sub_Region>> PostSub_Region(Sub_Region sub_region)
    {
        _context.Sub_Regions.Add(sub_region);
        await _context.SaveChangesAsync();

        return Ok( sub_region);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteSub_Region(int? id)
    {
        var sub_region = await _context.Sub_Regions.FindAsync(id);
        if (sub_region == null)
        {
            return NotFound();
        }

        _context.Sub_Regions.Remove(sub_region);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
