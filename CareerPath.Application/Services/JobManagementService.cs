using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CareerPath.Application.Interfaces;
using CareerPath.Domain.Recommendations;

namespace CareerPath.Application.Services
{
    public interface IJobManagementService
    {
        Task<Job> GetJobById(string jobId);
        Task<IEnumerable<Job>> SearchJobs(string searchTerm, string? location = null, string? experienceLevel = null);
        Task<IEnumerable<Job>> GetActiveJobs();
        Task<IEnumerable<Job>> GetJobsByCompany(string companyName);
        Task<Job> MarkJobAsFilled(string jobId);
        Task<Job> ExpireJob(string jobId);
        Task<Job> ReactivateJob(string jobId);
        Task<int> GetActiveJobsCount();
        Task<IEnumerable<Job>> GetRecentJobs(int count = 10);
    }

    public class JobManagementService : IJobManagementService
    {
        private readonly IJobRepository _jobRepository;
        private readonly IUnitOfWork _unitOfWork;

        public JobManagementService(IJobRepository jobRepository, IUnitOfWork unitOfWork)
        {
            _jobRepository = jobRepository ?? throw new ArgumentNullException(nameof(jobRepository));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        public async Task<Job> GetJobById(string jobId)
        {
            if (string.IsNullOrWhiteSpace(jobId))
                throw new ArgumentException("Job ID cannot be empty", nameof(jobId));

            var job = await _jobRepository.GetByIdAsync(jobId);
            if (job == null)
                throw new InvalidOperationException($"Job with ID {jobId} not found");

            // Check if job should be expired
            job.CheckAndExpireIfNeeded();
            if (job.DomainEvents.Count > 0)
            {
                await _jobRepository.UpdateJobAsync(job);
                await _unitOfWork.CompleteAsync();
            }

            return job;
        }

        public async Task<IEnumerable<Job>> SearchJobs(string searchTerm, string? location = null, string? experienceLevel = null)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                throw new ArgumentException("Search term cannot be empty", nameof(searchTerm));

            return await _jobRepository.SearchJobs(searchTerm, location, experienceLevel);
        }

        public async Task<IEnumerable<Job>> GetActiveJobs()
        {
            return await _jobRepository.GetJobsByStatus(JobStatus.Active);
        }

        public async Task<IEnumerable<Job>> GetJobsByCompany(string companyName)
        {
            if (string.IsNullOrWhiteSpace(companyName))
                throw new ArgumentException("Company name cannot be empty", nameof(companyName));

            var allJobs = await _jobRepository.GetAllAsync();
            return allJobs.Where(j => j.CompanyName.Contains(companyName, StringComparison.OrdinalIgnoreCase));
        }

        public async Task<Job> MarkJobAsFilled(string jobId)
        {
            var job = await GetJobById(jobId);
            
            job.MarkAsFilled();
            await _jobRepository.UpdateJobAsync(job);
            await _unitOfWork.CompleteAsync();

            return job;
        }

        public async Task<Job> ExpireJob(string jobId)
        {
            var job = await GetJobById(jobId);
            
            job.ExpireJob();
            await _jobRepository.UpdateJobAsync(job);
            await _unitOfWork.CompleteAsync();

            return job;
        }

        public async Task<Job> ReactivateJob(string jobId)
        {
            var job = await GetJobById(jobId);
            
            job.ReactivateJob();
            await _jobRepository.UpdateJobAsync(job);
            await _unitOfWork.CompleteAsync();

            return job;
        }

        public async Task<int> GetActiveJobsCount()
        {
            return await _jobRepository.CountActiveJobs();
        }

        public async Task<IEnumerable<Job>> GetRecentJobs(int count = 10)
        {
            var allJobs = await _jobRepository.GetAllAsync();
            return allJobs
                .Where(j => j.Status == JobStatus.Active)
                .OrderByDescending(j => j.PostingDate)
                .Take(count);
        }
    }
}