using Microsoft.AspNetCore.Mvc;
using CareerPath.Application.Interfaces;
using CareerPath.Contracts.Dto;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace CareerPath.Api.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ILogger<AuthController> _logger;
        
        public AuthController(IAuthService authService, ILogger<AuthController> logger)
        {
            _authService = authService ?? throw new ArgumentNullException(nameof(authService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
        
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            
            var (success, errorMessage) = await _authService.RegisterUserAsync(registerDto);

            if (success)
            {
                return Ok(new { Message = "User registered successfully" });
            }
            else
            {
                return BadRequest(new { Error = errorMessage });
            }
        }
        
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            
            var (token, errorMessage) = await _authService.LoginUserAsync(loginDto);

            if (token != null)
            {
                return Ok(new { Token = token });
            }
            else
            {
                return BadRequest(new { Error = errorMessage });
            }
        }
        
        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto forgotPasswordDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            
            try
            {
                var (success, errorMessage) = await _authService.ForgotPasswordAsync(forgotPasswordDto);

                if (success)
                {
                    return Ok(new { Message = "If your email exists in our system, you will receive password reset instructions shortly." });
                }
                else
                {
                    _logger.LogWarning("Password reset request failed: {ErrorMessage}", errorMessage);
                    return BadRequest(new { Error = "Unable to process your request. Please try again later." });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in ForgotPassword endpoint");
                return StatusCode(500, new { Error = "An unexpected error occurred. Please try again later." });
            }
        }
        
        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto resetPasswordDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            
            try
            {
                var (success, errorMessage) = await _authService.ResetPasswordAsync(resetPasswordDto);

                if (success)
                {
                    return Ok(new { Message = "Your password has been reset successfully. You can now log in with your new password." });
                }
                else
                {
                    return BadRequest(new { Error = errorMessage });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in ResetPassword endpoint");
                return StatusCode(500, new { Error = "An unexpected error occurred. Please try again later." });
            }
        }
        
        [HttpGet("hello")]
        public IActionResult Hello()
        {
            return Ok("API is working properly");
        }
    }
}
