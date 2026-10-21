namespace CareerPath.Application.Services;

/// <summary>
/// Search criteria for filtering and querying jobs
/// </summary>
public class JobSearchCriteria
{
    /// <summary>
    /// Keywords to search in title, description, and skills
    /// </summary>
    public string? Keywords { get; set; }

    /// <summary>
    /// Location to filter jobs (city, state, country)
    /// </summary>
    public string? Location { get; set; }

    /// <summary>
    /// Company name to filter
    /// </summary>
    public string? Company { get; set; }

    /// <summary>
    /// Experience level (e.g., "Entry Level", "Mid Level", "Senior Level")
    /// </summary>
    public string? ExperienceLevel { get; set; }

    /// <summary>
    /// Employment type (e.g., "Full-time", "Part-time", "Contract")
    /// </summary>
    public string? EmploymentType { get; set; }

    /// <summary>
    /// Source provider (e.g., "Jooble", "Indeed")
    /// </summary>
    public string? SourceProvider { get; set; }

    /// <summary>
    /// Minimum salary requirement
    /// </summary>
    public decimal? MinSalary { get; set; }

    /// <summary>
    /// Maximum salary limit
    /// </summary>
    public decimal? MaxSalary { get; set; }

    /// <summary>
    /// Only show jobs posted after this date
    /// </summary>
    public DateTime? PostedAfter { get; set; }

    /// <summary>
    /// Page number for pagination (1-based)
    /// </summary>
    public int PageNumber { get; set; } = 1;

    /// <summary>
    /// Number of results per page
    /// </summary>
    public int PageSize { get; set; } = 20;

    /// <summary>
    /// Field to sort by (e.g., "date", "title", "company", "location")
    /// </summary>
    public string? SortBy { get; set; } = "date";

    /// <summary>
    /// Sort in descending order (true) or ascending (false)
    /// </summary>
    public bool SortDescending { get; set; } = true;

    /// <summary>
    /// Validates the search criteria
    /// </summary>
    public bool IsValid()
    {
        return PageNumber > 0 && PageSize > 0 && PageSize <= 100;
    }

    /// <summary>
    /// Checks if any filter criteria is specified
    /// </summary>
    public bool HasFilters()
    {
        return !string.IsNullOrWhiteSpace(Keywords) ||
               !string.IsNullOrWhiteSpace(Location) ||
               !string.IsNullOrWhiteSpace(Company) ||
               !string.IsNullOrWhiteSpace(ExperienceLevel) ||
               !string.IsNullOrWhiteSpace(EmploymentType) ||
               !string.IsNullOrWhiteSpace(SourceProvider) ||
               MinSalary.HasValue ||
               MaxSalary.HasValue ||
               PostedAfter.HasValue;
    }
}
