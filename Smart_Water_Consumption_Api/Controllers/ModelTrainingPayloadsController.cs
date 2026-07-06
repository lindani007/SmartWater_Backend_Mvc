using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Smart_Water_Consumption_Api.Models;
using Smart_Water_Consumption_Api.Models.Db;

[Route("api/[controller]")]
[ApiController]
public class ModelTrainingPayloadsController : ControllerBase
{
    private readonly SmartWaterCnsumptionDbContentext _context;
    public ModelTrainingPayloadsController(SmartWaterCnsumptionDbContentext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ModelTrainingPayload>>> GetModelTrainingPayload()
    {
        return await _context.ModelTrainingPayloads.ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ModelTrainingPayload>> GetModelTrainingPayload(int id)
    {
        var modeltrainingpayload = await _context.ModelTrainingPayloads.FindAsync(id);

        if (modeltrainingpayload == null)
        {
            return NotFound();
        }

        return modeltrainingpayload;
    }

    
    [HttpPut("{id}")]
    public async Task<IActionResult> PutModelTrainingPayload(int? id, ModelTrainingPayload modeltrainingpayload)
    {
        if (id != modeltrainingpayload.Id)
        {
            return BadRequest();
        }

        _context.Entry(modeltrainingpayload).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!ModelTrainingPayloadExists(id))
            {
                return NotFound();
            }
            else
            {
                throw;
            }
        }

        return NoContent();
    }

    // POST: api/ModelTrainingPayload
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPost]
    public async Task<ActionResult<ModelTrainingPayload>> PostModelTrainingPayload(ModelTrainingPayload modeltrainingpayload)
    {
        _context.ModelTrainingPayloads.Add(modeltrainingpayload);
        await _context.SaveChangesAsync();

        return CreatedAtAction("GetModelTrainingPayload", new { id = modeltrainingpayload.Id }, modeltrainingpayload);
    }

    // DELETE: api/ModelTrainingPayload/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteModelTrainingPayload(int? id)
    {
        var modeltrainingpayload = await _context.ModelTrainingPayloads.FindAsync(id);
        if (modeltrainingpayload == null)
        {
            return NotFound();
        }

        _context.ModelTrainingPayloads.Remove(modeltrainingpayload);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool ModelTrainingPayloadExists(int? id)
    {
        return _context.ModelTrainingPayloads.Any(e => e.Id == id);
    }
}
