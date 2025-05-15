using AutoMapper;
using CareerPath.Contracts.Dto;
using CareerPath.Domain.Entities;
using System;

namespace CareerPath.Application.Profiles
{
    public class CompanyMapping : Profile
    {
        public CompanyMapping()
        {
            // Map from Company entity to CompanyDto (for reading)
            CreateMap<Company, CompanyDto>()
                .ForMember(dest => dest.OfficeLocation, opt => opt.MapFrom(src => src.officeLocation))
                .ForMember(dest => dest.Jobs, opt => opt.MapFrom(src => src.Jobs));

            // Map from JobDto to Domain Job entity for the Jobs collection
            CreateMap<JobDto, CareerPath.Domain.Entities.AIDataAnalysis.Job>();
                
            // Map from CreateCompanyDto to Company entity (for creating)
            CreateMap<CreateCompanyDto, Company>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.officeLocation, opt => opt.MapFrom(src => src.OfficeLocation))
                .ForMember(dest => dest.Jobs, opt => opt.Ignore());

            // Map from UpdateCompanyDto to Company entity (for updating)
            CreateMap<UpdateCompanyDto, Company>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.officeLocation, opt => opt.MapFrom(src => src.OfficeLocation))
                .ForMember(dest => dest.Jobs, opt => opt.Ignore())
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
        }
    }
} 