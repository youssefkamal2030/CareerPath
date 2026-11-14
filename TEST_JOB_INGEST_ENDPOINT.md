# Testing JobBoardService - Job Ingest Endpoint

## Endpoint Details

**HTTP Method:** `POST`  
**Route:** `/api/Jobs/ingest`  
**Controller:** `JobsController`  
**Action:** `IngestJobs`

## How It Works

This endpoint calls `JobBoardService.FetchAndIngestJobsAsync()` which:
1. Fetches jobs from Jooble API using your configured API key
2. Maps Jooble jobs to your internal Job domain model using `JoobleJobMapper`
3. Saves the jobs to your database (with duplicate checking)
4. Returns the list of newly ingested jobs

## API Configuration Status

✅ **Jooble API Key is configured** in `appsettings.json`:
```json
"Sources": {
    "Jooble_API_Key": "c43b401b-b24d-4068-84a9-be789bc49544"
}
```

## Request Examples

### Using cURL (PowerShell)

```powershell
# Basic search for .NET jobs
Invoke-RestMethod -Uri "https://localhost:7297/api/Jobs/ingest?searchTerm=.NET" -Method POST

# Search with location
Invoke-RestMethod -Uri "https://localhost:7297/api/Jobs/ingest?searchTerm=developer&location=New%20York" -Method POST

# Search for Python jobs in San Francisco
Invoke-RestMethod -Uri "https://localhost:7297/api/Jobs/ingest?searchTerm=Python&location=San%20Francisco" -Method POST
```

### Using HTTP File (CareerPath.http)

Add this to your `CareerPath.http` file:

```http
### Ingest Jobs from Jooble API
POST https://localhost:7297/api/Jobs/ingest?searchTerm=software%20engineer&location=California
Content-Type: application/json

###

### Ingest .NET Developer Jobs
POST https://localhost:7297/api/Jobs/ingest?searchTerm=.NET%20Developer
Content-Type: application/json

###

### Ingest Remote Python Jobs
POST https://localhost:7297/api/Jobs/ingest?searchTerm=Python&location=Remote
Content-Type: application/json
```

### Using Postman/Thunder Client

**URL:** `POST {{baseUrl}}/api/Jobs/ingest`

**Query Parameters:**
- `searchTerm` (required): e.g., "software engineer", ".NET", "Python developer"
- `location` (optional): e.g., "New York", "California", "Remote"

## Expected Response

### Success (200 OK)
```json
[
    {
        "jobId": "guid-here",
        "jobTitle": "Senior .NET Developer",
        "companyId": null,
        "companyName": "TechCorp Inc",
        "location": "New York, NY",
        "description": "Job description...",
        "requirements": ["C#", ".NET Core", "SQL Server"],
        "salaryRange": "$100K - $150K",
        "employmentType": "Full-time",
        "experienceLevel": "Senior",
        "postedDate": "2026-11-14T10:30:00Z",
        "externalId": "jooble-12345",
        "externalUrl": "https://jooble.org/job/...",
        "sourceProvider": "Jooble",
        "status": "Active"
    }
]
```

### Error (400 Bad Request)
```json
"Search term is required"
```

### Error (500 Internal Server Error)
If the Jooble API call fails or there's a database issue

## Common Issues & Troubleshooting

### 1. **500 Error - Configuration Issue**
**Check:** Is `ExternalApiSettings` bound correctly in `Program.cs`?

Look for this in `Program.cs`:
```csharp
builder.Services.Configure<ExternalApiSettings>(
    builder.Configuration.GetSection(ExternalApiSettings.SectionName)
);
```

### 2. **500 Error - Jooble API Failure**
**Check the logs for:**
- "Jooble API returned error: {StatusCode}"
- "HTTP error calling Jooble API"
- "Error deserializing Jooble API response"

**Possible causes:**
- Invalid API key
- Jooble API rate limiting
- Network/firewall issues
- Jooble API changes (response format)

### 3. **Empty Response but No Error**
This might mean:
- No jobs matched your search criteria
- All fetched jobs already exist in database (duplicate check working)

### 4. **Null Reference Exception**
Check that all dependencies are registered in DI:
- `IJobBoardService` → `JobBoardService`
- `IJobMapperFactory` → `JobMapperFactory`
- `IJobRepository` → `JobRepository`

## Debugging Steps

### Step 1: Check if service is running
```powershell
cd d:\Work\CareerPathProject\CareerPath\CareerPath
dotnet run
```

Look for the port in console output (usually 7297 for HTTPS).

### Step 2: Test with simple request
```powershell
# Replace port if different
Invoke-WebRequest -Uri "https://localhost:7297/api/Jobs/ingest?searchTerm=developer" -Method POST -SkipCertificateCheck
```

### Step 3: Check logs
Look in the console output for:
- "Fetching jobs from Jooble API: Query={Query}"
- "Successfully fetched {Count} jobs from Jooble API"
- "Mapped {Count} jobs successfully"
- "Successfully ingested {Count} new jobs"

### Step 4: Test Jooble API directly
```powershell
$headers = @{
    "Content-Type" = "application/json"
    "User-Agent" = "CareerPathApp/1.0"
}

$body = @{
    keywords = "developer"
    location = "New York"
    page = 1
} | ConvertTo-Json

Invoke-RestMethod -Uri "https://jooble.org/api/c43b401b-b24d-4068-84a9-be789bc49544" -Method POST -Headers $headers -Body $body
```

## Related Files

- **Service Implementation:** `CareerPath.Infrastructure/Services/JobBoardService.cs`
- **Controller:** `CareerPath/Controllers/JobsController.cs`
- **Mapper:** `CareerPath.Infrastructure/Mappers/JoobleJobMapper.cs`
- **Configuration:** `CareerPath/appsettings.json` (Sources section)
- **Domain Model:** `CareerPath.Domain/Recommendations/Job.cs`

## Next Steps After Successful Test

1. Check ingested jobs in database
2. Test job search endpoints to retrieve ingested jobs
3. Test job recommendations using ingested data
4. Set up background job ingestion service (if not already running)
