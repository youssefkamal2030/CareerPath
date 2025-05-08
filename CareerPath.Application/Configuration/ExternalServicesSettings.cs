namespace CareerPath.Application.Configuration
{
    public class ExternalServicesSettings
    {
        public AITeamApiSettings AITeamApi { get; set; }
    }

    public class AITeamApiSettings
    {
        public string BaseUrl { get; set; }
        public string RecommendJobsEndpoint { get; set; }

        public string RecommendJobsUrl => $"{BaseUrl.TrimEnd('/')}/{RecommendJobsEndpoint.TrimStart('/')}";
    }
} 