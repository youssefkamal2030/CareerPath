using Microsoft.Extensions.Logging;
using CareerPath.Application.Interfaces;
using CareerPath.Domain.Recommendations;

namespace CareerPath.Application.Services;

/// <summary>
/// Service for querying and retrieving jobs from the database with filtering, search, and pagination
/// This is the main service for displaying jobs to users
/// </summary>
public class JobQueryService : IJobQueryService
{
    private readonly IJobRepository _jobRepository;
    private readonly ILogger<JobQueryService> _logger;

    public JobQueryService(
        IJobRepository jobRepository,
        ILogger<JobQueryService> logger)
    {
        _jobRepository = jobRepository ?? throw new ArgumentNullException(nameof(jobRepository));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Gets a single job by ID
    /// </summary>
    public async Task<JobQueryResult> GetJobByIdAsync(string jobId)
    {
        try
        {
            _logger.LogInformation("Fetching job with ID: {JobId}", jobId);

            var job = await _jobRepository.GetByIdAsync(jobId);

            if (job == null)
            {
                _logger.LogWarning("Job with ID {JobId} not found", jobId);
                return JobQueryResult.NotFound($"Job with ID {jobId} not found");
            }

            return JobQueryResult.Success(new List<Job> { job }, 1);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching job with ID {JobId}", jobId);
            return JobQueryResult.Error($"Error retrieving job: {ex.Message}");
        }
    }

    /// <summary>
    /// Gets all active jobs with pagination
    /// </summary>
    public async Task<JobQueryResult> GetAllJobsAsync(int pageNumber = 1, int pageSize = 20)
    {
        try
        {
            _logger.LogInformation("Fetching all jobs - Page: {Page}, Size: {Size}", pageNumber, pageSize);

            var allJobs = await _jobRepository.GetAllAsync();
            var activeJobs = allJobs.Where(j => j.Status == JobStatus.Active).ToList();

            var totalCount = activeJobs.Count;
            var paginatedJobs = activeJobs
                .OrderByDescending(j => j.PostedDate)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            _logger.LogInformation("Retrieved {Count} jobs out of {Total}", paginatedJobs.Count, totalCount);

            return JobQueryResult.Success(paginatedJobs, totalCount, pageNumber, pageSize);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching all jobs");
            return JobQueryResult.Error($"Error retrieving jobs: {ex.Message}");
        }
    }

    /// <summary>
    /// Searches jobs with filters and pagination
    /// </summary>
    public async Task<JobQueryResult> SearchJobsAsync(JobSearchCriteria criteria)
    {
        try
        {
            _logger.LogInformation("Searching jobs with criteria: {@Criteria}", criteria);

            // Get all jobs (in a real scenario, this should be done at DB level with IQueryable)
            var allJobs = await _jobRepository.GetAllAsync();
            var query = allJobs.Where(j => j.Status == JobStatus.Active);

            // Apply filters
            if (!string.IsNullOrWhiteSpace(criteria.Keywords))
            {
                var keywords = criteria.Keywords.ToLowerInvariant();
                query = query.Where(j =>
                    j.JobTitle.ToLowerInvariant().Contains(keywords) ||
                    j.JobDescription.ToLowerInvariant().Contains(keywords) ||
                    j.RequiredSkills.ToLowerInvariant().Contains(keywords));
            }

            if (!string.IsNullOrWhiteSpace(criteria.Location))
            {
                var location = criteria.Location.ToLowerInvariant();
                query = query.Where(j => j.JobLocation.ToLowerInvariant().Contains(location));
            }

            if (!string.IsNullOrWhiteSpace(criteria.Company))
            {
                var company = criteria.Company.ToLowerInvariant();
                query = query.Where(j => j.CompanyName.ToLowerInvariant().Contains(company));
            }

            if (!string.IsNullOrWhiteSpace(criteria.ExperienceLevel))
            {
                query = query.Where(j => j.ExperienceLevel.Equals(criteria.ExperienceLevel, StringComparison.OrdinalIgnoreCase));
            }

            if (!string.IsNullOrWhiteSpace(criteria.EmploymentType))
            {
                query = query.Where(j => j.EmploymentType.Equals(criteria.EmploymentType, StringComparison.OrdinalIgnoreCase));
            }

            if (!string.IsNullOrWhiteSpace(criteria.SourceProvider))
            {
                query = query.Where(j => j.SourceProvider.Equals(criteria.SourceProvider, StringComparison.OrdinalIgnoreCase));
            }

            if (criteria.MinSalary.HasValue || criteria.MaxSalary.HasValue)
            {
                // Note: This is a basic filter - salary range parsing would need enhancement
                query = query.Where(j => !string.IsNullOrWhiteSpace(j.SalaryRange));
            }

            if (criteria.PostedAfter.HasValue)
            {
                query = query.Where(j => j.PostedDate >= criteria.PostedAfter.Value);
            }

            // Apply sorting
            query = criteria.SortBy?.ToLowerInvariant() switch
            {
                "title" => criteria.SortDescending
                    ? query.OrderByDescending(j => j.JobTitle)
                    : query.OrderBy(j => j.JobTitle),
                "company" => criteria.SortDescending
                    ? query.OrderByDescending(j => j.CompanyName)
                    : query.OrderBy(j => j.CompanyName),
                "location" => criteria.SortDescending
                    ? query.OrderByDescending(j => j.JobLocation)
                    : query.OrderBy(j => j.JobLocation),
                "date" or "posteddate" => criteria.SortDescending
                    ? query.OrderByDescending(j => j.PostedDate)
                    : query.OrderBy(j => j.PostedDate),
                _ => query.OrderByDescending(j => j.PostedDate) // Default: newest first
            };

            var totalCount = query.Count();

            // Apply pagination
            var paginatedJobs = query
                .Skip((criteria.PageNumber - 1) * criteria.PageSize)
                .Take(criteria.PageSize)
                .ToList();

            _logger.LogInformation("Search returned {Count} jobs out of {Total} total matches",
                paginatedJobs.Count, totalCount);

            return JobQueryResult.Success(paginatedJobs, totalCount, criteria.PageNumber, criteria.PageSize);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching jobs with criteria {@Criteria}", criteria);
            return JobQueryResult.Error($"Error searching jobs: {ex.Message}");
        }
    }

    /// <summary>
    /// Gets jobs by specific source provider (e.g., "Jooble", "Indeed")
    /// </summary>
    public async Task<JobQueryResult> GetJobsBySourceAsync(string sourceProvider, int pageNumber = 1, int pageSize = 20)
    {
        try
        {
            _logger.LogInformation("Fetching jobs from source: {Source}", sourceProvider);

            var jobs = await _jobRepository.GetJobsBySourceProvider(sourceProvider);
            var activeJobs = jobs.Where(j => j.Status == JobStatus.Active).ToList();

            var totalCount = activeJobs.Count;
            var paginatedJobs = activeJobs
                .OrderByDescending(j => j.PostedDate)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return JobQueryResult.Success(paginatedJobs, totalCount, pageNumber, pageSize);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching jobs from source {Source}", sourceProvider);
            return JobQueryResult.Error($"Error retrieving jobs: {ex.Message}");
        }
    }

    /// <summary>
    /// Gets recently posted jobs (within last N days)
    /// </summary>
    public async Task<JobQueryResult> GetRecentJobsAsync(int daysBack = 7, int pageNumber = 1, int pageSize = 20)
    {
        try
        {
            _logger.LogInformation("Fetching jobs posted in last {Days} days", daysBack);

            var cutoffDate = DateTime.UtcNow.AddDays(-daysBack);
            var allJobs = await _jobRepository.GetAllAsync();
            
            var recentJobs = allJobs
                .Where(j => j.Status == JobStatus.Active && j.PostedDate >= cutoffDate)
                .OrderByDescending(j => j.PostedDate)
                .ToList();

            var totalCount = recentJobs.Count;
            var paginatedJobs = recentJobs
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return JobQueryResult.Success(paginatedJobs, totalCount, pageNumber, pageSize);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching recent jobs");
            return JobQueryResult.Error($"Error retrieving recent jobs: {ex.Message}");
        }
    }

    /// <summary>
    /// Gets job statistics for dashboard/overview
    /// </summary>
    public async Task<JobStatistics> GetJobStatisticsAsync()
    {
        try
        {
            _logger.LogInformation("Calculating job statistics");

            var allJobs = await _jobRepository.GetAllAsync();
            var activeJobs = allJobs.Where(j => j.Status == JobStatus.Active).ToList();

            var statistics = new JobStatistics
            {
                TotalActiveJobs = activeJobs.Count,
                TotalJobs = allJobs.Count(),
                JobsBySource = activeJobs
                    .GroupBy(j => j.SourceProvider)
                    .ToDictionary(g => g.Key, g => g.Count()),
                JobsByExperienceLevel = activeJobs
                    .GroupBy(j => j.ExperienceLevel)
                    .ToDictionary(g => g.Key, g => g.Count()),
                JobsByEmploymentType = activeJobs
                    .GroupBy(j => j.EmploymentType)
                    .ToDictionary(g => g.Key, g => g.Count()),
                JobsPostedToday = activeJobs.Count(j => j.PostedDate.Date == DateTime.UtcNow.Date),
                JobsPostedThisWeek = activeJobs.Count(j => j.PostedDate >= DateTime.UtcNow.AddDays(-7)),
                JobsPostedThisMonth = activeJobs.Count(j => j.PostedDate >= DateTime.UtcNow.AddDays(-30)),
                TopCompanies = activeJobs
                    .GroupBy(j => j.CompanyName)
                    .OrderByDescending(g => g.Count())
                    .Take(10)
                    .ToDictionary(g => g.Key, g => g.Count()),
                TopLocations = activeJobs
                    .GroupBy(j => j.JobLocation)
                    .OrderByDescending(g => g.Count())
                    .Take(10)
                    .ToDictionary(g => g.Key, g => g.Count())
            };

            _logger.LogInformation("Calculated statistics: {Total} active jobs", statistics.TotalActiveJobs);

            return statistics;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calculating job statistics");
            throw;
        }
    }

    /// <summary>
    /// Gets suggested/related jobs based on a job's characteristics
    /// </summary>
    public async Task<JobQueryResult> GetSimilarJobsAsync(string jobId, int limit = 10)
    {
        try
        {
            _logger.LogInformation("Finding similar jobs to {JobId}", jobId);

            var referenceJob = await _jobRepository.GetByIdAsync(jobId);
            if (referenceJob == null)
            {
                return JobQueryResult.NotFound($"Reference job {jobId} not found");
            }

            var allJobs = await _jobRepository.GetAllAsync();
            
            // Find similar jobs based on title, company, location, or skills
            var similarJobs = allJobs
                .Where(j => j.JobId != jobId && j.Status == JobStatus.Active)
                .Where(j =>
                    j.CompanyName.Equals(referenceJob.CompanyName, StringComparison.OrdinalIgnoreCase) ||
                    j.JobLocation.Equals(referenceJob.JobLocation, StringComparison.OrdinalIgnoreCase) ||
                    j.ExperienceLevel.Equals(referenceJob.ExperienceLevel, StringComparison.OrdinalIgnoreCase) ||
                    HasCommonKeywords(j.JobTitle, referenceJob.JobTitle))
                .OrderByDescending(j => j.PostedDate)
                .Take(limit)
                .ToList();

            return JobQueryResult.Success(similarJobs, similarJobs.Count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error finding similar jobs for {JobId}", jobId);
            return JobQueryResult.Error($"Error retrieving similar jobs: {ex.Message}");
        }
    }

    #region Helper Methods

    private bool HasCommonKeywords(string title1, string title2)
    {
        var keywords1 = title1.ToLowerInvariant().Split(' ', StringSplitOptions.RemoveEmptyEntries);
        var keywords2 = title2.ToLowerInvariant().Split(' ', StringSplitOptions.RemoveEmptyEntries);
        
        return keywords1.Intersect(keywords2).Any();
    }

    #endregion
}
