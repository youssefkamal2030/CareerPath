using Microsoft.AspNetCore.Mvc;
using CareerPath.Application.Interfaces;
using CareerPath.Contracts.Dto;

namespace CareerPath.Api.Controllers
{
    [ApiController]
    [Route("auth")]
    public class AuthController : Controller
    {
        private readonly IAuthService _authService;
        
        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }
        
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
        {
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
            var (success, errorMessage) = await _authService.ForgotPasswordAsync(forgotPasswordDto);

            if (success)
            {
                // Always return a success message even if email does not exist (for security)
                return Ok(new { Message = "If your email exists in our system, you will receive password reset instructions." });
            }
            else
            {
                return BadRequest(new { Error = errorMessage });
            }
        }
        
        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto resetPasswordDto)
        {
            var (success, errorMessage) = await _authService.ResetPasswordAsync(resetPasswordDto);

            if (success)
            {
                return Ok(new { Message = "Password has been reset successfully. You can now log in with your new password." });
            }
            else
            {
                return BadRequest(new { Error = errorMessage });
            }
        }
        [HttpGet("hello")]
        public IActionResult hello()
        {
            return Ok("API IS WORKING ");
        }
    }
}
