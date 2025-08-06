using infoX.api.Models;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace infoX.api.Services
{
    public class WhatsAppService : IWhatsAppService
    {
        private readonly HttpClient _http;
        private readonly IConfiguration _config;
        private readonly IHttpClientFactory _httpClientFactory;

        public WhatsAppService(HttpClient http, IConfiguration config, IHttpClientFactory httpClientFactory)
        {
            _http = http;
            _config = config;
            _httpClientFactory = httpClientFactory;
            _http.BaseAddress = new Uri(_config["WhatsApp:ApiUrl"]);
            _http.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", _config["WhatsApp:AccessToken"]);
        }

        public async Task SendMessageAsync(Company company, string toNumber, string message)
        {
            //// Check if the phone number has opted in
            //if (!await HasOptedInAsync(toNumber))
            //{
            //    // If not opted in, send an opt-in request or create an opt-in link
            //    var optInLink = GenerateOptInLink(toNumber);
            //    var optInLink2 = GenerateOptInLink(_config["WhatsApp:PhoneNumber"], "I consent to receive messages from ADEI.");
            //    var optInMessage = $"Please opt-in to receive messages from us by clicking this link: {optInLink2}";

            //    await SendOptInRequestAsync(company, toNumber, optInMessage);
            //    throw new InvalidOperationException($"The phone number {toNumber} has not opted in. An opt-in request has been sent.");
            //}

            // Proceed with sending the message
            var phoneNumberId = _config["WhatsApp:PhoneNumberId"];
            var phoneNumbers = await GetMyPhoneNumberIdsAsync(_config["WhatsApp:WabaId"], _config["WhatsApp:AccessToken"]);

            // Generate appsecret_proof
            var appSecret = _config["WhatsApp:AppSecret"];
            var accessToken = _config["WhatsApp:AccessToken"];
            var appSecretProof = GenerateAppSecretProof(accessToken, appSecret);

            // Build the full URI with appsecret_proof
            var phoneId = phoneNumbers[0];
            var requestUri = $"{phoneId}/messages?appsecret_proof={appSecretProof}";

            // Build the payload
            var payload = new
            {
                messaging_product = "whatsapp",
                to = toNumber,
                type = "text",
                text = new { body = message }
            };

            // Serialize and wrap in StringContent
            var json = JsonSerializer.Serialize(payload);
            using var content = new StringContent(json, Encoding.UTF8, "application/json");

            // Post the request
            var resp = await _http.PostAsync(requestUri, content);
            var responseContent = await resp.Content.ReadAsStringAsync();
            Console.WriteLine($"Response Content: {responseContent}");
            Console.WriteLine($"Request URI: {requestUri}");
            Console.WriteLine($"Payload: {json}");
            Console.WriteLine($"Response Status Code: {resp.StatusCode}");
            Console.WriteLine($"Response Content: {await resp.Content.ReadAsStringAsync()}");

            if (!resp.IsSuccessStatusCode)
            {
                var error = await resp.Content.ReadAsStringAsync();
                throw new InvalidOperationException(
                    $"WhatsApp API call failed: {(int)resp.StatusCode} – {error}");
            }
        }

        private async Task<bool> HasOptedInAsync(string toNumber)
        {
            // Example: Check opt-in status from a database or external service
            // Replace with actual implementation
            return await Task.FromResult(false); // Assume the user has not opted in
        }

        private string GenerateOptInLink(string toNumber)
        {
            // Example: Generate a link for the user to opt-in
            var baseUrl = _config["WhatsApp:OptInBaseUrl"];
            return $"{baseUrl}?phone={Uri.EscapeDataString(toNumber)}";
        }

        public string GenerateOptInLink(string businessNumber, string predefinedMessage)
        {
            var encodedMessage = Uri.EscapeDataString(predefinedMessage);
            return $"https://wa.me/{businessNumber}?text={encodedMessage}";
        }

        private async Task SendOptInRequestAsync(Company company, string toNumber, string optInMessage)
        {
            var phoneNumberId = _config["WhatsApp:PhoneNumberId"];
            var phoneNumbers = await GetMyPhoneNumberIdsAsync(_config["WhatsApp:WabaId"], _config["WhatsApp:AccessToken"]);

            var appSecret = _config["WhatsApp:AppSecret"];
            var accessToken = _config["WhatsApp:AccessToken"];
            var appSecretProof = GenerateAppSecretProof(accessToken, appSecret);

            var phoneId = phoneNumbers[0];
            var requestUri = $"{phoneId}/messages?appsecret_proof={appSecretProof}";

            var payload = new
            {
                messaging_product = "whatsapp",
                to = toNumber,
                type = "text",
                text = new { body = optInMessage }
            };

            var json = JsonSerializer.Serialize(payload);
            using var content = new StringContent(json, Encoding.UTF8, "application/json");

            var resp = await _http.PostAsync(requestUri, content);
            var responseContent = await resp.Content.ReadAsStringAsync();
            Console.WriteLine($"Response Content: {responseContent}");

            if (!resp.IsSuccessStatusCode)
            {
                var error = await resp.Content.ReadAsStringAsync();
                throw new InvalidOperationException(
                    $"Failed to send opt-in request: {(int)resp.StatusCode} – {error}");
            }
        }

        private string GenerateAppSecretProof(string accessToken, string appSecret)
        {
            using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(appSecret));
            var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(accessToken));
            return BitConverter.ToString(hash).Replace("-", "").ToLower();
        }

        // New method to retrieve phone number IDs
        public async Task<List<string>> GetMyPhoneNumberIdsAsync(string wabaId, string accessToken)
        {
            try
            {
                // 1) compute the proof
                var appSecret = _config["WhatsApp:AppSecret"]!;        // your Meta App Secret
                var proof = GenerateAppSecretProof(accessToken, appSecret);

                // 2) build URL with both token and proof
                var url = new StringBuilder($"https://graph.facebook.com/v23.0/{wabaId}/phone_numbers");
                url.Append($"?fields=id,display_phone_number");
                url.Append($"&access_token={accessToken}");
                url.Append($"&appsecret_proof={proof}");

                // 3) call Graph API
                using var client = _httpClientFactory.CreateClient();
                var resp = await client.GetFromJsonAsync<GraphPhoneNumbersResponse>(url.ToString())
                           ?? throw new InvalidOperationException("Invalid response");
                return resp.Data.Select(p => p.Id).ToList();
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Failed to retrieve phone number IDs", ex);
            }
        }
    }

    // Supporting classes for the new method
    public class GraphPhoneNumbersResponse
    {
        public List<PhoneNumberEntry> Data { get; set; } = new();
    }

    public class PhoneNumberEntry
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = default!;

        [JsonPropertyName("display_phone_number")]
        public string DisplayPhoneNumber { get; set; } = default!;
    }
}
