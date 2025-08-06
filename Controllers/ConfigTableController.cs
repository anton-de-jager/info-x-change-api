using infoX.api.Data;
using infoX.api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[Route("api/[controller]")]
[ApiController]
public class ConfigTableController : ControllerBase
{
    private readonly PegasusConfigurationDbContext _context;

    public ConfigTableController(PegasusConfigurationDbContext context)
    {
        _context = context;
    }

    [HttpGet("select")]
    [Authorize]
    public async Task<IActionResult> GetAll()
    {
        try
        {
            var records = await _context.ConfigTables.ToListAsync();
            return Ok(records);
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = "Error retrieving config tables", details = ex.Message });
        }
    }

    [HttpPost("insert")]
    [Authorize]
    public async Task<IActionResult> Insert([FromBody] ConfigTable table)
    {
        try
        {
            var userId = User.FindFirst("userId")?.Value;

            _context.ConfigTables.Add(table);
            await _context.SaveChangesAsync();
            return Ok(new { message = "Config table inserted successfully", data = table });
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = "Error inserting config table", details = ex.Message });
        }
    }

    [HttpPut("update")]
    [Authorize]
    public async Task<IActionResult> Update([FromBody] ConfigTable table)
    {
        try
        {
            var existing = await _context.ConfigTables.FindAsync(table.Id);
            if (existing == null)
                return NotFound(new { error = "Config table not found" });

            existing = table;

            _context.ConfigTables.Update(existing);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Config table updated successfully", data = existing });
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = "Error updating config table", details = ex.Message });
        }
    }

    [HttpDelete("delete/{id}")]
    [Authorize]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var entity = await _context.ConfigTables.FindAsync(id);
            if (entity == null)
                return NotFound(new { error = "Config table not found" });

            _context.ConfigTables.Remove(entity);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Config table deleted successfully" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = "Error deleting config table", details = ex.Message });
        }
    }
}
