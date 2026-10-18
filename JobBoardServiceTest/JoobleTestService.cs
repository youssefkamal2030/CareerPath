using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace JobBoardServiceTest;

public class JoobleTestService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<JoobleTestService> _logger;
    private readonly ExternalApiSettings _apiSettings;
    private readonly string _joobleBaseUrl;
    private readonly string _apiKey;

    public JoobleTestService(
        HttpClient httpClient,
        ILogger<JoobleTestService> logger,
        IOptions<ExternalApiSettings> apiSettings)
    {
        _httpClient = httpClient;
        _logger = logger;
        _apiSettings = apiSettings.Value;
        _apiKey = _apiSettings.Jooble_API_Key;
        _joobleBaseUrl = "https://jooble.org/api/";

        _logger.LogInformation("JoobleTestService initialized with API key: {ApiKey}", 
            string.IsNullOrEmpty(_apiKey) ? "MISSING" : $"{_apiKey[..10]}...");
    }

    public async Task<JoobleApiResponse?> FetchJobsAsync(string keywords, string location, int page = 1)
    {
        try
        {
            _logger.LogInformation("Starting Jooble API call...");
            _logger.LogInformation("Keywords: {Keywords}, Location: {Location}, Page: {Page}", 
                keywords, location, page);

            // Build the API URL
            var apiUrl = $"{_joobleBaseUrl}{_apiKey}";
            _logger.LogInformation("API URL: {Url}", apiUrl);

            // Create request payload
            var requestBody = new JoobleSearchRequest
            {
                Keywords = keywords,
                Location = location,
                Page = page
            };

            // Serialize to JSON
            var jsonContent = JsonSerializer.Serialize(requestBody, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true
            });

            _logger.LogInformation("Request Body:\n{Body}", jsonContent);

            // Create HTTP request
            var httpRequest = new HttpRequestMessage(HttpMethod.Post, apiUrl)
            {
                Content = new StringContent(jsonContent, Encoding.UTF8, "application/json")
            };
            
            // Add headers to mimic browser request
            httpRequest.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36");
            httpRequest.Headers.Add("Accept", "application/json");
            httpRequest.Headers.Add("Accept-Language", "en-US,en;q=0.9");

            _logger.LogInformation("Sending HTTP POST request...");

            // Send request
            var httpResponse = await _httpClient.SendAsync(httpRequest);

            _logger.LogInformation("Response Status Code: {StatusCode}", httpResponse.StatusCode);

            // Check for success
            if (!httpResponse.IsSuccessStatusCode)
            {
                var errorContent = await httpResponse.Content.ReadAsStringAsync();
                _logger.LogError("Jooble API Error: {StatusCode}\nError Content:\n{Content}", 
                    httpResponse.StatusCode, errorContent);
                return null;
            }

            // Read response
            var responseContent = await httpResponse.Content.ReadAsStringAsync();
            _logger.LogInformation("Response received. Length: {Length} characters", responseContent.Length);
            _logger.LogDebug("Raw Response:\n{Response}", responseContent);

            // Deserialize
            var joobleResponse = JsonSerializer.Deserialize<JoobleApiResponse>(responseContent, 
                new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    PropertyNameCaseInsensitive = true
                });

            if (joobleResponse != null)
            {
                _logger.LogInformation("✓ Successfully parsed response!");
                _logger.LogInformation("Total Count: {TotalCount}", joobleResponse.TotalCount);
                _logger.LogInformation("Jobs Returned: {JobCount}", joobleResponse.Jobs.Count);
            }

            return joobleResponse;
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "HTTP error calling Jooble API");
            return null;
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "Error deserializing Jooble API response");
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error calling Jooble API");
            return null;
        }
    }
}
