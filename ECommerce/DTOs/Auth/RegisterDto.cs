namespace ECommerce.DTOs.Auth
{
    public record RegisterDto(
        string FullName,
        string UserName,
        string Email,
        string Password,
        string ConfirmPassword,
        string? PhoneNumber
    );

}
