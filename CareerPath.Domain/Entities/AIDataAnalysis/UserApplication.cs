using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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
        public virtual Candidate Candidate { get; set; }
        
        [ForeignKey("JobId")]
        public virtual Job Job { get; set; }
    }
} 