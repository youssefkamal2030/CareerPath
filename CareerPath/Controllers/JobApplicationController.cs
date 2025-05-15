using CareerPath.Application.Interfaces;
using CareerPath.Contracts.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;

namespace CareerPath.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class JobApplicationController : ControllerBase
    {
        private readonly IJobApplicationService _jobApplicationService;
        private readonly UserManager<ApplicationUser> _userManager;

        public JobApplicationController(IJobApplicationService jobApplicationService , UserManager<ApplicationUser> userManager)
        {
            _jobApplicationService = jobApplicationService;
            _userManager = userManager;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<JobApplicationDto>> GetById(int id)
        {
            var application = await _jobApplicationService.GetByIdAsync(id);
            if (application == null)
                return NotFound();

            return Ok(application);
        }

        [HttpGet("user")]
        public async Task<ActionResult<JobApplicationDto>> GetByUserId(string id)
        {
            var userId = _userManager.FindByIdAsync(id).ToString(); 
            var applications = await _jobApplicationService.GetByUserIdAsync(userId);
            return Ok(applications);
        }

        [HttpGet("jobs")]
        public async Task<ActionResult<JobApplicationDto>> GetJobs()
        {
            var applications = await _jobApplicationService.GetJobsAsync();
            return Ok(applications);
        }
        
     
     
    }
} 