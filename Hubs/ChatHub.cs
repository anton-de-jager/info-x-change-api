using Microsoft.AspNetCore.SignalR;
using infoX.api.Data;
using infoX.api.Models;
using infoX.api.Services;

namespace infoX.api.Hubs
{
    public class ChatHub : Hub
    {
        private readonly PegasusConfigurationDbContext _db;
        private readonly IWhatsAppService _wa;

        public ChatHub(PegasusConfigurationDbContext db, IWhatsAppService wa)
        {
            _db = db;
            _wa = wa;
        }

        public async Task SendMessage(int companyId, string toNumber, string message)
        {
            try
            {
                //var phoneNumberId = _wa.GetMyPhoneNumberIdsAsync(companyId.ToString(), null);
                
                var company = await _db.Company.FindAsync(companyId);
                if (company == null) throw new HubException("Company not found.");

                // Save outbound
                var msg = new Message
                {
                    CompanyId = companyId,
                    Direction = Direction.Outbound,
                    Content = message,
                    Timestamp = DateTime.UtcNow
                };
                _db.Messages.Add(msg);
                await _db.SaveChangesAsync();

                // Send via WhatsApp API
                await _wa.SendMessageAsync(company, toNumber, message);

                // Broadcast to caller
                await Clients.Caller.SendAsync("ReceiveMessage", msg);
            }
            catch (Exception ex)
            {
                string errorMessage = ex.Message ?? "An error occurred while sending the message.";
            }
        }
    }
}
