using System;
using System.Collections.Generic;

namespace CareerPath.Domain.Entities.AIDataAnalysis
{
    public abstract class BaseEntity
    {
        public Guid Id { get; set; }
        public string UserId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        
        // Navigation properties for CV analysis
        public PersonalInformation? PersonalInformation { get; set; }
        public ICollection<Skill>? Skills { get; set; }
        public ICollection<WorkExperience>? WorkExperiences { get; set; }
        public ICollection<Education>? Educations { get; set; }
        public ICollection<Project>? Projects { get; set; }
    }
} 