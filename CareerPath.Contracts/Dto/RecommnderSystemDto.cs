using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace CareerPath.Contracts.Dto
{
    public class RecommnderSystemDto
    {
        [JsonPropertyName("user_skills")]
        public string UserSkills { get; set; }

        [JsonPropertyName("job_descriptions")]
        public List<JobDescriptionDto> JobDescriptions { get; set; }
    }

    public class JobDescriptionDto
    {
        [JsonPropertyName("title")]
        public string Title { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }
    }
}
