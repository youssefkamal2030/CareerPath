namespace CareerPath.Contracts.Dto.JobRecommendation;

/// <summary>
/// A job with its match score and details
/// </summary>
public class JobMatchDto
{
    public Guid JobId { get; set; }
    public string JobTitle { get; set; } = string.Empty;
    public string CompanyName { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Requirements { get; set; } = string.Empty;
    public string SalaryRange { get; set; } = string.Empty;
    public DateTime? PostedDate { get; set; }
    public string Source { get; set; } = string.Empty;
    public string ExternalUrl { get; set; } = string.Empty;
    public JobMatchScoreDto MatchScore { get; set; } = null!;
    public DateTime MatchedAt { get; set; } = DateTime.UtcNow;
}

/// <summary>
/// Detailed match scoring breakdown
/// </summary>
public class JobMatchScoreDto
{
    public Guid JobId { get; set; }
    public decimal OverallScore { get; set; }
    public decimal SkillsScore { get; set; }
    public decimal ExperienceScore { get; set; }
    public decimal EducationScore { get; set; }
    public decimal LocationScore { get; set; }
    public List<string> MatchedSkills { get; set; } = new();
    public string Explanation { get; set; } = string.Empty;
}
