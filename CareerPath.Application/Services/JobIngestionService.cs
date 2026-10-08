using System;
using System.Threading.Tasks;
using CareerPath.Application.Interfaces;
using CareerPath.Contracts.External.JobBoard;
using CareerPath.Domain.Recommendations;

namespace CareerPath.Application.Services
{
    public interface IJobIngestionService
    {
        Task<Job> IngestJobFromExternalSource(ExternalJobDto externalJob);
        Task<bool> JobAlreadyExists(string externalId, string sourceProvider);
        Task<Job> UpdateExistingJob(string externalId, string sourceProvider, ExternalJobDto updatedJob);
        Task ExpireOldJobs(int daysOld = 30);
    }

    public class JobIngestionService : IJobIngestionService
    {
        private readonly IJobRepository _jobRepository;
        private readonly IUnitOfWork _unitOfWork;

        public JobIngestionService(IJobRepository jobRepository, IUnitOfWork unitOfWork)
        {
            _jobRepository = jobRepository ?? throw new ArgumentNullException(nameof(jobRepository));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        public async Task<Job> IngestJobFromExternalSource(ExternalJobDto externalJob)
        {
            if (externalJob == null)
                throw new ArgumentNullException(nameof(externalJob));

            // Check if job already exists
            if (await JobAlreadyExists(externalJob.ExternalId, externalJob.SourceProvider))
            {
                throw new InvalidOperationException(
                    $"Job {externalJob.ExternalId} from {externalJob.SourceProvider} already exists");
            }

            try
            {
                // Domain factory method handles validation and business rules
                var job = Job.FromExternalSource(
                    externalJob.ExternalId,
                    externalJob.Title,
                    externalJob.Company,
                    externalJob.Description,
                    externalJob.Location,
                    externalJob.SalaryRange,
                    externalJob.RequiredSkills,
                    externalJob.ExperienceLevel,
                    externalJob.EducationLevel,
                    externalJob.EmploymentType,
                    externalJob.PostedDate,
                    externalJob.ApplicationDeadline,
                    externalJob.SourceProvider,
                    externalJob.ExternalUrl,
                    externalJob.Industry,
                    externalJob.CertificationsRequired,
                    externalJob.RequiredLanguage);

                await _jobRepository.CreateJobAsync(job);
                await _unitOfWork.CompleteAsync();

                return job;
            }
            catch (ArgumentException)
            {
                // Domain validation failed, re-throw
                throw;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to ingest job: {ex.Message}", ex);
            }
        }

        public async Task<bool> JobAlreadyExists(string externalId, string sourceProvider)
        {
            if (string.IsNullOrWhiteSpace(externalId) || string.IsNullOrWhiteSpace(sourceProvider))
                return false;

            return await _jobRepository.ExistsByExternalIdAsync(externalId, sourceProvider);
        }

        public async Task<Job> UpdateExistingJob(string externalId, string sourceProvider, ExternalJobDto updatedJob)
        {
            var existingJob = await _jobRepository.GetByExternalIdAsync(externalId, sourceProvider);
            if (existingJob == null)
            {
                throw new InvalidOperationException($"Job {externalId} from {sourceProvider} not found");
            }

            // Update using domain method
            existingJob.UpdateFromExternalSource(
                updatedJob.Title,
                updatedJob.Description,
                updatedJob.ApplicationDeadline);

            await _jobRepository.UpdateJobAsync(existingJob);
            await _unitOfWork.CompleteAsync();

            return existingJob;
        }

        public async Task ExpireOldJobs(int daysOld = 30)
        {
            var cutoffDate = DateTime.UtcNow.AddDays(-daysOld);
            var oldJobs = await _jobRepository.GetActiveJobsOlderThan(cutoffDate);

            foreach (var job in oldJobs)
            {
                job.ExpireJob();
                await _jobRepository.UpdateJobAsync(job);
            }

            await _unitOfWork.CompleteAsync();
        }
    }
}