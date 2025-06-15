using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using CareerPath.Application.Interfaces;
using CareerPath.Contracts.Dto;
using CareerPath.Domain.Entities.AIDataAnalysis;
using Microsoft.Extensions.Logging;

namespace CareerPath.Application.Services
{
    public class JobsService : IJobsService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<JobsService> _logger;

        public JobsService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<JobsService> logger)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

    
        public async Task<IEnumerable<JobDto>> GetAllJobsAsync()
        {
            try
            {
                var jobs = await _unitOfWork.jobs.GetAllAsync();
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

                var job = await _unitOfWork.jobs.GetByIdAsync(id);
                
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

                var jobs = await _unitOfWork.jobs.GetByUserIdAsync(userId);  
                _logger.LogInformation("Retrieved jobs for user {UserId}", userId);
                return _mapper.Map<IEnumerable<JobDto>>(jobs);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving jobs for user {UserId}", userId);
                throw;
            }
        }

     
    }
}
