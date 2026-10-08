using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using CareerPath.Application.Services;

namespace CareerPath.Infrastructure.Services
{
    public class JobIngestionBackgroundService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<JobIngestionBackgroundService> _logger;
        private readonly TimeSpan _interval = TimeSpan.FromHours(6); // Run every 6 hours

        public JobIngestionBackgroundService(
            IServiceProvider serviceProvider,
            ILogger<JobIngestionBackgroundService> logger)
        {
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Job Ingestion Background Service started");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await IngestJobsFromExternalSources();
                    await ExpireOldJobs();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred during job ingestion background task");
                }

                await Task.Delay(_interval, stoppingToken);
            }

            _logger.LogInformation("Job Ingestion Background Service stopped");
        }

        private async Task IngestJobsFromExternalSources()
        {
            using var scope = _serviceProvider.CreateScope();
            var jobBoardService = scope.ServiceProvider.GetRequiredService<IJobBoardService>();

            _logger.LogInformation("Starting automatic job ingestion");

            try
            {
                var searchTerms = new[] { "Software Developer", "Data Analyst", "Product Manager", "DevOps Engineer" };
                
                foreach (var term in searchTerms)
                {
                    var jobs = await jobBoardService.FetchAndIngestJobsAsync(term);
                    _logger.LogInformation("Ingested {Count} jobs for search term: {Term}", jobs.Count, term);
                    
                    // Small delay between searches to be respectful to external APIs
                    await Task.Delay(TimeSpan.FromSeconds(2));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during automatic job ingestion");
            }
        }

        private async Task ExpireOldJobs()
        {
            using var scope = _serviceProvider.CreateScope();
            var jobIngestionService = scope.ServiceProvider.GetRequiredService<IJobIngestionService>();

            try
            {
                _logger.LogInformation("Expiring old jobs");
                await jobIngestionService.ExpireOldJobs(daysOld: 30);
                _logger.LogInformation("Completed expiring old jobs");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during job expiration");
            }
        }
    }
}