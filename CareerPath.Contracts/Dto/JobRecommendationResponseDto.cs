using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace CareerPath.Contracts.Dto
{
    public class JobRecommendationResponseDto
    {
        [JsonPropertyName("response")]
        public JobRecommendationResponse Response { get; set; }
    }

    public class JobRecommendationResponse
    {
        [JsonPropertyName("jobs")]
        public List<JobRecommendationDto> Jobs { get; set; }

        [JsonPropertyName("reason")]
        public string Reason { get; set; }
    }

    public class JobRecommendationDto
    {
        [JsonPropertyName("job_title")]
        public string JobTitle { get; set; }

        [JsonPropertyName("job_level")]
        public string JobLevel { get; set; }

        [JsonPropertyName("strengths_points")]
        public List<Strength> StrengthsPoints { get; set; }

        [JsonPropertyName("weakness_points")]
        public List<Weakness> WeaknessPoints { get; set; }

        [JsonPropertyName("reason")]
        public string Reason { get; set; }
    }

    public class Strength
    {
        [JsonPropertyName("skill_name")]
        public string SkillName { get; set; }

        [JsonPropertyName("proficiency_level")]
        public string ProficiencyLevel { get; set; }

        [JsonPropertyName("reason")]
        public string Reason { get; set; }
    }

    public class Weakness
    {
        [JsonPropertyName("skill_name")]
        public string SkillName { get; set; }

        [JsonPropertyName("proficiency_level")]
        public string ProficiencyLevel { get; set; }

        [JsonPropertyName("required_level")]
        public string RequiredLevel { get; set; }

        [JsonPropertyName("improvement_tips")]
        public string ImprovementTips { get; set; }
    }
}