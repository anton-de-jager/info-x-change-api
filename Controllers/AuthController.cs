using infoX.api.Data;
using infoX.api.Models;
using infoX.api.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.Extensions.Configuration;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Authorization;
using System.ComponentModel.Design;
using System.Net.Http;
using System.Text.Json;
using static System.Net.WebRequestMethods;
using System.Text.Json.Serialization;
using System.IO;
using Azure.Core;
using static Org.BouncyCastle.Crypto.Engines.SM2Engine;
using System.Net.Http.Json;

namespace infoX.api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly PegasusConfigurationDbContext _context;
        private readonly IEmailService _emailService;
        private readonly IConfiguration _configuration;

        public AuthController(PegasusConfigurationDbContext context, IEmailService emailService, IConfiguration configuration)
        {
            _context = context;
            _emailService = emailService;
            _configuration = configuration;
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            string resultString = "Email: " + request.Email;
            try
            {
                resultString += ", Password: " + request.Password;
                using var md5 = MD5.Create();
                resultString += ", md5";
                var inputBytes = Encoding.ASCII.GetBytes(request.Password);
                resultString += ", inputBytes: " + inputBytes.Length.ToString();
                var hashBytes = md5.ComputeHash(inputBytes);
                resultString += ", hashBytes: " + hashBytes.Length.ToString();
                var hashedPassword = BitConverter.ToString(hashBytes).Replace("-", "").ToLowerInvariant();
                resultString += ", hashedPassword: " + hashedPassword.Length.ToString();

                var user = await _context.User.FirstOrDefaultAsync(u => u.Email == request.Email && u.Password == hashedPassword);
                if (user == null)
                {
                    resultString += ", user not found";
                    //await _emailService.Send2FACodeAsync("anton@madproducts.co.za", resultString);
                    return Ok(new { result = false });
                }

                resultString += ", user: " + user.Name;

                var code = new Random().Next(100000, 999999).ToString();
                var expires = DateTime.UtcNow.AddMinutes(10);

                var twoFA = new User2FACode
                {
                    UserId = user.Id,
                    Code = code,
                    ExpiresAt = expires
                };

                _context.User2FACodes.Add(twoFA);
                await _context.SaveChangesAsync();
                resultString += ", DONE";

                await _emailService.Send2FACodeAsync(user.Email, code);
                resultString += ", Send2FACodeAsync";

                //await _emailService.Send2FACodeAsync("anton@madproducts.co.za", resultString);
                return Ok(new { result = true, code = code });
            }
            catch (Exception ex)
            {
                resultString += ", Error: " + ex.Message;
                await _emailService.Send2FACodeAsync("anton@madproducts.co.za", resultString);
                return StatusCode(StatusCodes.Status500InternalServerError, resultString);
            }
        }

        [HttpPost("verify")]
        [AllowAnonymous]
        public async Task<IActionResult> VerifyCode([FromBody] Verify2FARequest request)
        {
            var user = await _context.User.FirstOrDefaultAsync(u => u.Email == request.Email);
            if (user == null) return Unauthorized();

            var twoFA = await _context.User2FACodes
                .Where(c => c.UserId == user.Id && !c.IsUsed && c.ExpiresAt > DateTime.UtcNow)
                .OrderByDescending(c => c.ExpiresAt)
                .FirstOrDefaultAsync();

            if (twoFA == null || twoFA.Code != request.Code)
                return BadRequest("Invalid or expired code");

            twoFA.IsUsed = true;
            await _context.SaveChangesAsync();

            // Generate and return JWT token here
            var jwtSettings = _configuration.GetSection("Jwt");
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"]));

            var vwUser = await _context.VwUser.FirstOrDefaultAsync(u => u.Email == request.Email);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                new Claim("userId", user.Id.ToString()),
                new Claim("roleId", user.RoleId?.ToString() ?? "0"),
                new Claim("companyId", vwUser.CompanyId?.ToString() ?? "0"),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };


            var token = new JwtSecurityToken(
                issuer: jwtSettings["Issuer"],
                audience: jwtSettings["Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(Convert.ToDouble(jwtSettings["ExpiresInMinutes"])),
                signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
            );

            var jwt = new JwtSecurityTokenHandler().WriteToken(token);

            return Ok(new { accessToken = jwt, user = vwUser });
        }

        [HttpPost("sign-in-with-token")]
        [Authorize]
        public async Task<IActionResult> GetCurrentUser()
        {
            var userIdClaim = User.FindFirst("userId")?.Value;

            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
            {
                return Unauthorized();
            }

            var user = await _context.User.FindAsync(userId);
            if (user == null)
            {
                return Unauthorized();
            }

            var vwUser = await _context.VwUser.FirstOrDefaultAsync(u => u.Id == user.Id);

            // Generate and return JWT token here
            var jwtSettings = _configuration.GetSection("Jwt");
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"]));

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                new Claim("userId", user.Id.ToString()),
                new Claim("roleId", user.RoleId?.ToString() ?? "0"),
                new Claim("companyId", vwUser.CompanyId?.ToString() ?? "0"),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var token = new JwtSecurityToken(
                issuer: jwtSettings["Issuer"],
                audience: jwtSettings["Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(Convert.ToDouble(jwtSettings["ExpiresInMinutes"])),
                signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
            );

            var jwt = new JwtSecurityTokenHandler().WriteToken(token);

            return Ok(new { accessToken = jwt, user = vwUser });
        }

        [HttpPut("update-password")]
        [Authorize]
        public async Task<IActionResult> UpdatePassword([FromBody] UpdatePasswordRequest request)
        {
            try
            {
                var userIdClaim = User.FindFirst("userId")?.Value;
                if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
                {
                    return Unauthorized();
                }

                var user = await _context.User.FindAsync(userId);
                if (user == null)
                {
                    return NotFound(new { error = "User not found" });
                }

                using var md5 = MD5.Create();
                var inputBytes = Encoding.ASCII.GetBytes(request.NewPassword);
                var hashBytes = md5.ComputeHash(inputBytes);
                var hashedPassword = BitConverter.ToString(hashBytes).Replace("-", "").ToLowerInvariant();

                user.Password = hashedPassword;
                user.ChangedOn = DateTime.UtcNow;
                user.ChangedBy = userId;

                _context.User.Update(user);
                await _context.SaveChangesAsync();

                return Ok(new { message = "Password updated successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { error = "Error updating password", details = ex.Message });
            }
        }

        [HttpPut("update-user")]
        [Authorize]
        public async Task<IActionResult> UpdateUser([FromBody] UpdateUserRequest request)
        {
            try
            {
                var userIdClaim = User.FindFirst("userId")?.Value;
                if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
                {
                    return Unauthorized();
                }

                var user = await _context.User.FindAsync(userId);
                if (user == null)
                {
                    return NotFound(new { error = "User not found" });
                }

                user.CompanyId = request.CompanyId;
                user.RoleId = request.RoleId;
                user.Title = request.Title;
                user.Name = request.Name;
                user.Surname = request.Surname;
                user.KnownName = request.KnownName;
                user.MaidenName = request.MaidenName;
                user.Phone = request.Phone;
                user.Email = request.Email;
                user.IdNumber = request.IdNumber;
                user.Gender = request.Gender;
                user.Nationality = request.Nationality;
                user.Active = request.Active;
                user.ChangedOn = DateTime.UtcNow;
                user.ChangedBy = userId;

                _context.User.Update(user);
                await _context.SaveChangesAsync();

                return Ok(new { message = "User updated successfully", user });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { error = "Error updating user", details = ex.Message });
            }
        }
    }
}
