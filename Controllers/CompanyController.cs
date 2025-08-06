using infoX.api.Data;
using infoX.api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Data;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace infoX.api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CompanyController : ControllerBase
    {
        private readonly PegasusConfigurationDbContext _context;

        public CompanyController(PegasusConfigurationDbContext context)
        {
            _context = context;
        }

        [HttpGet("select/{id}")]
        [Authorize]
        public async Task<IActionResult> GetByCompanyId(int id)
        {
            try
            {
                var result = await _context.Dynamic
                    .FromSqlRaw("EXEC usp_select_company @id = {0}", id)
                    .ToListAsync();

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = "Error retrieving payload", details = ex.Message });
            }
        }

        [HttpGet("select-all")]
        [Authorize]
        public async Task<IActionResult> GetAllCompanies()
        {
            try
            {
                var result = await _context.Dynamic
                    .FromSqlRaw("EXEC usp_select_all_company")
                    .ToListAsync();

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = "Error retrieving companies", details = ex.Message });
            }
        }

        [HttpPost("insert")]
        [Authorize]
        public async Task<IActionResult> InsertCompany([FromBody] dynamic payload)
        {
            try
            {
                var userId = Convert.ToInt32(User.FindFirst("userId")?.Value);

                string description = payload.description;
                string clientSupportTelephone = payload.clientSupportTelephone;
                string clientSupportEmail = payload.clientSupportEmail;
                string registrationNo = payload.registrationNo;
                string vat = payload.vat;
                string physicalAddress = payload.physicalAddress;
                bool active = payload.active ?? true;
                DateTime createdOn = DateTime.UtcNow;
                int createdBy = userId;

                var result = await _context.Dynamic
                    .FromSqlRaw("EXEC usp_insert_company @description = {0}, @clientSupportTelephone = {1}, @clientSupportEmail = {2}, @registrationNo = {3}, @vat = {4}, @physicalAddress = {5}, @active = {6}, @createdOn = {7}, @createdBy = {8}, @changedOn = {9}, @changedBy = {10}",
                        description, clientSupportTelephone, clientSupportEmail, registrationNo, vat, physicalAddress, active, createdOn, createdBy, null, null)
                    .ToListAsync();

                return Ok(new { message = "Company inserted successfully", data = result });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = "Error inserting payload", details = ex.Message });
            }
        }

        [HttpPut("update")]
        [Authorize]
        public async Task<IActionResult> UpdateCompany([FromBody] dynamic payload)
        {
            try
            {
                var userId = Convert.ToInt32(User.FindFirst("userId")?.Value);

                int id = payload.Id;
                string description = payload.description;
                string clientSupportTelephone = payload.clientSupportTelephone;
                string clientSupportEmail = payload.clientSupportEmail;
                string registrationNo = payload.registrationNo;
                string vat = payload.vat;
                string physicalAddress = payload.physicalAddress;
                bool active = payload.active ?? true;
                DateTime createdOn = payload.createdOn;
                int createdBy = payload.createdBy;
                DateTime changedOn = DateTime.UtcNow;
                int changedBy = userId;

                var result = await _context.Dynamic
                    .FromSqlRaw("EXEC usp_update_company @id = {0}, @description = {1}, @clientSupportTelephone = {2}, @clientSupportEmail = {3}, @registrationNo = {4}, @vat = {5}, @physicalAddress = {6}, @active = {7}, @createdOn = {8}, @createdBy = {9}, @changedOn = {10}, @changedBy = {11}",
                        id, description, clientSupportTelephone, clientSupportEmail, registrationNo, vat, physicalAddress, active, createdOn, createdBy, changedOn, changedBy)
                    .ToListAsync();

                return Ok(new { message = "Company updated successfully", data = result });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = "Error updating payload", details = ex.Message });
            }
        }

        [HttpDelete("delete/{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteCompany(int id)
        {
            try
            {
                var userId = Convert.ToInt32(User.FindFirst("userId")?.Value);

                var changedOn = DateTime.UtcNow;
                var changedBy = userId;

                var result = await _context.Dynamic
                    .FromSqlRaw("EXEC usp_delete_company @id = {0}, @changedOn = {1}, @changedBy = {2}", id, changedOn, changedBy)
                    .ToListAsync();

                return Ok(new { message = "Company deleted successfully", data = result });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = "Error deleting payload", details = ex.Message });
            }
        }
    }
}
