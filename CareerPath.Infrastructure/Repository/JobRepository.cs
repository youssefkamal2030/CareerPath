using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CareerPath.Domain.Entities.AIDataAnalysis;
using CareerPath.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CareerPath.Infrastructure.Repository
{
    public class JobRepository : IJobRepository
    {
        private readonly AIDataAnalysisDbContext _context;
        private readonly ILogger<JobRepository> _logger;

        public JobRepository(AIDataAnalysisDbContext context, ILogger<JobRepository> logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
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

        public async Task<IEnumerable<Job>> GetAllAsync()
        {
            try
            {
                return await _context.Jobs
                    .AsNoTracking()
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving all jobs");
                throw;
            }
        }

        public async Task<Job> GetByIdAsync(string id)
        {
            try
            {
                return await _context.Jobs.FindAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving job with ID {JobId}", id);
                throw;
            }
        }

        public async Task<IEnumerable<Job>> GetByUserIdAsync(string userId)
        {
            try
            {
                return await _context.Jobs
                    .Where(j => j.CompanyName == userId) 
                    .AsNoTracking()
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving jobs for user {UserId}", userId);
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
    }
}
