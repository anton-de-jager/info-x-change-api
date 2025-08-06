using infoX.api.Data;
using infoX.api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[Route("api/[controller]")]
[ApiController]
public class RoleController : ControllerBase
{
    private readonly PegasusConfigurationDbContext _context;

    public RoleController(PegasusConfigurationDbContext context)
    {
        _context = context;
    }

    [HttpGet("select")]
    [Authorize]
    public async Task<IActionResult> GetAll()
    {
        try
        {
            var records = await _context.Role
                .FromSqlRaw("EXEC usp_select_all_role")
                .ToListAsync();
            return Ok(records);
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = "Error retrieving roles", details = ex.Message });
        }
    }

    [HttpPost("insert")]
    [Authorize]
    public async Task<IActionResult> Insert([FromBody] Role role)
    {
        try
        {
            var userId = User.FindFirst("userId")?.Value;

            await _context.Database.ExecuteSqlRawAsync(
                "EXEC usp_insert_role @Description, @CreatedOn, @ChangedOn, @ChangedBy",
                new[]
                {
                    new Microsoft.Data.SqlClient.SqlParameter("@Description", role.Description ?? (object)DBNull.Value),
                    new Microsoft.Data.SqlClient.SqlParameter("@CreatedOn", DateTime.UtcNow),
                    new Microsoft.Data.SqlClient.SqlParameter("@ChangedOn", null),
                    new Microsoft.Data.SqlClient.SqlParameter("@ChangedOn", null)
                });

            return Ok(new { message = "Role inserted successfully" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = "Error inserting role", details = ex.Message });
        }
    }

    [HttpPut("update")]
    [Authorize]
    public async Task<IActionResult> Update([FromBody] Role role)
    {
        try
        {
            int userId = Convert.ToInt32(User.FindFirst("userId")?.Value);

            var result = await _context.Database.ExecuteSqlRawAsync(
                "EXEC UpdateRole @Id, @Description, @ChangedOn, @ChangedBy",
                new[]
                {
                    new Microsoft.Data.SqlClient.SqlParameter("@Id", role.Id ?? (object)DBNull.Value),
                    new Microsoft.Data.SqlClient.SqlParameter("@Description", role.Description ?? (object)DBNull.Value),
                    new Microsoft.Data.SqlClient.SqlParameter("@CreatedOn", role.CreatedOn ?? (object)DBNull.Value),
                    new Microsoft.Data.SqlClient.SqlParameter("@ChangedOn", DateTime.UtcNow),
                    new Microsoft.Data.SqlClient.SqlParameter("@ChangedBy", userId)
                });

            if (result == 0)
                return NotFound(new { error = "Role not found" });

            return Ok(new { message = "Role updated successfully" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = "Error updating role", details = ex.Message });
        }
    }

    [HttpDelete("delete/{id}")]
    [Authorize]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var result = await _context.Database.ExecuteSqlRawAsync(
                "EXEC DeleteRole @Id",
                new Microsoft.Data.SqlClient.SqlParameter("@Id", id));

            if (result == 0)
                return NotFound(new { error = "Role not found" });

            return Ok(new { message = "Role deleted successfully" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = "Error deleting role", details = ex.Message });
        }
    }
}
