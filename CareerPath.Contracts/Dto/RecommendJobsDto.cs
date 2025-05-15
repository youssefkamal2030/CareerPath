using System;
using System.Collections.Generic;

namespace CareerPath.Contracts.Dto
{

    public class RecommendJob
    {
        public ICollection<SkillDto> Skills { get; set; }
        public ICollection<WorkExperienceDto> WorkExperiences { get; set; }
        public ICollection<ProjectDto> Projects { get; set; }
    }

    public class WorkExperienceDtoForAi
    {
        public string Title { get; set; }
        public string Company { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string Description { get; set; }
    }

    public class ProjectD
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public ICollection<SkillDto> Technologies { get; set; }
    }

    public class RecommendJobsResponseDto
    {
        public ICollection<RecommendedJob> Jobs { get; set; }
        public string Reason { get; set; }
    }

    public class RecommendedJob
    {
        public string JobTitle { get; set; }
        public string JobLevel { get; set; }
        public ICollection<StrengthPoint> StrengthsPoints { get; set; }
        public ICollection<WeaknessPoint> WeaknessPoints { get; set; }
    }

    public class StrengthPoint
    {
        public string SkillName { get; set; }
        public string ProficiencyLevel { get; set; }
        public string Reason { get; set; }
    }

    public class WeaknessPoint
    {
        public string SkillName { get; set; }
        public string ProficiencyLevel { get; set; }
        public string RequiredLevel { get; set; }
        public string ImprovementTips { get; set; }
    }
}