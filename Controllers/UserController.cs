using infoX.api.Data;
using infoX.api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[Route("api/[controller]")]
[ApiController]
public class UserController : ControllerBase
{
    private readonly PegasusConfigurationDbContext _context;

    public UserController(PegasusConfigurationDbContext context)
    {
        _context = context;
    }

    [HttpGet("select")]
    [Authorize]
    public async Task<IActionResult> GetAll()
    {
        try
        {
            var records = await _context.VwUser.ToListAsync();
            return Ok(records);
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = "Error retrieving users", details = ex.Message });
        }
    }

    [HttpPost("insert")]
    [Authorize]
    public async Task<IActionResult> Insert([FromBody] User user)
    {
        try
        {
            var userId = User.FindFirst("userId")?.Value;

            user.CreatedOn = DateTime.UtcNow;
            _context.User.Add(user);
            await _context.SaveChangesAsync();
            return Ok(new { message = "User inserted successfully", data = user });
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = "Error inserting user", details = ex.Message });
        }
    }

    [HttpPut("update")]
    [Authorize]
    public async Task<IActionResult> Update([FromBody] User user)
    {
        try
        {
            var userId = User.FindFirst("userId")?.Value;

            var existing = await _context.User.FindAsync(user.Id);
            if (existing == null)
                return NotFound(new { error = "User not found" });

            // Update the properties of the existing entity
            existing.CompanyId = user.CompanyId;
            existing.RoleId = user.RoleId;
            existing.Title = user.Title;
            existing.Name = user.Name;
            existing.Surname = user.Surname;
            existing.MaidenName = user.MaidenName;
            existing.KnownName = user.KnownName;
            existing.Phone = user.Phone;
            existing.Email = user.Email;
            existing.IdNumber = user.IdNumber;
            existing.Gender = user.Gender;
            existing.Nationality = user.Nationality;
            existing.ChangedOn = DateTime.UtcNow;
            existing.ChangedBy = Convert.ToInt32(userId);

            // Save changes
            await _context.SaveChangesAsync();

            return Ok(new { message = "User updated successfully", data = existing });
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = "Error updating user", details = ex.Message });
        }
    }

    [HttpDelete("delete/{id}")]
    [Authorize]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var entity = await _context.User.FindAsync(id);
            if (entity == null)
                return NotFound(new { error = "User not found" });

            _context.User.Remove(entity);
            await _context.SaveChangesAsync();

            return Ok(new { message = "User deleted successfully" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = "Error deleting user", details = ex.Message });
        }
    }
}
