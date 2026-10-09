using System;
using CareerPath.Domain.Common;
using CareerPath.Domain.Events;

namespace CareerPath.Domain.Recommendations
{
    public class Job : AggregateRoot
    {
        public string JobId { get; private set; } = string.Empty;
        public string ExternalId { get; private set; } = string.Empty; // From external provider
        public string JobTitle { get; private set; } = string.Empty;
        public string JobIndustry { get; private set; } = string.Empty;
        public string CompanyName { get; private set; } = string.Empty;
        public string JobDescription { get; private set; } = string.Empty;
        public string RequiredSkills { get; private set; } = string.Empty;
        public string ExperienceLevel { get; private set; } = string.Empty;
        public string EducationLevel { get; private set; } = string.Empty;
        public string CertificationsRequired { get; private set; } = string.Empty;
        public string RequiredLanguage { get; private set; } = string.Empty;
        public string Location { get; private set; } = string.Empty;
        public string SalaryRange { get; private set; } = string.Empty;
        public string EmploymentType { get; private set; } = string.Empty;
        public DateTime PostingDate { get; private set; }
        public DateTime? ApplicationDeadline { get; private set; }
        public string SourceProvider { get; private set; } = string.Empty;
        public string ExternalUrl { get; private set; } = string.Empty;
        
        // Domain-specific properties
        public JobStatus Status { get; private set; } = JobStatus.Active;
        public DateTime? ExpiredAt { get; private set; }
        public string? UserId { get; set; } // For manual job postings

        private Job() { } // For EF Core

        // Factory method for ingesting external jobs
        public static Job FromExternalSource(
            string externalId, 
            string title, 
            string company, 
            string description,
            string location,
            string salaryRange,
            string requiredSkills,
            string experienceLevel,
            string educationLevel,
            string employmentType,
            DateTime postedDate,
            DateTime? applicationDeadline,
            string sourceProvider,
            string externalUrl,
            string industry = "",
            string certificationsRequired = "",
            string requiredLanguage = "")
        {
            // Domain validation and business rules
            if (string.IsNullOrWhiteSpace(title))
                throw new ArgumentException("Job title is required", nameof(title));
            
            if (string.IsNullOrWhiteSpace(company))
                throw new ArgumentException("Company name is required", nameof(company));

            if (postedDate > DateTime.UtcNow)
                throw new ArgumentException("Job posting date cannot be in the future", nameof(postedDate));

            if (string.IsNullOrWhiteSpace(externalId))
                throw new ArgumentException("External ID is required for external jobs", nameof(externalId));

            var job = new Job
            {
                JobId = Guid.NewGuid().ToString(),
                ExternalId = externalId,
                JobTitle = title.Trim(),
                JobIndustry = industry?.Trim() ?? string.Empty,
                CompanyName = company.Trim(),
                JobDescription = description?.Trim() ?? string.Empty,
                RequiredSkills = requiredSkills?.Trim() ?? string.Empty,
                ExperienceLevel = experienceLevel?.Trim() ?? string.Empty,
                EducationLevel = educationLevel?.Trim() ?? string.Empty,
                CertificationsRequired = certificationsRequired?.Trim() ?? string.Empty,
                RequiredLanguage = requiredLanguage?.Trim() ?? string.Empty,
                Location = location?.Trim() ?? string.Empty,
                SalaryRange = salaryRange?.Trim() ?? string.Empty,
                EmploymentType = employmentType?.Trim() ?? string.Empty,
                PostingDate = postedDate,
                ApplicationDeadline = applicationDeadline,
                SourceProvider = sourceProvider,
                ExternalUrl = externalUrl,
                Status = JobStatus.Active,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            // Domain event
            job.AddDomainEvent(new JobIngestedEvent(job.JobId, job.ExternalId, job.SourceProvider));
            
            return job;
        }

        // Factory method for manual job creation
        public static Job CreateManualJob(
            string title,
            string company,
            string description,
            string location,
            string userId,
            DateTime? applicationDeadline = null)
        {
            if (string.IsNullOrWhiteSpace(title))
                throw new ArgumentException("Job title is required", nameof(title));
            
            if (string.IsNullOrWhiteSpace(company))
                throw new ArgumentException("Company name is required", nameof(company));

            if (string.IsNullOrWhiteSpace(userId))
                throw new ArgumentException("User ID is required for manual jobs", nameof(userId));

            var job = new Job
            {
                JobId = Guid.NewGuid().ToString(),
                ExternalId = string.Empty, // No external ID for manual jobs
                JobTitle = title.Trim(),
                CompanyName = company.Trim(),
                JobDescription = description?.Trim() ?? string.Empty,
                Location = location?.Trim() ?? string.Empty,
                PostingDate = DateTime.UtcNow,
                ApplicationDeadline = applicationDeadline,
                SourceProvider = "Manual",
                ExternalUrl = string.Empty,
                Status = JobStatus.Active,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                UserId = userId
            };

            return job;
        }

        // Behavioral methods
        public void MarkAsFilled()
        {
            if (Status == JobStatus.Expired)
                throw new InvalidOperationException("Cannot mark expired job as filled");

            Status = JobStatus.Filled;
            UpdatedAt = DateTime.UtcNow;
            AddDomainEvent(new JobFilledEvent(JobId));
        }

        public void ExpireJob()
        {
            Status = JobStatus.Expired;
            ExpiredAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
            AddDomainEvent(new JobExpiredEvent(JobId));
        }

        public void PauseJob()
        {
            if (Status == JobStatus.Expired)
                throw new InvalidOperationException("Cannot pause expired job");

            Status = JobStatus.Paused;
            UpdatedAt = DateTime.UtcNow;
        }

        public void ReactivateJob()
        {
            if (Status == JobStatus.Expired)
                throw new InvalidOperationException("Cannot reactivate expired job");

            Status = JobStatus.Active;
            UpdatedAt = DateTime.UtcNow;
        }

        public void UpdateFromExternalSource(
            string title,
            string description,
            DateTime? applicationDeadline)
        {
            if (Status == JobStatus.Expired)
                throw new InvalidOperationException("Cannot update expired job");

            if (string.IsNullOrWhiteSpace(ExternalId))
                throw new InvalidOperationException("Cannot update manual job from external source");

            JobTitle = title?.Trim() ?? JobTitle;
            JobDescription = description?.Trim() ?? JobDescription;
            ApplicationDeadline = applicationDeadline;
            UpdatedAt = DateTime.UtcNow;

            AddDomainEvent(new JobUpdatedEvent(JobId));
        }

        public bool IsExpired() => Status == JobStatus.Expired || 
                                  (ApplicationDeadline.HasValue && ApplicationDeadline.Value < DateTime.UtcNow);

        public bool IsFromExternalSource() => !string.IsNullOrWhiteSpace(ExternalId);

        public void CheckAndExpireIfNeeded()
        {
            if (ApplicationDeadline.HasValue && 
                ApplicationDeadline.Value < DateTime.UtcNow && 
                Status == JobStatus.Active)
            {
                ExpireJob();
            }
        }
    }
}