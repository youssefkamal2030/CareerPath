using CareerPath.Application.Services;
using Microsoft.AspNetCore.Mvc;
using CareerPath.Contracts.Dto;
using Microsoft.AspNetCore.Identity;
using CareerPath.Domain.Entities;
using CareerPath.Application.Interfaces;
using AutoMapper;
using static System.Net.Http.HttpClient;
namespace CareerPath.Api.Controllers
{
    [ApiController]
    [Route("api/ai")]
    public class AIEndPointsController : ControllerBase
    {
        private readonly ICVAnalysisService _cvAnalysisService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<AIEndPointsController> _logger;
        private readonly IMapper _mapper;

        public AIEndPointsController(
            ICVAnalysisService cvAnalysisService,
            UserManager<ApplicationUser> userManager,
            ILogger<AIEndPointsController> logger,
            IMapper mapper)
        {
            _cvAnalysisService = cvAnalysisService;
            _userManager = userManager;
            _logger = logger;
            _mapper = mapper;
        }


        [HttpPost("extract")]
        public async Task<IActionResult> SaveExtractedData([FromBody] CVAnalysisDto data)
        {
            try
            {
                if (data == null || string.IsNullOrEmpty(data.PersonalInformation?.Email))
                {
                    return BadRequest("Invalid CV analysis data");
                }

                var existingUser = await _userManager.FindByEmailAsync(data.PersonalInformation.Email);
                if (existingUser == null)
                {
                    return NotFound($"User with email {data.PersonalInformation.Email} not found");
                }

                var result = await _cvAnalysisService.SaveCVAnalysisAsync(existingUser.Id, data);
                if (!result)
                {
                    return StatusCode(500, "Failed to save CV analysis data");
                }

                return Ok(new { message = "CV analysis data saved successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving CV analysis data for user {Email}",
                    data?.PersonalInformation?.Email);
                return StatusCode(500, "An error occurred while processing your request");
            }
        }

        [HttpGet("analysis/{email}")]
        public async Task<IActionResult> GetCVAnalysis(string email)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(email);
                if (user == null)
                {
                    return NotFound($"User with email {email} not found");
                }

                var analysis = await _cvAnalysisService.GetCVAnalysisByUserIdAsync(user.Id);
                if (analysis == null)
                {
                    return NotFound("No CV analysis data found for this user");
                }

                return Ok(analysis);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving CV analysis data for user {Email}", email);
                return StatusCode(500, "An error occurred while processing your request");
            }
        }


        [HttpPost("recommend/{userId}")]
        public async Task<IActionResult> RecommendJobsById(string userId)
        {
            try
            {
                var response = await _cvAnalysisService.RecommendJobsAsync(userId);
                _logger.LogInformation("Recommended jobs for user {UserId}", userId);
                _logger.LogInformation("Response: {Response}", response);
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error recommending jobs for user {Email}", userId);
                return StatusCode(500, "An error occurred while processing your request");
            }
        }
        [HttpPost("recommenderSystem/{userId}")]
        public async Task<IActionResult> RecommenderSystem(string userId)
        {
            try
            {
                var response = await _cvAnalysisService.RecommnderSystem(userId);
                _logger.LogInformation("Recommended jobs for user {UserId}", userId);
                _logger.LogInformation("Response: {Response}", response);
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error recommending jobs for user {Email}", userId);
                return StatusCode(500, "An error occurred while processing your request");
            }
        }
        //[HttpPost("CV /{userId}")]
        //public async Task<IActionResult> UserCV([FromBody] CV cv)
        //{

        //}

    }
}
