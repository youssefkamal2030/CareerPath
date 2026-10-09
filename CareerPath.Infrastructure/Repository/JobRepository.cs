using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CareerPath.Application.Interfaces;
using CareerPath.Domain.Recommendations;
using CareerPath.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CareerPath.Infrastructure.Repository
{
    public class JobRepository : IJobRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<JobRepository> _logger;

        public JobRepository(ApplicationDbContext context, ILogger<JobRepository> logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<Job> CreateJobAsync(Job job)
        {
            try
            {
                if (job == null)
                {
                    throw new ArgumentNullException(nameof(job));
                }

                await _context.Jobs.AddAsync(job);
                await _context.SaveChangesAsync();
                
                _logger.LogInformation("Job with ID {JobId} was created successfully", job.JobId);
                return job;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating a new job");
                throw;
            }
        }

        public async Task<Job> UpdateJobAsync(Job job)
        {
            try
            {
                if (job == null)
                {
                    throw new ArgumentNullException(nameof(job));
                }

                var existingJob = await _context.Jobs.FindAsync(job.JobId);
                if (existingJob == null)
                {
                    _logger.LogWarning("Update job failed: Job with ID {JobId} not found", job.JobId);
                    return null;
                }

                // Update all properties
                _context.Entry(existingJob).CurrentValues.SetValues(job);
                
                await _context.SaveChangesAsync();
                _logger.LogInformation("Job with ID {JobId} was updated successfully", job.JobId);
                
                return existingJob;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating job with ID {JobId}", job?.JobId);
                throw;
            }
        }

        public async Task<bool> DeleteJobAsync(string id)
        {
            try
            {
                var job = await _context.Jobs.FindAsync(id);
                if (job == null)
                {
                    _logger.LogWarning("Delete job failed: Job with ID {JobId} not found", id);
                    return false;
                }

                _context.Jobs.Remove(job);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Job with ID {JobId} was deleted successfully", id);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting job with ID {JobId}", id);
                throw;
            }
        }

        public async Task<Job> GetByIdAsync(string id)
        {
            try
            {
                return await _context.Jobs
                    .AsNoTracking()
                    .FirstOrDefaultAsync(j => j.JobId == id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving job with ID {JobId}", id);
                throw;
            }
        }

        public async Task<IEnumerable<Job>> GetAllAsync()
        {
            try
            {
                return await _context.Jobs
                    .AsNoTracking()
                    .OrderByDescending(j => j.PostingDate)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving all jobs");
                throw;
            }
        }

        public async Task<IEnumerable<Job>> GetByUserIdAsync(string userId)
        {
            try
            {
                return await _context.Jobs
                    .Where(j => j.UserId == userId)
                    .AsNoTracking()
                    .OrderByDescending(j => j.PostingDate)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving jobs for user {UserId}", userId);
                throw;
            }
        }

        // New methods for external job ingestion
        public async Task<bool> ExistsByExternalIdAsync(string externalId, string sourceProvider)
        {
            try
            {
                return await _context.Jobs
                    .AsNoTracking()
                    .AnyAsync(j => j.ExternalId == externalId && j.SourceProvider == sourceProvider);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking if job exists: ExternalId={ExternalId}, Provider={Provider}", 
                    externalId, sourceProvider);
                throw;
            }
        }

        public async Task<Job?> GetByExternalIdAsync(string externalId, string sourceProvider)
        {
            try
            {
                return await _context.Jobs
                    .AsNoTracking()
                    .FirstOrDefaultAsync(j => j.ExternalId == externalId && j.SourceProvider == sourceProvider);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving job by external ID: ExternalId={ExternalId}, Provider={Provider}", 
                    externalId, sourceProvider);
                throw;
            }
        }

        public async Task<IEnumerable<Job>> GetActiveJobsOlderThan(DateTime cutoffDate)
        {
            try
            {
                return await _context.Jobs
                    .Where(j => j.Status == JobStatus.Active && j.PostingDate < cutoffDate)
                    .AsNoTracking()
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving active jobs older than {CutoffDate}", cutoffDate);
                throw;
            }
        }

        public async Task<IEnumerable<Job>> GetJobsBySourceProvider(string sourceProvider)
        {
            try
            {
                return await _context.Jobs
                    .Where(j => j.SourceProvider == sourceProvider)
                    .AsNoTracking()
                    .OrderByDescending(j => j.PostingDate)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving jobs by source provider {Provider}", sourceProvider);
                throw;
            }
        }

        public async Task<IEnumerable<Job>> GetJobsByStatus(JobStatus status)
        {
            try
            {
                return await _context.Jobs
                    .Where(j => j.Status == status)
                    .AsNoTracking()
                    .OrderByDescending(j => j.PostingDate)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving jobs by status {Status}", status);
                throw;
            }
        }

        public async Task<int> CountActiveJobs()
        {
            try
            {
                return await _context.Jobs
                    .CountAsync(j => j.Status == JobStatus.Active);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error counting active jobs");
                throw;
            }
        }

        public async Task<IEnumerable<Job>> SearchJobs(string searchTerm, string? location = null, string? experienceLevel = null)
        {
            try
            {
                var query = _context.Jobs
                    .Where(j => j.Status == JobStatus.Active)
                    .AsQueryable();

                // Search in job title, description, or company name
                if (!string.IsNullOrWhiteSpace(searchTerm))
                {
                    query = query.Where(j => 
                        j.JobTitle.Contains(searchTerm) ||
                        j.JobDescription.Contains(searchTerm) ||
                        j.CompanyName.Contains(searchTerm) ||
                        j.RequiredSkills.Contains(searchTerm));
                }

                // Filter by location
                if (!string.IsNullOrWhiteSpace(location))
                {
                    query = query.Where(j => j.Location.Contains(location));
                }

                // Filter by experience level
                if (!string.IsNullOrWhiteSpace(experienceLevel))
                {
                    query = query.Where(j => j.ExperienceLevel.Contains(experienceLevel));
                }

                return await query
                    .AsNoTracking()
                    .OrderByDescending(j => j.PostingDate)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching jobs with term: {SearchTerm}", searchTerm);
                throw;
            }
        }
    }
}
