using AutoMapper;
using CareerPath.Application.Interfaces;
using CareerPath.Contracts.Dto;
using CareerPath.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CareerPath.Application.Services
{
    public class JobApplicationService : IJobApplicationService
    {
        private readonly IJobApplicationRepository _repository;
        private readonly IMapper _mapper;

        public JobApplicationService(IJobApplicationRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<JobApplicationDto> GetByIdAsync(int id)
        {
            var application = await _repository.GetByIdAsync(id);
            return _mapper.Map<JobApplicationDto>(application);
        }

        public async Task<IEnumerable<JobApplicationDto>> GetByUserIdAsync(string userId)
        {
            var applications = await _repository.GetByUserIdAsync(userId);
            return _mapper.Map<IEnumerable<JobApplicationDto>>(applications);
        }
        public async Task<IEnumerable<JobApplicationDto>> GetJobsAsync()
        {
            var applications = await _repository.GetAllJobsAsync();
            return _mapper.Map<IEnumerable<JobApplicationDto>>(applications);
        }

   
    }
} 