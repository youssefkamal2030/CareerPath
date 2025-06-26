using AutoMapper;
using CareerPath.Contracts.Dto;
using CareerPath.Domain.Entities.AIDataAnalysis;
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