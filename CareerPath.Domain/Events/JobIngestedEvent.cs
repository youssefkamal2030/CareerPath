using System;

namespace CareerPath.Domain.Events
{
    public class JobIngestedEvent : DomainEvent
    {
        public string JobId { get; set; }
        public string ExternalId { get; set; }
        public string SourceProvider { get; set; }
        public DateTime IngestedAt { get; set; }

        public JobIngestedEvent(string jobId, string externalId, string sourceProvider)
        {
            JobId = jobId;
            ExternalId = externalId;
            SourceProvider = sourceProvider;
            IngestedAt = DateTime.UtcNow;
            EventType = nameof(JobIngestedEvent);
        }
    }
}