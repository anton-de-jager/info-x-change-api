using infoX.api.Data;
using infoX.api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[Route("api/[controller]")]
[ApiController]
public class PlatformUserController : ControllerBase
{
    private readonly PegasusConfigurationDbContext _context;

    public PlatformUserController(PegasusConfigurationDbContext context)
    {
        _context = context;
    }

    [HttpGet("select")]
    [Authorize]
    public async Task<IActionResult> GetAll()
    {
        try
        {
            var records = await _context.PlatformUser.ToListAsync();
            return Ok(records);
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = "Error retrieving groups", details = ex.Message });
        }
    }

    [HttpPost("insert")]
    [Authorize]
    public async Task<IActionResult> Insert([FromBody] PlatformUser group)
    {
        try
        {
            var userId = User.FindFirst("userId")?.Value;

            group.CreatedBy = Convert.ToInt32(userId);
            group.CreatedOn = DateTime.UtcNow;
            _context.PlatformUser.Add(group);
            await _context.SaveChangesAsync();
            return Ok(new { message = "PlatformUser inserted successfully", data = group });
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = "Error inserting group", details = ex.Message });
        }
    }

    [HttpPut("update")]
    [Authorize]
    public async Task<IActionResult> Update([FromBody] PlatformUser group)
    {
        try
        {
            var existing = await _context.PlatformUser.FindAsync(group.Id);
            if (existing == null)
                return NotFound(new { error = "PlatformUser not found" });

            existing = group;

            var userId = Convert.ToInt32(User.FindFirst("userId")?.Value);

            existing.ChangedBy = Convert.ToInt32(userId);
            existing.ChangedOn = DateTime.UtcNow;

            _context.PlatformUser.Update(existing);
            await _context.SaveChangesAsync();

            return Ok(new { message = "PlatformUser updated successfully", data = existing });
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
            var entity = await _context.PlatformUser.FindAsync(id);
            if (entity == null)
                return NotFound(new { error = "PlatformUser not found" });

            _context.PlatformUser.Remove(entity);
            await _context.SaveChangesAsync();

            return Ok(new { message = "PlatformUser deleted successfully" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = "Error deleting group", details = ex.Message });
        }
    }
}
