using System;
using System.ComponentModel.DataAnnotations;

namespace CareerPath.Contracts.Dto
{
    public class CreateJobDto
    {
        [Required(ErrorMessage = "Job title is required")]
        public string JobTitle { get; set; }

        [Required(ErrorMessage = "Job industry is required")]
        public string JobIndustry { get; set; }

        [Required(ErrorMessage = "Company name is required")]
        public string CompanyName { get; set; }

        [Required(ErrorMessage = "Job description is required")]
        public string JobDescription { get; set; }

        [Required(ErrorMessage = "Required skills are required")]
        public string RequiredSkills { get; set; }

        [Required(ErrorMessage = "Experience level is required")]
        public string ExperienceLevel { get; set; }

        [Required(ErrorMessage = "Education level is required")]
        public string EducationLevel { get; set; }

        public string CertificationsRequired { get; set; }

        public string RequiredLanguage { get; set; }

        [Required(ErrorMessage = "Location is required")]
        public string Location { get; set; }

        public string SalaryRange { get; set; }

        [Required(ErrorMessage = "Employment type is required")]
        public string EmploymentType { get; set; }

        public DateTime? ApplicationDeadline { get; set; }

        public int? Age { get; set; }

        public string Gender { get; set; }

        public string Nationality { get; set; }
    }
} 