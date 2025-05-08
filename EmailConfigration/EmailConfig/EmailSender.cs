using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MimeKit;

namespace EmailConfigration.EmailConfig
{
    public class EmailSender
    {
        private readonly IEmailService _emailService;

        public EmailSender(IEmailService emailService)
        {
            _emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
        }

        /// <summary>
        /// Send a simple email to a single recipient
        /// </summary>
        public async Task SendEmailAsync(string to, string subject, string content)
        {
            var toAddresses = new List<MailboxAddress> { new MailboxAddress(to, to) };
            var message = new Message(toAddresses, subject, content);
            await _emailService.SendEmailAsync(message);
        }

        /// <summary>
        /// Send an email to multiple recipients
        /// </summary>
        public async Task SendEmailToMultipleRecipientsAsync(IEnumerable<string> toAddresses, string subject, string content)
        {
            var mailboxAddresses = new List<MailboxAddress>();
            foreach (var address in toAddresses)
            {
                mailboxAddresses.Add(new MailboxAddress(address, address));
            }

            var message = new Message(mailboxAddresses, subject, content);
            await _emailService.SendEmailAsync(message);
        }

        /// <summary>
        /// Create a job application confirmation email
        /// </summary>
        public async Task SendJobApplicationConfirmationAsync(string to, string jobTitle, string companyName)
        {
            string subject = $"Your application for {jobTitle} at {companyName} has been received";
            string content = $@"
                <html>
                    <body>
                        <h1>Application Received</h1>
                        <p>Dear applicant,</p>
                        <p>Thank you for your interest in the <strong>{jobTitle}</strong> position at <strong>{companyName}</strong>.</p>
                        <p>We have received your application and will review it shortly. If your qualifications match our requirements, we will contact you to schedule an interview.</p>
                        <p>Best regards,</p>
                        <p>The Career Path Team</p>
                    </body>
                </html>
            ";

            await SendEmailAsync(to, subject, content);
        }

        /// <summary>
        /// Send a new job posting notification
        /// </summary>
        public async Task SendJobPostingNotificationAsync(IEnumerable<string> subscribers, string jobTitle, string companyName, string jobId)
        {
            string subject = $"New Job Opportunity: {jobTitle} at {companyName}";
            string content = $@"
                <html>
                    <body>
                        <h1>New Job Opportunity</h1>
                        <p>Hello,</p>
                        <p>A new job matching your preferences has just been posted:</p>
                        <h2>{jobTitle}</h2>
                        <h3>{companyName}</h3>
                        <p>View the full details and apply by visiting your Career Path dashboard or clicking the link below:</p>
                        <p><a href=""https://careerpath.com/jobs/{jobId}"">View Job Details</a></p>
                        <p>Best regards,</p>
                        <p>The Career Path Team</p>
                    </body>
                </html>
            ";

            await SendEmailToMultipleRecipientsAsync(subscribers, subject, content);
        }
    }
} 