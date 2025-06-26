using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace CareerPath.Contracts.Dto
{
    public class JobRecommendationResponseDto
    {
        
        [JsonPropertyName("recommendations")]
        public List<JobRecommendationDto> Recommendations { get; set; }
    }

    public class JobRecommendationDto
    {

        public string job_title { get; set; }


        public string job_level { get; set; }

        public List<Strength> strengths_points { get; set; }
        public List<Weakness> weakness_points { get; set; }

        public string reason { get; set; }
    }
    public class Strength
    {
        public string skill_name { get; set; }
        public string proficiency_level { get; set; }
        public string reason { get; set; }
    }
    public class Weakness
    {
        public string skill_name { get; set; }
        public string proficiency_level { get; set; }
        public string required_level { get; set; }
        public string improvement_tips { get; set; }
    }

}