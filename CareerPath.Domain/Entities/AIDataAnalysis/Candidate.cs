using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace CareerPath.Domain.Entities.AIDataAnalysis
{
    public class Candidate
    {
        [Key]
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
        
        [ForeignKey("UserId")]
        [JsonIgnore]
        public ApplicationUser? User { get; set; }
    }
} 