using MailKit.Net.Smtp;
using Microsoft.Extensions.Options;
using MimeKit;
using System.Threading.Tasks;
using WebApplicationCSST.Service.Configuration;

namespace WebApplicationCSST.Service
{
    public class EmailService : IEmailService
    {
        private EmailSettings _settings { get; set; }

        public EmailService(IOptions<EmailSettings> settings)
        {
            _settings = settings.Value;
        }

        public async Task Send(string toAdresse, string toUsername, string messageHTML)
        {
            MimeMessage message = new MimeMessage();

            message.From.Add(new MailboxAddress(_settings.NameFrom, _settings.EmailFrom));
            message.To.Add(new MailboxAddress(toUsername, toAdresse));
            message.Subject = _settings.EmailSubject;

            BodyBuilder body = new BodyBuilder
            {
                HtmlBody = messageHTML
            };

            message.Body = body.ToMessageBody();

            using (SmtpClient client = new SmtpClient())
            {
                client.Connect(_settings.SmtpServer, _settings.SmtpPort);
                await client.AuthenticateAsync(_settings.LoginSmtpServer, _settings.PasswordSmtpServer);

                await client.SendAsync(message);
                await client.DisconnectAsync(true);
            }
        }
    }
}
