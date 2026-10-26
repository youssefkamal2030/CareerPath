using CareerPath.Application.Interfaces;
using CareerPath.Contracts.Dto.CVAnalysis;

namespace CareerPath.Application.Services;

/// <summary>
/// Service for processing CVs and extracting structured data
/// </summary>
public class CVProcessingService : ICVProcessingService
{
    private readonly ICVAnalysisService _cvAnalysisService;

    public CVProcessingService(ICVAnalysisService cvAnalysisService)
    {
        _cvAnalysisService = cvAnalysisService;
    }

    public async Task<CVAnalysisResponse> AnalyzeCVAsync(CVAnalysisRequest request)
    {
        if (request == null)
            throw new ArgumentNullException(nameof(request));

        if (request.CVFile == null || request.CVFile.Length == 0)
            throw new ArgumentException("CV file is required", nameof(request));

        try
        {
            // Save CV file using existing service
            var userCV = await _cvAnalysisService.SaveCvFile(request.CVFile, request.UserId);

            // Extract CV data using existing service
            var analysisResult = await _cvAnalysisService.ExtractCVDataAsync(request.UserId);

            var extractedData = ExtractStructuredData(analysisResult);

            return new CVAnalysisResponse
            {
                AnalysisId = userCV.Id,
                UserId = request.UserId,
                ExtractedData = extractedData,
                AnalyzedAt = DateTime.UtcNow,
                Success = true
            };
        }
        catch (Exception ex)
        {
            return new CVAnalysisResponse
            {
                UserId = request.UserId,
                Success = false,
                ErrorMessage = $"Failed to analyze CV: {ex.Message}",
                AnalyzedAt = DateTime.UtcNow
            };
        }
    }

    public async Task<ExtractedCVDataDto?> GetExtractedCVDataAsync(string analysisId)
    {
        if (string.IsNullOrWhiteSpace(analysisId))
            throw new ArgumentException("Analysis ID is required", nameof(analysisId));

        try
        {
            var analysis = await _cvAnalysisService.ExtractCVDataAsync(analysisId);
            
            if (analysis == null)
                return null;

            return ExtractStructuredData(analysis);
        }
        catch
        {
            return null;
        }
    }

    private ExtractedCVDataDto ExtractStructuredData(CVAnalysisDto analysisResult)
    {
        var extractedData = new ExtractedCVDataDto();

        if (analysisResult.FullName != null)
            extractedData.FullName = analysisResult.FullName;

        if (analysisResult.Email != null)
            extractedData.Email = analysisResult.Email;

        if (analysisResult.PhoneNumber != null)
            extractedData.PhoneNumber = analysisResult.PhoneNumber;

        if (analysisResult.Location != null)
            extractedData.Location = analysisResult.Location;

        if (analysisResult.Skills != null)
        {
            extractedData.Skills = ParseSkillsFromString(analysisResult.Skills);
        }

        if (analysisResult.WorkExperience != null)
        {
            extractedData.WorkExperience = analysisResult.WorkExperience
                .Select(we => new WorkExperienceDto
                {
                    JobTitle = we.JobTitle ?? string.Empty,
                    Company = we.Company ?? string.Empty,
                    StartDate = ToDateTime(we.StartYear, we.StartMonth),
                    EndDate = ToDateTime(we.EndYear, we.EndMonth),
                    Description = we.JobDescription ?? string.Empty,
                    IsCurrentRole = !we.EndYear.HasValue
                })
                .ToList();
        }

        if (analysisResult.Education != null)
        {
            extractedData.Education = analysisResult.Education
                .Select(ed => new EducationDto
                {
                    Degree = ed.Degree ?? string.Empty,
                    Institution = ed.Institution ?? string.Empty,
                    FieldOfStudy = ed.FieldOfStudy ?? string.Empty,
                    StartYear = ed.StartYear,
                    EndYear = ed.EndYear
                })
                .ToList();
        }

        if (analysisResult.YearsOfExperience.HasValue)
        {
            extractedData.TotalYearsOfExperience = analysisResult.YearsOfExperience.Value;
        }
        else
        {
            extractedData.TotalYearsOfExperience = CalculateTotalYearsOfExperience(extractedData.WorkExperience);
        }

        if (analysisResult.Certifications != null)
        {
            extractedData.Certifications = ParseListFromString(analysisResult.Certifications);
        }

        if (analysisResult.Languages != null)
        {
            extractedData.Languages = ParseListFromString(analysisResult.Languages);
        }

        return extractedData;
    }

    private DateTime? ToDateTime(int? year, int? month)
    {
        if (!year.HasValue) return null;
        return new DateTime(year.Value, month ?? 1, 1);
    }

    private List<string> ParseSkillsFromString(string skillsString)
    {
        if (string.IsNullOrWhiteSpace(skillsString))
            return new List<string>();

        return skillsString
            .Split(new[] { ',', ';', '\n', '|' }, StringSplitOptions.RemoveEmptyEntries)
            .Select(s => s.Trim())
            .Where(s => !string.IsNullOrWhiteSpace(s))
            .ToList();
    }

    private List<string> ParseListFromString(string listString)
    {
        if (string.IsNullOrWhiteSpace(listString))
            return new List<string>();

        return listString
            .Split(new[] { ',', ';', '\n' }, StringSplitOptions.RemoveEmptyEntries)
            .Select(s => s.Trim())
            .Where(s => !string.IsNullOrWhiteSpace(s))
            .ToList();
    }

    private List<WorkExperienceDto> ParseWorkExperience(List<WorkExperienceDto> workExperiences)
    {
        return workExperiences ?? new List<WorkExperienceDto>();
    }

    private List<EducationDto> ParseEducation(List<EducationDto> educations)
    {
        return educations ?? new List<EducationDto>();
    }

    private DateTime? ParseDate(dynamic dateValue)
    {
        if (dateValue == null)
            return null;

        if (dateValue is DateTime dt)
            return dt;

        if (DateTime.TryParse(dateValue.ToString(), out var parsedDate))
            return parsedDate;

        return null;
    }

    private int? ParseYear(dynamic yearValue)
    {
        if (yearValue == null)
            return null;

        if (yearValue is int year)
            return year;

        if (int.TryParse(yearValue.ToString(), out var parsedYear))
            return parsedYear;

        if (DateTime.TryParse(yearValue.ToString(), out var date))
            return date.Year;

        return null;
    }

    private int CalculateTotalYearsOfExperience(List<WorkExperienceDto> workExperiences)
    {
        if (workExperiences == null || !workExperiences.Any())
            return 0;

        var totalMonths = 0;

        foreach (var exp in workExperiences)
        {
            var start = exp.StartDate ?? DateTime.Now.AddYears(-2);
            var end = exp.EndDate ?? (exp.IsCurrentRole ? DateTime.Now : start.AddYears(1));

            var months = ((end.Year - start.Year) * 12) + end.Month - start.Month;
            totalMonths += Math.Max(0, months);
        }

        return totalMonths / 12;
    }
}
