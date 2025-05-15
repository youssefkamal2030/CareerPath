using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace CareerPath.Domain.Entities.AIDataAnalysis
{
    public class Skill
    {
        [Key]
        public string Id { get; set; }
        
        [MaxLength(100)]
        public string? SkillName { get; set; }
        
        [MaxLength(50)]
        public string? ProficiencyLevel { get; set; }

        public string? UserId { get; set; }
        
        [ForeignKey("UserId")]
        public ApplicationUser? User { get; set; }
        
        // Navigation properties
        public string? PersonalInformationId { get; set; }
        
        [ForeignKey("PersonalInformationId")]
        [JsonIgnore]
        public PersonalInformation? PersonalInformation { get; set; }
    }
} 