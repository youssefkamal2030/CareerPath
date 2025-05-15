using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CareerPath.Domain.Entities
{
    public class JobApplication
    {
        public int Id { get; set; }
        public string JobId { get; set; }
        public string userId { get; set; }
        public string ApplicationStatus { get; set; }

        public DateTime ApplicationDate { get; set; }

        public string ResumeUrl { get; set; }
        public string? CoverLetterUrl { get; set; }

        public ApplicationUser User { get; set; }

    }
}
