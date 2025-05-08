using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using CareerPath.Application.Interfaces;
using CareerPath.Contracts.Dto;
using System.Threading.Tasks;
using System.Security.Claims;
using System;
using Microsoft.Extensions.Logging;
using CareerPath.Application.Services;
using System.Collections.Generic;

namespace CareerPath.Api.Controllers
{
    [ApiController]
    [Route("api/jobs")]
    public class JobsController : ControllerBase
    {
        private readonly IJobsService _jobsService;
        private readonly JobNotificationService _notificationService;
        private readonly ILogger<JobsController> _logger;

        public JobsController(
            IJobsService jobsService, 
            JobNotificationService notificationService,
            ILogger<JobsController> logger)
        {
            _jobsService = jobsService ?? throw new ArgumentNullException(nameof(jobsService));
            _notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Get all jobs
        /// </summary>
        /// <returns>List of all available jobs</returns>
        [HttpGet]
        [ProducesResponseType(typeof(JobDto[]), 200)]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var jobs = await _jobsService.GetAllJobsAsync();
                return Ok(jobs);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all jobs");
                return StatusCode(500, new { Error = "An error occurred while retrieving jobs" });
            }
        }

        /// <summary>
        /// Get a job by ID
        /// </summary>
        /// <param name="id">Job ID</param>
        /// <returns>Job details</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(JobDto), 200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetById(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    return BadRequest(new { Error = "Job ID cannot be empty" });
                }

                var job = await _jobsService.GetJobByIdAsync(id);
                if (job == null)
                {
                    return NotFound(new { Error = $"Job with ID {id} not found" });
                }

                return Ok(job);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving job with ID {JobId}", id);
                return StatusCode(500, new { Error = "An error occurred while retrieving the job" });
            }
        }

        /// <summary>
        /// Get jobs for a specific user/company
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <returns>List of jobs for the user</returns>
        [HttpGet("user/{userId}")]
        [ProducesResponseType(typeof(JobDto[]), 200)]
        public async Task<IActionResult> GetByUserId(string userId)
        {
            try
            {
                if (string.IsNullOrEmpty(userId))
                {
                    return BadRequest(new { Error = "User ID cannot be empty" });
                }

                var jobs = await _jobsService.GetJobsByUserIdAsync(userId);
                return Ok(jobs);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving jobs for user {UserId}", userId);
                return StatusCode(500, new { Error = "An error occurred while retrieving jobs" });
            }
        }

        /// <summary>
        /// Create a new job
        /// </summary>
        /// <param name="createJobDto">Job creation data</param>
        /// <returns>Created job details</returns>
        [HttpPost]
        [Authorize]
        [ProducesResponseType(typeof(JobDto), 201)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> Create([FromBody] CreateJobDto createJobDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized();
                }

                var createdJob = await _jobsService.CreateJobAsync(createJobDto);
                
                // Send job notifications in the background - don't await
                _ = Task.Run(async () =>
                {
                    try
                    {
                        // In a real application, you would fetch relevant subscribers based on job criteria
                        // For this example, we'll use a placeholder list
                        List<string> relevantSubscribers = GetRelevantSubscribers(createdJob);
                        
                        if (relevantSubscribers.Count > 0)
                        {
                            await _notificationService.NotifyNewJobPostingAsync(createdJob, relevantSubscribers);
        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Failed to send job notifications");
                    }
                });
                
                return CreatedAtAction(nameof(GetById), new { id = createdJob.JobId }, createdJob);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Validation error when creating job");
                return BadRequest(new { Error = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating job");
                return StatusCode(500, new { Error = "An error occurred while creating the job" });
            }
        }

        /// <summary>
        /// Update an existing job
        /// </summary>
        /// <param name="id">Job ID</param>
        /// <param name="updateJobDto">Job update data</param>
        /// <returns>Updated job details</returns>
        [HttpPut("{id}")]
        [Authorize]
        [ProducesResponseType(typeof(JobDto), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Update(string id, [FromBody] UpdateJobDto updateJobDto)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    return BadRequest(new { Error = "Job ID cannot be empty" });
                }

                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized();
                }

                // Get the job to verify ownership (future enhancement)
                // var existingJob = await _jobsService.GetJobByIdAsync(id);
                // if (existingJob.CompanyName != userId) // This is a simplified check
                //     return Forbid();

                var updatedJob = await _jobsService.UpdateJobAsync(id, updateJobDto);
                if (updatedJob == null)
                {
                    return NotFound(new { Error = $"Job with ID {id} not found" });
                }

                return Ok(updatedJob);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Validation error when updating job {JobId}", id);
                return BadRequest(new { Error = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating job {JobId}", id);
                return StatusCode(500, new { Error = "An error occurred while updating the job" });
            }
        }

        /// <summary>
        /// Delete a job
        /// </summary>
        /// <param name="id">Job ID</param>
        /// <returns>No content if successful</returns>
        [HttpDelete("{id}")]
        [Authorize]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Delete(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    return BadRequest(new { Error = "Job ID cannot be empty" });
                }

                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized();
                }

                // Get the job to verify ownership (future enhancement)
                // var existingJob = await _jobsService.GetJobByIdAsync(id);
                // if (existingJob == null)
                //     return NotFound(new { Error = $"Job with ID {id} not found" });
                // if (existingJob.CompanyName != userId)
                //     return Forbid();

                var result = await _jobsService.DeleteJobAsync(id);
                if (!result)
                {
                    return NotFound(new { Error = $"Job with ID {id} not found" });
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting job {JobId}", id);
                return StatusCode(500, new { Error = "An error occurred while deleting the job" });
            }
        }

        /// <summary>
        /// Apply for a job
        /// </summary>
        /// <param name="id">Job ID</param>
        /// <param name="email">Applicant email</param>
        /// <returns>Success status</returns>
        [HttpPost("{id}/apply")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> ApplyForJob(string id, [FromBody] JobApplicationDto application)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    return BadRequest(new { Error = "Job ID cannot be empty" });
                }

                if (application == null || string.IsNullOrWhiteSpace(application.Email))
                {
                    return BadRequest(new { Error = "Applicant email is required" });
                }

                var job = await _jobsService.GetJobByIdAsync(id);
                if (job == null)
                {
                    return NotFound(new { Error = $"Job with ID {id} not found" });
                }

                // In a real application, you would save the application to the database
                // For this example, we'll just send a confirmation email
                
                // Send application confirmation email in the background - don't await
                _ = Task.Run(async () =>
                {
                    try
                    {
                        await _notificationService.SendApplicationConfirmationAsync(application.Email, job);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Failed to send application confirmation email");
                    }
                });

                return Ok(new { Success = true, Message = "Application submitted successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error applying for job {JobId}", id);
                return StatusCode(500, new { Error = "An error occurred while submitting your application" });
            }
        }

        /// <summary>
        /// Get a list of relevant subscribers based on job criteria
        /// </summary>
        /// <param name="job">The job</param>
        /// <returns>List of subscriber emails</returns>
        private List<string> GetRelevantSubscribers(JobDto job)
        {
            // In a real application, you would fetch subscribers from a database
            // based on their preferences (skills, location, etc.) matching the job criteria
            // For this example, we'll return a placeholder list
            return new List<string>
            {
                "subscriber1@example.com",
                "subscriber2@example.com"
            };
        }
    }

    public class JobApplicationDto
    {
        public string Email { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public string Resume { get; set; } // This could be a file URL or base64 encoded content
    }
}
