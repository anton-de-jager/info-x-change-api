using infoX.api.Data;
using infoX.api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Data;
using System.Text.Json;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace infoX.api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DynamicController : ControllerBase
    {
        private readonly PegasusConfigurationDbContext _contextConfiguration;
        private readonly PegasusDataWarehouseDbContext _contextDataWarehouse;
        private readonly PegasusDataLakeDbContext _contextDataLake;
        private readonly PegasusDataMartDbContext _contextDataMart;
        private readonly WhatsAppDbContext _contextWhatsApp;

        public DynamicController(
            PegasusConfigurationDbContext contextConfiguration, 
            PegasusDataWarehouseDbContext contextDataWarehouse,
            PegasusDataLakeDbContext contextDataLake,
            PegasusDataMartDbContext contextDataMart,
            WhatsAppDbContext contextWhatsApp
            )
        {
            _contextConfiguration = contextConfiguration;
            _contextDataWarehouse = contextDataWarehouse;
            _contextDataLake = contextDataLake;
            _contextDataMart = contextDataMart;
            _contextWhatsApp = contextWhatsApp;
        }

        [HttpGet("select-all/{tableId}")]
        [Authorize]
        public async Task<IActionResult> GetAll(int tableId)
        {
            try
            {
                var userId = Convert.ToInt32(User.FindFirst("userId")?.Value);

                var table = await _contextConfiguration.ConfigTables.FindAsync(tableId);
                if (table == null)
                {
                    return NotFound(new { error = "Table not found" });
                }

                var sqlQuery = "";

                switch (table.DatabaseName)
                {
                    case "configuration":
                        // Build the SQL query
                        sqlQuery = $"EXEC usp_select_all_{table.Property} @userId";

                        using (var connection = _contextConfiguration.Database.GetDbConnection())
                        {
                            await connection.OpenAsync();

                            using (var command = connection.CreateCommand())
                            {
                                command.CommandText = sqlQuery;
                                command.CommandType = CommandType.Text;

                                // Add the parameter
                                var userIdParam = command.CreateParameter();
                                userIdParam.ParameterName = "@userId";
                                userIdParam.Value = userId;
                                command.Parameters.Add(userIdParam);

                                // Execute the query and read the results
                                using (var reader = await command.ExecuteReaderAsync())
                                {
                                    var results = new List<object>();

                                    while (await reader.ReadAsync())
                                    {
                                        var row = new
                                        {
                                            Id = reader["id"],
                                            Result = reader["result"],
                                            Message = reader["message"]
                                        };
                                        results.Add(row);
                                    }

                                    return Ok(results);
                                }
                            }
                        }
                    case "dataWarehouse":
                        // Build the SQL query
                        sqlQuery = $"EXEC usp_select_all_{table.Property} @userId, @interval";

                        using (var connection = _contextDataWarehouse.Database.GetDbConnection())
                        {
                            await connection.OpenAsync();

                            using (var command = connection.CreateCommand())
                            {
                                command.CommandText = sqlQuery;
                                command.CommandType = CommandType.Text;

                                // Add the parameter
                                var userIdParam = command.CreateParameter();
                                userIdParam.ParameterName = "@userId";
                                userIdParam.Value = userId;
                                command.Parameters.Add(userIdParam);

                                // Add the parameter
                                var intervalParam = command.CreateParameter();
                                intervalParam.ParameterName = "@interval";
                                intervalParam.Value = userId;
                                command.Parameters.Add(intervalParam);

                                // Execute the query and read the results
                                using (var reader = await command.ExecuteReaderAsync())
                                {
                                    var results = new List<object>();

                                    while (await reader.ReadAsync())
                                    {
                                        var row = new
                                        {
                                            Id = reader["id"],
                                            Result = reader["result"],
                                            Message = reader["message"]
                                        };
                                        results.Add(row);
                                    }

                                    return Ok(results);
                                }
                            }
                        }
                    case "dataLake":
                        // Build the SQL query
                        sqlQuery = $"EXEC usp_select_all_{table.Property} @userId, @interval";

                        using (var connection = _contextDataLake.Database.GetDbConnection())
                        {
                            await connection.OpenAsync();

                            using (var command = connection.CreateCommand())
                            {
                                command.CommandText = sqlQuery;
                                command.CommandType = CommandType.Text;

                                // Add the parameter
                                var userIdParam = command.CreateParameter();
                                userIdParam.ParameterName = "@userId";
                                userIdParam.Value = userId;
                                command.Parameters.Add(userIdParam);

                                // Add the parameter
                                var intervalParam = command.CreateParameter();
                                intervalParam.ParameterName = "@interval";
                                intervalParam.Value = userId;
                                command.Parameters.Add(intervalParam);

                                // Execute the query and read the results
                                using (var reader = await command.ExecuteReaderAsync())
                                {
                                    var results = new List<object>();

                                    while (await reader.ReadAsync())
                                    {
                                        var row = new
                                        {
                                            Id = reader["id"],
                                            Result = reader["result"],
                                            Message = reader["message"]
                                        };
                                        results.Add(row);
                                    }

                                    return Ok(results);
                                }
                            }
                        }
                    case "dataMart":
                        // Build the SQL query
                        sqlQuery = $"EXEC usp_InfoX_{table.Property} @userId, @interval";

                        using (var connection = _contextDataMart.Database.GetDbConnection())
                        {
                            await connection.OpenAsync();

                            using (var command = connection.CreateCommand())
                            {
                                command.CommandText = sqlQuery;
                                command.CommandType = CommandType.Text;

                                // Add the parameter
                                var userIdParam = command.CreateParameter();
                                userIdParam.ParameterName = "@userId";
                                userIdParam.Value = userId;
                                command.Parameters.Add(userIdParam);

                                // Add the parameter
                                var intervalParam = command.CreateParameter();
                                intervalParam.ParameterName = "@interval";
                                intervalParam.Value = userId;
                                command.Parameters.Add(intervalParam);

                                // Execute the query and read the results
                                using (var reader = await command.ExecuteReaderAsync())
                                {
                                    var results = new List<object>();

                                    while (await reader.ReadAsync())
                                    {
                                        var row = new
                                        {
                                            Id = reader["id"],
                                            Result = reader["result"],
                                            Message = reader["message"]
                                        };
                                        results.Add(row);
                                    }

                                    return Ok(results);
                                }
                            }
                        }
                    case "whatsapp":
                        // Build the SQL query
                        sqlQuery = $"EXEC usp_select_all_{table.Property} @userId, @interval";

                        using (var connection = _contextWhatsApp.Database.GetDbConnection())
                        {
                            await connection.OpenAsync();

                            using (var command = connection.CreateCommand())
                            {
                                command.CommandText = sqlQuery;
                                command.CommandType = CommandType.Text;

                                // Add the parameter
                                var userIdParam = command.CreateParameter();
                                userIdParam.ParameterName = "@userId";
                                userIdParam.Value = userId;
                                command.Parameters.Add(userIdParam);

                                // Add the parameter
                                var intervalParam = command.CreateParameter();
                                intervalParam.ParameterName = "@interval";
                                intervalParam.Value = userId;
                                command.Parameters.Add(intervalParam);

                                // Execute the query and read the results
                                using (var reader = await command.ExecuteReaderAsync())
                                {
                                    var results = new List<object>();

                                    while (await reader.ReadAsync())
                                    {
                                        var row = new
                                        {
                                            Id = reader["id"],
                                            Result = reader["result"],
                                            Message = reader["message"]
                                        };
                                        results.Add(row);
                                    }

                                    return Ok(results);
                                }
                            }
                        }
                    case "url":
                        return Ok();
                    default:
                        return Ok();
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = "Error retrieving records", details = ex.Message });
            }
        }


        [HttpGet("select-all/{tableId}/{property}/{id}")]
        [Authorize]
        public async Task<IActionResult> GetAll(int tableId, string property, int id)
        {
            try
            {
                var userId = Convert.ToInt32(User.FindFirst("userId")?.Value);

                var table = await _contextConfiguration.ConfigTables.FindAsync(tableId);
                if (table == null)
                {
                    return NotFound(new { error = "Table not found" });
                }

                switch (table.DatabaseName)
                {
                    case "configuration":

                        break;
                    case "dataWarehouse":

                        break;
                    case "whatsapp":

                        break;
                    case "url":

                        break;
                    default:

                        break;
                }

                var result = await _contextConfiguration.Dynamic
                    .FromSqlRaw($"EXEC usp_select_all_{table.Property}_by_{property} @userId = {{0}}, @id = {{1}}", userId, id)
                    .ToListAsync();

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = "Error retrieving records", details = ex.Message });
            }
        }

        [HttpGet("select-lookups/{table}")]
        [Authorize]
        public async Task<IActionResult> GetLookups(string table)
        {
            try
            {
                var userId = Convert.ToInt32(User.FindFirst("userId")?.Value);

                var result = await _contextConfiguration.Dynamic
                    .FromSqlRaw($"EXEC usp_select_all_{table} @userId = {{0}}", userId)
                    .ToListAsync();

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = "Error retrieving records", details = ex.Message });
            }
        }

        [HttpPost("insert/{tableId}")]
        [Authorize]
        public async Task<IActionResult> Insert([FromBody] dynamic payload, int tableId)
        {
            try
            {
                var table = await _contextConfiguration.ConfigTables.FindAsync(tableId);
                if (table == null)
                {
                    return NotFound(new { error = "Table not found" });
                }

                var columns = await _contextConfiguration.ConfigColumns.Where(x => x.ConfigTableId == tableId && x.IncludeInsert == true).OrderBy(x => x.Index).ToListAsync();
                if (columns.Count <= 0)
                {
                    return NotFound(new { error = "Columns not found" });
                }

                var userId = Convert.ToInt32(User.FindFirst("userId")?.Value);
                var currentDate = DateTime.UtcNow;
                var active = true;

                switch (table.DatabaseName)
                {
                    case "configuration":

                        break;
                    case "dataWarehouse":

                        break;
                    case "whatsapp":

                        break;
                    case "url":

                        break;
                    default:

                        break;
                }

                // Build dynamic SQL query and parameters
                var sqlQuery = "EXEC usp_insert_" + table.Property;
                var sqlParameters = new List<object>();

                foreach (var column in columns)
                {
                    if (column.Property == "createdBy" || column.Property == "createdOn" || column.Property == "active")
                    {
                        continue; // Skip createdBy and createdOn
                    }

                    //sqlQuery += $" @{column.Property} = {{{sqlParameters.Count}}},";
                    //sqlParameters.Add(payload.GetType().GetProperty(column.Property)?.GetValue(payload, null) ?? DBNull.Value);

                    object value = null;

                    // Check if payload is a JSON object
                    if (payload is JsonElement jsonElement)
                    {
                        // Access property dynamically from JSON
                        if (jsonElement.TryGetProperty(column.Property, out var propertyValue))
                        {
                            value = propertyValue.ValueKind == JsonValueKind.Null ? DBNull.Value : propertyValue.ToString();
                        }
                    }
                    else if (payload is IDictionary<string, object> dynamicPayload)
                    {
                        // Access property dynamically if payload is a dictionary
                        dynamicPayload.TryGetValue(column.Property, out value);
                    }
                    else
                    {
                        // Use reflection for strongly-typed objects
                        value = payload.GetType().GetProperty(column.Property)?.GetValue(payload, null);
                    }

                    sqlQuery += $" @{column.Property} = {{{sqlParameters.Count}}},";
                    sqlParameters.Add(value ?? DBNull.Value);
                }

                // Add createdBy and createdOn to the query
                sqlQuery += $" @createdBy = {{{sqlParameters.Count}}}, @createdOn = {{{sqlParameters.Count + 1}}}, @active = {{{sqlParameters.Count + 2}}}";
                sqlParameters.Add(userId);
                sqlParameters.Add(currentDate);
                sqlParameters.Add(active);

                // Execute the dynamic query
                var result = await _contextConfiguration.Dynamic
                    .FromSqlRaw(sqlQuery, sqlParameters.ToArray())
                    .ToListAsync();

                return Ok(new { message = "Record inserted successfully", data = result });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = "Error inserting payload", details = ex.Message });
            }
        }

        [HttpPut("update/{tableId}")]
        [Authorize]
        public async Task<IActionResult> Update([FromBody] dynamic payload, int tableId)
        {
            try
            {
                var table = await _contextConfiguration.ConfigTables.FindAsync(tableId);
                if (table == null)
                {
                    return NotFound(new { error = "Table not found" });
                }

                var columns = await _contextConfiguration.ConfigColumns
                    .Where(x => x.ConfigTableId == tableId && x.IncludeUpdate == true)
                    .OrderBy(x => x.Index)
                    .ToListAsync();

                if (columns.Count <= 0)
                {
                    return NotFound(new { error = "Columns not found for update" });
                }

                var userId = Convert.ToInt32(User.FindFirst("userId")?.Value);
                var currentDate = DateTime.UtcNow;

                switch (table.DatabaseName)
                {
                    case "configuration":

                        break;
                    case "dataWarehouse":

                        break;
                    case "whatsapp":

                        break;
                    case "url":

                        break;
                    default:

                        break;
                }

                // Build dynamic SQL query and parameters
                var sqlQuery = "EXEC usp_update_" + table.Property;
                var sqlParameters = new List<object>();

                foreach (var column in columns)
                {
                    if (column.Property == "changedBy" || column.Property == "changedOn")
                    {
                        continue; // Skip changedBy and changedOn
                    }

                    //sqlQuery += $" @{column.Property} = {{{sqlParameters.Count}}},";
                    //sqlParameters.Add(payload.GetType().GetProperty(column.Property)?.GetValue(payload, null) ?? DBNull.Value);

                    object value = null;

                    // Check if payload is a JSON object
                    if (payload is JsonElement jsonElement)
                    {
                        // Access property dynamically from JSON
                        if (jsonElement.TryGetProperty(column.Property, out var propertyValue))
                        {
                            value = propertyValue.ValueKind == JsonValueKind.Null ? DBNull.Value : propertyValue.ToString();
                        }
                    }
                    else if (payload is IDictionary<string, object> dynamicPayload)
                    {
                        // Access property dynamically if payload is a dictionary
                        dynamicPayload.TryGetValue(column.Property, out value);
                    }
                    else
                    {
                        // Use reflection for strongly-typed objects
                        value = payload.GetType().GetProperty(column.Property)?.GetValue(payload, null);
                    }

                    sqlQuery += $" @{column.Property} = {{{sqlParameters.Count}}},";
                    sqlParameters.Add(value ?? DBNull.Value);
                }

                // Add id, changedBy, and changedOn to the query
                sqlQuery += $" @changedBy = {{{sqlParameters.Count}}}, @changedOn = {{{sqlParameters.Count + 1}}}";
                sqlParameters.Add(userId);
                sqlParameters.Add(currentDate);

                // Execute the dynamic query
                var result = await _contextConfiguration.Dynamic
                    .FromSqlRaw(sqlQuery, sqlParameters.ToArray())
                    .ToListAsync();

                return Ok(new { message = "Record updated successfully", data = result });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = "Error updating payload", details = ex.Message });
            }
        }

        [HttpDelete("delete/{tableId}/{id}")]
        [Authorize]
        public async Task<IActionResult> Delete(int tableId, int id)
        {
            try
            {
                var table = await _contextConfiguration.ConfigTables.FindAsync(tableId);
                if (table == null)
                {
                    return NotFound(new { error = "Table not found" });
                }

                var userId = Convert.ToInt32(User.FindFirst("userId")?.Value);
                var currentDate = DateTime.UtcNow;

                switch (table.DatabaseName)
                {
                    case "configuration":

                        break;
                    case "dataWarehouse":

                        break;
                    case "whatsapp":

                        break;
                    case "url":

                        break;
                    default:

                        break;
                }

                // Build dynamic SQL query and parameters
                var sqlQuery = "EXEC usp_delete_" + table.Property;
                var sqlParameters = new List<object>();

                // Add id, changedBy, and changedOn to the query
                sqlQuery += $" @id = {{{sqlParameters.Count}}}, @changedBy = {{{sqlParameters.Count + 1}}}, @changedOn = {{{sqlParameters.Count + 2}}}";
                sqlParameters.Add(id);
                sqlParameters.Add(userId);
                sqlParameters.Add(currentDate);

                // Execute the dynamic query
                var result = await _contextConfiguration.Dynamic
                    .FromSqlRaw(sqlQuery, sqlParameters.ToArray())
                    .ToListAsync();

                return Ok(new { message = "Record deleted successfully", data = result });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = "Error deleting payload", details = ex.Message });
            }
        }
    }
}
