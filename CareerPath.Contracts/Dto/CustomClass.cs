using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using CareerPath.Domain.Entities.AIDataAnalysis;

namespace CareerPath.Contracts.Dto
{
    public class CustomClass
    {
        [JsonPropertyName("skills")]
        public ICollection<Skill> skills { get; set; }
        [JsonPropertyName("work_experience")]
        public ICollection<WorkExperience> workExperiences { get; set; }
        [JsonPropertyName("projects")]
        public ICollection<Project> projects { get; set; }
    }
}
