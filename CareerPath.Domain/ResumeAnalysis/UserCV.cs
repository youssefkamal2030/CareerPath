using System;
using System.Collections.Generic;
using CareerPath.Domain.Identity;

namespace CareerPath.Domain.ResumeAnalysis
{
    public class UserCV
    {
        public Guid Id { get; set; }

        public string UserId { get; set; }

        public byte[] FileData { get; set; }

        public string FileName { get; set; }

        public string ContentType { get; set; }
        public DateTime UploadDate { get; set; }

        public ApplicationUser ApplicationUser { get; set; }
    }
}
