using infoX.api.Models;

namespace infoX.api.Services
{
    public interface IWhatsAppService
    {
        Task SendMessageAsync(Company company, string toNumber, string message);
        Task<List<string>> GetMyPhoneNumberIdsAsync(string wabaId, string accessToken);
    }
}
