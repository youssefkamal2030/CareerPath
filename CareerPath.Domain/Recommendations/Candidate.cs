using System;
using CareerPath.Domain.Identity;

namespace CareerPath.Domain.Recommendations
{
    public class Candidate
    {
        public string Id { get; set; }
        
        public string Location { get; set; }
        
        public string Skills { get; set; }
        
        public string ExperienceLevel { get; set; }
        
        public string EducationLevel { get; set; }
        
        public string Certifications { get; set; }
        
        public string Languages { get; set; }
        
        public string ExpectedSalary { get; set; }
        
        public int? Age { get; set; }
        
        public string Gender { get; set; }
        
        public string Nationality { get; set; }
        
        public string? UserId { get; set; }
        
        public ApplicationUser? User { get; set; }
        public string FullName { get; set; }
    }
} 