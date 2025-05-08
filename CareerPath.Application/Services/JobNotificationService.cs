using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CareerPath.Application.Interfaces;
using CareerPath.Contracts.Dto;
using EmailConfigration.EmailConfig;
using Microsoft.Extensions.Logging;

namespace CareerPath.Application.Services
{
    public class JobNotificationService
    {
        private readonly EmailSender _emailSender;
        private readonly ILogger<JobNotificationService> _logger;

        public JobNotificationService(EmailSender emailSender, ILogger<JobNotificationService> logger)
        {
            _emailSender = emailSender ?? throw new ArgumentNullException(nameof(emailSender));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Send a notification about a new job posting to subscribers
        /// </summary>
        /// <param name="job">The newly created job</param>
        /// <param name="subscriberEmails">List of subscriber email addresses</param>
        public async Task NotifyNewJobPostingAsync(JobDto job, IEnumerable<string> subscriberEmails)
        {
            try
            {
                _logger.LogInformation("Sending job notification for job {JobId} to {SubscriberCount} subscribers", 
                    job.JobId, subscriberEmails);
                
                await _emailSender.SendJobPostingNotificationAsync(
                    subscriberEmails,
                    job.JobTitle,
                    job.CompanyName,
                    job.JobId);
                
                _logger.LogInformation("Successfully sent job notifications for job {JobId}", job.JobId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send job notifications for job {JobId}: {ErrorMessage}", 
                    job.JobId, ex.Message);
                // Don't rethrow - we don't want to fail the job creation if notifications fail
            }
        }

        /// <summary>
        /// Send a confirmation email to an applicant
        /// </summary>
        /// <param name="applicantEmail">The applicant's email address</param>
        /// <param name="job">The job being applied for</param>
        public async Task SendApplicationConfirmationAsync(string applicantEmail, JobDto job)
        {
            if (string.IsNullOrWhiteSpace(applicantEmail) || !applicantEmail.IsValidEmail())
            {
                _logger.LogWarning("Invalid email address for application confirmation: {Email}", applicantEmail);
                return;
            }

            try
            {
                await _emailSender.SendJobApplicationConfirmationAsync(
                    applicantEmail,
                    job.JobTitle,
                    job.CompanyName);
                
                _logger.LogInformation("Successfully sent application confirmation to {ApplicantEmail} for job {JobId}", 
                    applicantEmail, job.JobId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send application confirmation to {ApplicantEmail} for job {JobId}: {ErrorMessage}", 
                    applicantEmail, job.JobId, ex.Message);
                // Don't rethrow - we don't want to fail the application if confirmation fails
            }
        }
    }
} 