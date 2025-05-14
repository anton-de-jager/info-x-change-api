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
        private readonly AppDbContext _context;

        public DashboardWhatsAppController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost("dashboard/{companyId}/{interval}")]
        [Authorize]
        public async Task<IActionResult> GetByCompanyId(int companyId, string interval)
        {
            try
            {
                var records = await _context.dashboardWhatsApp
                    .FromSqlRaw("EXEC usp_dashboard_whatsApp " + companyId.ToString() + ", '" + interval + "';")
                    .ToListAsync();

                return Ok(records);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = "Error retrieving data", details = ex.Message });
            }
        }

        [HttpPost("messages/{companyId}/{interval}")]
        [Authorize]
        public async Task<IActionResult> GetMessages(int companyId, string interval)
        {
            try
            {
                var records = await _context.VwMessages
                    .FromSqlRaw("EXEC usp_dashboard_messages " + companyId.ToString() + ", '" + interval + "';")
                    .ToListAsync();

                return Ok(records);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = "Error retrieving data", details = ex.Message });
            }
        }

        [HttpPost("users/{companyId}/{interval}")]
        [Authorize]
        public async Task<IActionResult> GetUsers(int companyId, string interval)
        {
            try
            {
                var records = await _context.VwMessages
                    .FromSqlRaw("EXEC usp_dashboard_users " + companyId.ToString() + ", '" + interval + "';")
                    .ToListAsync();

                return Ok(records);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = "Error retrieving data", details = ex.Message });
            }
        }

        [HttpPost("arrangements/{companyId}/{interval}")]
        [Authorize]
        public async Task<IActionResult> GetArrangements(int companyId, string interval)
        {
            try
            {
                var records = await _context.VwMessages
                    .FromSqlRaw("EXEC usp_dashboard_arrangements " + companyId.ToString() + ", '" + interval + "';")
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
