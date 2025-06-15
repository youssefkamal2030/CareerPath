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
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public JobApplicationService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<JobApplicationDto> GetByIdAsync(int id)
        {
            var application = await _unitOfWork.JobApplications.GetByIdAsync(id);
            return _mapper.Map<JobApplicationDto>(application);
        }

        public async Task<IEnumerable<JobApplicationDto>> GetByUserIdAsync(string userId)
        {
            var applications = await _unitOfWork.JobApplications.GetByUserIdAsync(userId);
            return _mapper.Map<IEnumerable<JobApplicationDto>>(applications);
        }
        public async Task<IEnumerable<JobApplicationDto>> GetJobsAsync()
        {
            var applications = await _unitOfWork.JobApplications.GetAllJobsAsync();
            return _mapper.Map<IEnumerable<JobApplicationDto>>(applications);
        }

   
    }
} 