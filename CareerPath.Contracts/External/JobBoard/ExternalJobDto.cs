using System;

namespace CareerPath.Contracts.External.JobBoard
{
    public class ExternalJobDto
    {
        public string ExternalId { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Company { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public string SalaryRange { get; set; } = string.Empty;
        public string RequiredSkills { get; set; } = string.Empty;
        public string ExperienceLevel { get; set; } = string.Empty;
        public string EducationLevel { get; set; } = string.Empty;
        public string EmploymentType { get; set; } = string.Empty;
        public DateTime PostedDate { get; set; }
        public DateTime? ApplicationDeadline { get; set; }
        public string SourceProvider { get; set; } = string.Empty; // "Adzuna", "RemoteOK", etc.
        public string ExternalUrl { get; set; } = string.Empty;
        public string Industry { get; set; } = string.Empty;
        public string CertificationsRequired { get; set; } = string.Empty;
        public string RequiredLanguage { get; set; } = string.Empty;
    }
}