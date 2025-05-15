using System;

namespace CareerPath.Contracts.Dto
{
    public class JobApplicationDto
    {
        public int Id { get; set; }
        public string JobId { get; set; }
        public string UserId { get; set; }
        public string ApplicationStatus { get; set; }
        public DateTime ApplicationDate { get; set; }
        public string ResumeUrl { get; set; }
        public string? CoverLetterUrl { get; set; }
    }
} 