using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using CareerPath.Application.Services;
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
        private readonly ILogger<JobsController> _logger;

        public JobsController(
            IJobManagementService jobManagementService,
            IJobBoardService jobBoardService,
            ILogger<JobsController> logger)
        {
            _jobManagementService = jobManagementService ?? throw new ArgumentNullException(nameof(jobManagementService));
            _jobBoardService = jobBoardService ?? throw new ArgumentNullException(nameof(jobBoardService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Job>>> GetJobs()
        {
            try
            {
                var jobs = await _jobManagementService.GetActiveJobs();
                return Ok(jobs);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving jobs");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Job>> GetJob(string id)
        {
            try
            {
                var job = await _jobManagementService.GetJobById(id);
                return Ok(job);
            }
            catch (InvalidOperationException)
            {
                return NotFound($"Job with ID {id} not found");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving job {JobId}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<Job>>> SearchJobs(
            [FromQuery] string searchTerm,
            [FromQuery] string? location = null,
            [FromQuery] string? experienceLevel = null)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(searchTerm))
                    return BadRequest("Search term is required");

                var jobs = await _jobManagementService.SearchJobs(searchTerm, location, experienceLevel);
                return Ok(jobs);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching jobs");
                return StatusCode(500, "Internal server error");
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

        [HttpGet("stats")]
        public async Task<ActionResult<object>> GetJobStats()
        {
            try
            {
                var activeCount = await _jobManagementService.GetActiveJobsCount();
                var recentJobs = await _jobManagementService.GetRecentJobs(5);

                return Ok(new
                {
                    ActiveJobsCount = activeCount,
                    RecentJobs = recentJobs
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving job statistics");
                return StatusCode(500, "Internal server error");
            }
        }
    }
}