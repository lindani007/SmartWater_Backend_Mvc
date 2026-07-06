using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Smart_Water_Consumption_Api.Models;
using Smart_Water_Consumption_Api.Models.Db;

[Route("api/[controller]")]
[ApiController]
public class OwnerPaymentsController : ControllerBase
{
    private readonly SmartWaterCnsumptionDbContentext _context;
    public OwnerPaymentsController(SmartWaterCnsumptionDbContentext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<OwnerPayment>>> GetAllOwnersPayments()
    {
        return await _context.OwnerPayments.ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<OwnerPayment>> GetOwnerPayment(int id)
    {
        var ownerpayment = await _context.OwnerPayments.FindAsync(id);

        if (ownerpayment == null)
        {
            return NotFound();
        }

        return ownerpayment;
    }

    [HttpPost]
    public async Task<ActionResult<OwnerPayment>> PostOwnerPayment(OwnerPayment ownerpayment)
    {
        _context.OwnerPayments.Add(ownerpayment);
        await _context.SaveChangesAsync();

        return Ok(ownerpayment);
    }
}
