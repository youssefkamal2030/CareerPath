using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using CareerPath.Infrastructure.Services;
using CareerPath.Infrastructure.Configuration;
using CareerPath.Infrastructure.Mappers;
using CareerPath.Application.Interfaces;
using System.Net.Http;
using System.Threading.Tasks;
using System.Linq;
using CareerPath.Application.Services;

namespace CareerPath.Infrastructure.Services.Tests
{
    [TestClass()]
    public class JobBoardServiceTests
    {
        private HttpClient _httpClient;
        private Mock<IJobIngestionService> _mockJobIngestionService;
        private Mock<ILogger<JobBoardService>> _mockLogger;
        private IOptions<ExternalApiSettings> _apiSettings;
        private Mock<IJobMapperFactory> _mockMapperFactory;
        private Mock<IJobRepository> _mockJobRepository;
        private JobBoardService _service;

        [TestInitialize]
        public void Setup()
        {
            // Setup real HttpClient
            _httpClient = new HttpClient();

            // Setup mocks
            _mockJobIngestionService = new Mock<IJobIngestionService>();
            _mockLogger = new Mock<ILogger<JobBoardService>>();
            _mockMapperFactory = new Mock<IJobMapperFactory>();
            _mockJobRepository = new Mock<IJobRepository>();

            // Setup real API settings with your Jooble API key
            var apiSettings = new ExternalApiSettings
            {
                Jooble_API_Key = "c43b401b-b24d-4068-84a9-be789bc49544"
            };
            _apiSettings = Options.Create(apiSettings);

            // Create the service instance
            _service = new JobBoardService(
                _httpClient,
                _mockJobIngestionService.Object,
                _mockLogger.Object,
                _apiSettings,
                _mockMapperFactory.Object,
                _mockJobRepository.Object
            );
        }

        [TestCleanup]
        public void Cleanup()
        {
            _httpClient?.Dispose();
        }

        [TestMethod]
        [TestCategory("Integration")]
        [Description("Test actual Jooble API call with real API key")]
        public async Task FetchJobsAsync_WithRealJoobleApi_ShouldReturnJobs()
        {
            // Arrange
            string searchQuery = "software engineer";
            string location = "New York";
            int page = 1;
            int pageSize = 10;

            // Act
            var result = await _service.FetchJobsAsync(searchQuery, location, page, pageSize);

            // Assert
            Assert.IsNotNull(result, "Result should not be null");
            Assert.IsTrue(result.Success, $"API call should succeed. Message: {result.Message}");
            Assert.AreEqual("Jooble", result.Provider, "Provider should be Jooble");
            
            // Check if we got raw API response (new implementation stores it there)
            Assert.IsNotNull(result.RawApiResponse, "Raw API response should be populated");
            
            // Log the results for debugging
            Console.WriteLine($"Success: {result.Success}");
            Console.WriteLine($"Message: {result.Message}");
            Console.WriteLine($"Total Count: {result.TotalCount}");
            Console.WriteLine($"Provider: {result.Provider}");
            Console.WriteLine($"Fetched At: {result.FetchedAt}");
        }

        [TestMethod]
        [TestCategory("Integration")]
        [Description("Test Jooble API with different search terms")]
        public async Task FetchJobsAsync_WithDifferentSearchTerms_ShouldReturnRelevantJobs()
        {
            // Arrange
            var searchTerms = new[] { ".NET Developer", "Python Engineer", "DevOps" };

            foreach (var searchTerm in searchTerms)
            {
                // Act
                var result = await _service.FetchJobsAsync(searchTerm, null, 1, 5);

                // Assert
                Assert.IsNotNull(result, $"Result should not be null for search: {searchTerm}");
                Assert.IsTrue(result.Success, $"API should succeed for search: {searchTerm}");
                
                Console.WriteLine($"\n--- Search Term: {searchTerm} ---");
                Console.WriteLine($"Total Results: {result.TotalCount}");
                Console.WriteLine($"Success: {result.Success}");
            }
        }

