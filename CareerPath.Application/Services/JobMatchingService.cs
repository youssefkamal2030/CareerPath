using CareerPath.Application.Interfaces;
using CareerPath.Contracts.Dto.CVAnalysis;
using CareerPath.Contracts.Dto.JobRecommendation;
using CareerPath.Domain.Recommendations;

namespace CareerPath.Application.Services;

/// <summary>
/// Service for matching CVs with jobs and calculating match scores
/// </summary>
public class JobMatchingService : IJobMatchingService
{
    private readonly IJobQueryService _jobQueryService;

    public JobMatchingService(IJobQueryService jobQueryService)
    {
        _jobQueryService = jobQueryService;
    }

    public async Task<List<JobMatchDto>> MatchJobsAsync(ExtractedCVDataDto cvData, JobMatchCriteria criteria)
    {
        if (cvData == null)
            throw new ArgumentNullException(nameof(cvData));

        // Get candidate jobs
        var candidateJobs = await GetCandidateJobsAsync(cvData, criteria);

        // Score and rank each job
        var matchedJobs = new List<JobMatchDto>();

        foreach (var job in candidateJobs)
        {
            var matchScore = CalculateMatchScore(cvData, job, criteria);

            if (matchScore.OverallScore >= criteria.MinimumScore)
            {
                matchedJobs.Add(new JobMatchDto
                {
                    JobId = job.Id,
                    JobTitle = job.Title,
                    CompanyName = job.Company,
                    Location = job.Location ?? string.Empty,
                    Description = job.Description ?? string.Empty,
                    Requirements = job.Requirements ?? string.Empty,
                    SalaryRange = job.Salary ?? string.Empty,
                    PostedDate = job.PostedDate,
                    Source = job.Source ?? string.Empty,
                    ExternalUrl = job.ExternalUrl ?? string.Empty,
                    MatchScore = matchScore,
                    MatchedAt = DateTime.UtcNow
                });
            }
        }

        return matchedJobs.OrderByDescending(m => m.MatchScore.OverallScore).Take(criteria.MaxResults).ToList();
    }

    public JobMatchScoreDto CalculateMatchScore(ExtractedCVDataDto cvData, Job job, JobMatchCriteria criteria)
    {
        var matchScore = new JobMatchScoreDto { JobId = job.Id };

        matchScore.SkillsScore = CalculateSkillsScore(cvData.Skills, job);
        matchScore.MatchedSkills = FindMatchedSkills(cvData.Skills, job);
        matchScore.ExperienceScore = CalculateExperienceScore(cvData.TotalYearsOfExperience, cvData.WorkExperience, job);
        matchScore.EducationScore = CalculateEducationScore(cvData.Education, job);
        matchScore.LocationScore = CalculateLocationScore(cvData.Location, job.Location);

        matchScore.OverallScore = Math.Round(
            (matchScore.SkillsScore * criteria.SkillsWeight) +
            (matchScore.ExperienceScore * criteria.ExperienceWeight) +
            (matchScore.EducationScore * criteria.EducationWeight) +
            (matchScore.LocationScore * criteria.LocationWeight), 2);

        matchScore.Explanation = GenerateMatchExplanation(matchScore, cvData, job);

        return matchScore;
    }

    private async Task<List<Job>> GetCandidateJobsAsync(ExtractedCVDataDto cvData, JobMatchCriteria criteria)
    {
        var searchCriteria = new JobSearchCriteria
        {
            Keywords = string.Join(" ", cvData.Skills.Take(5)),
            Location = criteria.PreferredLocations.Any() ? criteria.PreferredLocations.First() : cvData.Location,
            PageSize = criteria.MaxResults * 3,
            PageNumber = 1
        };

        var queryResult = await _jobQueryService.SearchJobsAsync(searchCriteria);
        return queryResult.Jobs.ToList();
    }

    private decimal CalculateSkillsScore(List<string> cvSkills, Job job)
    {
        if (!cvSkills.Any()) return 0;

        var jobText = $"{job.Title} {job.Description} {job.Requirements}".ToLowerInvariant();
        var matchedCount = cvSkills.Count(skill => jobText.Contains(skill.ToLowerInvariant()));

        return Math.Round((decimal)matchedCount / cvSkills.Count * 100, 2);
    }

    private List<string> FindMatchedSkills(List<string> cvSkills, Job job)
    {
        var jobText = $"{job.Title} {job.Description} {job.Requirements}".ToLowerInvariant();
        return cvSkills.Where(skill => jobText.Contains(skill.ToLowerInvariant())).ToList();
    }

    private decimal CalculateExperienceScore(int totalYears, List<WorkExperienceDto> workExperiences, Job job)
    {
        var jobText = $"{job.Title} {job.Description} {job.Requirements}".ToLowerInvariant();
        var requiredYears = ExtractRequiredYears(jobText);

        decimal score = requiredYears.HasValue
            ? totalYears >= requiredYears.Value ? 100 : (decimal)totalYears / requiredYears.Value * 100
            : Math.Min(100, totalYears * 10);

        if (workExperiences != null)
        {
            foreach (var exp in workExperiences)
            {
                if (CalculateStringSimilarity(exp.JobTitle.ToLowerInvariant(), job.Title.ToLowerInvariant()) > 0.6m)
                {
                    score = Math.Min(100, score * 1.2m);
                    break;
                }
            }
        }

        return Math.Round(Math.Min(100, score), 2);
    }

