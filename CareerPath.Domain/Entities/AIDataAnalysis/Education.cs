namespace CareerPath.Domain.Entities.AIDataAnalysis
{
    public class Education : BaseEntity
    {
        public string? Institution { get; set; }
        public string? Degree { get; set; }
        public string? FieldOfStudy { get; set; }
        public int? StartYear { get; set; }
        public int? StartMonth { get; set; }
        public int? EndYear { get; set; }
        public int? EndMonth { get; set; }
        public string? EducationLevel { get; set; }
    }
} 