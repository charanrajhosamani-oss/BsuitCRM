using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System.Net;
using System.Net.Mail;

namespace BSuit.Infrastructure.Email
{


    public class SmtpEmailService
    {
        private readonly IConfiguration _config;

        public SmtpEmailService(IConfiguration config)
        {
            _config = config;
        }

        public async Task SendAsync(string to, string subject, string html)
        {
            var smtp = new SmtpClient(
                _config["EmailSettings:Smtp:Host"],
                int.Parse(_config["EmailSettings:Smtp:Port"]))
            {
                Credentials = new NetworkCredential(
                    _config["EmailSettings:Smtp:Username"],
                    _config["EmailSettings:Smtp:Password"]),
                EnableSsl = true
            };

            var mail = new MailMessage
            {
                From = new MailAddress(_config["EmailSettings:Smtp:From"]),
                Subject = subject,
                Body = html,
                IsBodyHtml = true
            };

            mail.To.Add(to);
            await smtp.SendMailAsync(mail);
        }

        public async Task SendAsync(
    List<string> to,
    List<string>? cc,
    List<string>? bcc,
    string subject,
    string html,
    List<IFormFile>? attachments = null)
        {
            var smtp = new SmtpClient(
                _config["EmailSettings:Smtp:Host"],
                int.Parse(_config["EmailSettings:Smtp:Port"]))
            {
                Credentials = new NetworkCredential(
                    _config["EmailSettings:Smtp:Username"],
                    _config["EmailSettings:Smtp:Password"]),
                EnableSsl = true
            };

            var mail = new MailMessage
            {
                From = new MailAddress(_config["EmailSettings:Smtp:From"]),
                Subject = subject,
                Body = html,
                IsBodyHtml = true
            };

            // ✅ TO
            if (to != null)
            {
                foreach (var email in to.Where(e => !string.IsNullOrWhiteSpace(e)))
                {
                    mail.To.Add(email.Trim());
                }
            }

            // ✅ CC
            if (cc != null)
            {
                foreach (var email in cc.Where(e => !string.IsNullOrWhiteSpace(e)))
                {
                    mail.CC.Add(email.Trim());
                }
            }

            // ✅ BCC
            if (bcc != null)
            {
                foreach (var email in bcc.Where(e => !string.IsNullOrWhiteSpace(e)))
                {
                    mail.Bcc.Add(email.Trim());
                }
            }

            // ✅ Attachments
            if (attachments != null && attachments.Any())
            {
                foreach (var file in attachments)
                {
                    if (file.Length > 0)
                    {
                        var stream = file.OpenReadStream();
                        var attachment = new Attachment(stream, file.FileName, file.ContentType);
                        mail.Attachments.Add(attachment);
                    }
                }
            }

            await smtp.SendMailAsync(mail);
        }
    }
}
