using CareerPath.Domain.Recommendations;

namespace CareerPath.Application.Services;

/// <summary>
/// Result wrapper for job query operations with pagination and status information
/// </summary>
public class JobQueryResult
{
    public bool IsSuccess { get; set; }
    public string? Message { get; set; }
    public List<Job> Jobs { get; set; } = new List<Job>();
    public int TotalCount { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalPages => PageSize > 0 ? (int)Math.Ceiling((double)TotalCount / PageSize) : 0;
    public bool HasNextPage => PageNumber < TotalPages;
    public bool HasPreviousPage => PageNumber > 1;

    /// <summary>
    /// Creates a successful result with jobs
    /// </summary>
    public static JobQueryResult Success(List<Job> jobs, int totalCount, int pageNumber = 1, int pageSize = 20)
    {
        return new JobQueryResult
        {
            IsSuccess = true,
            Jobs = jobs,
            TotalCount = totalCount,
            PageNumber = pageNumber,
            PageSize = pageSize,
            Message = $"Retrieved {jobs.Count} job(s)"
        };
    }

    /// <summary>
    /// Creates an error result
    /// </summary>
    public static JobQueryResult Error(string message)
    {
        return new JobQueryResult
        {
            IsSuccess = false,
            Message = message,
            Jobs = new List<Job>(),
            TotalCount = 0
        };
    }

    /// <summary>
    /// Creates a not found result
    /// </summary>
    public static JobQueryResult NotFound(string message)
    {
        return new JobQueryResult
        {
            IsSuccess = false,
            Message = message,
            Jobs = new List<Job>(),
            TotalCount = 0
        };
    }
}
