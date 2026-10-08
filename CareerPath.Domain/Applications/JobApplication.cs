using System;
using CareerPath.Domain.Recommendations;
using CareerPath.Domain.Identity;

namespace CareerPath.Domain.Applications
{
    public class JobApplication
    {
        public string ApplicationId { get; set; }
        
        public string CandidateId { get; set; }
        
        public string JobId { get; set; }
        
        public DateTime ApplicationDate { get; set; }
        
        public string ApplicationStatus { get; set; }
        
        public DateTime? FollowUpReminder { get; set; }
        
        public string? UserId { get; set; }
        
  
    }
} 