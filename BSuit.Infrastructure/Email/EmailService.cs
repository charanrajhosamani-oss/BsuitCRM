using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSuit.Infrastructure.Email
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _config;
        private readonly SmtpEmailService _smtp;
        private readonly GraphEmailService _graph;

        public EmailService(IConfiguration config,
            SmtpEmailService smtp,
            GraphEmailService graph)
        {
            _config = config;
            _smtp = smtp;
            _graph = graph;
        }

        public async Task SendAsync(string to, string subject, string htmlBody)
        {
            var provider = _config["EmailSettings:Provider"];

            if (provider == "GRAPH")
                await _graph.SendAsync(to, subject, htmlBody);
            else
                await _smtp.SendAsync(to, subject, htmlBody);
        }

        public async Task SendAsync(
    List<string> to,
    List<string>? cc,
    List<string>? bcc,
    string subject,
    string htmlBody,
    List<IFormFile>? attachments = null)
        {
            var provider = _config["EmailSettings:Provider"];

            if (provider == "GRAPH")
            {
                //await _graph.SendAsync(to, cc, bcc, subject, htmlBody, attachments);
            }
            else
            {
                await _smtp.SendAsync(to, cc, bcc, subject, htmlBody, attachments);
            }
        }
    }
}
