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
    public class PaymentsController : ControllerBase
    {
        private readonly PegasusDataWarehouseDbContext _context;

        public PaymentsController(PegasusDataWarehouseDbContext context)
        {
            _context = context;
        }

        [HttpGet("ozow")]
        [Authorize]
        public async Task<IActionResult> GetOzow()
        {
            try
            {
                var companyId = User.FindFirst("companyId")?.Value;

                var records = await _context.Payment
                    .FromSqlRaw("EXEC [usp_payments_ozow] " + companyId)
                    .ToListAsync();

                return Ok(records);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = "Error retrieving data", details = ex.Message });
            }
        }

        [HttpGet("payfast")]
        [Authorize]
        public async Task<IActionResult> GetPayfast()
        {
            try
            {
                var companyId = User.FindFirst("companyId")?.Value;

                var records = await _context.Payment
                    .FromSqlRaw("EXEC [usp_payments_payfast] " + companyId)
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
