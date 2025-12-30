using ECommerce.DTOs;
using ECommerce.DTOs.Auth;
using ECommerce.Models;

namespace ECommerce.Services.Interfaces
{
    public interface IAuthService
    {
        Task<ResponseDto> RegisterAsync(RegisterDto registerDto);
        Task<ResponseDto> LoginAsync(LoginDto loginDto);
        Task<ResponseDto> ChangePasswordAsync(string userId, ChangePasswordDto changePasswordDto);
        Task<ResponseDto> ForgotPasswordAsync(ForgotPasswordDto forgotPasswordDto);
        Task<ResponseDto> ResetPasswordAsync(ResetPasswordDto resetPasswordDto);
        Task<AuthResponseDto> GenerateJwtTokenAsync(ApplicationUser user);
    }
}