using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore.Metadata;

namespace CareerPath.Domain.Entities.AIDataAnalysis
{
    public class UserCV
    {
        [Key]
        public Guid Id { get; set; }

        public string UserId { get; set; }

        public byte[] FileData { get; set; }

        public string FileName { get; set; }

        public string ContentType { get; set; }
        public DateTime UploadDate { get; set; }

        [ForeignKey("UserId")]
        public ApplicationUser ApplicationUser { get; set; }
    }
}
