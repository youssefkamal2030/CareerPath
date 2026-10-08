using System;

namespace CareerPath.Domain.Events
{
    public class JobExpiredEvent : DomainEvent
    {
        public string JobId { get; set; }
        public DateTime ExpiredAt { get; set; }

        public JobExpiredEvent(string jobId)
        {
            JobId = jobId;
            ExpiredAt = DateTime.UtcNow;
            EventType = nameof(JobExpiredEvent);
        }
    }
}