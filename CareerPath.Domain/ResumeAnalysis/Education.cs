using System;
using CareerPath.Domain.Identity;

namespace CareerPath.Domain.ResumeAnalysis
{
    public class Education 
    {
        public string Id { get; set; }
        public string? Institution { get; set; }
        public string? Degree { get; set; }
        public string? FieldOfStudy { get; set; }
        public int? StartYear { get; set; }
        public int? StartMonth { get; set; }
        public int? EndYear { get; set; }
        public int? EndMonth { get; set; }
        public string? EducationLevel { get; set; }
        public string? UserId { get; set; }

        public ApplicationUser? User { get; set; }
    }
} 