    private int? ExtractRequiredYears(string jobText)
    {
        var match = System.Text.RegularExpressions.Regex.Match(jobText, @"(\d+)\+?\s*years?\s*(?:of\s*)?(?:experience|exp)");
        return match.Success && int.TryParse(match.Groups[1].Value, out var years) ? years : null;
    }

    private decimal CalculateEducationScore(List<EducationDto> educations, Job job)
    {
        if (!educations.Any()) return 50;

        var jobText = $"{job.Title} {job.Description} {job.Requirements}".ToLowerInvariant();
        var requiresBachelors = jobText.Contains("bachelor");
        var requiresMasters = jobText.Contains("master");
        var requiresPhd = jobText.Contains("phd");

        var highestLevel = GetHighestDegreeLevel(educations);

        decimal score = 50;
        if (requiresPhd && highestLevel >= 4) score = 100;
        else if (requiresMasters && highestLevel >= 3) score = 100;
        else if (requiresBachelors && highestLevel >= 2) score = 100;
        else if (highestLevel >= 2) score = 80;
        else if (highestLevel >= 1) score = 60;

        return Math.Round(score, 2);
    }

    private int GetHighestDegreeLevel(List<EducationDto> educations)
    {
        var maxLevel = 0;
        foreach (var edu in educations)
        {
            var degree = edu.Degree.ToLowerInvariant();
            int level = 0;
            if (degree.Contains("phd") || degree.Contains("doctorate")) level = 4;
            else if (degree.Contains("master")) level = 3;
            else if (degree.Contains("bachelor")) level = 2;
            else if (degree.Contains("associate") || degree.Contains("diploma")) level = 1;
            maxLevel = Math.Max(maxLevel, level);
        }
        return maxLevel;
    }

    private decimal CalculateLocationScore(string? cvLocation, string? jobLocation)
    {
        if (string.IsNullOrWhiteSpace(cvLocation) || string.IsNullOrWhiteSpace(jobLocation)) return 50;

        var cvLoc = cvLocation.ToLowerInvariant();
        var jobLoc = jobLocation.ToLowerInvariant();

        if (cvLoc == jobLoc) return 100;
        if (jobLoc.Contains("remote") || jobLoc.Contains("anywhere")) return 100;

        var similarity = CalculateStringSimilarity(cvLoc, jobLoc);
        if (similarity > 0.7m) return 90;
        if (similarity > 0.4m) return 70;

        return 30;
    }

    private decimal CalculateStringSimilarity(string str1, string str2)
    {
        if (string.IsNullOrEmpty(str1) || string.IsNullOrEmpty(str2)) return 0;
        var maxLength = Math.Max(str1.Length, str2.Length);
        if (maxLength == 0) return 1.0m;
        var distance = LevenshteinDistance(str1, str2);
        return 1.0m - ((decimal)distance / maxLength);
    }

    private int LevenshteinDistance(string s, string t)
    {
        var n = s.Length;
        var m = t.Length;
        var d = new int[n + 1, m + 1];

        if (n == 0) return m;
        if (m == 0) return n;

        for (var i = 0; i <= n; i++) d[i, 0] = i;
        for (var j = 0; j <= m; j++) d[0, j] = j;

        for (var i = 1; i <= n; i++)
        {
            for (var j = 1; j <= m; j++)
            {
                var cost = (t[j - 1] == s[i - 1]) ? 0 : 1;
                d[i, j] = Math.Min(Math.Min(d[i - 1, j] + 1, d[i, j - 1] + 1), d[i - 1, j - 1] + cost);
            }
        }

        return d[n, m];
    }

    private string GenerateMatchExplanation(JobMatchScoreDto score, ExtractedCVDataDto cvData, Job job)
    {
        var explanations = new List<string>();

        if (score.SkillsScore >= 70)
            explanations.Add($"Strong skills match ({score.MatchedSkills.Count}/{cvData.Skills.Count} skills)");
        else if (score.SkillsScore >= 40)
            explanations.Add($"Moderate skills match ({score.MatchedSkills.Count} skills)");
        else
            explanations.Add("Limited skills overlap");

        if (score.ExperienceScore >= 80)
            explanations.Add($"Experience aligns well ({cvData.TotalYearsOfExperience} years)");
        else if (score.ExperienceScore >= 50)
            explanations.Add($"Some relevant experience ({cvData.TotalYearsOfExperience} years)");

        if (score.EducationScore >= 80)
            explanations.Add("Education requirements met");

        if (score.LocationScore >= 90)
            explanations.Add("Excellent location match");
        else if (score.LocationScore < 50)
            explanations.Add("Different location");

        return string.Join(". ", explanations) + ".";
    }
}
