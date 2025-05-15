using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace CareerPath.Domain.Entities.AIDataAnalysis
{
    public class PersonalInformation
    {
        [Key]
        public string Id { get; set; }
        
        public string? UserId { get; set; }
        
        [MaxLength(100)]
        public string? Name { get; set; }
        
        [MaxLength(100)]
        public string? Email { get; set; }
        
        [MaxLength(20)]
        public string? Phone { get; set; }
        
        [MaxLength(200)]
        public string? Address { get; set; }

        // Navigation properties
        [ForeignKey("UserId")]
        [JsonIgnore]
        public ApplicationUser? User { get; set; }
        public ICollection<Skill>? Skills { get; set; }
        public ICollection<WorkExperience>? WorkExperiences { get; set; }
        public ICollection<Education>? Educations { get; set; }
        public ICollection<Project>? Projects { get; set; }
    }
} 