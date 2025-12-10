namespace ECommerce.DTOs.Auth
{
    public class AuthResponseDto
    {
        public string Token { get; init; }
        public DateTime ExpiresAt { get; init; }

    }
}
