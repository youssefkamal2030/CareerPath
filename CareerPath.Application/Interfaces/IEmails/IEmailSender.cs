using System.Collections.Generic;
using System.Threading.Tasks;

namespace CareerPath.Application.Interfaces.IEmails
{
    public interface IEmailSender
    {
        /// <summary>
        /// Send a simple email to a single recipient
        /// </summary>
        Task SendEmailAsync(string to, string subject, string content, bool isHtml = true);

        /// <summary>
        /// Send an email to multiple recipients
        /// </summary>
        Task SendEmailToMultipleRecipientsAsync(IEnumerable<string> toAddresses, string subject, string content, bool isHtml = true);

        /// <summary>
        /// Create a job application confirmation email
        /// </summary>
        Task SendJobApplicationConfirmationAsync(string to, string jobTitle, string companyName);

        /// <summary>
        /// Send a new job posting notification
        /// </summary>
        Task SendJobPostingNotificationAsync(IEnumerable<string> subscribers, string jobTitle, string companyName, string jobId);
    }
}
