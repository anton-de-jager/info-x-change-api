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
        private readonly PegasusDataWarehouseDbContext _context;

        public DebitOrderArrangementsController(PegasusDataWarehouseDbContext context)
        {
            _context = context;
        }

        [HttpGet("select")]
        [Authorize]
        public async Task<IActionResult> Get()
        {
            try
            {
                var companyId = User.FindFirst("companyId")?.Value;

                var records = await _context.DebitOrderArrangements
                    .FromSqlRaw("SELECT * FROM debitOrderArrangements WHERE CompanyID = " + companyId)
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
