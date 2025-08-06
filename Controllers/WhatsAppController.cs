using Microsoft.AspNetCore.Mvc;
using infoX.api.Data;
using infoX.api.Models;
using Microsoft.AspNetCore.SignalR;
using infoX.api.Hubs;
using System.Text.Json.Nodes;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using System.Text.Json;
using System.Security.Cryptography;
using System.Text;
using System.ComponentModel;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Options;

namespace infoX.api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WhatsAppController : ControllerBase
    {
        private readonly PegasusConfigurationDbContext _db;
        private readonly IConfiguration _config;
        private readonly HttpClient _http;

        public WhatsAppController(PegasusConfigurationDbContext db, IConfiguration config, IHttpClientFactory httpFactory)
        {
            _db = db;
            _config = config;
            _http = httpFactory.CreateClient();
        }

        // 1) Enrollment callback: store phoneNumberId
        [HttpPost("enroll")]
        public async Task<IActionResult> Enroll([FromBody] EnrollRequest req)
        {
            var companyId = int.Parse(User.FindFirst("companyId")!.Value);
            var comp = await _db.Company.FindAsync(companyId);
            comp.PhoneNumberId = req.PhoneNumberId;
            await _db.SaveChangesAsync();
            return Ok();
        }

        // 2) Generate opt‑in link
        [HttpGet("optin-link")]
        public async Task<IActionResult> GetOptInLink()
        {
            try
            {
                // 1) Load config + company
                var wabaId = _config["WhatsApp:WabaId"]!;
                var token = _config["WhatsApp:AccessToken"]!;
                var secret = _config["WhatsApp:AppSecret"]!;
                var company = await _db.Company.FindAsync(
                                  int.Parse(User.FindFirst("companyId")!.Value))
                              ?? throw new InvalidOperationException("Company not found");
                var myPhoneId = company.PhoneNumberId
                                ?? throw new InvalidOperationException("No PhoneNumberId set");

                // 2) Compute appsecret_proof
                string proof;
                using (var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(secret)))
                {
                    proof = BitConverter
                              .ToString(hmac.ComputeHash(Encoding.UTF8.GetBytes(token)))
                              .Replace("-", "")
                              .ToLower();
                }

                // 3) Call the Business‑Management phone_numbers edge, asking for optin_link
                var url = new StringBuilder()
                    .Append(_config["WhatsApp:ApiUrl"]!).Append(wabaId).Append("/phone_numbers")
                    .Append("?fields=id,display_phone_number,optin_link")
                    .Append($"&access_token={Uri.EscapeDataString(token)}")
                    .Append($"&appsecret_proof={proof}")
                    .ToString();

                var resp = await _http.GetFromJsonAsync<JsonElement>(url);
                var entries = resp.GetProperty("data").EnumerateArray();
                var match = entries.FirstOrDefault(n => n.GetProperty("id").GetString() == myPhoneId);

                if (match.ValueKind == JsonValueKind.Undefined)
                    return NotFound(new { error = "PhoneNumberId not found on WABA" });

                var display = match.GetProperty("display_phone_number").GetString()!;

                // 3) Strip non‑digits and leading “+”
                var digits = new string(display.Where(char.IsDigit).ToArray());

                // 4) Your custom opt‑in message
                var plainText = "I opt in to receive messages from infoX";
                var encoded = Uri.EscapeDataString(plainText);

                // 5) Build the wa.me link
                var waLink = $"https://wa.me/{digits}?text={encoded}";

                return Ok(new { waLink });

                //var link = match.GetProperty("optin_link").GetString()!;
                //return Ok(new OptInLinkResponse { Url = link });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    error = "Failed to get opt‑in link",
                    details = ex.Message
                });
            }
        }

        // 3.1) List message templates
        [HttpGet("templates")]
        public async Task<IActionResult> GetTemplates()
        {
            try
            {
                var apiUrl = _config["WhatsApp:ApiUrl"]!;
                var wabaId = _config["WhatsApp:WabaId"]!;
                var token = _config["WhatsApp:AccessToken"]!;
                var secret = _config["WhatsApp:AppSecret"]!;

                // Compute appsecret_proof
                string proof;
                using (var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(secret)))
                {
                    proof = BitConverter
                              .ToString(hmac.ComputeHash(Encoding.UTF8.GetBytes(token)))
                              .Replace("-", "")
                              .ToLower();
                }

                // Build URL on the WABA node, requesting only `name`
                var url = new StringBuilder()
                    .Append(apiUrl).Append(wabaId)
                    .Append("/message_templates")
                    .Append("?fields=name")
                    .Append($"&access_token={Uri.EscapeDataString(token)}")
                    .Append($"&appsecret_proof={proof}")
                    .ToString();

                var resp = await _http.GetFromJsonAsync<JsonElement>(url);
                var names = resp
                    .GetProperty("data")
                    .EnumerateArray()
                    .Select(x => x.GetProperty("name").GetString()!)
                    .ToArray();

                return Ok(names);
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    error = "Failed to retrieve templates",
                    details = ex.Message
                });
            }
        }

        [HttpPost("send")]
        public async Task<IActionResult> Send([FromBody] SendRequest req)
        {
            // 1) Load config + company as before…
            var apiUrl = _config["WhatsApp:ApiUrl"]!;
            var company = await _db.Company.FindAsync(int.Parse(User.FindFirst("companyId")!.Value))
                               ?? throw new InvalidOperationException("Company not found");
            var phoneNumberId = company.PhoneNumberId!;
            var token = _config["WhatsApp:AccessToken"]!;
            var secret = _config["WhatsApp:AppSecret"]!;

            // 2) Compute appsecret_proof…
            string proof;
            using (var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(secret)))
            {
                proof = BitConverter.ToString(hmac
                        .ComputeHash(Encoding.UTF8.GetBytes(token)))
                    .Replace("-", "").ToLower();
            }

            // 3) Base URL
            var baseUrl = $"{apiUrl}{phoneNumberId}/messages" +
                          $"?access_token={Uri.EscapeDataString(token)}" +
                          $"&appsecret_proof={proof}";

            // Serializer options
            var jsonOpts = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
            };

            foreach (var row in req.CsvRows)
            {
                // a) Clean number
                var raw = row["phone"].Trim();
                var toNumber = raw.StartsWith("+") ? raw[1..] : raw;

                // b) Build parameters
                var parameters = row
                    .Where(kv => kv.Key != "phone")
                    .Select(kv => new Parameter { Text = kv.Value })
                    .ToArray();

                // c) Build template object dynamically
                var templateObj = new Dictionary<string, object>
                {
                    ["name"] = req.TemplateName,
                    ["language"] = new { code = "en_US" }
                };

                if (parameters.Length > 0)
                {
                    templateObj["components"] = new[] {
                new {
                    type       = "body",
                    parameters = parameters
                }
            };
                }

                if (toNumber.Length > 8)
                {
                    // d) Build root payload
                    var payload = new Dictionary<string, object>
                    {
                        ["messaging_product"] = "whatsapp",
                        ["to"] = toNumber,
                        ["type"] = "template",
                        ["template"] = templateObj
                    };

                    // e) Serialize & log
                    var json = System.Text.Json.JsonSerializer.Serialize(payload, jsonOpts);
                    Console.WriteLine("Outgoing JSON: " + json);

                    // f) Send
                    var content = new StringContent(json, Encoding.UTF8, "application/json");
                    var response = await _http.PostAsync(baseUrl, content);

                    if (!response.IsSuccessStatusCode)
                    {
                        // Grab the real error from Facebook
                        var body = await response.Content.ReadAsStringAsync();
                        return StatusCode((int)response.StatusCode, new
                        {
                            error = "WhatsApp API error",
                            status = response.StatusCode,
                            details = body
                        });
                    }
                }
            }

            return Ok(new { status = "Messages sent" });
        }
    }

    public class EnrollRequest { public string PhoneNumberId { get; set; } }
    public class OptInLinkResponse { public string Url { get; set; } }
    public class SendRequest
    {
        public string TemplateName { get; set; }
        public Dictionary<string, string>[] CsvRows { get; set; }
    }

    public class TemplateMessage
    {
        [JsonPropertyName("messaging_product")]
        public string MessagingProduct { get; set; } = "whatsapp";

        [JsonPropertyName("to")]
        public string To { get; set; }

        [JsonPropertyName("type")]
        public string Type { get; set; } = "template";

        [JsonPropertyName("template")]
        public TemplateDetail Template { get; set; }
    }

    public class TemplateDetail
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("language")]
        public Language Language { get; set; }

        [JsonPropertyName("components")]
        public Component[] Components { get; set; }
    }

    public class Language
    {
        [JsonPropertyName("code")]
        public string Code { get; set; } = "en_US";
    }

    public class Component
    {
        [JsonPropertyName("type")]
        public string Type { get; set; } = "body";

        [JsonPropertyName("parameters")]
        public Parameter[] Parameters { get; set; }
    }

    public class Parameter
    {
        [JsonPropertyName("type")]
        public string Type { get; set; } = "text";

        [JsonPropertyName("text")]
        public string Text { get; set; }
    }
}
