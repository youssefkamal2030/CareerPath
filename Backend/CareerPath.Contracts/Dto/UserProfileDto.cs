using System;
using System.Collections.Generic;

namespace CareerPath.Contracts.Dto
{
    public class UserProfileDto
    {
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FullName { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Bio { get; set; }
        public string Location { get; set; }
        public string AvatarUrl { get; set; }
        public string CoverUrl { get; set; }
        public string JobTitle { get; set; }
        public List<string> Skills { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
} 