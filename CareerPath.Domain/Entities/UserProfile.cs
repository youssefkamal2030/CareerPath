using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CareerPath.Domain.Entities
{
    public class UserProfile : AggregateRoot
    {
        public string  Id { get; private set; }
        public string FirstName { get; private set; }
        public string? LastName { get; private set; }
        public string Username { get; private set; }
        public string Email { get; private set; }
        public string? Bio { get; private set; }
        public string? Location { get; private set; }
        public string? CoverUrl { get; private set; }
        public string? Experiences { get; private set; } = string.Empty;
        public List<string> Skills { get; private set; } = new List<string>();

        public string? AvatarUrl { get; private set; }
        public string? JobTitle { get; private set; }
        public DateTime CreatedAt { get;  set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get;  set; } = DateTime.UtcNow;

        public UserProfile(string id, string firstName, string lastName, string avatarUrl, string username = null, string email = null)
        {
            Id = id;
            FirstName = firstName;
            LastName = lastName;
            AvatarUrl = avatarUrl;
            Username = username;
            Email = email;
        }

        public UserProfile(string id, string username, string email)
        {
            Id = id;
            Username = username;
            Email = email;
            FirstName = username; 
            LastName = string.Empty;
            AvatarUrl = string.Empty; 
        }

        public string FullName => $"{FirstName} {LastName}";

        public void UpdateProfile(string? firstName, string? lastName, string? bio, string? location, 
            string? avatarUrl, string? coverUrl, string? jobTitle, List<string>? skills)
        {
            if (firstName != null) FirstName = firstName;
            if (lastName != null) LastName = lastName;
            if (bio != null) Bio = bio;
            if (location != null) Location = location;
            if (avatarUrl != null) AvatarUrl = avatarUrl;
            if (coverUrl != null) CoverUrl = coverUrl;
            if (jobTitle != null) JobTitle = jobTitle;
            if (skills != null) Skills = skills;
            UpdatedAt = DateTime.UtcNow;

            AddDomainEvent(new UserProfileUpdatedEvent
            {
                UserId = Id,
                FirstName = FirstName,
                LastName = LastName,
                Skills = Skills,
                UpdatedAt = UpdatedAt
            });
        }
    }
}
