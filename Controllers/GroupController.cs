using infoX.api.Data;
using infoX.api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[Route("api/[controller]")]
[ApiController]
public class GroupController : ControllerBase
{
    private readonly PegasusConfigurationDbContext _context;

    public GroupController(PegasusConfigurationDbContext context)
    {
        _context = context;
    }

    [HttpGet("select")]
    [Authorize]
    public async Task<IActionResult> GetAll()
    {
        try
        {
            var records = await _context.Group.ToListAsync();
            return Ok(records);
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = "Error retrieving groups", details = ex.Message });
        }
    }

    [HttpPost("insert")]
    [Authorize]
    public async Task<IActionResult> Insert([FromBody] Group group)
    {
        try
        {
            var userId = Convert.ToInt32(User.FindFirst("userId")?.Value);

            group.CreatedBy = userId;
            group.CreatedOn = DateTime.UtcNow;
            _context.Group.Add(group);
            await _context.SaveChangesAsync();
            return Ok(new { message = "Group inserted successfully", data = group });
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = "Error inserting group", details = ex.Message });
        }
    }

    [HttpPut("update")]
    [Authorize]
    public async Task<IActionResult> Update([FromBody] Group group)
    {
        try
        {
            var userId = Convert.ToInt32(User.FindFirst("userId")?.Value);

            var existing = await _context.Group.FindAsync(group.Id);
            if (existing == null)
                return NotFound(new { error = "Group not found" });

            existing.Description = group.Description;
            existing.CompanyId = group.CompanyId;
            existing.ChangedOn = DateTime.UtcNow;
            existing.ChangedBy = userId;

            _context.Group.Update(existing);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Group updated successfully", data = existing });
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = "Error updating group", details = ex.Message });
        }
    }

    [HttpDelete("delete/{id}")]
    [Authorize]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var entity = await _context.Group.FindAsync(id);
            if (entity == null)
                return NotFound(new { error = "Group not found" });

            _context.Group.Remove(entity);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Group deleted successfully" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = "Error deleting group", details = ex.Message });
        }
    }
}
