using Microsoft.AspNetCore.Mvc;
using infoX.api.Data;
using infoX.api.Models;
using Microsoft.AspNetCore.SignalR;
using infoX.api.Hubs;
using System.Text.Json.Nodes;
using System.Text.RegularExpressions;
using Newtonsoft.Json;

namespace infoX.api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MetaController : ControllerBase
    {
        private readonly PegasusConfigurationDbContext _db;
        private readonly IConfiguration _config;

        public MetaController(PegasusConfigurationDbContext db, IConfiguration config)
        {
            _db = db;
            _config = config;
        }

        [HttpPost("exchange-meta-token")]
        public async Task<IActionResult> ExchangeMetaToken([FromBody] ExchangeTokenRequest request)
        {
            if (string.IsNullOrEmpty(request.Code))
            {
                return BadRequest(new { error = "Authorization code is required." });
            }

            try
            {
                var accessToken = await GetPermanentAccessToken(request.Code);
                return Ok(new { accessToken });
            }
            catch (Exception ex)
            {
                // Log the error for debugging
                Console.WriteLine($"Error: {ex.Message}");
                return BadRequest(new { error = ex.Message });
            }
        }


        private async Task<string> GetPermanentAccessToken(string code)
        {
            var httpClient = new HttpClient();
            var companyId = Convert.ToInt32(User.FindFirst("companyId")?.Value);
            var company = _db.Company.FirstOrDefault(c => c.Id == companyId);

            if (company == null)
            {
                throw new Exception("Company not found for the authenticated user.");
            }

            // Step 1: Exchange code for short-lived access token
            var tokenResponse = await httpClient.PostAsync(
                "https://graph.facebook.com/v23.0/oauth/access_token",
                new FormUrlEncodedContent(new Dictionary<string, string>
                {
            { "client_id", _config["WhatsApp:ClientId"] },
            { "client_secret", _config["WhatsApp:AppSecret"] },
            { "redirect_uri", _config["WhatsApp:RedirectUri"] },
            { "code", code }
                })
            );

            var tokenJson = await tokenResponse.Content.ReadAsStringAsync();
            if (!tokenResponse.IsSuccessStatusCode)
            {
                throw new Exception($"Error exchanging code for short-lived token: {tokenJson}");
            }

            var tokenData = JsonConvert.DeserializeObject<dynamic>(tokenJson);
            var shortLivedToken = (string)tokenData.access_token;

            // Step 2: Exchange short-lived token for long-lived token
            var longLivedResponse = await httpClient.GetAsync(
                $"https://graph.facebook.com/v23.0/oauth/access_token?" +
                $"grant_type=fb_exchange_token&" +
                $"client_id={_config["WhatsApp:ClientId"]}&" +
                $"client_secret={_config["WhatsApp:AppSecret"]}&" +
                $"fb_exchange_token={shortLivedToken}"
            );

            var longLivedJson = await longLivedResponse.Content.ReadAsStringAsync();
            if (!longLivedResponse.IsSuccessStatusCode)
            {
                throw new Exception($"Error exchanging short-lived token for long-lived token: {longLivedJson}");
            }

            var longLivedData = JsonConvert.DeserializeObject<dynamic>(longLivedJson);
            return (string)longLivedData.access_token;
        }

    }
}
public class ExchangeTokenRequest
{
    public string? Code { get; set; }
}
