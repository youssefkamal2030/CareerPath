namespace CareerPath.Domain.Recommendations;

/// <summary>
/// Domain entity for job matching criteria and weights
/// </summary>
public class JobMatchCriteria
{
    public decimal SkillsWeight { get; set; } = 0.4m;
    public decimal ExperienceWeight { get; set; } = 0.3m;
    public decimal EducationWeight { get; set; } = 0.2m;
    public decimal LocationWeight { get; set; } = 0.1m;
    public decimal MinimumScore { get; set; } = 50m;
    public int MaxResults { get; set; } = 20;
    public List<string> PreferredLocations { get; set; } = new();
    public List<string> PreferredSources { get; set; } = new();
    public bool IncludeRemoteJobs { get; set; } = true;
    public int MaxJobAgeDays { get; set; } = 30;

    public void ValidateWeights()
    {
        var totalWeight = SkillsWeight + ExperienceWeight + EducationWeight + LocationWeight;
        if (Math.Abs(totalWeight - 1.0m) > 0.01m)
            throw new InvalidOperationException("Weights must sum to 1.0");

        if (SkillsWeight < 0 || ExperienceWeight < 0 || EducationWeight < 0 || LocationWeight < 0)
            throw new InvalidOperationException("Weights cannot be negative");
    }
}
