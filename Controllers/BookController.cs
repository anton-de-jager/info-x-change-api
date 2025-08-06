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
    public class BookController : ControllerBase
    {
        private readonly PegasusConfigurationDbContext _context;

        public BookController(PegasusConfigurationDbContext context)
        {
            _context = context;
        }

        [HttpGet("select")]
        [Authorize]
        public async Task<IActionResult> GetByBookId()
        {
            try
            {
                var userIdClaim = User.FindFirst("userId")?.Value;

                var records = await _context.Book
                    .FromSqlRaw("SELECT * FROM book")
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
        public async Task<IActionResult> InsertBook([FromBody] Book company)
        {
            try
            {
                var userId = Convert.ToInt32(User.FindFirst("userId")?.Value);

                company.CreatedOn = DateTime.UtcNow;
                company.CreatedBy = userId;
                _context.Book.Add(company);
                await _context.SaveChangesAsync();

                return Ok(new { message = "Book inserted successfully", data = company });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = "Error inserting company", details = ex.Message });
            }
        }

        [HttpPut("update")]
        [Authorize]
        public async Task<IActionResult> UpdateBook([FromBody] Book company)
        {
            try
            {
                var existingBook = await _context.Book.FindAsync(company.Id);
                if (existingBook == null)
                {
                    return NotFound(new { error = "Book not found" });
                }

                var userId = Convert.ToInt32(User.FindFirst("userId")?.Value);

                existingBook.Description = company.Description;
                existingBook.DepartmentId = company.DepartmentId;
                existingBook.ChangedOn = DateTime.UtcNow;
                existingBook.ChangedBy = userId;

                _context.Book.Update(existingBook);
                await _context.SaveChangesAsync();

                return Ok(new { message = "Book updated successfully", company = existingBook });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = "Error updating company", details = ex.Message });
            }
        }

        [HttpDelete("delete/{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteBook(int id)
        {
            try
            {
                var company = await _context.Book.FindAsync(id);
                if (company == null)
                {
                    return NotFound(new { error = "Book not found" });
                }

                _context.Book.Remove(company);
                await _context.SaveChangesAsync();

                return Ok(new { message = "Book deleted successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = "Error deleting company", details = ex.Message });
            }
        }
    }
}
