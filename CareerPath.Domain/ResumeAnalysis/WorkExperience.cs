using System;
using CareerPath.Domain.Identity;

namespace CareerPath.Domain.ResumeAnalysis
{
    public class WorkExperience 
    {
        public string Id { get; set; }
        
        public string? JobTitle { get; set; }
        
        public string? JobLevel { get; set; }
        
        public string? Company { get; set; }
        
        public int? StartYear { get; set; }
        public int? StartMonth { get; set; }
        public int? EndYear { get; set; }
        public int? EndMonth { get; set; }
        
        public string? JobDescription { get; set; }
        
        public string? UserId { get; set; }
        
        public ApplicationUser? User { get; set; }
    }
} 