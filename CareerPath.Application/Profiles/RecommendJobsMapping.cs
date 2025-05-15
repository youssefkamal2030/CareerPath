using AutoMapper;
using CareerPath.Contracts.Dto;
using CareerPath.Domain.Entities.AIDataAnalysis;

namespace CareerPath.Application.Profiles
{
    public class RecommendJobsMapping : Profile
    {
        public RecommendJobsMapping()
        {
            // Map from CVAnalysisDto to RecommendJobsRequestDto
            CreateMap<CVAnalysisDto, RecommendJob>()
                .ForMember(dest => dest.Skills, opt => opt.MapFrom(src => src.Skills))
                .ForMember(dest => dest.Projects, opt => opt.Ignore())  // Custom mapping needed
                .ForMember(dest => dest.WorkExperiences, opt => opt.Ignore());  // Custom mapping needed

            // Map from WorkExperienceDto to WorkExperience
            //CreateMap<WorkExperienceDtoForAi, WorkExperience>()
            //    .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.JobTitle))
            //    .ForMember(dest => dest.Company, opt => opt.MapFrom(src => src.Company))
            //    .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.JobDescription))
            //    .ForMember(dest => dest.StartDate, opt => opt.MapFrom(src =>
            //        src.StartYear.HasValue && src.StartMonth.HasValue ?
            //        new DateTime(src.StartYear.Value, src.StartMonth.Value, 1) :
            //        DateTime.MinValue));
               

            //// Map from ProjectDto to Project
            //CreateMap<ProjectDtoforAi, Project>()
            //    .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.ProjectName))
            //    .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
            //    .ForMember(dest => dest.StartDate, opt => opt.MapFrom(src => src.StartDate ?? DateTime.MinValue))
            //    .ForMember(dest => dest.EndDate, opt => opt.MapFrom(src => src.EndDate))
            //    .ForMember(dest => dest.Technologies, opt => opt.Ignore());  // Need custom mapping or additional data

            //CreateMap<Skill, SkillDto>();
            //CreateMap<WorkExperience, WorkExperience>()
            //    .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.JobTitle))
            //    .ForMember(dest => dest.StartDate, opt => opt.MapFrom(src => new DateTime(src.StartYear ?? DateTime.Now.Year, src.StartMonth ?? 1, 1)))
            //    .ForMember(dest => dest.EndDate, opt => opt.MapFrom(src => 
            //        src.EndYear.HasValue && src.EndMonth.HasValue 
            //            ? new DateTime(src.EndYear.Value, src.EndMonth.Value, 1) 
            //            : null));

            //CreateMap<Project, Project>()
            //    .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.ProjectName))
            //    .ForMember(dest => dest.Technologies, opt => opt.MapFrom(src => src.Skills));
        }
    }
} 