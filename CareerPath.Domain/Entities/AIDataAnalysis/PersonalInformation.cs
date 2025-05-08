namespace CareerPath.Domain.Entities.AIDataAnalysis
{
    public class PersonalInformation : BaseEntity
    {
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? Address { get; set; }

        // Navigation properties
        public ICollection<Skill>? Skills { get; set; }
        public ICollection<WorkExperience>? WorkExperiences { get; set; }
        public ICollection<Education>? Educations { get; set; }
        public ICollection<Project>? Projects { get; set; }
    }
} 