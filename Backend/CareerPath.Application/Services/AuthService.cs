using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CareerPath.Application.Interfaces;
using Microsoft.AspNetCore.Identity;
using CareerPath.Contracts.Dto;

namespace CareerPath.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ITokenService _jwtTokenService;

        public AuthService(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, ITokenService tokenService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _jwtTokenService = tokenService;
        }

        public async Task<(string? token, string? errorMessage)> LoginUserAsync(LoginDto user)
        {
            var existingUser = await _userManager.FindByEmailAsync(user.Email);
            if (existingUser == null)
            {
                return (null, "User not found");
            }

            var signInResult = await _signInManager.PasswordSignInAsync(
                existingUser.Email,
                user.Password,
                isPersistent: false,
                lockoutOnFailure: false
            );

            if (!signInResult.Succeeded)
            {
                if (signInResult.IsLockedOut) return (null, "User is locked out");
                if (signInResult.IsNotAllowed) return (null, "Login not allowed");
                return (null, "Incorrect password");
            }

            var token = _jwtTokenService.GenerateToken(existingUser);
            return (token, null);
        }

        public async Task<(bool success, string? errorMessage)> RegisterUserAsync(RegisterDto user)
        {
            var existingEmail = await _userManager.FindByEmailAsync(user.Email);
            if (existingEmail != null)
            {
                return (false, "Email already in use");
            }

            var newUser = new ApplicationUser(user.Email, user.Password);
            var result = await _userManager.CreateAsync(newUser, user.Password);

            if (!result.Succeeded)
            {
                var errorMessage = string.Join(", ", result.Errors.Select(e => e.Description));
                return (false, errorMessage);
            }

            return (true, null);
        }
    }
}
