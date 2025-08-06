using infoX.api.Data;
using infoX.api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace infoX.api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SegmentController : ControllerBase
    {
        private readonly PegasusConfigurationDbContext _context;

        public SegmentController(PegasusConfigurationDbContext context)
        {
            _context = context;
        }

        [HttpGet("select")]
        [Authorize]
        public async Task<IActionResult> GetBySegmentId()
        {
            try
            {
                var userIdClaim = User.FindFirst("userId")?.Value;

                var records = await _context.Segment
                    .FromSqlRaw("SELECT * FROM segment")
                    .ToListAsync();

                return Ok(records);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = "Error retrieving data", details = ex.Message });
            }
        }

        [HttpPost("insert")]
        [Authorize]
        public async Task<IActionResult> InsertSegment([FromBody] Segment company)
        {
            try
            {
                var userId = Convert.ToInt32(User.FindFirst("userId")?.Value);

                company.CreatedOn = DateTime.UtcNow;
                company.CreatedBy = userId;
                _context.Segment.Add(company);
                await _context.SaveChangesAsync();

                return Ok(new { message = "Segment inserted successfully", data = company });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = "Error inserting company", details = ex.Message });
            }
        }

        [HttpPut("update")]
        [Authorize]
        public async Task<IActionResult> UpdateSegment([FromBody] Segment company)
        {
            try
            {
                var existingSegment = await _context.Segment.FindAsync(company.Id);
                if (existingSegment == null)
                {
                    return NotFound(new { error = "Segment not found" });
                }

                var userId = Convert.ToInt32(User.FindFirst("userId")?.Value);

                existingSegment.Description = company.Description;
                existingSegment.BookId = company.BookId;
                existingSegment.ChangedOn = DateTime.UtcNow;
                existingSegment.ChangedBy = userId;

                _context.Segment.Update(existingSegment);
                await _context.SaveChangesAsync();

                return Ok(new { message = "Segment updated successfully", company = existingSegment });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = "Error updating company", details = ex.Message });
            }
        }

        [HttpDelete("delete/{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteSegment(int id)
        {
            try
            {
                var company = await _context.Segment.FindAsync(id);
                if (company == null)
                {
                    return NotFound(new { error = "Segment not found" });
                }

                _context.Segment.Remove(company);
                await _context.SaveChangesAsync();

                return Ok(new { message = "Segment deleted successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = "Error deleting company", details = ex.Message });
            }
        }
    }
}
