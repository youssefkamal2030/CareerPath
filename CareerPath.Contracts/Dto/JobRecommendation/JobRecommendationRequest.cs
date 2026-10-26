namespace CareerPath.Contracts.Dto.JobRecommendation;

/// <summary>
/// Request for generating job recommendations from CV analysis
/// </summary>
public class JobRecommendationRequest
{
    public string CVAnalysisId { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
    public int MaxResults { get; set; } = 20;
    public decimal MinimumMatchScore { get; set; } = 50m;
    public bool IncludeExplanations { get; set; } = true;
    public List<string> PreferredLocations { get; set; } = new();
    public List<string> PreferredSources { get; set; } = new();
}
