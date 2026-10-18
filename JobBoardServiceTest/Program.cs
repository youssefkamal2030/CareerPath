using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace JobBoardServiceTest;

class Program
{
    static async Task Main(string[] args)
    {
        Console.WriteLine("==============================================");
        Console.WriteLine("   Jooble API Test - Isolated Service Test");
        Console.WriteLine("==============================================\n");

        // Build configuration
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false)
            .Build();

        // Setup dependency injection
        var services = new ServiceCollection();

        // Add logging
        services.AddLogging(builder =>
        {
            builder.AddConsole();
            builder.SetMinimumLevel(LogLevel.Debug);
        });

        // Add configuration
        services.Configure<ExternalApiSettings>(
            configuration.GetSection(ExternalApiSettings.SectionName));

        // Add HttpClient
        services.AddHttpClient<JoobleTestService>();

        // Build service provider
        var serviceProvider = services.BuildServiceProvider();

        // Get the test service
        var testService = serviceProvider.GetRequiredService<JoobleTestService>();
        var logger = serviceProvider.GetRequiredService<ILogger<Program>>();

        // Output file path
        var outputFile = Path.Combine(Directory.GetCurrentDirectory(), "jooble_test_results.json");
        var logFile = Path.Combine(Directory.GetCurrentDirectory(), "jooble_test_log.txt");

        try
        {
            // Test parameters - using exact same params that worked in online tester
            var keywords = "it";
            var location = "Bern";
            var page = 1;

            Console.WriteLine($"Testing Jooble API with:");
            Console.WriteLine($"  Keywords: {keywords}");
            Console.WriteLine($"  Location: {location}");
            Console.WriteLine($"  Page: {page}\n");
            Console.WriteLine("Calling API...\n");

            // Call the API
            var result = await testService.FetchJobsAsync(keywords, location, page);

            if (result != null)
            {
                Console.WriteLine("\n==============================================");
                Console.WriteLine("✓ SUCCESS - API Call Completed!");
                Console.WriteLine("==============================================\n");
                Console.WriteLine($"Total Jobs Available: {result.TotalCount}");
                Console.WriteLine($"Jobs Returned: {result.Jobs.Count}\n");

                // Display first 3 jobs
                Console.WriteLine("--- Sample Jobs (first 3) ---\n");
                foreach (var job in result.Jobs.Take(3))
                {
                    Console.WriteLine($"Title: {job.Title}");
                    Console.WriteLine($"Company: {job.Company}");
                    Console.WriteLine($"Location: {job.Location}");
                    Console.WriteLine($"Type: {job.Type}");
                    Console.WriteLine($"Salary: {job.Salary}");
                    Console.WriteLine($"Link: {job.Link}");
                    Console.WriteLine($"ID: {job.Id}");
                    Console.WriteLine($"Snippet: {job.Snippet[..Math.Min(100, job.Snippet.Length)]}...");
                    Console.WriteLine();
                }

                // Save full results to file
                var jsonOptions = new JsonSerializerOptions
                {
                    WriteIndented = true,
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                };

                var jsonContent = JsonSerializer.Serialize(result, jsonOptions);
                await File.WriteAllTextAsync(outputFile, jsonContent);

                Console.WriteLine($"\n✓ Full results saved to: {outputFile}");

                // Create a summary log
                var logContent = $@"Jooble API Test Results
========================
Test Date: {DateTime.Now}
Keywords: {keywords}
Location: {location}
Page: {page}

Results:
--------
Total Jobs Available: {result.TotalCount}
Jobs Returned: {result.Jobs.Count}

Sample Jobs:
------------
{string.Join("\n\n", result.Jobs.Take(5).Select(j => $@"
Title: {j.Title}
Company: {j.Company}
Location: {j.Location}
Type: {j.Type}
Salary: {j.Salary}
Source: {j.Source}
Link: {j.Link}
Updated: {j.Updated}
ID: {j.Id}
Snippet: {j.Snippet}
"))}

Full JSON response saved to: {outputFile}
";

                await File.WriteAllTextAsync(logFile, logContent);
                Console.WriteLine($"✓ Summary log saved to: {logFile}\n");

                // Validate data structure
                Console.WriteLine("\n--- Data Structure Validation ---");
                Console.WriteLine($"✓ API returned valid response structure");
                Console.WriteLine($"✓ TotalCount field: {result.TotalCount}");
                Console.WriteLine($"✓ Jobs array length: {result.Jobs.Count}");
                
                if (result.Jobs.Any())
                {
                    var firstJob = result.Jobs.First();
                    Console.WriteLine("\n✓ Job object structure validated:");
                    Console.WriteLine($"  - Has Title: {!string.IsNullOrEmpty(firstJob.Title)}");
                    Console.WriteLine($"  - Has Company: {!string.IsNullOrEmpty(firstJob.Company)}");
                    Console.WriteLine($"  - Has Location: {!string.IsNullOrEmpty(firstJob.Location)}");
                    Console.WriteLine($"  - Has Link: {!string.IsNullOrEmpty(firstJob.Link)}");
                    Console.WriteLine($"  - Has ID: {!string.IsNullOrEmpty(firstJob.Id)}");
                    Console.WriteLine($"  - Has Snippet: {!string.IsNullOrEmpty(firstJob.Snippet)}");
                }

                Console.WriteLine("\n==============================================");
                Console.WriteLine("Test completed successfully!");
                Console.WriteLine("==============================================");
            }
            else
            {
                Console.WriteLine("\n==============================================");
                Console.WriteLine("✗ FAILED - No data returned from API");
                Console.WriteLine("==============================================");
                Console.WriteLine("\nCheck the logs above for error details.");
                
                var errorLog = $@"Jooble API Test - FAILED
========================
Test Date: {DateTime.Now}
Keywords: {keywords}
Location: {location}
Page: {page}

Error: API returned null response
Check console output for detailed error messages.
";
                await File.WriteAllTextAsync(logFile, errorLog);
                Console.WriteLine($"\nError log saved to: {logFile}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("\n==============================================");
            Console.WriteLine("✗ ERROR - Exception occurred");
            Console.WriteLine("==============================================");
            Console.WriteLine($"\nException Type: {ex.GetType().Name}");
            Console.WriteLine($"Message: {ex.Message}");
            Console.WriteLine($"\nStack Trace:\n{ex.StackTrace}");

            var errorLog = $@"Jooble API Test - ERROR
========================
Test Date: {DateTime.Now}

Exception: {ex.GetType().Name}
Message: {ex.Message}

Stack Trace:
{ex.StackTrace}
";
            await File.WriteAllTextAsync(logFile, errorLog);
            Console.WriteLine($"\nError details saved to: {logFile}");
        }

        Console.WriteLine("\nPress any key to exit...");
        Console.ReadKey();
    }
}
