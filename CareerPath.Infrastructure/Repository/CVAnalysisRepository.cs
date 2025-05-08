using CareerPath.Application.Interfaces;
using CareerPath.Contracts.Dto;
using CareerPath.Domain.Entities.AIDataAnalysis;
using CareerPath.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CareerPath.Infrastructure.Repository
{
    public class CVAnalysisRepository : ICVAnalysisRepository
    {
        private readonly AIDataAnalysisDbContext _context;

        public CVAnalysisRepository(AIDataAnalysisDbContext context)
        {
            _context = context;
        }

        public async Task<CVAnalysisDto?> GetCVAnalysisByUserIdAsync(string userId)
        {
            var personalInfo = await _context.PersonalInformations
                .Include(p => p.Skills)
                .Include(p => p.WorkExperiences)
                .Include(p => p.Educations)
                .Include(p => p.Projects)
                .FirstOrDefaultAsync(p => p.UserId == userId);

            if (personalInfo == null)
                return null;

            return new CVAnalysisDto
            {
                PersonalInformation = new PersonalInformationDto
                {
                    Name = personalInfo.Name,
                    Email = personalInfo.Email,
                    Phone = personalInfo.Phone,
                    Address = personalInfo.Address
                },
                Skills = personalInfo.Skills?.Select(s => new SkillDto
                {
                    SkillName = s.SkillName,
                    ProficiencyLevel = s.ProficiencyLevel
                }).ToList() ?? new List<SkillDto>(),
                WorkExperiences = personalInfo.WorkExperiences?.Select(w => new WorkExperienceDto
                {
                    JobTitle = w.JobTitle,
                    JobLevel = w.JobLevel,
                    Company = w.Company,
                    StartYear = w.StartYear,
                    StartMonth = w.StartMonth,
                    EndYear = w.EndYear,
                    EndMonth = w.EndMonth,
                    JobDescription = w.JobDescription
                }).ToList() ?? new List<WorkExperienceDto>(),
                Educations = personalInfo.Educations?.Select(e => new EducationDto
                {
                    Institution = e.Institution,
                    Degree = e.Degree,
                    FieldOfStudy = e.FieldOfStudy,
                    StartYear = e.StartYear,
                    StartMonth = e.StartMonth,
                    EndYear = e.EndYear,
                    EndMonth = e.EndMonth,
                    EducationLevel = e.EducationLevel
                }).ToList() ?? new List<EducationDto>(),
                Projects = personalInfo.Projects?.Select(p => new ProjectDto
                {
                    ProjectName = p.ProjectName,
                    StartDate = p.StartDate,
                    EndDate = p.EndDate,
                    Url = p.Url,
                    Description = p.Description
                }).ToList() ?? new List<ProjectDto>()
            };
        }

        public async Task<bool> SaveCVAnalysisAsync(string userId, CVAnalysisDto cvAnalysis)
        {
            try
            {
                var personalInfo = new PersonalInformation
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    CreatedAt = DateTime.UtcNow,
                    Name = cvAnalysis.PersonalInformation.Name,
                    Email = cvAnalysis.PersonalInformation.Email,
                    Phone = cvAnalysis.PersonalInformation.Phone,
                    Address = cvAnalysis.PersonalInformation.Address
                };

                _context.PersonalInformations.Add(personalInfo);

                // Add related entities
                if (cvAnalysis.Skills?.Any() == true)
                {
                    personalInfo.Skills = cvAnalysis.Skills.Select(s => new Skill
                    {
                        Id = Guid.NewGuid(),
                        UserId = userId,
                        CreatedAt = DateTime.UtcNow,
                        PersonalInformationId = personalInfo.Id,
                        SkillName = s.SkillName,
                        ProficiencyLevel = s.ProficiencyLevel
                    }).ToList();
                }

                // Similar for other entities...

                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
} 