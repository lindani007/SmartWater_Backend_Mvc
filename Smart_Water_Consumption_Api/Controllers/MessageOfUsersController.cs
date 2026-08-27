using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Smart_Water_Consumption_Api.Models;
using Smart_Water_Consumption_Api.Models.Db;

[Route("api/[controller]")]
[ApiController]
public class MessageOfUsersController : ControllerBase
{
    private readonly SmartWaterCnsumptionDbContentext _context;
    public MessageOfUsersController(SmartWaterCnsumptionDbContentext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<MessageOfUser>>> GetMessageOfUser()
    {
        return await _context.MessageOfUsers.ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<List<MessageOfUser>>> GetMessageOfUser(int id)
    {
        var messageofuser = await _context.MessageOfUsers.Where(m => m.ChatId == id).ToListAsync();

        if (messageofuser.Count == 0)
        {
            return NotFound(new List<MessageOfUser>());
        }

        return Ok(messageofuser);
    }

    
    [HttpPost]
    public async Task<ActionResult<MessageOfUser>> PostMessageOfUser(MessageOfUser messageofuser)
    {
        _context.MessageOfUsers.Add(messageofuser);
        await _context.SaveChangesAsync();

        return Ok(messageofuser);
    }

    
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteMessageOfUser(int? id)
    {
        var messageofuser = await _context.MessageOfUsers.Where(m => m.Id == id).ToListAsync();
        if (messageofuser == null || messageofuser.Count == 0)
        {
            return NotFound();
        }

        _context.MessageOfUsers.RemoveRange(messageofuser);
        await _context.SaveChangesAsync();

        return Ok(messageofuser);
    }
}
