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
            // Map from Job entity to JobDto (for reading)
            CreateMap<Job, JobDto>();

            // Map from CreateJobDto to Job entity (for creating)
            CreateMap<CreateJobDto, Job>()
                .ForMember(dest => dest.JobId, opt => opt.Ignore())
                .ForMember(dest => dest.PostingDate, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.ApplicationDeadline, opt => opt.MapFrom(src => 
                    src.ApplicationDeadline ?? DateTime.UtcNow.AddMonths(1)));

            // Map from UpdateJobDto to Job entity (for updating)
            CreateMap<UpdateJobDto, Job>()
                .ForMember(dest => dest.JobId, opt => opt.Ignore())
                .ForMember(dest => dest.PostingDate, opt => opt.Ignore())
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
        }
    }
} 