namespace CareerPath.Application.Services;

/// <summary>
/// Aggregated statistics about jobs in the system
/// Useful for dashboards and analytics
/// </summary>
public class JobStatistics
{
    /// <summary>
    /// Total number of active jobs in the system
    /// </summary>
    public int TotalActiveJobs { get; set; }

    /// <summary>
    /// Total number of all jobs (including inactive/expired)
    /// </summary>
    public int TotalJobs { get; set; }

    /// <summary>
    /// Number of jobs grouped by source provider (e.g., Jooble: 150, Indeed: 200)
    /// </summary>
    public Dictionary<string, int> JobsBySource { get; set; } = new();

    /// <summary>
    /// Number of jobs grouped by experience level
    /// </summary>
    public Dictionary<string, int> JobsByExperienceLevel { get; set; } = new();

    /// <summary>
    /// Number of jobs grouped by employment type
    /// </summary>
    public Dictionary<string, int> JobsByEmploymentType { get; set; } = new();

    /// <summary>
    /// Number of jobs posted today
    /// </summary>
    public int JobsPostedToday { get; set; }

    /// <summary>
    /// Number of jobs posted in the last 7 days
    /// </summary>
    public int JobsPostedThisWeek { get; set; }

    /// <summary>
    /// Number of jobs posted in the last 30 days
    /// </summary>
    public int JobsPostedThisMonth { get; set; }

    /// <summary>
    /// Top 10 companies by number of job postings
    /// </summary>
    public Dictionary<string, int> TopCompanies { get; set; } = new();

    /// <summary>
    /// Top 10 locations by number of job postings
    /// </summary>
    public Dictionary<string, int> TopLocations { get; set; } = new();

    /// <summary>
    /// Average jobs posted per day (last 30 days)
    /// </summary>
    public double AverageJobsPerDay => JobsPostedThisMonth / 30.0;
}
