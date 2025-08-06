//using System.Net.Mail;
//using System.Net;
//using System.Text;
//using MailKit.Security;
//using MailKit;
using MimeKit;
using MailKit.Net.Smtp;
using MailKit.Security;
using MailKit;

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
            //SendEmailTemplate(toEmail, code);
            SendWithMailKit(toEmail, code);

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

        public void SendWithMailKit(string toEmail, string code)
        {
            try
            {
                var message = new MimeMessage();
                message.From.Add(new MailboxAddress("InfoXchange", "login@infox.co.za"));
                message.To.Add(MailboxAddress.Parse(toEmail));
                message.Subject = "InfoXchange – Authentication Code";

                var path = Path.Combine(System.IO.Directory.GetCurrentDirectory(), "EmailTemplates");
                var file = Path.Combine(path, "2fa.html");

                var imagePath = Path.Combine(path, "images");
                var image = Path.Combine(imagePath, "logo-text.png");

                string html = System.IO.File.ReadAllText(file).Replace("__code__", code);

                // Create the BodyBuilder to handle the HTML and embedded image
                var bodyBuilder = new BodyBuilder
                {
                    HtmlBody = html
                };

                // Add the image as an attachment and set its ContentId
                var imageAttachment = bodyBuilder.LinkedResources.Add(image);
                imageAttachment.ContentId = "MyImage";

                // Set the body of the message
                message.Body = bodyBuilder.ToMessageBody();


                //// For now, just plain test body:
                //message.Body = new TextPart("html")
                //{
                //    Text = $"<!DOCTYPE html>\r\n\r\n<html lang=\\\"en\\\" xmlns:o=\\\"urn:schemas-microsoft-com:office:office\\\" xmlns:v=\\\"urn:schemas-microsoft-com:vml\\\">\r\n<head>\r\n<title></title>\r\n<meta content=\\\"text/html; charset=utf-8\\\" http-equiv=\\\"Content-Type\\\"/>\r\n<meta content=\\\"width=device-width, initial-scale=1.0\\\" name=\\\"viewport\\\"/>\r\n<!--[if mso]><xml><o:OfficeDocumentSettings><o:PixelsPerInch>96</o:PixelsPerInch><o:AllowPNG/></o:OfficeDocumentSettings></xml><![endif]-->\r\n<style>\r\n\t\t* {{\r\n\t\t\tbox-sizing: border-box;\r\n\t\t}}\r\n\r\n\t\tbody {{\r\n\t\t\tmargin: 0;\r\n\t\t\tpadding: 0;\r\n\t\t}}\r\n\r\n\t\ta[x-apple-data-detectors] {{\r\n\t\t\tcolor: inherit !important;\r\n\t\t\ttext-decoration: inherit !important;\r\n\t\t}}\r\n\r\n\t\t#MessageViewBody a {{\r\n\t\t\tcolor: inherit;\r\n\t\t\ttext-decoration: none;\r\n\t\t}}\r\n\r\n\t\tp {{\r\n\t\t\tline-height: inherit\r\n\t\t}}\r\n\r\n\t\t.desktop_hide,\r\n\t\t.desktop_hide table {{\r\n\t\t\tmso-hide: all;\r\n\t\t\tdisplay: none;\r\n\t\t\tmax-height: 0px;\r\n\t\t\toverflow: hidden;\r\n\t\t}}\r\n\r\n\t\t@media (max-width:520px) {{\r\n\t\t\t.desktop_hide table.icons-inner {{\r\n\t\t\t\tdisplay: inline-block !important;\r\n\t\t\t}}\r\n\r\n\t\t\t.icons-inner {{\r\n\t\t\t\ttext-align: center;\r\n\t\t\t}}\r\n\r\n\t\t\t.icons-inner td {{\r\n\t\t\t\tmargin: 0 auto;\r\n\t\t\t}}\r\n\r\n\t\t\t.row-content {{\r\n\t\t\t\twidth: 100% !important;\r\n\t\t\t}}\r\n\r\n\t\t\t.mobile_hide {{\r\n\t\t\t\tdisplay: none;\r\n\t\t\t}}\r\n\r\n\t\t\t.stack .column {{\r\n\t\t\t\twidth: 100%;\r\n\t\t\t\tdisplay: block;\r\n\t\t\t}}\r\n\r\n\t\t\t.mobile_hide {{\r\n\t\t\t\tmin-height: 0;\r\n\t\t\t\tmax-height: 0;\r\n\t\t\t\tmax-width: 0;\r\n\t\t\t\toverflow: hidden;\r\n\t\t\t\tfont-size: 0px;\r\n\t\t\t}}\r\n\r\n\t\t\t.desktop_hide,\r\n\t\t\t.desktop_hide table {{\r\n\t\t\t\tdisplay: table !important;\r\n\t\t\t\tmax-height: none !important;\r\n\t\t\t}}\r\n\t\t}}\r\n\t</style>\r\n</head>\r\n<body style=\\\"background-color: #FFFFFF; margin: 0; padding: 0; -webkit-text-size-adjust: none; text-size-adjust: none;\\\">\r\n<table border=\\\"0\\\" cellpadding=\\\"0\\\" cellspacing=\\\"0\\\" class=\\\"nl-container\\\" role=\\\"presentation\\\" style=\\\"mso-table-lspace: 0pt; mso-table-rspace: 0pt; background-color: #FFFFFF;\\\" width=\\\"100%\\\">\r\n<tbody>\r\n<tr>\r\n<td>\r\n<table align=\\\"center\\\" border=\\\"0\\\" cellpadding=\\\"0\\\" cellspacing=\\\"0\\\" class=\\\"row row-1\\\" role=\\\"presentation\\\" style=\\\"mso-table-lspace: 0pt; mso-table-rspace: 0pt;\\\" width=\\\"100%\\\">\r\n<tbody>\r\n<tr>\r\n<td>\r\n<table align=\\\"center\\\" border=\\\"0\\\" cellpadding=\\\"0\\\" cellspacing=\\\"0\\\" class=\\\"row-content stack\\\" role=\\\"presentation\\\" style=\\\"mso-table-lspace: 0pt; mso-table-rspace: 0pt; color: #000000; width: 500px;\\\" width=\\\"500\\\">\r\n<tbody>\r\n<tr>\r\n<td class=\\\"column column-1\\\" style=\\\"mso-table-lspace: 0pt; mso-table-rspace: 0pt; font-weight: 400; text-align: left; vertical-align: top; padding-top: 5px; padding-bottom: 5px; border-top: 0px; border-right: 0px; border-bottom: 0px; border-left: 0px;\\\" width=\\\"100%\\\">\r\n<table border=\\\"0\\\" cellpadding=\\\"0\\\" cellspacing=\\\"0\\\" class=\\\"image_block block-1\\\" role=\\\"presentation\\\" style=\\\"mso-table-lspace: 0pt; mso-table-rspace: 0pt;\\\" width=\\\"100%\\\">\r\n<tr>\r\n<td class=\\\"pad\\\" style=\\\"width:100%;padding-right:0px;padding-left:0px;\\\">\r\n<div align=\\\"center\\\" class=\\\"alignment\\\" style=\\\"line-height:10px\\\"><img src=\\\"cid:MyImage\\\" id=\\\"img\\\" style=\\\"display: block; height: auto; border: 0; width: 225px; max-width: 100%;\\\" width=\\\"225\\\"/></div>\r\n</td>\r\n</tr>\r\n</table>\r\n<table border=\\\"0\\\" cellpadding=\\\"10\\\" cellspacing=\\\"0\\\" class=\\\"divider_block block-2\\\" role=\\\"presentation\\\" style=\\\"mso-table-lspace: 0pt; mso-table-rspace: 0pt;\\\" width=\\\"100%\\\">\r\n<tr>\r\n<td class=\\\"pad\\\">\r\n<div align=\\\"center\\\" class=\\\"alignment\\\">\r\n<table border=\\\"0\\\" cellpadding=\\\"0\\\" cellspacing=\\\"0\\\" role=\\\"presentation\\\" style=\\\"mso-table-lspace: 0pt; mso-table-rspace: 0pt;\\\" width=\\\"100%\\\">\r\n<tr>\r\n<td class=\\\"divider_inner\\\" style=\\\"font-size: 1px; line-height: 1px; border-top: 1px solid #BBBBBB;\\\"><span> </span></td>\r\n</tr>\r\n</table>\r\n</div>\r\n</td>\r\n</tr>\r\n</table>\r\n<table border=\\\"0\\\" cellpadding=\\\"0\\\" cellspacing=\\\"0\\\" class=\\\"heading_block block-3\\\" role=\\\"presentation\\\" style=\\\"mso-table-lspace: 0pt; mso-table-rspace: 0pt;\\\" width=\\\"100%\\\">\r\n<tr>\r\n<td class=\\\"pad\\\" style=\\\"width:100%;text-align:center;\\\">\r\n\t<h1 style=\\\"margin: 0; color: #555555; font-size: 23px; font-family: Arial, Helvetica Neue, Helvetica, sans-serif; line-height: 120%; text-align: center; direction: ltr; font-weight: 700; letter-spacing: normal; margin-top: 0; margin-bottom: 0;\\\">Verification Code<br /></h1>\r\n</td>\r\n</tr>\r\n</table>\r\n<table border=\\\"0\\\" cellpadding=\\\"0\\\" cellspacing=\\\"0\\\" class=\\\"paragraph_block block-5\\\" role=\\\"presentation\\\" style=\\\"mso-table-lspace: 0pt; mso-table-rspace: 0pt; word-break: break-word;\\\" width=\\\"100%\\\">\r\n<tr>\r\n<td class=\\\"pad\\\" style=\\\"padding-top:40px;padding-right:10px;padding-bottom:70px;padding-left:10px;\\\">\r\n<div style=\\\"color:#000000;font-size:14px;font-family:Arial, Helvetica Neue, Helvetica, sans-serif;font-weight:400;line-height:120%;text-align:center;direction:ltr;letter-spacing:0px;mso-line-height-alt:16.8px;\\\">\r\n\t<h1 style=\\\"margin: 0; color: #363636; font-size: 18px; font-family: Arial, Helvetica Neue, Helvetica, sans-serif; line-height: 120%; text-align: center; direction: ltr; font-weight: 700; letter-spacing: normal; margin-top: 0; margin-bottom: 0;\\\">{code}<br /></h1>\r\n</div>\r\n</td>\r\n</tr>\r\n</table>\r\n</td>\r\n</tr>\r\n</tbody>\r\n</table>\r\n</td>\r\n</tr>\r\n</tbody>\r\n</table>\r\n<table align=\\\"center\\\" border=\\\"0\\\" cellpadding=\\\"0\\\" cellspacing=\\\"0\\\" class=\\\"row row-2\\\" role=\\\"presentation\\\" style=\\\"mso-table-lspace: 0pt; mso-table-rspace: 0pt;\\\" width=\\\"100%\\\">\r\n<tbody>\r\n<tr>\r\n<td>\r\n<table align=\\\"center\\\" border=\\\"0\\\" cellpadding=\\\"0\\\" cellspacing=\\\"0\\\" class=\\\"row-content stack\\\" role=\\\"presentation\\\" style=\\\"mso-table-lspace: 0pt; mso-table-rspace: 0pt; color: #000000; width: 500px;\\\" width=\\\"500\\\">\r\n<tbody>\r\n<tr>\r\n<td class=\\\"column column-1\\\" style=\\\"mso-table-lspace: 0pt; mso-table-rspace: 0pt; font-weight: 400; text-align: left; vertical-align: top; padding-top: 5px; padding-bottom: 5px; border-top: 0px; border-right: 0px; border-bottom: 0px; border-left: 0px;\\\" width=\\\"100%\\\">\r\n<table border=\\\"0\\\" cellpadding=\\\"0\\\" cellspacing=\\\"0\\\" class=\\\"icons_block block-1\\\" role=\\\"presentation\\\" style=\\\"mso-table-lspace: 0pt; mso-table-rspace: 0pt;\\\" width=\\\"100%\\\">\r\n<tr>\r\n<td class=\\\"pad\\\" style=\\\"vertical-align: middle; color: #9d9d9d; font-family: inherit; font-size: 15px; padding-bottom: 5px; padding-top: 5px; text-align: center;\\\">\r\n<table cellpadding=\\\"0\\\" cellspacing=\\\"0\\\" role=\\\"presentation\\\" style=\\\"mso-table-lspace: 0pt; mso-table-rspace: 0pt;\\\" width=\\\"100%\\\">\r\n<tr>\r\n<td class=\\\"alignment\\\" style=\\\"vertical-align: middle; text-align: center;\\\">\r\n<!--[if vml]><table align=\\\"left\\\" cellpadding=\\\"0\\\" cellspacing=\\\"0\\\" role=\\\"presentation\\\" style=\\\"display:inline-block;padding-left:0px;padding-right:0px;mso-table-lspace: 0pt;mso-table-rspace: 0pt;\\\"><![endif]-->\r\n<!--[if !vml]><!-->\r\n<table cellpadding=\\\"0\\\" cellspacing=\\\"0\\\" class=\\\"icons-inner\\\" role=\\\"presentation\\\" style=\\\"mso-table-lspace: 0pt; mso-table-rspace: 0pt; display: inline-block; margin-right: -4px; padding-left: 0px; padding-right: 0px;\\\">\r\n<!--<![endif]-->\r\n</table>\r\n</td>\r\n</tr>\r\n</table>\r\n</td>\r\n</tr>\r\n</table>\r\n</td>\r\n</tr>\r\n</tbody>\r\n</table>\r\n</td>\r\n</tr>\r\n</tbody>\r\n</table>\r\n</td>\r\n</tr>\r\n</tbody>\r\n</table><!-- End -->\r\n</body>\r\n</html>"
                //};
                ////message.Body = Mail_Body(code, "2fa");

                // Log every step to this file:
                using var client = new SmtpClient(new ProtocolLogger(@"C:\temp\mailkit.log"));

                // Choose the right socket option:
                try
                {
                    client.Connect("mail.infox.co.za", 587, SecureSocketOptions.StartTls);
                }
                catch (Exception ex)
                {
                    // or for SSL-on-connect:
                    client.Connect("mail.infox.co.za", 465, SecureSocketOptions.SslOnConnect);
                }

                client.Authenticate("login@infox.co.za", "JSFJ6m8Vs2JTUFh");
                client.Send(message);
                client.Disconnect(true);
            }
            catch (Exception ex)
            {
                // Log the error to a file or console
                System.IO.File.WriteAllText("c:\\temp\\mailkit_error.txt", ex.ToString());
            }
        }

        //private string SendEmailTemplate(string email, string code)
        //{
        //    try
        //    {
        //        using var mail = new MailMessage();
        //        using var smtpClient = new SmtpClient("mail.infox.co.za", 465) // or 587 if reachable
        //        {
        //            EnableSsl = true,
        //            UseDefaultCredentials = false,
        //            Credentials = new NetworkCredential("login@infox.co.za", "JSFJ6m8Vs2JTUFh"),
        //            DeliveryMethod = SmtpDeliveryMethod.Network,
        //            Timeout = 200_000
        //        };

        //        mail.From = new MailAddress("login@infox.co.za", "InfoXchange");
        //        mail.To.Add(email);
        //        mail.Subject = "InfoXchange – Authentication Code";
        //        mail.IsBodyHtml = true;
        //        mail.AlternateViews.Add(Mail_Body(code, "2fa"));

        //        try
        //        {
        //            smtpClient.Send(mail);
        //            return "OK";
        //        }
        //        catch (SmtpException smtpEx)
        //        {
        //            // Log status code and inner exception
        //            var details = new StringBuilder();
        //            details.AppendLine($"SMTP StatusCode: {smtpEx.StatusCode}");
        //            details.AppendLine($"Message: {smtpEx.Message}");
        //            if (smtpEx.InnerException != null)
        //                details.AppendLine($"Inner: {smtpEx.InnerException}");
        //            // write details to a file or console
        //            System.IO.File.WriteAllText("c:\\temp\\smtp_error.txt", details.ToString());
        //            return "ERROR (SMTP): " + smtpEx.Message;
        //        }
        //        catch (Exception ex)
        //        {
        //            System.IO.File.WriteAllText("c:\\temp\\smtp_error.txt", ex.ToString());
        //            return "ERROR: " + ex.Message;
        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        return "ERROR: " + ex.Message;
        //    }
        //}

        //private string SendEmailTemplate(string email, string code)
        //{
        //    try
        //    {
        //        MailMessage mail = new MailMessage();
        //        SmtpClient smtpClient = new SmtpClient();
        //        mail.From = new MailAddress("login@infox.co.za", "InfoXchange");
        //        mail.To.Add(email);
        //        mail.Subject = "InfoXchange - Authentication Code";
        //        mail.IsBodyHtml = true;
        //        mail.AlternateViews.Add(Mail_Body(code, "2fa"));
        //        smtpClient.Port = 587;
        //        smtpClient.Host = "mail.infox.co.za";
        //        smtpClient.EnableSsl = true;
        //        smtpClient.UseDefaultCredentials = false;
        //        smtpClient.Credentials = new NetworkCredential("login@infox.co.za", "JSFJ6m8Vs2JTUFh");
        //        smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
        //        smtpClient.Send(mail);

        //        return "OK";
        //    }
        //    catch (Exception ex)
        //    {
        //        return "ERROR: " + ex.Message;
        //    }
        //}

        private System.Net.Mail.AlternateView Mail_Body(string code, string template)
        {
            var path = Path.Combine(System.IO.Directory.GetCurrentDirectory(), "EmailTemplates");
            var file = Path.Combine(path, template + ".html");

            var imagePath = Path.Combine(path, "images");
            var image = Path.Combine(imagePath, "logo-text.png");

            string html = System.IO.File.ReadAllText(file).Replace("__code__", code);

            System.Net.Mail.LinkedResource Img = new System.Net.Mail.LinkedResource(image, System.Net.Mime.MediaTypeNames.Image.Jpeg);
            Img.ContentId = "MyImage";
            System.Net.Mail.AlternateView AV = System.Net.Mail.AlternateView.CreateAlternateViewFromString(html, null, System.Net.Mime.MediaTypeNames.Text.Html);
            AV.LinkedResources.Add(Img);

            return AV;
        }
    }

}
