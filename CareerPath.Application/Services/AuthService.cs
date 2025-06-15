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
using EmailConfigration.EmailConfig;
using Microsoft.Extensions.Logging;

namespace CareerPath.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ITokenService _jwtTokenService;
        private readonly EmailSender _emailSender;
        private readonly ILogger<AuthService> _logger;
        private readonly IUnitOfWork _unitOfWork;

        public AuthService(
            UserManager<ApplicationUser> userManager, 
            SignInManager<ApplicationUser> signInManager, 
            ITokenService tokenService,
            EmailSender emailSender,
            ILogger<AuthService> logger,
            IUnitOfWork unitOfWork)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _jwtTokenService = tokenService;
            _emailSender = emailSender;
            _logger = logger;
            _unitOfWork = unitOfWork;
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
                     await _unitOfWork.UserProfiles.AddAsync(userProfile);
                    await _unitOfWork.CompleteAsync();

                    
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
                if (string.IsNullOrEmpty(forgotPasswordDto.Email))
                {
                    return (false, "Email is required");
                }

                // Normalize the email for consistent lookup
                var email = forgotPasswordDto.Email.NormalizeEmail();
                
                if (!email.IsValidEmail())
                {
                    return (false, "Invalid email format");
                }

                var user = await _userManager.FindByEmailAsync(email);
                
                // Always return success for security reasons (don't reveal if email exists)
                if (user == null)
                {
                    _logger.LogInformation("Password reset requested for non-existent email: {Email}", email);
                    return (true, null);
                }

                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                var encodedToken = HttpUtility.UrlEncode(token);
                
                // Build the reset URL - update this with your actual frontend URL
                var resetLink = $"https://careerpath.com/reset-password?email={HttpUtility.UrlEncode(user.Email)}&token={encodedToken}";
                
                var subject = "Reset Your CareerPath Password";
                var content = $@"
                <html>
                <head>
                    <style>
                        body {{ font-family: Arial, sans-serif; line-height: 1.6; color: #333; }}
                        .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
                        .header {{ background-color: #4A6FFF; color: white; padding: 15px; text-align: center; border-radius: 5px 5px 0 0; }}
                        .content {{ padding: 20px; background-color: #f9f9f9; border-left: 1px solid #ddd; border-right: 1px solid #ddd; }}
                        .button {{ display: inline-block; background-color: #4A6FFF; color: white; padding: 12px 24px; text-decoration: none; border-radius: 4px; font-weight: bold; margin: 20px 0; }}
                        .footer {{ font-size: 12px; color: #777; margin-top: 30px; text-align: center; padding: 15px; background-color: #f1f1f1; border-radius: 0 0 5px 5px; }}
                        .warning {{ color: #e74c3c; font-size: 13px; }}
                    </style>
                </head>
                <body>
                    <div class='container'>
                        <div class='header'>
                            <h1>Password Reset</h1>
                        </div>
                        <div class='content'>
                            <p>Hello {user.UserName},</p>
                            <p>We received a request to reset your password for your CareerPath account. To complete the process, please click the button below:</p>
                            <p style='text-align: center;'><a href='{resetLink}' class='button'>Reset My Password</a></p>
                            <p>If the button doesn't work, you can also copy and paste this link into your browser:</p>
                            <p style='word-break: break-all; background-color: #f1f1f1; padding: 10px; border-radius: 4px;'>{resetLink}</p>
                            <p class='warning'><strong>Important:</strong> This link will expire in 24 hours for security reasons.</p>
                            <p>If you didn't request a password reset, please ignore this email or contact support if you have concerns about your account security.</p>
                            <p>Thank you,<br>The CareerPath Team</p>
                        </div>
                        <div class='footer'>
                            <p>© {DateTime.Now.Year} CareerPath. All rights reserved.</p>
                            <p>This is an automated message, please do not reply.</p>
                        </div>
                    </div>
                </body>
                </html>";

                // Send email using our EmailSender
                await _emailSender.SendEmailAsync(user.Email, subject, content);
                
                _logger.LogInformation("Password reset email sent to: {Email}", email);
                return (true, null);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in ForgotPasswordAsync: {ErrorMessage}", ex.Message);
                return (false, "Failed to process your request. Please try again later.");
            }
        }

        public async Task<(bool success, string? errorMessage)> ResetPasswordAsync(ResetPasswordDto resetPasswordDto)
        {
            try
            {
                if (string.IsNullOrEmpty(resetPasswordDto.Email))
                {
                    return (false, "Email is required");
                }

                if (string.IsNullOrEmpty(resetPasswordDto.Token))
                {
                    return (false, "Reset token is required");
                }

                if (string.IsNullOrEmpty(resetPasswordDto.NewPassword))
                {
                    return (false, "New password is required");
                }

                // Normalize the email for consistent lookup
                var email = resetPasswordDto.Email.NormalizeEmail();

                var user = await _userManager.FindByEmailAsync(email);
                if (user == null)
                {
                    _logger.LogWarning("Password reset attempted for non-existent email: {Email}", email);
                    return (false, "Invalid request");
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
                    _logger.LogWarning("Password reset failed for {Email}: {Errors}", email, errors);
                    return (false, errors);
                }

                // Send a confirmation email that password was changed
                try
                {
                    var subject = "Your CareerPath Password Has Been Reset";
                    var content = $@"
                    <html>
                    <head>
                        <style>
                            body {{ font-family: Arial, sans-serif; line-height: 1.6; color: #333; }}
                            .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
                            .header {{ background-color: #4A6FFF; color: white; padding: 15px; text-align: center; border-radius: 5px 5px 0 0; }}
                            .content {{ padding: 20px; background-color: #f9f9f9; border-left: 1px solid #ddd; border-right: 1px solid #ddd; }}
                            .button {{ display: inline-block; background-color: #4A6FFF; color: white; padding: 12px 24px; text-decoration: none; border-radius: 4px; font-weight: bold; margin: 20px 0; }}
                            .footer {{ font-size: 12px; color: #777; margin-top: 30px; text-align: center; padding: 15px; background-color: #f1f1f1; border-radius: 0 0 5px 5px; }}
                            .alert {{ color: #e74c3c; }}
                        </style>
                    </head>
                    <body>
                        <div class='container'>
                            <div class='header'>
                                <h1>Password Reset Successful</h1>
                            </div>
                            <div class='content'>
                                <p>Hello {user.UserName},</p>
                                <p>Your password for CareerPath has been successfully reset.</p>
                                <p>You can now log in with your new password.</p>
                                <p style='text-align: center;'><a href='https://careerpath.com/login' class='button'>Log In Now</a></p>
                                <p class='alert'><strong>Important:</strong> If you did not request this password change, please contact our support team immediately.</p>
                                <p>Thank you,<br>The CareerPath Team</p>
                            </div>
                            <div class='footer'>
                                <p>© {DateTime.Now.Year} CareerPath. All rights reserved.</p>
                                <p>This is an automated message, please do not reply.</p>
                            </div>
                        </div>
                    </body>
                    </html>";

                    await _emailSender.SendEmailAsync(user.Email, subject, content);
                    _logger.LogInformation("Password reset confirmation email sent to: {Email}", email);
                }
                catch (Exception ex)
                {
                    // Log but don't fail the password reset if confirmation email fails
                    _logger.LogError(ex, "Failed to send password reset confirmation email to {Email}", email);
                }

                return (true, null);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in ResetPasswordAsync: {ErrorMessage}", ex.Message);
                return (false, "Failed to process your request. Please try again later.");
            }
        }
    }
}
