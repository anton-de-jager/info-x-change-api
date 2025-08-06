using infoX.api.Data;
using infoX.api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[Route("api/[controller]")]
[ApiController]
public class DepartmentController : ControllerBase
{
    private readonly PegasusConfigurationDbContext _context;

    public DepartmentController(PegasusConfigurationDbContext context)
    {
        _context = context;
    }

    [HttpGet("select")]
    [Authorize]
    public async Task<IActionResult> GetAll()
    {
        try
        {
            var userIdClaim = User.FindFirst("userId")?.Value;
            var companyIdClaim = User.FindFirst("companyId")?.Value;

            var records = await _context.Department.ToListAsync();
            return Ok(records);
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = "Error retrieving departments", details = ex.Message });
        }
    }

    [HttpPost("insert")]
    [Authorize]
    public async Task<IActionResult> Insert([FromBody] Department department)
    {
        try
        {
            var userId = Convert.ToInt32(User.FindFirst("userId")?.Value);

            department.CreatedOn = DateTime.UtcNow;
            department.CreatedBy = userId;
            _context.Department.Add(department);
            await _context.SaveChangesAsync();
            return Ok(new { message = "Department inserted successfully", data = department });
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = "Error inserting department", details = ex.Message });
        }
    }

    [HttpPut("update")]
    [Authorize]
    public async Task<IActionResult> Update([FromBody] Department department)
    {
        try
        {
            var existing = await _context.Department.FindAsync(department.Id);
            if (existing == null)
                return NotFound(new { error = "Department not found" });

            var userId = Convert.ToInt32(User.FindFirst("userId")?.Value);

            existing.Description = department.Description;
            existing.CompanyId = department.CompanyId;
            existing.ChangedOn = DateTime.UtcNow;
            existing.ChangedBy = userId;

            _context.Department.Update(existing);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Department updated successfully", data = existing });
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = "Error updating department", details = ex.Message });
        }
    }

    [HttpDelete("delete/{id}")]
    [Authorize]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var entity = await _context.Department.FindAsync(id);
            if (entity == null)
                return NotFound(new { error = "Department not found" });

            _context.Department.Remove(entity);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Department deleted successfully" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = "Error deleting department", details = ex.Message });
        }
    }
}
