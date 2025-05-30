﻿using infoX.api.Data;
using infoX.api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace infoX.api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ConfigController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ConfigController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost("table/{id}")]
        [Authorize]
        public async Task<IActionResult> GetConfigTable(Guid id)
        {
            var table = await _context.ConfigTables
                .Where(c => c.Id == id)
                .FirstOrDefaultAsync();

            return Ok(table);
        }

        [HttpPost("columns/{id}")]
        [Authorize]
        public async Task<IActionResult> GetConfigColumns(Guid id)
        {
            var columns = await _context.ConfigColumns
                .Where(c => c.ConfigTableId == id)
                .OrderBy(c => c.Index)
                .ToListAsync();

            return Ok(columns);
        }
    }
}
