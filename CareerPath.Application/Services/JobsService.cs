using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using CareerPath.Application.AIDataAnalysis_Interfaces;
using CareerPath.Application.Interfaces;
using CareerPath.Contracts.Dto;
using CareerPath.Domain.Entities.AIDataAnalysis;
using Microsoft.Extensions.Logging;

namespace CareerPath.Application.Services
{
    public class JobsService : IJobsService
    {
        private readonly IJobsRepository _jobsRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<JobsService> _logger;

        public JobsService(IJobsRepository jobsRepository, IMapper mapper, ILogger<JobsService> logger)
        {
            _jobsRepository = jobsRepository ?? throw new ArgumentNullException(nameof(jobsRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<JobDto> CreateJobAsync(CreateJobDto createJobDto)
        {
            try
            {
                if (createJobDto == null)
                {
                    throw new ArgumentNullException(nameof(createJobDto));
                }

                // Map DTO to domain entity
                var job = _mapper.Map<Job>(createJobDto);

                // Generate a job ID
                job.JobId = $"JOB-{DateTime.UtcNow.Year}-{Guid.NewGuid().ToString().Substring(0, 8)}";

                // Create job through repository
                var createdJob = await _jobsRepository.CreateJobAsync(job);
                _logger.LogInformation("Job {JobId} created successfully", job.JobId);

                // Map back to DTO
                return _mapper.Map<JobDto>(createdJob);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating job");
                throw;
            }
        }

        public async Task<bool> DeleteJobAsync(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    throw new ArgumentException("Job ID cannot be null or empty", nameof(id));
                }

                var result = await _jobsRepository.DeleteJobAsync(id);
                
                if (result)
                {
                    _logger.LogInformation("Job {JobId} deleted successfully", id);
                }
                else
                {
                    _logger.LogWarning("Job {JobId} not found for deletion", id);
                }
                
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting job {JobId}", id);
                throw;
            }
        }

        public async Task<IEnumerable<JobDto>> GetAllJobsAsync()
        {
            try
            {
                var jobs = await _jobsRepository.GetAllAsync();
                _logger.LogInformation("Retrieved all jobs");
                return _mapper.Map<IEnumerable<JobDto>>(jobs);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all jobs");
                throw;
            }
        }

        public async Task<JobDto> GetJobByIdAsync(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    throw new ArgumentException("Job ID cannot be null or empty", nameof(id));
                }

                var job = await _jobsRepository.GetByIdAsync(id);
                
                if (job == null)
                {
                    _logger.LogWarning("Job {JobId} not found", id);
                    return null;
                }
                
                return _mapper.Map<JobDto>(job);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving job {JobId}", id);
                throw;
            }
        }

        public async Task<IEnumerable<JobDto>> GetJobsByUserIdAsync(string userId)
        {
            try
            {
                if (string.IsNullOrEmpty(userId))
                {
                    throw new ArgumentException("User ID cannot be null or empty", nameof(userId));
                }

                var jobs = await _jobsRepository.GetByUserIdAsync(userId);
                _logger.LogInformation("Retrieved jobs for user {UserId}", userId);
                return _mapper.Map<IEnumerable<JobDto>>(jobs);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving jobs for user {UserId}", userId);
                throw;
            }
        }

        public async Task<JobDto> UpdateJobAsync(string id, UpdateJobDto updateJobDto)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    throw new ArgumentException("Job ID cannot be null or empty", nameof(id));
                }

                if (updateJobDto == null)
                {
                    throw new ArgumentNullException(nameof(updateJobDto));
                }

                // Get existing job
                var existingJob = await _jobsRepository.GetByIdAsync(id);
                if (existingJob == null)
                {
                    _logger.LogWarning("Job {JobId} not found for update", id);
                    return null;
                }

                // Map update DTO to entity, preserving original values for properties not in the DTO
                _mapper.Map(updateJobDto, existingJob);

                // Update job through repository
                var updatedJob = await _jobsRepository.UpdateJobAsync(existingJob);
                
                if (updatedJob == null)
                {
                    _logger.LogWarning("Failed to update job {JobId}", id);
                    return null;
                }
                
                _logger.LogInformation("Job {JobId} updated successfully", id);
                return _mapper.Map<JobDto>(updatedJob);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating job {JobId}", id);
                throw;
            }
        }
    }
}
