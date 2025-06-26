using AutoMapper;
using CareerPath.Contracts.Dto;
using CareerPath.Domain.Entities;
using CareerPath.Domain.Entities.AIDataAnalysis;
using System;

namespace CareerPath.Application.Profiles
{
    public class CompanyMapping : Profile
    {
        public CompanyMapping()
        {
            CreateMap<Company, CompanyDto>()
                .ForMember(dest => dest.OfficeLocation, opt => opt.MapFrom(src => src.officeLocation))
                .ForMember(dest => dest.Jobs, opt => opt.MapFrom(src => src.Jobs));

            CreateMap<Job, JobDto>();
                
         
            CreateMap<CreateCompanyDto, Company>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.officeLocation, opt => opt.MapFrom(src => src.OfficeLocation))
                .ForMember(dest => dest.Jobs, opt => opt.Ignore());

            CreateMap<UpdateCompanyDto, Company>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.officeLocation, opt => opt.MapFrom(src => src.OfficeLocation))
                .ForMember(dest => dest.Jobs, opt => opt.Ignore())
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
        }
    }
} 