using Mailjet.Client.TransactionalEmails;
using Mailjet.Client;
using Microsoft.AspNetCore.Identity.UI.Services;
using Mailjet.Client.Resources;
using Microsoft.Extensions.Configuration;

namespace GunNGoneBetter_Utility
{
    public class EmailSender : IEmailSender
    {
        private IConfiguration configuration;

        // данные всегда отлавливаются в конструкторе при использовании
        // внедрению зависимости 
        public EmailSender(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            return Execute(email, subject, htmlMessage);
        }

        public async Task Execute(string email, string subject, string htmlMessage)
        {
            // получение информации из appsettings.json
            MailJetSettings mailJetSettings = configuration.GetSection("MailJet").Get<MailJetSettings>();

            MailjetClient client = new MailjetClient(mailJetSettings.ApiKey, mailJetSettings.SecretKey);
            MailjetRequest request = new MailjetRequest
            {
                Resource = Send.Resource
            };


            var emailMessage = new TransactionalEmailBuilder()
                .WithFrom(new SendContact(mailJetSettings.Email))
                .WithSubject(subject)
                .WithHtmlPart(htmlMessage)
                .WithTo(new SendContact(email))
                .Build();

            var response = await client.SendTransactionalEmailAsync(emailMessage);
        }
    }
}
