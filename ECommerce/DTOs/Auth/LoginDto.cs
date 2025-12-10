using System.ComponentModel.DataAnnotations;

namespace ECommerce.DTOs.Auth
{
    public class LoginDto
    {
        [EmailAddress]
        public string Email { get; init; }

        public string Password { get; init; }
    }
}
