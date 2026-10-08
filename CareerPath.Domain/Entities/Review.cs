using System;

namespace CareerPath.Domain.Entities
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