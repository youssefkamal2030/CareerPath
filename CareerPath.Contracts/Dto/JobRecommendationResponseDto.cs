using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace CareerPath.Contracts.Dto
{
    public class JobRecommendationResponseDto
    {
        // Add properties that match the response from the API
        // This is a placeholder - replace with actual response structure
        [JsonPropertyName("recommendations")]
        public List<JobRecommendationDto> Recommendations { get; set; }
    }

    public class JobRecommendationDto
    {
        [JsonPropertyName("jobTitle")]
        public string JobTitle { get; set; }
        
        [JsonPropertyName("company")]
        public string Company { get; set; }
        
        [JsonPropertyName("description")]
        public string Description { get; set; }
        
        [JsonPropertyName("matchScore")]
        public double MatchScore { get; set; }
    }
} 