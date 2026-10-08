using System;
using CareerPath.Domain.Identity;
using CareerPath.Domain.ResumeAnalysis;

namespace CareerPath.Domain.Recommendations
{
    public class Skill
    {
        public string Id { get; set; }
        
        public string? SkillName { get; set; }
        
        public string? ProficiencyLevel { get; set; }

        public string? UserId { get; set; }
        
        public ApplicationUser? User { get; set; }
        
        // Navigation properties
        public string? PersonalInformationId { get; set; }
        
        public PersonalInformation? PersonalInformation { get; set; }
    }
} 