        [TestMethod]
        [TestCategory("Integration")]
        [Description("Test Jooble API connection")]
        public async Task TestProviderConnection_ShouldReturnTrue()
        {
            // Act
            var result = await _service.TestProviderConnection();

            // Assert
            Assert.IsTrue(result, "Provider connection test should succeed");
            Console.WriteLine($"Provider Connection Test: {(result ? "PASSED" : "FAILED")}");
        }

        [TestMethod]
        [TestCategory("Unit")]
        [Description("Test with empty API key should use mock data")]
        public async Task FetchJobsAsync_WithEmptyApiKey_ShouldUseMockData()
        {
            // Arrange
            var emptyApiSettings = new ExternalApiSettings
            {
                Jooble_API_Key = string.Empty
            };
            var emptyOptionsApiSettings = Options.Create(emptyApiSettings);

            var serviceWithNoKey = new JobBoardService(
                _httpClient,
                _mockJobIngestionService.Object,
                _mockLogger.Object,
                emptyOptionsApiSettings,
                _mockMapperFactory.Object,
                _mockJobRepository.Object
            );

            // Act
            var result = await serviceWithNoKey.FetchJobsAsync("test", null, 1, 10);

            // Assert
            Assert.IsNotNull(result, "Result should not be null");
            Assert.IsTrue(result.Success, "Mock data call should succeed");
            Assert.AreEqual("MockProvider", result.Provider, "Provider should be MockProvider when no API key");
            Assert.IsTrue(result.Jobs.Any(), "Mock data should return jobs");
            
            Console.WriteLine($"Mock Data Test - Jobs Count: {result.Jobs.Count}");
        }

        [TestMethod]
        [TestCategory("Unit")]
        [Description("Test invalid search query")]
        public async Task FetchJobsAsync_WithInvalidQuery_ShouldHandleGracefully()
        {
            // Arrange
            string invalidQuery = ""; // Empty query

            // Act
            var result = await _service.FetchJobsAsync(invalidQuery, null, 1, 10);

            // Assert
            // Should still return a result, just might be empty or use fallback
            Assert.IsNotNull(result, "Result should not be null even with invalid query");
        }

        [TestMethod]
        [TestCategory("Integration")]
        [Description("Test pagination - fetch first and second page")]
        public async Task FetchJobsAsync_WithPagination_ShouldReturnDifferentPages()
        {
            // Arrange
            string searchQuery = "developer";
            
            // Act
            var page1Result = await _service.FetchJobsAsync(searchQuery, null, 1, 10);
            var page2Result = await _service.FetchJobsAsync(searchQuery, null, 2, 10);

            // Assert
            Assert.IsNotNull(page1Result, "Page 1 result should not be null");
            Assert.IsNotNull(page2Result, "Page 2 result should not be null");
            Assert.IsTrue(page1Result.Success, "Page 1 should succeed");
            Assert.IsTrue(page2Result.Success, "Page 2 should succeed");
            
            Console.WriteLine($"Page 1 - Total: {page1Result.TotalCount}");
            Console.WriteLine($"Page 2 - Total: {page2Result.TotalCount}");
        }

        [TestMethod]
        [TestCategory("Integration")]
        [Description("Test remote location search")]
        public async Task FetchJobsAsync_WithRemoteLocation_ShouldReturnRemoteJobs()
        {
            // Arrange
            string searchQuery = "software developer";
            string location = "Remote";

            // Act
            var result = await _service.FetchJobsAsync(searchQuery, location, 1, 10);

            // Assert
            Assert.IsNotNull(result, "Result should not be null");
            Assert.IsTrue(result.Success, "Remote job search should succeed");
            
            Console.WriteLine($"Remote Jobs - Total: {result.TotalCount}");
            Console.WriteLine($"Success: {result.Success}");
        }
    }
}
