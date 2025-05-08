namespace CareerPath.Contracts.Dto
{
    public class CVAnalysisDto
    {
        public PersonalInformationDto PersonalInformation { get; set; }
        public List<SkillDto> Skills { get; set; }
        public List<WorkExperienceDto> WorkExperiences { get; set; }
        public List<EducationDto> Educations { get; set; }
        public List<ProjectDto> Projects { get; set; }
    }

    public class PersonalInformationDto
    {
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? Address { get; set; }
    }

    public class SkillDto
    {
        public string? SkillName { get; set; }
        public string? ProficiencyLevel { get; set; }
    }

    public class WorkExperienceDto
    {
        public string? JobTitle { get; set; }
        public string? JobLevel { get; set; }
        public string? Company { get; set; }
        public int? StartYear { get; set; }
        public int? StartMonth { get; set; }
        public int? EndYear { get; set; }
        public int? EndMonth { get; set; }
        public string? JobDescription { get; set; }
    }

    public class EducationDto
    {
        public string? Institution { get; set; }
        public string? Degree { get; set; }
        public string? FieldOfStudy { get; set; }
        public int? StartYear { get; set; }
        public int? StartMonth { get; set; }
        public int? EndYear { get; set; }
        public int? EndMonth { get; set; }
        public string? EducationLevel { get; set; }
    }

    public class ProjectDto
    {
        public string? ProjectName { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? Url { get; set; }
        public string? Description { get; set; }
    }
}