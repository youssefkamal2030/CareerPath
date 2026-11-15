using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace CareerPath.InfrastructureTests.Standalone
{
    /// <summary>
    /// Standalone test to verify Jooble API integration without full project dependencies
    /// </summary>
    public class JoobleApiStandaloneTest
    {
        private const string JOOBLE_API_KEY = "c43b401b-b24d-4068-84a9-be789bc49544";
        private const string JOOBLE_BASE_URL = "https://jooble.org/api/";

        public static async Task Main(string[] args)
        {
            Console.WriteLine("==============================================");
            Console.WriteLine("Jooble API Standalone Integration Test");
            Console.WriteLine("==============================================\n");

            var tester = new JoobleApiStandaloneTest();
            
            // Test 1: Basic API Call
            await tester.TestBasicApiCall();
            
            // Test 2: Different Search Terms
            await tester.TestDifferentSearchTerms();
            
            // Test 3: Location-based Search
            await tester.TestLocationSearch();
            
            // Test 4: Pagination
            await tester.TestPagination();

            Console.WriteLine("\n==============================================");
            Console.WriteLine("All Tests Completed!");
            Console.WriteLine("==============================================");
            Console.WriteLine("\nPress any key to exit...");
            Console.ReadKey();
        }

        public async Task TestBasicApiCall()
        {
            Console.WriteLine("\n--- Test 1: Basic API Call ---");
            try
            {
                var result = await CallJoobleApi("software engineer", "New York", 1);
                
                if (result != null)
                {
                    Console.WriteLine($"✓ SUCCESS: Retrieved {result.TotalCount} total jobs");
                    Console.WriteLine($"  Jobs in response: {result.Jobs?.Count ?? 0}");
                    
                    if (result.Jobs != null && result.Jobs.Count > 0)
                    {
                        var firstJob = result.Jobs[0];
                        Console.WriteLine($"\n  Sample Job:");
                        Console.WriteLine($"    Title: {firstJob.Title}");
                        Console.WriteLine($"    Company: {firstJob.Company}");
                        Console.WriteLine($"    Location: {firstJob.Location}");
                        Console.WriteLine($"    Link: {firstJob.Link}");
                    }
                }
                else
                {
                    Console.WriteLine("✗ FAILED: No result returned");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"✗ FAILED: {ex.Message}");
                Console.WriteLine($"  Stack: {ex.StackTrace}");
            }
        }

        public async Task TestDifferentSearchTerms()
        {
            Console.WriteLine("\n--- Test 2: Different Search Terms ---");
            string[] searchTerms = { ".NET Developer", "Python Engineer", "DevOps", "Data Scientist" };
            
            foreach (var term in searchTerms)
            {
                try
                {
                    var result = await CallJoobleApi(term, null, 1);
                    Console.WriteLine($"  {term,-20}: {result?.TotalCount ?? 0} jobs found");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"  {term,-20}: FAILED - {ex.Message}");
                }
                
                await Task.Delay(500); // Be nice to the API
            }
        }

        public async Task TestLocationSearch()
        {
            Console.WriteLine("\n--- Test 3: Location-based Search ---");
            string[] locations = { "New York", "San Francisco", "Remote", "London" };
            
            foreach (var location in locations)
            {
                try
                {
                    var result = await CallJoobleApi("developer", location, 1);
                    Console.WriteLine($"  {location,-15}: {result?.TotalCount ?? 0} jobs found");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"  {location,-15}: FAILED - {ex.Message}");
                }
                
                await Task.Delay(500);
            }
        }

        public async Task TestPagination()
        {
            Console.WriteLine("\n--- Test 4: Pagination ---");
            try
            {
                var page1 = await CallJoobleApi("developer", null, 1);
                var page2 = await CallJoobleApi("developer", null, 2);
                
                Console.WriteLine($"  Page 1: {page1?.Jobs?.Count ?? 0} jobs");
                Console.WriteLine($"  Page 2: {page2?.Jobs?.Count ?? 0} jobs");
                Console.WriteLine($"  Total available: {page1?.TotalCount ?? 0}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"  FAILED: {ex.Message}");
            }
        }

        private async Task<JoobleApiResponse?> CallJoobleApi(string keywords, string? location, int page)
        {
            using var httpClient = new HttpClient();
            httpClient.Timeout = TimeSpan.FromSeconds(30);
            
            var apiUrl = $"{JOOBLE_BASE_URL}{JOOBLE_API_KEY}";

            var requestBody = new
            {
                keywords = keywords,
                location = location ?? string.Empty,
                page = page
            };

            var jsonContent = JsonSerializer.Serialize(requestBody, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            Console.WriteLine($"\n  Request: {keywords} @ {location ?? "Any"} (Page {page})");
            Console.WriteLine($"  URL: {apiUrl}");
            Console.WriteLine($"  Body: {jsonContent}");

            var httpRequest = new HttpRequestMessage(HttpMethod.Post, apiUrl)
            {
                Content = new StringContent(jsonContent, Encoding.UTF8, "application/json")
            };

            httpRequest.Headers.Add("User-Agent", "CareerPathApp/1.0");
            httpRequest.Headers.Add("Accept", "application/json");

            var httpResponse = await httpClient.SendAsync(httpRequest);

            Console.WriteLine($"  Response Status: {(int)httpResponse.StatusCode} {httpResponse.StatusCode}");

            if (!httpResponse.IsSuccessStatusCode)
            {
                var errorContent = await httpResponse.Content.ReadAsStringAsync();
                Console.WriteLine($"  Error Content: {errorContent}");
                return null;
            }

            var responseContent = await httpResponse.Content.ReadAsStringAsync();
            Console.WriteLine($"  Response Length: {responseContent.Length} chars");

            var joobleResponse = JsonSerializer.Deserialize<JoobleApiResponse>(responseContent, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                PropertyNameCaseInsensitive = true
            });

            return joobleResponse;
        }
    }

    // Minimal DTOs for testing
    public class JoobleApiResponse
    {
        public int TotalCount { get; set; }
        public List<JoobleJob> Jobs { get; set; } = new();
    }

    public class JoobleJob
    {
        public string Title { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public string Company { get; set; } = string.Empty;
        public string Link { get; set; } = string.Empty;
        public string Snippet { get; set; } = string.Empty;
        public string Salary { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public string Updated { get; set; } = string.Empty;
        public string Source { get; set; } = string.Empty;
    }
}
