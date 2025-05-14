namespace infoX.api.Services
{
    public interface IEmailService
    {
        Task Send2FACodeAsync(string toEmail, string code);
    }
}
