using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CareerPath.Domain.Recommendations;

namespace CareerPath.Application.Interfaces
{
    public interface IJobRepository
    {
        Task<Job> CreateJobAsync(Job job);
        Task<Job> UpdateJobAsync(Job job);
        Task<bool> DeleteJobAsync(string id);
        Task<Job> GetByIdAsync(string id);
        Task<IEnumerable<Job>> GetAllAsync();
        Task<IEnumerable<Job>> GetByUserIdAsync(string userId);
        
        // New methods for external job ingestion
        Task<bool> ExistsByExternalIdAsync(string externalId, string sourceProvider);
        Task<Job?> GetByExternalIdAsync(string externalId, string sourceProvider);
        Task<IEnumerable<Job>> GetActiveJobsOlderThan(DateTime cutoffDate);
        Task<IEnumerable<Job>> GetJobsBySourceProvider(string sourceProvider);
        Task<IEnumerable<Job>> GetJobsByStatus(JobStatus status);
        Task<int> CountActiveJobs();
        Task<IEnumerable<Job>> SearchJobs(string searchTerm, string? location = null, string? experienceLevel = null);
    }
}