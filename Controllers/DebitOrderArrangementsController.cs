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
    public class DebitOrderArrangementsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public DebitOrderArrangementsController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost("{companyId}")]
        [Authorize]
        public async Task<IActionResult> GetByCompanyId(int companyId)
        {
            try
            {
                var records = await _context.DebitOrderArrangements
                    .FromSqlRaw("SELECT * FROM debitOrderArrangements WHERE CompanyID = " + companyId.ToString())
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
