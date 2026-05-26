using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSuit.Infrastructure.Email
{
    public interface IEmailService
    {
        Task SendAsync(string to, string subject, string htmlBody);

        // 🔹 Advanced (new)
        Task SendAsync(List<string> to, List<string>? cc, List<string>? bcc, string subject, string htmlBody, List<IFormFile>? attachments = null);
    }
}
