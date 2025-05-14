using System.Net.Mail;
using System.Net;

namespace infoX.api.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _config;

        public EmailService(IConfiguration config)
        {
            _config = config;
        }

        public async Task Send2FACodeAsync(string toEmail, string code)
        {
            SendEmailTemplate(toEmail, code);

            //var client = new SmtpClient(_config["Email:Smtp"], int.Parse(_config["Email:Port"]))
            //{
            //    Credentials = new NetworkCredential(_config["Email:Username"], _config["Email:Password"]),
            //    //EnableSsl = true
            //};

            //var message = new MailMessage
            //{
            //    From = new MailAddress(_config["Email:From"]),
            //    Subject = "Your 2FA Code",
            //    IsBodyHtml = true,
            //    Body = $"<html><body><h2>Here is your login code:</h2><p style='font-size:24px;font-weight:bold'>{code}</p></body></html>"
            //};
            //message.To.Add(toEmail);

            //await client.SendMailAsync(message);
        }

        private string SendEmailTemplate(string email, string code)
        {
            try
            {
                MailMessage mail = new MailMessage();
                SmtpClient smtpClient = new SmtpClient();
                mail.From = new MailAddress("login@infox.co.za", "InfoXchange");
                mail.To.Add(email);
                //mail.Bcc.Add("anton@madproducts.co.za");
                mail.Subject = "InfoXchange - Authentication Code";
                mail.IsBodyHtml = true;
                //mail.Body = $"<html><body><h2>Here is your authentication code:</h2><p style='font-size:24px;font-weight:bold'>{code}</p></body></html>";
                mail.AlternateViews.Add(Mail_Body(code, "2fa"));
                smtpClient.Port = 587;
                smtpClient.Host = "mail.infox.co.za";
                //smtpClient.EnableSsl = true;
                smtpClient.UseDefaultCredentials = false;
                smtpClient.Credentials = new NetworkCredential("login@infox.co.za", "JSFJ6m8Vs2JTUFh");
                smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
                smtpClient.Send(mail);

                return "OK";
            }
            catch (Exception ex)
            {
                return "ERROR: " + ex.Message;
            }
        }

        private AlternateView Mail_Body(string code, string template)
        {
            var path = Path.Combine(System.IO.Directory.GetCurrentDirectory(), "EmailTemplates");
            var file = Path.Combine(path, template + ".html");

            var imagePath = Path.Combine(path, "images");
            var image = Path.Combine(imagePath, "logo-text.png");

            string html = System.IO.File.ReadAllText(file).Replace("__code__", code);

            LinkedResource Img = new LinkedResource(image, System.Net.Mime.MediaTypeNames.Image.Jpeg);
            Img.ContentId = "MyImage";
            AlternateView AV = AlternateView.CreateAlternateViewFromString(html, null, System.Net.Mime.MediaTypeNames.Text.Html);
            AV.LinkedResources.Add(Img);

            return AV;
        }
    }

}
