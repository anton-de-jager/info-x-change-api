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
    public class DashboardWhatsAppController : ControllerBase
    {
        private readonly WhatsAppDbContext _context;

        public DashboardWhatsAppController(WhatsAppDbContext context)
        {
            _context = context;
        }

        [HttpGet("dashboard/{companyId}/{interval}")]
        [Authorize]
        public async Task<IActionResult> Get(int companyId, string interval)
        {
            try
            {
                var records = await _context.dashboardWhatsApp
                    .FromSqlRaw("EXEC usp_dashboard_whatsApp " + companyId + ", '" + interval + "';")
                    .ToListAsync();

                return Ok(records);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = "Error retrieving data", details = ex.Message });
            }
        }

        [HttpGet("messages/{companyId}/{interval}")]
        [Authorize]
        public async Task<IActionResult> GetMessages(int companyId, string interval)
        {
            try
            {
                var records = await _context.VwMessages
                    .FromSqlRaw("EXEC usp_dashboard_messages " + companyId + ", '" + interval + "';")
                    .ToListAsync();

                return Ok(records);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = "Error retrieving data", details = ex.Message });
            }
        }

        [HttpGet("users/{companyId}/{interval}")]
        [Authorize]
        public async Task<IActionResult> GetUsers(int companyId, string interval)
        {
            try
            {
                var records = await _context.VwMessages
                    .FromSqlRaw("EXEC usp_dashboard_users " + companyId + ", '" + interval + "';")
                    .ToListAsync();

                return Ok(records);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = "Error retrieving data", details = ex.Message });
            }
        }

        [HttpGet("arrangements/{companyId}/{interval}")]
        [Authorize]
        public async Task<IActionResult> GetArrangements(int companyId, string interval)
        {
            try
            {
                var records = await _context.VwMessages
                    .FromSqlRaw("EXEC usp_dashboard_arrangements " + companyId + ", '" + interval + "';")
                    .ToListAsync();

                return Ok(records);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = "Error retrieving data", details = ex.Message });
            }
        }
    }
}
