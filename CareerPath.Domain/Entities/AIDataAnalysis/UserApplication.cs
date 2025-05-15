using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace CareerPath.Domain.Entities.AIDataAnalysis
{
    public class UserApplication
    {
        [Key]
        public string ApplicationId { get; set; }
        
        public string CandidateId { get; set; }
        
        public string JobId { get; set; }
        
        public DateTime ApplicationDate { get; set; }
        
        public string ApplicationStatus { get; set; }
        
        public DateTime? FollowUpReminder { get; set; }
        
        [ForeignKey("CandidateId")]
        [JsonIgnore]
        public virtual Candidate Candidate { get; set; }
        
        [ForeignKey("JobId")]
        [JsonIgnore]
        public virtual Job Job { get; set; }
        
        public string? UserId { get; set; }
        
        [ForeignKey("UserId")]
        [JsonIgnore]
        public ApplicationUser? User { get; set; }
    }
} 