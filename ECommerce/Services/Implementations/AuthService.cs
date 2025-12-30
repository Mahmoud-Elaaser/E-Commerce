using ECommerce.DTOs;
using ECommerce.DTOs.Auth;
using ECommerce.Models;
using ECommerce.Services.Interfaces;
using ECommerce.Settings;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ECommerce.Services.Implementations
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmailService _emailService;
        private readonly JwtOptions _options;

        public AuthService(UserManager<ApplicationUser> userManager,
            IEmailService emailService,
            IOptions<JwtOptions> options)
        {
            _userManager = userManager;
            _emailService = emailService;
            _options = options.Value;
        }

        public async Task<ResponseDto> RegisterAsync(RegisterDto registerDto)
        {
            var appUser = new ApplicationUser
            {
                FullName = registerDto.FullName,
                UserName = registerDto.UserName,
                Email = registerDto.Email,
                PhoneNumber = registerDto.PhoneNumber
            };

            var result = await _userManager.CreateAsync(appUser, registerDto.Password);
            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(e => e.Description).ToList();
                return ResponseDto.ValidationFailure(400, "User registration failed.", errors);
            }

            await _userManager.AddToRoleAsync(appUser, "User");
            var token = await GenerateJwtTokenAsync(appUser);

            return ResponseDto.Success(
                201,
                "User registered successfully.",
                new { Token = token.Token, Expiry = token.ExpiresAt, UserId = appUser.Id, Email = appUser.Email }
            );
        }

        public async Task<ResponseDto> LoginAsync(LoginDto loginDto)
        {
            var user = await _userManager.FindByEmailAsync(loginDto.Email);
            if (user == null)
            {
                return ResponseDto.Failure(404, $"This email: {loginDto.Email} isn't exist, Please register first");
            }

            var result = await _userManager.CheckPasswordAsync(user, loginDto.Password);
            if (!result)
            {
                return ResponseDto.Failure(401, "Invalid email or password.");
            }

            var token = await GenerateJwtTokenAsync(user);

            return ResponseDto.Success(
                200,
                "Login successful.",
                new { Token = token.Token, Expiry = token.ExpiresAt, UserId = user.Id, Email = user.Email }
            );
        }

        public async Task<ResponseDto> ChangePasswordAsync(string userId, ChangePasswordDto changePasswordDto)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return ResponseDto.Failure(404, "User not found.");
            }

            var result = await _userManager.ChangePasswordAsync(
                user,
                changePasswordDto.CurrentPassword,
                changePasswordDto.NewPassword
            );

            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(e => e.Description).ToList();
                return ResponseDto.ValidationFailure(400, "Password change failed.", errors);
            }

            return ResponseDto.Success(200, "Password changed successfully.");
        }

        public async Task<ResponseDto> ForgotPasswordAsync(ForgotPasswordDto forgotPasswordDto)
        {
            var user = await _userManager.FindByEmailAsync(forgotPasswordDto.Email);
            if (user == null)
            {
                return ResponseDto.Success(
                    200,
                    "If the email exists, a password reset link has been sent."
                );
            }

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);

            await _emailService.SendEmailAsync(
                user.Email!,
                "Password Reset",
                $"Use this token to reset your password: {token}"
            );

            return ResponseDto.Success(
                200,
                "If the email exists, a password reset link has been sent."
            );
        }

        public async Task<ResponseDto> ResetPasswordAsync(ResetPasswordDto resetPasswordDto)
        {
            var user = await _userManager.FindByEmailAsync(resetPasswordDto.Email);
            if (user == null)
                return ResponseDto.Failure(400, "Invalid reset request.");

            var result = await _userManager.ResetPasswordAsync(user, resetPasswordDto.Token, resetPasswordDto.NewPassword);

            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(e => e.Description).ToList();
                return ResponseDto.ValidationFailure(400, "Password reset failed.", errors);
            }

            return ResponseDto.Success(200, "Password reset successfully.");
        }

        public async Task<ResponseDto> AssignRoleAsync(AssignRoleDto assignRoleDto)
        {
            var user = await _userManager.FindByIdAsync(assignRoleDto.UserId);
            if (user == null)
                return ResponseDto.Failure(404, "User not found.");

            var userRoles = await _userManager.GetRolesAsync(user);
            if (userRoles.Contains(assignRoleDto.RoleName))
                return ResponseDto.Failure(400, $"User already has the '{assignRoleDto.RoleName}' role.");

            var result = await _userManager.AddToRoleAsync(user, assignRoleDto.RoleName);

            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(e => e.Description).ToList();
                return ResponseDto.ValidationFailure(400, "Failed to assign role.", errors);
            }

            return ResponseDto.Success(
                200,
                $"Role '{assignRoleDto.RoleName}' assigned successfully to user '{user.Email}'."
            );
        }

        public async Task<ResponseDto> RemoveRoleAsync(RemoveRoleDto removeRoleDto)
        {
            var user = await _userManager.FindByIdAsync(removeRoleDto.UserId);
            if (user == null)
                return ResponseDto.Failure(404, "User not found.");

            var userRoles = await _userManager.GetRolesAsync(user);
            if (!userRoles.Contains(removeRoleDto.RoleName))
                return ResponseDto.Failure(400, $"User does not have the '{removeRoleDto.RoleName}' role.");

            var result = await _userManager.RemoveFromRoleAsync(user, removeRoleDto.RoleName);

            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(e => e.Description).ToList();
                return ResponseDto.ValidationFailure(400, "Failed to remove role.", errors);
            }

            return ResponseDto.Success(
                200,
                $"Role '{removeRoleDto.RoleName}' removed successfully from user '{user.Email}'."
            );
        }

        public async Task<AuthResponseDto> GenerateJwtTokenAsync(ApplicationUser user)
        {
            var roles = await _userManager.GetRolesAsync(user);
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.FullName),
                new Claim(ClaimTypes.Email, user.Email!),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
            };

            claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.SecretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _options.Issuer,
                audience: _options.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddDays(_options.DurationInDays),
                signingCredentials: creds
            );

            return new AuthResponseDto
            {
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                ExpiresAt = token.ValidTo
            };
        }
    }
}