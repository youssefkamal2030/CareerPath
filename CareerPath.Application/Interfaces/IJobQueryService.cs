using CareerPath.Application.Services;

namespace CareerPath.Application.Interfaces;

/// <summary>
/// Service interface for querying and retrieving jobs with various filters
/// </summary>
public interface IJobQueryService
{
    /// <summary>
    /// Gets a single job by its ID
    /// </summary>
    Task<JobQueryResult> GetJobByIdAsync(string jobId);

    /// <summary>
    /// Gets all active jobs with pagination
    /// </summary>
    Task<JobQueryResult> GetAllJobsAsync(int pageNumber = 1, int pageSize = 20);

    /// <summary>
    /// Searches jobs based on multiple criteria
    /// </summary>
    Task<JobQueryResult> SearchJobsAsync(JobSearchCriteria criteria);

    /// <summary>
    /// Gets jobs from a specific source provider (e.g., Jooble, Indeed)
    /// </summary>
    Task<JobQueryResult> GetJobsBySourceAsync(string sourceProvider, int pageNumber = 1, int pageSize = 20);

    /// <summary>
    /// Gets recently posted jobs within the specified number of days
    /// </summary>
    Task<JobQueryResult> GetRecentJobsAsync(int daysBack = 7, int pageNumber = 1, int pageSize = 20);

    /// <summary>
    /// Gets comprehensive job statistics for dashboards
    /// </summary>
    Task<JobStatistics> GetJobStatisticsAsync();

    /// <summary>
    /// Gets jobs similar to a reference job
    /// </summary>
    Task<JobQueryResult> GetSimilarJobsAsync(string jobId, int limit = 10);
}
