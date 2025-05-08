using System;
using System.Threading.Tasks;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using Microsoft.Extensions.Logging;

namespace EmailConfigration.EmailConfig
{
    public class SmtpEmailService : IEmailService
    {
        private readonly EmailConfigration _emailConfig;
        private readonly ILogger<SmtpEmailService> _logger;

        public SmtpEmailService(EmailConfigration emailConfigration, ILogger<SmtpEmailService> logger)
        {
            _emailConfig = emailConfigration ?? throw new ArgumentNullException(nameof(emailConfigration));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task SendEmailAsync(Message message)
        {
            try
            {
                var emailMessage = CreateEmailMessage(message);
                await SendAsync(emailMessage);
                _logger.LogInformation("Email sent successfully to {Recipients}", string.Join(", ", message.To));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send email: {ErrorMessage}", ex.Message);
                throw;
            }
        }

        private MimeMessage CreateEmailMessage(Message message)
        {
            var emailMessage = new MimeMessage();
            emailMessage.From.Add(new MailboxAddress("Career Path", _emailConfig.from));
            emailMessage.To.AddRange(message.To);
            emailMessage.Subject = message.subject;
            emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Html) { Text = message.Content };
            return emailMessage;
        }

        private async Task SendAsync(MimeMessage mailMessage)
        {
            using (var client = new SmtpClient())
            {
                try
                {
                   await client.ConnectAsync(_emailConfig.SmtpServer, _emailConfig.Port, SecureSocketOptions.Auto);
                    
                    // Only authenticate if credentials are provided
                    if (!string.IsNullOrEmpty(_emailConfig.UserName) && !string.IsNullOrEmpty(_emailConfig.Password))
                    {
                        await client.AuthenticateAsync(_emailConfig.UserName, _emailConfig.Password);
                    }
                    
                    await client.SendAsync(mailMessage);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "SMTP Error: {ErrorMessage}", ex.Message);
                    throw;
                }
                finally
                {
                    await client.DisconnectAsync(true);
                    client.Dispose();
                }
            }
        }
    }
}