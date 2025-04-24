using System.ComponentModel.DataAnnotations;

namespace CareerPath.Contracts.Dto
{
    public class ResetPasswordDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Token { get; set; }

        [Required]
        [MinLength(8, ErrorMessage = "Password must be at least 8 characters")]
        public string NewPassword { get; set; }

        [Required]
        [Compare("NewPassword", ErrorMessage = "Passwords must match")]
        public string ConfirmPassword { get; set; }
    }
} 