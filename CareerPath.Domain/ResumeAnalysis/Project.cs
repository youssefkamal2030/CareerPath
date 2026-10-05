using System;
using CareerPath.Domain.Identity;

namespace CareerPath.Domain.ResumeAnalysis
{
    public class Project 
    {
        public string Id { get; set; }

        public string? ProjectName { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? Url { get; set; }
        public string? Description { get; set; }
        
        public string? UserId { get; set; }
        
        public ApplicationUser? User { get; set; }
    }
} 