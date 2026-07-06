using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Smart_Water_Consumption_Api.Models;
using Smart_Water_Consumption_Api.Models.Db;

[Route("api/[controller]")]
[ApiController]
public class OwnersController : ControllerBase
{
    private readonly SmartWaterCnsumptionDbContentext _context;
    public OwnersController(SmartWaterCnsumptionDbContentext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Owner>>> GetAllOwners()
    {
        return await _context.Owners.ToListAsync();
    }

    [HttpGet("{accountNumber}")]
    public async Task<ActionResult<Owner>> GetOwner(string accountNumber)
    {
        var owner = await _context.Owners.FirstOrDefaultAsync(x => x.Owner_AccountNumber == accountNumber);

        if (owner == null)
        {
            return NotFound();
        }

        return owner;
    }

    
    [HttpPut]
    public async Task<IActionResult> PutOwner( Owner owner)
    {
        if ( owner == null)
        {
            return BadRequest();
        }

        _context.Owners.Update(owner);
        await _context.SaveChangesAsync();
        return Ok(owner);
    }

    
    [HttpPost]
    public async Task<ActionResult<Owner>> PostOwner(Owner owner)
    {
        if(owner == null)
            return BadRequest("Owner is required");
        _context.Owners.Add(owner);
        await _context.SaveChangesAsync();

        return Ok(owner);
    }

    [HttpDelete("{owner_idnumber}")]
    public async Task<IActionResult> DeleteOwner(string? owner_idnumber)
    {
        var owner = await _context.Owners.FindAsync(owner_idnumber);
        if (owner == null)
        {
            return NotFound();
        }

        _context.Owners.Remove(owner);
        await _context.SaveChangesAsync();

        return NoContent();
    }

}
