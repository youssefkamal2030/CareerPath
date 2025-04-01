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
    }
}
