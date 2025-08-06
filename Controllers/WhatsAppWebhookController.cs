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
    public class WhatsAppWebhookController : ControllerBase
    {
        private readonly PegasusConfigurationDbContext _db;
        private readonly IHubContext<ChatHub> _hub;
        private readonly IConfiguration _config;

        public WhatsAppWebhookController(PegasusConfigurationDbContext db, IHubContext<ChatHub> hub, IConfiguration config)
        {
            _db = db;
            _hub = hub;
            _config = config;
        }

        // Validation handshake
        [HttpGet]
        public IActionResult Get([FromQuery] string hub_mode, [FromQuery] string hub_challenge, [FromQuery] string hub_verify_token)
        {
            return hub_verify_token == "<YOUR_VERIFY_TOKEN>"
                ? Ok(hub_challenge)
                : Forbid();
        }

        // Inbound
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] JsonObject payload)
        {
            // parse incoming
            var entry = payload["entry"]?[0]?["changes"]?[0]?["value"];
            var from = entry?["messages"]?[0]?["from"]?.ToString();
            var text = entry?["messages"]?[0]?["text"]?["body"]?.ToString();

            // find company by phoneNumberId & WABA
            string phoneNumId = entry?["metadata"]?["phone_number_id"]?.ToString();
            var company = _db.Company.FirstOrDefault(u => u.PhoneNumberId == phoneNumId);
            if (company == null) return Ok();

            // Save inbound
            var msg = new Message
            {
                CompanyId = company.Id,
                Direction = Direction.Inbound,
                Content = text,
                Timestamp = DateTime.UtcNow
            };
            _db.Messages.Add(msg);
            await _db.SaveChangesAsync();

            // Push to connected clients
            var userId = Convert.ToInt32(User.FindFirst("userId")?.Value);
            await _hub.Clients.User(userId.ToString()).SendAsync("ReceiveMessage", msg);

            return Ok();
        }

        [HttpPost("exchange-meta-token")]
        public async Task<IActionResult> ExchangeMetaToken([FromBody] ExchangeTokenRequest request)
        {
            try
            {
                // Exchange authorization code for access token
                var accessToken = await GetPermanentAccessToken(request.Code);

                return Ok(new { accessToken });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        private async Task<string> GetPermanentAccessToken(string code)
        {
            var httpClient = new HttpClient();

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
            var longLivedData = JsonConvert.DeserializeObject<dynamic>(longLivedJson);

            return (string)longLivedData.access_token;
        }
    }

    public class ExchangeTokenRequest
    {
        public string Code { get; set; }
    }
}
