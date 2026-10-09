using AutoMapper;
using CareerPath.Contracts.Dto;
using CareerPath.Domain.Applications;

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