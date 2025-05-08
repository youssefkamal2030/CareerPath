using System;
using System.ComponentModel.DataAnnotations;

namespace CareerPath.Domain.Entities.AIDataAnalysis
{
    public class Job
    {
        [Key]
        public string JobId { get; set; }
        
        public string JobTitle { get; set; }
        
        public string JobIndustry { get; set; }
        
        public string CompanyName { get; set; }
        
        public string JobDescription { get; set; }
        
        public string RequiredSkills { get; set; }
        
        public string ExperienceLevel { get; set; }
        
        public string EducationLevel { get; set; }
        
        public string CertificationsRequired { get; set; }
        
        public string RequiredLanguage { get; set; }
        
        public string Location { get; set; }
        
        public string SalaryRange { get; set; }
        
        public string EmploymentType { get; set; }
        
        public DateTime PostingDate { get; set; }
        
        public DateTime ApplicationDeadline { get; set; }
        
        public int? Age { get; set; }
        
        public string Gender { get; set; }
        
        public string Nationality { get; set; }
    }
} 