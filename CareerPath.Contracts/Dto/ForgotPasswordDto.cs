using System.ComponentModel.DataAnnotations;

namespace CareerPath.Contracts.Dto
{
    public class ForgotPasswordDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
} 