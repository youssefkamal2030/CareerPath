using AutoMapper;
using CareerPath.Contracts.Dto;
using CareerPath.Domain.Entities;

namespace CareerPath.Application.Profiles
{
    public class JobApplicationMapping : Profile
    {
        public JobApplicationMapping()
        {
            CreateMap<JobApplication, JobApplicationDto>();
           
        }
    }
} 