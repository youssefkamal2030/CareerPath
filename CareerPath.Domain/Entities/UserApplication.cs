using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CareerPath.Domain.Entities.AIDataAnalysis;

namespace CareerPath.Domain.Entities
{
    public class UserApplication
    {
        public string ApplicationId { get; set; }
        public string CandidateId { get; set; }
        public string JobId { get; set; }
        public string JobName { get; set; }
        public DateTime ApplicationDate { get; set; }
        public string ApplicationStatus { get; set; }
        public DateTime? FollowUpReminder { get; set; }
        // Navigation properties
        public virtual Candidate Candidate { get; set; }
        public virtual Job Job { get; set; }
    }
}
