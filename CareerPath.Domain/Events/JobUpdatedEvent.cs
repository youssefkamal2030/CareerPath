using System;

namespace CareerPath.Domain.Events
{
    public class JobUpdatedEvent : DomainEvent
    {
        public string JobId { get; set; }
        public DateTime UpdatedAt { get; set; }

        public JobUpdatedEvent(string jobId)
        {
            JobId = jobId;
            UpdatedAt = DateTime.UtcNow;
            EventType = nameof(JobUpdatedEvent);
        }
    }
}