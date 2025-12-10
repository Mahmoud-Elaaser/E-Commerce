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
        private readonly JwtOptions _options;

        public AuthService(UserManager<ApplicationUser> userManager,
            IOptions<JwtOptions> options)
        {
            _userManager = userManager;
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
                var errors = result.Errors.Select(e => e.Description);
                return new ResponseDto
                {
                    IsSucceeded = false,
                    Message = "User registration failed."

                };
            }

            await _userManager.AddToRoleAsync(appUser, "User");
            var token = await GenerateJwtTokenAsync(appUser);
            return new ResponseDto
            {
                IsSucceeded = true,
                Status = 201,
                Message = "User registered successfully.",
                Model = new { Token = token, UserId = appUser.Id, Email = appUser.Email }

            };

        }
        public async Task<ResponseDto> LoginAsync(LoginDto loginDto)
        {
            var user = await _userManager.FindByEmailAsync(loginDto.Email);
            if (user == null)
            {
                return new ResponseDto
                {
                    IsSucceeded = false,
                    Message = $"This email: {loginDto.Email} isn't exist, Please register first",
                    Status = 404
                };
            }

            var result = await _userManager.CheckPasswordAsync(user, loginDto.Password);
            if (!result)
            {
                return new ResponseDto
                {
                    IsSucceeded = false,
                    Message = "Invalid email or password.",
                    Status = 401
                };
            }
            var token = await GenerateJwtTokenAsync(user);

            return new ResponseDto
            {
                IsSucceeded = true,
                Status = 200,
                Message = "Login successful.",
                Model = new { Token = token.Token, Expiry = token.ExpiresAt, UserId = user.Id, Email = user.Email }
            };
        }





        public async Task<AuthResponseDto> GenerateJwtTokenAsync(ApplicationUser user)
        {
            var roles = await _userManager.GetRolesAsync(user);
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.FullName),
                new Claim(ClaimTypes.Email, user.Email!)
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
