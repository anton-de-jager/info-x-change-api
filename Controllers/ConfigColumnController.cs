using infoX.api.Data;
using infoX.api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[Route("api/[controller]")]
[ApiController]
public class ConfigColumnController : ControllerBase
{
    private readonly PegasusConfigurationDbContext _context;

    public ConfigColumnController(PegasusConfigurationDbContext context)
    {
        _context = context;
    }

    [HttpGet("select")]
    [Authorize]
    public async Task<IActionResult> GetAll()
    {
        try
        {
            var records = await _context.ConfigColumns.ToListAsync();
            return Ok(records);
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = "Error retrieving config columns", details = ex.Message });
        }
    }

    [HttpPost("insert")]
    [Authorize]
    public async Task<IActionResult> Insert([FromBody] ConfigColumn column)
    {
        try
        {
            var userId = User.FindFirst("userId")?.Value;

            _context.ConfigColumns.Add(column);
            await _context.SaveChangesAsync();
            return Ok(new { message = "Config column inserted successfully", data = column });
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = "Error inserting config column", details = ex.Message });
        }
    }

    [HttpPut("update")]
    [Authorize]
    public async Task<IActionResult> Update([FromBody] ConfigColumn column)
    {
        try
        {
            var existing = await _context.ConfigColumns.FindAsync(column.Id);
            if (existing == null)
                return NotFound(new { error = "Config column not found" });

            existing.Property = column.Property;
            existing.Label = column.Label;
            existing.DataType = column.DataType;
            existing.FilterType = column.FilterType;
            existing.VisibleTable = column.VisibleTable;
            existing.VisibleDialog = column.VisibleDialog;
            existing.VisibleFilter = column.VisibleFilter;
            existing.Index = column.Index;
            existing.AllowUpdate = column.AllowUpdate;
            existing.Required = column.Required;
            existing.IsForeignKey = column.IsForeignKey;
            existing.IncludeInsert = column.IncludeInsert;
            existing.IncludeUpdate = column.IncludeUpdate;

            _context.ConfigColumns.Update(existing);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Config column updated successfully", data = existing });
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = "Error updating config column", details = ex.Message });
        }
    }

    [HttpDelete("delete/{id}")]
    [Authorize]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var entity = await _context.ConfigColumns.FindAsync(id);
            if (entity == null)
                return NotFound(new { error = "Config column not found" });

            _context.ConfigColumns.Remove(entity);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Config column deleted successfully" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = "Error deleting config column", details = ex.Message });
        }
    }
}
