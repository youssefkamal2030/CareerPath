using System;

namespace CareerPath.Domain.Events
{
    public class JobFilledEvent : DomainEvent
    {
        public string JobId { get; set; }
        public DateTime FilledAt { get; set; }

        public JobFilledEvent(string jobId)
        {
            JobId = jobId;
            FilledAt = DateTime.UtcNow;
            EventType = nameof(JobFilledEvent);
        }
    }
}