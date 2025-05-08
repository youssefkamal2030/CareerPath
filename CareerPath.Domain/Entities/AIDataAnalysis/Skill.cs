namespace CareerPath.Domain.Entities.AIDataAnalysis
{
    public class Skill : BaseEntity
    {
        public string? SkillName { get; set; }
        public string? ProficiencyLevel { get; set; }

        // Navigation properties
        public Guid PersonalInformationId { get; set; }
        public PersonalInformation? PersonalInformation { get; set; }
    }
} 