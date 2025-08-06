using infoX.api.Data;
using infoX.api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[Route("api/[controller]")]
[ApiController]
public class PlatformController : ControllerBase
{
    private readonly PegasusConfigurationDbContext _context;

    public PlatformController(PegasusConfigurationDbContext context)
    {
        _context = context;
    }

    [HttpGet("select")]
    [Authorize]
    public async Task<IActionResult> GetAll()
    {
        try
        {
            var records = await _context.Platform.ToListAsync();
            return Ok(records);
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = "Error retrieving platforms", details = ex.Message });
        }
    }

    [HttpPost("insert")]
    [Authorize]
    public async Task<IActionResult> Insert([FromBody] Platform platform)
    {
        try
        {
            var userId = User.FindFirst("userId")?.Value;

            platform.CreatedBy = Convert.ToInt32(userId);
            platform.CreatedOn = DateTime.UtcNow;
            _context.Platform.Add(platform);
            await _context.SaveChangesAsync();
            return Ok(new { message = "Platform inserted successfully", data = platform });
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = "Error inserting platform", details = ex.Message });
        }
    }

    [HttpPut("update")]
    [Authorize]
    public async Task<IActionResult> Update([FromBody] Platform platform)
    {
        try
        {
            var existing = await _context.Platform.FindAsync(platform.Id);
            if (existing == null)
                return NotFound(new { error = "Platform not found" });

            var userId = Convert.ToInt32(User.FindFirst("userId")?.Value);

            existing.Description = platform.Description;
            existing.ChangedOn = DateTime.UtcNow;
            existing.ChangedBy = userId;

            _context.Platform.Update(existing);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Platform updated successfully", data = existing });
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = "Error updating platform", details = ex.Message });
        }
    }

    [HttpDelete("delete/{id}")]
    [Authorize]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var entity = await _context.Platform.FindAsync(id);
            if (entity == null)
                return NotFound(new { error = "Platform not found" });

            _context.Platform.Remove(entity);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Platform deleted successfully" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = "Error deleting platform", details = ex.Message });
        }
    }
}
