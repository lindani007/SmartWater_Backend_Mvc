using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Smart_Water_Consumption_Api.Models;
using Smart_Water_Consumption_Api.Models.Db;

[Route("api/[controller]")]
[ApiController]
public class Sub_RegionConsumptionController : ControllerBase
{
    private readonly SmartWaterCnsumptionDbContentext _context;
    public Sub_RegionConsumptionController(SmartWaterCnsumptionDbContentext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Sub_RegionConsumption>>> GetAllSub_RegionConsumption()
    {
        return await _context.Sub_RegionConsumptions.ToListAsync();
    }

    
    [HttpGet("{id}")]
    public async Task<ActionResult<Sub_RegionConsumption>> GetSub_RegionConsumption(int id)
    {
        var sub_regionconsumption = await _context.Sub_RegionConsumptions.FindAsync(id);

        if (sub_regionconsumption == null)
        {
            return NotFound();
        }

        return sub_regionconsumption;
    }

    [HttpPut]
    public async Task<ActionResult<Sub_RegionConsumption>> UpdateSub_RegionConsumption(List<Sub_RegionConsumption> sub_regionconsumption)
    {
        foreach (var item in sub_regionconsumption)
        {
                bool exist = await _context.Sub_RegionConsumptions.AnyAsync(x => x.Id == item.Id);
            if(exist)
            {
                _context.Sub_RegionConsumptions.Update(item);
            }
            else
            {
               await _context.Sub_RegionConsumptions.AddAsync(item);
            }
        }
        await _context.SaveChangesAsync();

        return Ok(sub_regionconsumption);
    }


    [HttpPost]
    public async Task<ActionResult<Sub_RegionConsumption>> PostSub_RegionConsumption(Sub_RegionConsumption sub_regionconsumption)
    {
        _context.Sub_RegionConsumptions.Add(sub_regionconsumption);
        await _context.SaveChangesAsync();

        return Ok( sub_regionconsumption);
    }

}
