using System;
using System.Threading.Tasks;
using CareerPath.Application.Interfaces.IEmails;
using MailKit;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;
/*[next-session]
 * you made the message and the email service and installed the library , 
 * next you need to make the smpt server from gmail and get the password and username and put it in the appsettings.json file and then you can use the email service to send emails
 * and then make an html message to send to the client and then you can use the email service to send the email to the client
 * and finally test the api that it's working and the email is sent to the client
 */
namespace EmailConfigration.EmailConfig
{
    public class EmailService : IEmailService
    {
        private readonly SmtpOptions _options;

        public EmailService(IOptions<SmtpOptions> options)
        {
            _options = options.Value;
        }

        public async Task SendAsync(
            EmailMessage message,
            CancellationToken cancellationToken = default)
        {
            // 1. Construct the email
            var email = new MimeMessage();

            email.From.Add(
                new MailboxAddress(
                    _options.FromName,
                    _options.FromEmail));

            email.To.Add(
                MailboxAddress.Parse(message.To));

            email.Subject = message.Subject;

            // 2. Construct the email body
            var bodyBuilder = new BodyBuilder();

            if (message.IsHtml)
                bodyBuilder.HtmlBody = message.Body;
            else
                bodyBuilder.TextBody = message.Body;

            email.Body = bodyBuilder.ToMessageBody();

            // 3. Connect to the SMTP server
            using var smtp = new SmtpClient();

            await smtp.ConnectAsync(
                _options.Host,
                _options.Port,
                SecureSocketOptions.StartTls,
                cancellationToken);

            // 4. Authenticate
            await smtp.AuthenticateAsync(
                _options.UserName,
                _options.Password,
                cancellationToken);

            // 5. Send the email
            await smtp.SendAsync(email, cancellationToken);

            // 6. Disconnect
            await smtp.DisconnectAsync(
                true,
                cancellationToken);
        }
    }
}