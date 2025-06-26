using System.Collections.Generic;

namespace CareerPath.Contracts.Dto
{
    public class UpdateUserProfileDto
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Bio { get; set; }
        public string? Location { get; set; }
        public string? AvatarUrl { get; set; }
        public string? CoverUrl { get; set; }
        public string? JobTitle { get; set; }
        public List<string>? Skills { get; set; }
    }
} 