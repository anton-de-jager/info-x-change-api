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
        private readonly AppDbContext _context;

        public PaymentsController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost("ozow/{companyId}")]
        [Authorize]
        public async Task<IActionResult> GetOzow(int companyId)
        {
            try
            {
                var records = await _context.Payment
                    .FromSqlRaw("EXEC [usp_payments_ozow] " + companyId.ToString())
                    .ToListAsync();

                return Ok(records);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = "Error retrieving data", details = ex.Message });
            }
        }

        [HttpPost("payfast/{companyId}")]
        [Authorize]
        public async Task<IActionResult> GetPayfast(int companyId)
        {
            try
            {
                var records = await _context.Payment
                    .FromSqlRaw("EXEC [usp_payments_payfast] " + companyId.ToString())
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
