using System.Text.Json.Serialization;

namespace CareerPath.Contracts.External.Jooble;

/// <summary>
/// Request model for Jooble API job search
/// </summary>
public class JoobleSearchRequest
{
    [JsonPropertyName("keywords")]
    public string Keywords { get; set; } = string.Empty;

    [JsonPropertyName("location")]
    public string Location { get; set; } = string.Empty;

    [JsonPropertyName("radius")]
    public string? Radius { get; set; }

    [JsonPropertyName("salary")]
    public string? Salary { get; set; }

    [JsonPropertyName("page")]
    public int Page { get; set; } = 1;
}

/// <summary>
/// Response model from Jooble API
/// Raw response structure from Jooble - kept separate from internal models
/// </summary>
public class JoobleApiResponse
{
    [JsonPropertyName("totalCount")]
    public int TotalCount { get; set; }

    [JsonPropertyName("jobs")]
    public List<JoobleJob> Jobs { get; set; } = new List<JoobleJob>();
}

/// <summary>
/// Individual job from Jooble API
/// This is the raw structure returned by Jooble
/// </summary>
public class JoobleJob
{
    [JsonPropertyName("title")]
    public string Title { get; set; } = string.Empty;

    [JsonPropertyName("location")]
    public string Location { get; set; } = string.Empty;

    [JsonPropertyName("snippet")]
    public string Snippet { get; set; } = string.Empty;

    [JsonPropertyName("salary")]
    public string Salary { get; set; } = string.Empty;

    [JsonPropertyName("source")]
    public string Source { get; set; } = string.Empty;

    [JsonPropertyName("type")]
    public string Type { get; set; } = string.Empty;

    [JsonPropertyName("link")]
    public string Link { get; set; } = string.Empty;

    [JsonPropertyName("company")]
    public string Company { get; set; } = string.Empty;

    [JsonPropertyName("updated")]
    public string Updated { get; set; } = string.Empty;

    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;
}
