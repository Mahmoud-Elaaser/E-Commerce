using ECommerce.DTOs;
using ECommerce.DTOs.Auth;
using ECommerce.Models;

namespace ECommerce.Services.Interfaces
{
    public interface IAuthService
    {
        Task<ResponseDto> RegisterAsync(RegisterDto registerDto);
        Task<ResponseDto> LoginAsync(LoginDto loginDto);



        Task<AuthResponseDto> GenerateJwtTokenAsync(ApplicationUser user);
    }
}
