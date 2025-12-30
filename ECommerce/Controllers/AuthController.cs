using ECommerce.DTOs.Auth;
using ECommerce.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ECommerce.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _authService.RegisterAsync(registerDto);

            if (!result.IsSucceeded)
                return BadRequest(result);

            return StatusCode(result.Status, result);
        }

        [HttpPost("confirm-email")]
        public async Task<IActionResult> ConfirmEmail([FromBody] ConfirmEmailDto confirmEmailDto)
        {
            var result = await _authService.ConfirmEmailAsync(confirmEmailDto);

            if (!result.IsSucceeded)
                return BadRequest(result);

            return StatusCode(result.Status, result.Message);
        }

        [HttpPost("resend-confirmation-email")]
        public async Task<IActionResult> ResendConfirmationEmail([FromBody] ResendConfirmationEmailDto resendDto)
        {
            var result = await _authService.ResendConfirmationEmailAsync(resendDto);

            if (!result.IsSucceeded)
                return BadRequest(result);

            return StatusCode(result.Status, result.Message);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _authService.LoginAsync(loginDto);

            if (!result.IsSucceeded)
                return StatusCode(result.Status, result);

            return StatusCode(result.Status, result);
        }

        [Authorize]
        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto changePasswordDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized(new { Message = "User not authenticated" });

            var result = await _authService.ChangePasswordAsync(userId, changePasswordDto);

            if (!result.IsSucceeded)
                return StatusCode(result.Status, result);

            return StatusCode(result.Status, result.Message);
        }

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto forgotPasswordDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _authService.ForgotPasswordAsync(forgotPasswordDto);

            return StatusCode(result.Status, result.Message);
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto resetPasswordDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _authService.ResetPasswordAsync(resetPasswordDto);

            if (!result.IsSucceeded)
                return BadRequest(result);

            return StatusCode(result.Status, result.Message);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("assign-role")]
        public async Task<IActionResult> AssignRole([FromBody] AssignRoleDto assignRoleDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _authService.AssignRoleAsync(assignRoleDto);

            if (!result.IsSucceeded)
                return BadRequest(result);

            return StatusCode(result.Status, result.Message);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("remove-role")]
        public async Task<IActionResult> RemoveRole([FromBody] RemoveRoleDto removeRoleDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _authService.RemoveRoleAsync(removeRoleDto);

            if (!result.IsSucceeded)
                return BadRequest(result);

            return StatusCode(result.Status, result.Message);
        }
    }
}