using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Smart_Water_Consumption_Api.Models;
using Smart_Water_Consumption_Api.Models.Db;

[Route("api/[controller]")]
[ApiController]
public class EmployeesController : ControllerBase
{
    private readonly SmartWaterCnsumptionDbContentext _context;
    public EmployeesController(SmartWaterCnsumptionDbContentext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Employee>>> GetAllEmployees()
    {
        return await _context.Employees.ToListAsync();
    }

    [HttpGet("{employee_number}")]
    public async Task<ActionResult<Employee>> GetEmployee(string employee_number)
    {
        var employee = await _context.Employees.FindAsync(employee_number);

        if (employee == null)
        {
            return NotFound($"Employee with employee number {employee_number} was not found");
        }

        return employee;
    }

    
    [HttpPut]
    public async Task<IActionResult> EditEmployee(Employee employee)
    {
        if (employee == null)
        {
            return BadRequest("Please enter employee details to update");
        }
        _context.Employees.Update(employee);
        await _context.SaveChangesAsync();
        return Ok(employee);
    }

    
    [HttpPost]
    public async Task<ActionResult<Employee>> AddEmployee(Employee employee)
    {
        _context.Employees.Add(employee);
        await _context.SaveChangesAsync();

        return Ok(employee);
    }

    [HttpDelete("{employee_number}")]
    public async Task<IActionResult> DeleteEmployee(string? employee_number)
    {
        var employee = await _context.Employees.FindAsync(employee_number);
        if (employee == null)
        {
            return NotFound($"Employee with employee number {employee_number} was not found");
        }

        _context.Employees.Remove(employee);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
