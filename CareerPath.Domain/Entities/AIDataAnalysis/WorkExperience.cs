using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace CareerPath.Domain.Entities.AIDataAnalysis
{
    public class WorkExperience 
    {
        [Key]
        public string Id { get; set; }
        
        [MaxLength(100)]
        public string? JobTitle { get; set; }
        
        [MaxLength(50)]
        public string? JobLevel { get; set; }
        
        [MaxLength(100)]
        public string? Company { get; set; }
        
        public int? StartYear { get; set; }
        public int? StartMonth { get; set; }
        public int? EndYear { get; set; }
        public int? EndMonth { get; set; }
        
        [MaxLength(2000)]
        public string? JobDescription { get; set; }
        
        public string? UserId { get; set; }
        
        [ForeignKey("UserId")]
        [JsonIgnore]
        public ApplicationUser? User { get; set; }
    }
} 