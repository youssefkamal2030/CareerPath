using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace CareerPath.Contracts.Dto
{
    public class SkillRecommendationResponseDto
    {
        [JsonPropertyName("recommendations")]
        public List<SkillRecommendationItem> Recommendations { get; set; }
    }

    public class SkillRecommendationItem
    {
        [JsonPropertyName("job_title")]
        public string JobTitle { get; set; }
        
        [JsonPropertyName("similarity_score")]
        public double SimilarityScore { get; set; }
    }
} 