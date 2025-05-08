namespace CareerPath.Domain.Entities.AIDataAnalysis
{
    public class WorkExperience : BaseEntity
    {
        public string? JobTitle { get; set; }
        public string? JobLevel { get; set; }
        public string? Company { get; set; }
        public int? StartYear { get; set; }
        public int? StartMonth { get; set; }
        public int? EndYear { get; set; }
        public int? EndMonth { get; set; }
        public string? JobDescription { get; set; }
    }
} 