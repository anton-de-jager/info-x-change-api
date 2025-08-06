using infoX.api.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Data;
using System.Threading.Tasks;

namespace infoX.api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TableController : ControllerBase
    {
        private readonly PegasusDataWarehouseDbContext _context;

        public TableController(PegasusDataWarehouseDbContext context)
        {
            _context = context;
        }

        [HttpGet("{tableName}/{companyId}")]
        [Authorize]
        public async Task<IActionResult> GetByCompanyId(string tableName, string companyId)
        {
            var sql = "SELECT * FROM [" + tableName + "] WHERE CompanyId = '2'";

            try
            {
                var result = await _context.DynamicResults.FromSqlRaw(sql).ToListAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = "Invalid table or query.", details = ex.Message });
            }
        }
    }
}
