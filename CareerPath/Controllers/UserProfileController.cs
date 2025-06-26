using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using CareerPath.Application.Interfaces;
using CareerPath.Contracts.Dto;
using System.Threading.Tasks;
using System.Security.Claims;

namespace CareerPath.Api.Controllers
{
    [ApiController]
    [Route("api/profiles")]
    [Authorize]
    public class UserProfileController : ControllerBase
    {
        private readonly IUserProfileService _userProfileService;

        public UserProfileController(IUserProfileService userProfileService)
        {
            _userProfileService = userProfileService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var profiles = await _userProfileService.GetAllAsync();
            return Ok(profiles);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var profile = await _userProfileService.GetByIdAsync(id);
            if (profile == null)
                return NotFound(new { Error = "Profile not found or database error occurred" });

            return Ok(profile);
        }
        #region
        //[HttpPost]
        //public async Task<IActionResult> Create([FromBody] CreateUserProfileDto dto)
        //{
        //    var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        //    if (string.IsNullOrEmpty(userId))
        //        return Unauthorized();

        //    var existingProfile = await _userProfileService.GetByIdAsync(userId);
        //    if (existingProfile != null)
        //        return BadRequest(new { Error = "User profile already exists" });

        //    var createdProfile = await _userProfileService.CreateAsync(userId, dto);
        //    if (createdProfile == null)
        //        return StatusCode(500, new { Error = "An error occurred while creating the profile. Please try again later." });

        //    return CreatedAtAction(nameof(GetById), new { id = createdProfile.Id }, createdProfile);
        //}
#endregion // create profile 

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] UpdateUserProfileDto dto)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            if (id != userId)
                return Forbid();

            var updatedProfile = await _userProfileService.UpdateAsync(id, dto);
            if (updatedProfile == null)
                return StatusCode(500, new { Error = "An error occurred while updating the profile. Please try again later." });

            return Ok(updatedProfile);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            if (id != userId)
                return Forbid();

            var result = await _userProfileService.DeleteAsync(id);
            if (!result)
                return StatusCode(500, new { Error = "An error occurred while deleting the profile. Please try again later." });

            return NoContent();
        }
    }
} 