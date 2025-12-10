using System.ComponentModel.DataAnnotations;

namespace ECommerce.DTOs.Auth
{
    public class RegisterDto
    {
        [Required]
        public string FullName { get; init; }
        [Required]
        public string UserName { get; init; }
        [Required]
        [EmailAddress]
        public string Email { get; init; }

        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&#])[A-Za-z\d@$!%*?&#]{8,}$",
            ErrorMessage = "Password must be at least 8 characters long, contain at least one uppercase letter, one lowercase letter, one number, and one special character.")]
        [Required]
        public required string Password { get; init; }

        [Required, Compare("Password", ErrorMessage = "Passwords don't match.")]
        public string ConfirmPassword { get; init; }

        [Phone]
        public string? PhoneNumber { get; init; }
    }
}
