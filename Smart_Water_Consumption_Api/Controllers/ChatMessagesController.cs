using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Smart_Water_Consumption_Api.Models;
using Smart_Water_Consumption_Api.Models.Db;

[Route("api/[controller]")]
[ApiController]
public class ChatMessagesController : ControllerBase
{
    private readonly SmartWaterCnsumptionDbContentext _context;
    public ChatMessagesController(SmartWaterCnsumptionDbContentext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ChatMessage>>> GetChatMessage()
    {
        return await _context.ChatMessages.ToListAsync();
    }

    [HttpGet("{userid}")]
    public async Task<ActionResult<List<ChatMessage>>> GetChatMessage(string userid)
    {
        var chatmessage = await _context.ChatMessages.Where(m => m.UserId == userid).ToListAsync();

        if (chatmessage == null || chatmessage.Count == 0)
        {
            return NotFound("Chat message not found");
        }

        return  Ok(chatmessage.OrderByDescending(x => x.Timestamp)) ;
    }

    
    [HttpPost]
    public async Task<ActionResult<ChatMessage>> PostChatMessage(ChatMessage chatmessage)
    {
        _context.ChatMessages.Add(chatmessage);
        await _context.SaveChangesAsync();

        return Ok(chatmessage);
    }

    
    [HttpDelete("{chatid}")]
    public async Task<IActionResult> DeleteChatMessage(int? chatid)
    {
        var chatmessage = await _context.ChatMessages.FindAsync(chatid);
        if (chatmessage == null)
        {
            return NotFound("Chat message not found");
        }

        _context.ChatMessages.Remove(chatmessage);
        await _context.SaveChangesAsync();

        return Ok(chatmessage);
    }
}
