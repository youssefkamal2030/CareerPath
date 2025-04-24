using CareerPath.Application.Interfaces;
using Microsoft.Extensions.Configuration;
using System;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace CareerPath.Application.Services
{
    public class SmtpEmailService : IEmailService
    {
        private readonly string _smtpServer;
        private readonly int _smtpPort;
        private readonly string _smtpUsername;
        private readonly string _smtpPassword;
        private readonly string _senderEmail;
        private readonly string _senderName;
        private readonly bool _enableSsl;

        public SmtpEmailService(IConfiguration configuration)
        {
            var emailConfig = configuration.GetSection("EmailSettings");
            _smtpServer = emailConfig["SmtpServer"];
            _smtpPort = int.Parse(emailConfig["SmtpPort"] ?? "587");
            _smtpUsername = emailConfig["SmtpUsername"];
            _smtpPassword = emailConfig["SmtpPassword"];
            _senderEmail = emailConfig["SenderEmail"];
            _senderName = emailConfig["SenderName"];
            _enableSsl = bool.Parse(emailConfig["EnableSsl"] ?? "true");
        }

        public async Task SendEmailAsync(string to, string subject, string body, bool isHtml = true)
        {
            try
            {
                var message = new MailMessage
                {
                    From = new MailAddress(_senderEmail, _senderName),
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = isHtml
                };
                
                message.To.Add(to);

                using (var client = new SmtpClient(_smtpServer, _smtpPort))
                {
                    client.UseDefaultCredentials = false;
                    client.Credentials = new NetworkCredential(_smtpUsername, _smtpPassword);
                    client.EnableSsl = _enableSsl;
                    
                    await client.SendMailAsync(message);
                }
            }
            catch (Exception ex)
            {
                // In a production app, you would log this error
                Console.WriteLine($"Error sending email: {ex.Message}");
                throw;
            }
        }
    }
} 