using CareerPath.Domain.Identity;
using CareerPath.Domain.Recommendations;

namespace CareerPath.Domain.ResumeAnalysis
{
    public class PersonalInformation
    {
        public string Id { get; set; }
        
        public string? UserId { get; set; }
        
        public string? Name { get; set; }
        
        public string? Email { get; set; }
        
        public string? Phone { get; set; }
        
        public string? Address { get; set; }

        // Navigation properties
        public ApplicationUser? User { get; set; }
        public ICollection<Skill>? Skills { get; set; }
        public ICollection<WorkExperience>? WorkExperiences { get; set; }
        public ICollection<Education>? Educations { get; set; }
        public ICollection<Project>? Projects { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
} 