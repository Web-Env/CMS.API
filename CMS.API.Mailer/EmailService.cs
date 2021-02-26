using CMS.API.Infrastructure.Settings;
using MailKit.Net.Smtp;
using MimeKit;
using System.Threading.Tasks;

namespace CMS.API.Mailer
{
    public class EmailService
    {
        private readonly SmtpSettings _smtpSettings;

        public EmailService(SmtpSettings smtpSettings)
        {
            _smtpSettings = smtpSettings;
        }

        public async Task SendEmail(string emailToName, string emailToAddress, string emailSubject, string emailBody)
        {
            var message = new MimeMessage();
            message.To.Add(new MailboxAddress(emailToName, emailToAddress));
            message.From.Add(new MailboxAddress(_smtpSettings.EmailFromName, _smtpSettings.EmailFromAddress));
            message.Subject = emailSubject;
            message.Body = new TextPart("html") { Text = emailBody };
            var smtpClient = new SmtpClient();

            await smtpClient.ConnectAsync(_smtpSettings.EmailSmtpHost, _smtpSettings.EmailSmtpPort, true);
            await smtpClient.AuthenticateAsync(_smtpSettings.EmailSmtpUsername, _smtpSettings.EmailSmtpPassword);

            await smtpClient.SendAsync(message);
        }
    }
}
