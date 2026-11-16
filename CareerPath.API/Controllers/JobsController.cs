using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using CareerPath.Application.Services;
using CareerPath.Application.Interfaces;
using CareerPath.Infrastructure.Services;
using CareerPath.Domain.Recommendations;

namespace CareerPath.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class JobsController : ControllerBase
    {
        private readonly IJobManagementService _jobManagementService;
        private readonly IJobBoardService _jobBoardService;
        private readonly IJobQueryService _jobQueryService;
        private readonly ILogger<JobsController> _logger;

        public JobsController(
            IJobManagementService jobManagementService,
            IJobBoardService jobBoardService,
            IJobQueryService jobQueryService,
            ILogger<JobsController> logger)
        {
            _jobManagementService = jobManagementService ?? throw new ArgumentNullException(nameof(jobManagementService));
            _jobBoardService = jobBoardService ?? throw new ArgumentNullException(nameof(jobBoardService));
            _jobQueryService = jobQueryService ?? throw new ArgumentNullException(nameof(jobQueryService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Gets all jobs with pagination
        /// </summary>
        /// <param name="pageNumber">Page number (default: 1)</param>
        /// <param name="pageSize">Items per page (default: 20, max: 100)</param>
        [HttpGet]
        public async Task<ActionResult<object>> GetJobs(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 20)
        {
            try
            {
                if (pageNumber < 1)
                    return BadRequest("Page number must be greater than 0");

                if (pageSize < 1 || pageSize > 100)
                    return BadRequest("Page size must be between 1 and 100");

                var result = await _jobQueryService.GetAllJobsAsync(pageNumber, pageSize);

                if (!result.IsSuccess)
                {
                    _logger.LogWarning("Failed to retrieve jobs: {Message}", result.Message);
                    return StatusCode(500, new { message = result.Message });
                }

                return Ok(new
                {
                    jobs = result.Jobs,
                    pagination = new
                    {
                        currentPage = result.PageNumber,
                        pageSize = result.PageSize,
                        totalCount = result.TotalCount,
                        totalPages = result.TotalPages,
                        hasNextPage = result.HasNextPage,
                        hasPreviousPage = result.HasPreviousPage
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving jobs");
                return StatusCode(500, new { message = "Internal server error" });
            }
        }

        /// <summary>
        /// Gets a single job by ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<Job>> GetJob(string id)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(id))
                    return BadRequest("Job ID is required");

                var result = await _jobQueryService.GetJobByIdAsync(id);

                if (!result.IsSuccess || !result.Jobs.Any())
                {
                    return NotFound(new { message = $"Job with ID {id} not found" });
                }

                return Ok(result.Jobs.First());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving job {JobId}", id);
                return StatusCode(500, new { message = "Internal server error" });
            }
        }

        /// <summary>
        /// Advanced search with multiple filters
        /// </summary>
        [HttpPost("search")]
        public async Task<ActionResult<object>> SearchJobs([FromBody] JobSearchCriteria criteria)
        {
            try
            {
                if (criteria == null)
                    return BadRequest("Search criteria is required");

                if (!criteria.IsValid())
                    return BadRequest("Invalid search criteria: page number and size must be positive, page size max 100");

                var result = await _jobQueryService.SearchJobsAsync(criteria);

                if (!result.IsSuccess)
                {
                    _logger.LogWarning("Job search failed: {Message}", result.Message);
                    return StatusCode(500, new { message = result.Message });
                }

                return Ok(new
                {
                    jobs = result.Jobs,
                    pagination = new
                    {
                        currentPage = result.PageNumber,
                        pageSize = result.PageSize,
                        totalCount = result.TotalCount,
                        totalPages = result.TotalPages,
                        hasNextPage = result.HasNextPage,
                        hasPreviousPage = result.HasPreviousPage
                    },
                    criteria = new
                    {
                        keywords = criteria.Keywords,
                        location = criteria.Location,
                        experienceLevel = criteria.ExperienceLevel,
                        employmentType = criteria.EmploymentType,
                        hasFilters = criteria.HasFilters()
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching jobs");
                return StatusCode(500, new { message = "Internal server error" });
            }
        }

        /// <summary>
        /// Simple search by keyword (legacy endpoint for backward compatibility)
        /// </summary>
        [HttpGet("search")]
        public async Task<ActionResult<object>> SearchJobsSimple(
            [FromQuery] string searchTerm,
            [FromQuery] string? location = null,
            [FromQuery] string? experienceLevel = null,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 20)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(searchTerm))
                    return BadRequest("Search term is required");

                var criteria = new JobSearchCriteria
                {
                    Keywords = searchTerm,
                    Location = location,
                    ExperienceLevel = experienceLevel,
                    PageNumber = pageNumber,
                    PageSize = pageSize
                };

                var result = await _jobQueryService.SearchJobsAsync(criteria);

                if (!result.IsSuccess)
                {
                    return StatusCode(500, new { message = result.Message });
                }

                return Ok(new
                {
                    jobs = result.Jobs,
                    totalCount = result.TotalCount,
                    currentPage = result.PageNumber,
                    totalPages = result.TotalPages
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching jobs");
                return StatusCode(500, new { message = "Internal server error" });
            }
        }

        [HttpPost("ingest")]
        public async Task<ActionResult<List<Job>>> IngestJobs(
            [FromQuery] string searchTerm,
            [FromQuery] string? location = null)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(searchTerm))
                    return BadRequest("Search term is required");

                var jobs = await _jobBoardService.FetchAndIngestJobsAsync(searchTerm, location);
                return Ok(jobs);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error ingesting jobs");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPut("{id}/fill")]
        public async Task<ActionResult<Job>> MarkJobAsFilled(string id)
        {
            try
            {
                var job = await _jobManagementService.MarkJobAsFilled(id);
                return Ok(job);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error marking job {JobId} as filled", id);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPut("{id}/expire")]
        public async Task<ActionResult<Job>> ExpireJob(string id)
        {
            try
            {
                var job = await _jobManagementService.ExpireJob(id);
                return Ok(job);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error expiring job {JobId}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPut("{id}/reactivate")]
        public async Task<ActionResult<Job>> ReactivateJob(string id)
        {
            try
            {
                var job = await _jobManagementService.ReactivateJob(id);
                return Ok(job);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error reactivating job {JobId}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Gets recently posted jobs (within last N days)
        /// </summary>
        [HttpGet("recent")]
        public async Task<ActionResult<object>> GetRecentJobs(
            [FromQuery] int daysBack = 7,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 20)
        {
            try
            {
                if (daysBack < 1 || daysBack > 365)
                    return BadRequest("Days back must be between 1 and 365");

                var result = await _jobQueryService.GetRecentJobsAsync(daysBack, pageNumber, pageSize);

                if (!result.IsSuccess)
                {
                    return StatusCode(500, new { message = result.Message });
                }

                return Ok(new
                {
                    jobs = result.Jobs,
                    daysBack,
                    pagination = new
                    {
                        currentPage = result.PageNumber,
                        pageSize = result.PageSize,
                        totalCount = result.TotalCount,
                        totalPages = result.TotalPages
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving recent jobs");
                return StatusCode(500, new { message = "Internal server error" });
            }
        }

        /// <summary>
        /// Gets jobs from a specific source provider (e.g., Jooble, Indeed)
        /// </summary>
        [HttpGet("source/{sourceProvider}")]
        public async Task<ActionResult<object>> GetJobsBySource(
            string sourceProvider,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 20)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(sourceProvider))
                    return BadRequest("Source provider is required");

                var result = await _jobQueryService.GetJobsBySourceAsync(sourceProvider, pageNumber, pageSize);

                if (!result.IsSuccess)
                {
                    return StatusCode(500, new { message = result.Message });
                }

                return Ok(new
                {
                    source = sourceProvider,
                    jobs = result.Jobs,
                    pagination = new
                    {
                        currentPage = result.PageNumber,
                        pageSize = result.PageSize,
                        totalCount = result.TotalCount,
                        totalPages = result.TotalPages
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving jobs from source {Source}", sourceProvider);
                return StatusCode(500, new { message = "Internal server error" });
            }
        }

        /// <summary>
        /// Gets similar/related jobs based on a reference job
        /// </summary>
        [HttpGet("{id}/similar")]
        public async Task<ActionResult<object>> GetSimilarJobs(
            string id,
            [FromQuery] int limit = 10)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(id))
                    return BadRequest("Job ID is required");

                if (limit < 1 || limit > 50)
                    return BadRequest("Limit must be between 1 and 50");

                var result = await _jobQueryService.GetSimilarJobsAsync(id, limit);

                if (!result.IsSuccess)
                {
                    if (result.Message?.Contains("not found") == true)
                        return NotFound(new { message = result.Message });

                    return StatusCode(500, new { message = result.Message });
                }

                return Ok(new
                {
                    referenceJobId = id,
                    similarJobs = result.Jobs,
                    count = result.TotalCount
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving similar jobs for {JobId}", id);
                return StatusCode(500, new { message = "Internal server error" });
            }
        }

        /// <summary>
        /// Gets comprehensive job statistics for dashboards
        /// </summary>
        [HttpGet("stats")]
        public async Task<ActionResult<JobStatistics>> GetJobStats()
        {
            try
            {
                var statistics = await _jobQueryService.GetJobStatisticsAsync();

                return Ok(statistics);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving job statistics");
                return StatusCode(500, new { message = "Internal server error" });
            }
        }
    }
}