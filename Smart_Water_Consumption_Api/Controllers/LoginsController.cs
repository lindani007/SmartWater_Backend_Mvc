using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Smart_Water_Consumption_Api.Models;
using Smart_Water_Consumption_Api.Models.Db;

[Route("api/[controller]")]
[ApiController]
public class LoginsController : ControllerBase
{
    private readonly SmartWaterCnsumptionDbContentext _context;
    public LoginsController(SmartWaterCnsumptionDbContentext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Login>>> GetLogin()
    {
        return await _context.Logins.ToListAsync();
    }

    [HttpGet("account/{id}")]
    public async Task<ActionResult<Login>> GetCitezen(string id)
    {
        var login = await _context.Logins.FirstOrDefaultAsync(x => x.Account_Number == id);

        if (login == null)
        {
            return NotFound($"User with Account Number {id} was not found");
        }

        return login;
    }

    [HttpGet("employeenumber/{id}")]
    public async Task<ActionResult<Login>> GetEmployee(string id)
    {
        var login = await _context.Logins.FirstOrDefaultAsync(x => x.Staff_Id == id);

        if (login == null)
        {
            return NotFound($"Employee with {id} not found");
        }

        return login;
    }

    [HttpPut]
    public async Task<IActionResult> UpdatePassword(Login login)
    {
        if (login == null)
        {
            return BadRequest("User datails are required");
        }
        if(!string.IsNullOrEmpty( login.Staff_Id))
        {
            bool exists = _context.Logins.Any(x => x.Staff_Id == login.Staff_Id);
            if(exists)
            {
                _context.Logins.Update(login);
                await _context.SaveChangesAsync();
                return Ok(login);
            }
            else
            {
                return BadRequest("User does not exist");
            }
        }
        else if(!string.IsNullOrEmpty(login.Account_Number))
        {
            bool exists = _context.Logins.Any(x => x.Account_Number == login.Account_Number);
            if (exists)
            {
                _context.Logins.Update(login);
                await _context.SaveChangesAsync();
                return Ok(login);
            }
            else
            {
                return BadRequest("User does not exist");
            }
        }
        
        return BadRequest("Please enter details to update");
    }

    
    [HttpPost]
    public async Task<ActionResult<Login>> PostLogin(Login login)
    {
        if (login == null)
            return BadRequest("Please enter details");
        if (string.IsNullOrEmpty(login.Staff_Id) && string.IsNullOrEmpty(login.Account_Number))
        {
            return BadRequest("Please enter a username to register");
        }
        if (!string.IsNullOrEmpty(login.Staff_Id) && !string.IsNullOrEmpty(login.Account_Number))
        {
            return BadRequest("Cannot provide both Staff ID and Account Number.");
        }
        if (!string.IsNullOrEmpty(login.Staff_Id))
        {
            _context.Logins.Add(login);
            await _context.SaveChangesAsync();

            return Ok(login);
        }
        else if (!string.IsNullOrEmpty(login.Account_Number))
        {
            _context.Logins.Add(login);
            await _context.SaveChangesAsync();

            return Ok(login);
        }
        return BadRequest("Un able to register you right now");
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteLogin(int? id)
    {
        var login = await _context.Logins.FindAsync(id);
        if (login == null)
        {
            return NotFound();
        }

        _context.Logins.Remove(login);
        await _context.SaveChangesAsync();

        return NoContent();
    }


}
