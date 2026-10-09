using AutoMapper;
using CareerPath.Contracts.Dto;
using CareerPath.Domain.Recommendations;
using System;

namespace CareerPath.Application.Profiles
{
    public class JobMapping : Profile
    {
        public JobMapping()
        {
            CreateMap<Job, JobDto>();

        }
    }
} 