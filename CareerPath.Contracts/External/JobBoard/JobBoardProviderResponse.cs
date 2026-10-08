using System;
using System.Collections.Generic;

namespace CareerPath.Contracts.External.JobBoard
{
    public class JobBoardProviderResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public List<ExternalJobDto> Jobs { get; set; } = new List<ExternalJobDto>();
        public int TotalCount { get; set; }
        public string Provider { get; set; } = string.Empty;
        public DateTime FetchedAt { get; set; }
    }
}