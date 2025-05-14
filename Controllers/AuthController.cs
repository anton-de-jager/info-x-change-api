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
using System.Text;
using Microsoft.AspNetCore.Authorization;

namespace infoX.api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IEmailService _emailService;
        private readonly IConfiguration _configuration;

        public AuthController(AppDbContext context, IEmailService emailService, IConfiguration configuration)
        {
            _context = context;
            _emailService = emailService;
            _configuration = configuration;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            using var md5 = MD5.Create();
            var inputBytes = Encoding.ASCII.GetBytes(request.Password);
            var hashBytes = md5.ComputeHash(inputBytes);
            var hashedPassword = BitConverter.ToString(hashBytes).Replace("-", "").ToLowerInvariant();

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email && u.Password == hashedPassword);

            if (user == null)
                return Ok(new { result = false });

            var code = new Random().Next(100000, 999999).ToString();
            var expires = DateTime.UtcNow.AddMinutes(10);

            var twoFA = new User2FACode
            {
                UserId = user.ID,
                Code = code,
                ExpiresAt = expires
            };

            _context.User2FACodes.Add(twoFA);
            await _context.SaveChangesAsync();

            await _emailService.Send2FACodeAsync(user.Email, code);

            return Ok(new { result = true });
        }

        [HttpPost("verify")]
        public async Task<IActionResult> VerifyCode([FromBody] Verify2FARequest request)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
            if (user == null) return Unauthorized();

            var twoFA = await _context.User2FACodes
                .Where(c => c.UserId == user.ID && !c.IsUsed && c.ExpiresAt > DateTime.UtcNow)
                .OrderByDescending(c => c.ExpiresAt)
                .FirstOrDefaultAsync();

            if (twoFA == null || twoFA.Code != request.Code)
                return BadRequest("Invalid or expired code");

            twoFA.IsUsed = true;
            await _context.SaveChangesAsync();

            // Generate and return JWT token here
            var jwtSettings = _configuration.GetSection("Jwt");
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"]));

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                new Claim("userId", user.ID.ToString()),
                new Claim("role", user.User_Role?.ToString() ?? "0"),
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

            UserDto userDto = new UserDto();

            userDto.ID = user.ID;
            userDto.Company_Id = user.Company_Id;
            userDto.Title = user.Title;
            userDto.Name = user.Name;
            userDto.Surname = user.Surname;
            userDto.MaidenName = user.MaidenName;
            userDto.KnownName = user.KnownName;
            userDto.Email = user.Email;
            userDto.IdNumber = user.IdNumber;
            userDto.Gender = user.Gender;
            userDto.Nationality = user.Nationality;
            userDto.Created_At = user.Created_At;
            userDto.Updated_At = user.Updated_At;
            userDto.User_Role = user.User_Role;
            userDto.Phone_Number = user.Phone_Number;

            return Ok(new { accessToken = jwt, user = userDto });
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

            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                return Unauthorized();
            }

            // Generate and return JWT token here
            var jwtSettings = _configuration.GetSection("Jwt");
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"]));

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                new Claim("userId", user.ID.ToString()),
                new Claim("role", user.User_Role?.ToString() ?? "0"),
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

            var userDto = new UserDto
            {
                ID = user.ID,
                Company_Id = user.Company_Id,
                Title = user.Title,
                Name = user.Name,
                Surname = user.Surname,
                MaidenName = user.MaidenName,
                KnownName = user.KnownName,
                Email = user.Email,
                IdNumber = user.IdNumber,
                Gender = user.Gender,
                Nationality = user.Nationality,
                Created_At = user.Created_At,
                Updated_At = user.Updated_At,
                User_Role = user.User_Role,
                Phone_Number = user.Phone_Number
            };

            return Ok(new { accessToken = jwt, user = userDto });
        }

    }

}
