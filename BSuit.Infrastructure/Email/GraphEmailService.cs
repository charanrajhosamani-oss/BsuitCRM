using Azure.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Graph;
using Microsoft.Graph.Models;
using System.Net.Mail;

namespace BSuit.Infrastructure.Email
{
    public class GraphEmailService
    {
        private readonly IConfiguration _config;

        public GraphEmailService(IConfiguration config)
        {
            _config = config;
        }

        public async Task SendAsync(string to, string subject, string html)
        {
            var client = new GraphServiceClient(new ClientSecretCredential(
                _config["EmailSettings:Graph:TenantId"],
                _config["EmailSettings:Graph:ClientId"],
                _config["EmailSettings:Graph:ClientSecret"]));

            var message = new Message
            {
                Subject = subject,
                Body = new ItemBody
                {
                    ContentType = BodyType.Html,
                    Content = html
                },
                ToRecipients = new List<Recipient>
            {
                new Recipient
                {
                    EmailAddress = new EmailAddress { Address = to }
                }
            }
            };

            //Grant Admin Consent
            try
            {
                await client
                    .Users[_config["EmailSettings:Graph:From"]]
                    .SendMail
                    .PostAsync(new Microsoft.Graph.Users.Item.SendMail.SendMailPostRequestBody
                    {
                        Message = message,
                        SaveToSentItems = true
                    });
            }
            catch(Exception ex) 
            {
                var msg = ex.Message;
            }
            //await client.Users[_config["EmailSettings:Graph:From"]].SendMail(message, null).Request().PostAsync();
        }
    }
}
