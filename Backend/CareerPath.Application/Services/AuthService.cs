using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CareerPath.Application.Interfaces;
using Microsoft.AspNetCore.Identity;
using CareerPath.Contracts.Dto;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Web;
using CareerPath.Domain.Entities;

namespace CareerPath.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ITokenService _jwtTokenService;
        private readonly IEmailService _emailService;
        private readonly IUserProfileRepository _userProfileRepository;

        public AuthService(
            UserManager<ApplicationUser> userManager, 
            SignInManager<ApplicationUser> signInManager, 
            ITokenService tokenService,
            IEmailService emailService,
            IUserProfileRepository userProfileRepository)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _jwtTokenService = tokenService;
            _emailService = emailService;
            _userProfileRepository = userProfileRepository;
        }

        public async Task<(string? token, string? errorMessage)> LoginUserAsync(LoginDto user)
        {
            try
            {
                var existingUser = await _userManager.FindByEmailAsync(user.Email);
                if (existingUser == null)
                {
                    return (null, "User not found");
                }

                var signInResult = await _signInManager.PasswordSignInAsync(
                    existingUser.UserName,
                    user.Password,
                    isPersistent: false,
                    lockoutOnFailure: false
                );

                if (!signInResult.Succeeded)
                {
                    if (signInResult.IsLockedOut) return (null, "User is locked out");
                    if (signInResult.IsNotAllowed) return (null, "Login not allowed");
                    
                    Console.WriteLine($"Failed login attempt for user: {existingUser.Email}");
                    return (null, "Incorrect password");
                }

                var token = _jwtTokenService.GenerateToken(existingUser);
                return (token, null);
            }
            catch (SqlException ex)
            {
                Console.WriteLine($"SQL Error in LoginUserAsync: {ex.Message}");
                return (null, "Database connection error. Please try again later.");
            }
            catch (DbUpdateException ex)
            {
                Console.WriteLine($"DB Update Error in LoginUserAsync: {ex.Message}");
                return (null, "Database error. Please try again later.");
            }
            catch (Exception ex)
            {
                // Log the exception details for debugging
                Console.WriteLine($"Error in LoginUserAsync: {ex.Message}");
                return (null, "An unexpected error occurred. Please try again later.");
            }
        }

        public async Task<(bool success, string? errorMessage)> RegisterUserAsync(RegisterDto user)
        {
            try
            {
                var existingEmail = await _userManager.FindByEmailAsync(user.Email);
                if (existingEmail != null)
                {
                    return (false, "Email already in use");
                }

                // Create the application user with default ProfileID (empty string)
                var newUser = new ApplicationUser(user.Email, user.Password, user.Username);
                var result = await _userManager.CreateAsync(newUser, user.Password);

                if (!result.Succeeded)
                {
                    var errorMessage = string.Join(", ", result.Errors.Select(e => e.Description));
                    return (false, errorMessage);
                }

                // At this point, the user is created with an empty ProfileID
                try
                {
                    // Get the newly created user to ensure we have the correct ID
                    var createdUser = await _userManager.FindByEmailAsync(user.Email);
                    if (createdUser == null)
                    {
                        return (false, "Failed to retrieve newly created user");
                    }

                    var username = !string.IsNullOrEmpty(user.Username) ? user.Username : user.Email;
                    
                    // Create UserProfile with the same ID as ApplicationUser for direct 1:1 relationship
                    var userProfile = new UserProfile(createdUser.Id, username, user.Email);
                    
                    // Save the profile
                    var createdProfile = await _userProfileRepository.CreateAsync(userProfile);
                    
                    // Update the ApplicationUser with the ProfileID reference
                    createdUser.ProfileID = userProfile.Id;
                    createdUser.SetProfile(userProfile);
                    
                    await _userManager.UpdateAsync(createdUser);
                    
                    return (true, null);
                }
                catch (Exception ex)
                {
                    // Log any errors with profile creation
                    Console.WriteLine($"Error creating profile: {ex.Message}");
                    
                    // Try to clean up the user if profile creation failed
                    try
                    {
                        var createdUser = await _userManager.FindByEmailAsync(user.Email);
                        if (createdUser != null)
                        {
                            await _userManager.DeleteAsync(createdUser);
                        }
                    }
                    catch { /* Ignore cleanup errors */ }
                    
                    return (false, "Error creating user profile. Please try again.");
                }
            }
            catch (SqlException ex)
            {
                Console.WriteLine($"SQL error in RegisterUserAsync: {ex.Message}");
                return (false, "Database connection error. Please try again later.");
            }
            catch (DbUpdateException ex)
            {
                Console.WriteLine($"DB Update error in RegisterUserAsync: {ex.Message}");
                return (false, "Database error. Please try again later.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in RegisterUserAsync: {ex.Message}");
                return (false, "An unexpected error occurred. Please try again later.");
            }
        }

        public async Task<(bool success, string? errorMessage)> ForgotPasswordAsync(ForgotPasswordDto forgotPasswordDto)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(forgotPasswordDto.Email);
                if (user == null)
                {
                    return (true, null); //always return true for security
                }

                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                
                var encodedToken = HttpUtility.UrlEncode(token);
                
                var resetLink = $"https://yourapp.com/reset-password?email={HttpUtility.UrlEncode(user.Email)}&token={encodedToken}";
                
                var subject = "Reset Your CareerPath Password";
                var body = $@"
                <html>
                <head>
                    <style>
                        body {{ font-family: Arial, sans-serif; line-height: 1.6; color: #333; }}
                        .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
                        .header {{ background-color: #4CAF50; color: white; padding: 10px; text-align: center; }}
                        .content {{ padding: 20px; }}
                        .button {{ display: inline-block; background-color: #4CAF50; color: white; padding: 10px 20px; text-decoration: none; border-radius: 5px; }}
                        .footer {{ font-size: 12px; color: #777; margin-top: 30px; }}
                    </style>
                </head>
                <body>
                    <div class='container'>
                        <div class='header'>
                            <h1>Password Reset Request</h1>
                        </div>
                        <div class='content'>
                            <p>Hi {user.UserName},</p>
                            <p>We received a request to reset your password. Click the button below to create a new password:</p>
                            <p><a href='{resetLink}' class='button'>Reset Password</a></p>
                            <p>If the button doesn't work, you can also copy and paste this link into your browser:</p>
                            <p>{resetLink}</p>
                            <p>This link will expire in 24 hours.</p>
                            <p>If you didn't request a password reset, you can ignore this email.</p>
                        </div>
                        <div class='footer'>
                            <p>© {DateTime.Now.Year} CareerPath. All rights reserved.</p>
                        </div>
                    </div>
                </body>
                </html>";

                // Send email with the reset link
                await _emailService.SendEmailAsync(user.Email, subject, body);
                
                return (true, null);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in ForgotPasswordAsync: {ex.Message}");
                return (false, "Failed to process your request. Please try again later.");
            }
        }

        public async Task<(bool success, string? errorMessage)> ResetPasswordAsync(ResetPasswordDto resetPasswordDto)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(resetPasswordDto.Email);
                if (user == null)
                {
                    return (false, "Invalid request.");
                }

                // Reset the password
                var result = await _userManager.ResetPasswordAsync(
                    user, 
                    resetPasswordDto.Token, 
                    resetPasswordDto.NewPassword
                );

                if (!result.Succeeded)
                {
                    var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                    return (false, errors);
                }

                return (true, null);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in ResetPasswordAsync: {ex.Message}");
                return (false, "Failed to reset password. Please try again later.");
            }
        }
    }
}
