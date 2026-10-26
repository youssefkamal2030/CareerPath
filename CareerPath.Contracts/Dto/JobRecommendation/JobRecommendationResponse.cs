namespace CareerPath.Contracts.Dto.JobRecommendation;

/// <summary>
/// Complete job recommendation result with ranked matches
/// </summary>
public class JobRecommendationResponse
{
    public string CVAnalysisId { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
    public List<JobMatchDto> MatchedJobs { get; set; } = new();
    public int TotalJobsEvaluated { get; set; }
    public int RecommendationCount { get; set; }
    public DateTime GeneratedAt { get; set; } = DateTime.UtcNow;
    public long ProcessingTimeMs { get; set; }
    public Dictionary<string, object> Metadata { get; set; } = new();
}
