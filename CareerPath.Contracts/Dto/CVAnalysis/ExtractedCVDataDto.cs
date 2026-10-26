namespace CareerPath.Contracts.Dto.CVAnalysis;

/// <summary>
/// Structured data extracted from CV
/// </summary>
public class ExtractedCVDataDto
{
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public List<string> Skills { get; set; } = new();
    public List<WorkExperienceDto> WorkExperience { get; set; } = new();
    public List<EducationDto> Education { get; set; } = new();
    public int TotalYearsOfExperience { get; set; }
    public List<string> Certifications { get; set; } = new();
    public List<string> Languages { get; set; } = new();
}

public class WorkExperienceDto
{
    public string JobTitle { get; set; } = string.Empty;
    public string Company { get; set; } = string.Empty;
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public bool IsCurrentRole { get; set; }
    public string Description { get; set; } = string.Empty;
}

public class EducationDto
{
    public string Degree { get; set; } = string.Empty;
    public string Institution { get; set; } = string.Empty;
    public string FieldOfStudy { get; set; } = string.Empty;
    public int? StartYear { get; set; }
    public int? EndYear { get; set; }
    public decimal? GPA { get; set; }
}
