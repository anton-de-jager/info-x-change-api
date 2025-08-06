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
    public class BotController : ControllerBase
    {
        private readonly PegasusConfigurationDbContext _context;

        public BotController(PegasusConfigurationDbContext context)
        {
            _context = context;
        }

        [HttpGet("select")]
        [Authorize]
        public async Task<IActionResult> GetByBotId()
        {
            try
            {
                var userIdClaim = User.FindFirst("userId")?.Value;

                var records = await _context.Bot
                    .FromSqlRaw("SELECT * FROM bot")
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
        public async Task<IActionResult> InsertBot([FromBody] Bot company)
        {
            try
            {
                var userId = Convert.ToInt32(User.FindFirst("userId")?.Value);

                company.CreatedOn = DateTime.UtcNow;
                company.CreatedBy = userId;
                _context.Bot.Add(company);
                await _context.SaveChangesAsync();

                return Ok(new { message = "Bot inserted successfully", data = company });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = "Error inserting company", details = ex.Message });
            }
        }

        [HttpPut("update")]
        [Authorize]
        public async Task<IActionResult> UpdateBot([FromBody] Bot company)
        {
            try
            {
                var existingBot = await _context.Bot.FindAsync(company.Id);
                if (existingBot == null)
                {
                    return NotFound(new { error = "Bot not found" });
                }

                var userId = Convert.ToInt32(User.FindFirst("userId")?.Value);

                existingBot.Description = company.Description;
                existingBot.WhatsAppNumber = company.WhatsAppNumber;
                existingBot.WaLink = company.WaLink;
                existingBot.DepartmentId = company.DepartmentId;
                existingBot.ChangedOn = DateTime.UtcNow;
                existingBot.ChangedBy = userId;

                _context.Bot.Update(existingBot);
                await _context.SaveChangesAsync();

                return Ok(new { message = "Bot updated successfully", company = existingBot });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = "Error updating company", details = ex.Message });
            }
        }

        [HttpDelete("delete/{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteBot(int id)
        {
            try
            {
                var company = await _context.Bot.FindAsync(id);
                if (company == null)
                {
                    return NotFound(new { error = "Bot not found" });
                }

                _context.Bot.Remove(company);
                await _context.SaveChangesAsync();

                return Ok(new { message = "Bot deleted successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = "Error deleting company", details = ex.Message });
            }
        }
    }
}
