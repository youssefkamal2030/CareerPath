using System;

namespace CareerPath.Domain.Entities.AIDataAnalysis
{
    public class Project : BaseEntity
    {
        public string? ProjectName { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? Url { get; set; }
        public string? Description { get; set; }
    }
} 