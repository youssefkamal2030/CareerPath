using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace CareerPath.Contracts.Dto
{
    public class JobRecommendationRequestDto
    {
        [JsonPropertyName("skills")]
        public List<SkillDto> Skills { get; set; }
        
        [JsonPropertyName("work_experience")]
        public List<WorkExperienceDto> WorkExperiences { get; set; }
        
        [JsonPropertyName("projects")]
        public List<ProjectDto> Projects { get; set; }
        
     
    }
} 