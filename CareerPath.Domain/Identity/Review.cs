using System;
using CareerPath.Domain.Identity;

namespace CareerPath.Domain.Identity
{
    public class Review
    {
        public Guid Id { get; private set; }
        public string UserProfileId { get; private set; }
        public virtual UserProfile UserProfile { get; private set; }
        public string Comment { get; private set; }
        public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;

    }
} 