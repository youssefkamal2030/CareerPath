using AutoMapper;
using CareerPath.Application.Interfaces;
using CareerPath.Contracts.Dto;
using CareerPath.Domain.Entities.AIDataAnalysis;
using CareerPath.Infrastructure.Data;
using Microsoft.AspNetCore.Http;
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
            throw new NotImplementedException();
            //try
            //{
            //    var personalInfo = new PersonalInformation
            //    {
            //        Id = Guid.NewGuid(),
            //        UserId = userId,
            //        CreatedAt = DateTime.UtcNow,
            //        Name = cvAnalysis.PersonalInformation.Name,
            //        Email = cvAnalysis.PersonalInformation.Email,
            //        Phone = cvAnalysis.PersonalInformation.Phone,
            //        Address = cvAnalysis.PersonalInformation.Address
            //    };

            //    _context.PersonalInformations.Add(personalInfo);

            //    if (cvAnalysis.Skills?.Any() == true)
            //    {
            //        personalInfo.Skills = cvAnalysis.Skills.Select(s => new Skill
            //        {
            //            Id = Guid.NewGuid(),
            //            UserId = userId,
            //            CreatedAt = DateTime.UtcNow,
            //            PersonalInformationId = personalInfo.Id,
            //            SkillName = s.SkillName,
            //            ProficiencyLevel = s.ProficiencyLevel
            //        }).ToList();
            //    }


            //    await _context.SaveChangesAsync();
            //    return true;
            //}
            //catch
            //{
            //    return false;
            //}
        }
        //this method retrives the data for recommending the jobs on the platform
        public async Task<JobRecommendationRequestDto> GetUserDataForRecommendationAsync(string userId)
        {
            var personalinfo = await _context.PersonalInformations
                .Include(p => p.Skills)
                .Include(p => p.WorkExperiences)
                .Include(p => p.Projects)
                .FirstOrDefaultAsync(p => p.UserId == userId);
            if (personalinfo == null)
                return null;
        
            var skillDtos = personalinfo.Skills?.Select(s => new SkillDto
            {
                SkillName = s.SkillName,
                ProficiencyLevel = s.ProficiencyLevel
            }).ToList() ?? new List<SkillDto>();
        
            var workExperienceDtos = personalinfo.WorkExperiences?.Select(w => new WorkExperienceDto
            {
                JobTitle = w.JobTitle,
                JobLevel = w.JobLevel,
                Company = w.Company,
                StartYear = w.StartYear,
                StartMonth = w.StartMonth,
                EndYear = w.EndYear,
                EndMonth = w.EndMonth,
                JobDescription = w.JobDescription
            }).ToList() ?? new List<WorkExperienceDto>();
        
            var projectDtos = personalinfo.Projects?.Select(p => new ProjectDto
            {
                ProjectName = p.ProjectName,
                StartDate = p.StartDate,
                EndDate = p.EndDate,
                Url = p.Url,
                Description = p.Description
            }).ToList() ?? new List<ProjectDto>();
        
            var jobDescriptions = workExperienceDtos
                .Where(w => !string.IsNullOrEmpty(w.JobDescription))
                .Select(w => w.JobDescription)
                .ToList();

            return new JobRecommendationRequestDto
            {
                Skills = skillDtos,
                WorkExperiences = workExperienceDtos,
                Projects = projectDtos,
            };
        }
     //this method for Recommending the top 5 jobs not related to the data stored 
        public async Task<RecommnderSystemDto> RecommnderSystem(string userId)
        {
            var personalInfo = await _context.PersonalInformations
                .Include(p => p.Skills)
                .FirstOrDefaultAsync(p => p.UserId == userId);
            
            if (personalInfo == null)
                return null;
            
            var userSkills = string.Join(", ", 
                personalInfo.Skills?
                    .Where(s => !string.IsNullOrEmpty(s.SkillName))
                    .Select(s => s.SkillName) ?? 
                Enumerable.Empty<string>());
            
            var jobs = await _context.Jobs.ToListAsync();
            
            var jobDescriptionsList = new List<JobDescriptionDto>();
            
            foreach (var job in jobs)
            {
                jobDescriptionsList.Add(new JobDescriptionDto
                {
                    Title = job.JobTitle,
                    Description = job.JobDescription
                });
            }
            
            // Add a placeholder if no jobs found
            if (!jobDescriptionsList.Any())
            {
                jobDescriptionsList.Add(new JobDescriptionDto
                {
                    Title = "Software Developer",
                    Description = "Experienced developer with skills in web development."
                });
            }
            
            return new RecommnderSystemDto
            {
                UserSkills = userSkills,
                JobDescriptions = jobDescriptionsList
            };
        }
         public async Task<(byte[] FileData, string FileName, string ContentType)?> GetUserCVAsync(string userId)
        {
            var userCV = await _context.userCVs
                .FirstOrDefaultAsync(cv => cv.UserId == userId);

            if (userCV == null)
                return null;

            return (userCV.FileData, userCV.FileName, userCV.ContentType);
        }
        public async Task<UserCV> SaveUserCV(IFormFile userCv, string userId)
        {
            // Input validation
            if (userCv == null || userCv.Length == 0)
                throw new ArgumentException("No file was uploaded or file is empty");

            if (string.IsNullOrWhiteSpace(userId))
                throw new ArgumentException("User ID cannot be null or empty");

            // File type validation
            if (!userCv.ContentType.Equals("application/pdf", StringComparison.OrdinalIgnoreCase))
                throw new ArgumentException("Only PDF files are allowed");

            // File size validation (optional - add if needed)
            const int maxFileSize = 10 * 1024 * 1024; // 10MB
            if (userCv.Length > maxFileSize)
                throw new ArgumentException($"File size exceeds maximum allowed size of {maxFileSize / (1024 * 1024)}MB");

            try
            {
                var existingCv = await _context.userCVs
                    .FirstOrDefaultAsync(cv => cv.UserId == userId);

                if (existingCv != null)
                {
                    using var memoryStream = new MemoryStream();
                    await userCv.CopyToAsync(memoryStream);

                    existingCv.FileName = userCv.FileName;
                    existingCv.ContentType = userCv.ContentType;
                    existingCv.FileData = memoryStream.ToArray();
                    existingCv.UploadDate = DateTime.UtcNow;

                    _context.userCVs.Update(existingCv);
                    await _context.SaveChangesAsync();
                    return existingCv;
                }
                else
                {
                    using var memoryStream = new MemoryStream();
                    await userCv.CopyToAsync(memoryStream);

                    var newUserCV = new UserCV
                    {
                        Id = Guid.NewGuid(),
                        UserId = userId,
                        FileName = userCv.FileName,
                        ContentType = userCv.ContentType,
                        FileData = memoryStream.ToArray(),
                        UploadDate = DateTime.UtcNow
                    };

                    _context.userCVs.Add(newUserCV);
                    await _context.SaveChangesAsync();
                    return newUserCV;
                }
            }
            catch (Exception ex) when (!(ex is ArgumentException))
            {
                throw new Exception($"Database error occurred while saving CV: {ex.Message}", ex);
            }
        }


    }

}