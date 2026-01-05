using Ecommerce.API.Constants;
using Ecommerce.API.Data.Contexts;
using Ecommerce.API.Data.Entities;
using Ecommerce.API.Dtos.Requests.Auth;
using Ecommerce.API.Dtos.Responses.Auth;
using Ecommerce.API.Services.Interface;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.API.Services.Implementations
{
    public class AuthService : IAuthService

    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ITokenService _tokenService;
        private readonly ApplicationDbContext _context;

        public AuthService(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ITokenService tokenService,
            ApplicationDbContext context
            )
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _tokenService = tokenService;
            _context = context;
        }

        public async Task<AuthResponse> LoginAsync(LoginRequest request)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null || !user.IsActive)
            {
                throw new UnauthorizedAccessException("Invalid credentials.");
            }
            var result = await _signInManager.CheckPasswordSignInAsync(user, request.Password, false);
            if (!result.Succeeded)
            {
                throw new UnauthorizedAccessException("Invalid credentials.");
            }
            var roles = await _userManager.GetRolesAsync(user);
            var token = _tokenService.GenerateJwtToken(user, roles);
            var refreshToken = _tokenService.GenerateRefreshToken();
            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiry = DateTime.UtcNow.AddDays(AppConstants.RefreshTokenExpiryDays);
            await _userManager.UpdateAsync(user);

            return new AuthResponse
            {
                Token = token,
                RefreshToken = refreshToken,
                TokenExpiry = DateTime.UtcNow.AddMinutes(AppConstants.JwtTokenExpiryMinutes),
                RefreshTokenExpiry = user.RefreshTokenExpiry.Value,
                User = new UserResponse
                {
                    Id = user.Id,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email!,
                    Roles = roles.ToList(),
                    CreatedAt = user.CreatedAt,

                }
            };
        }

        public async Task<AuthResponse> RegisterAsync(RegisterRequest request)
        {
            var existingUser = await _userManager.FindByEmailAsync(request.Email);
            if (existingUser != null)
            {
                throw new ArgumentException("Email is already registered.");
            }
            var user = new ApplicationUser
            {
                UserName = request.Email,
                Email = request.Email,
                FirstName = request.FirstName,
                LastName = request.LastName,
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            };
            var result = await _userManager.CreateAsync(user, request.Password);
            if (!result.Succeeded)
            {
                throw new ArgumentException($"Registration failed: {string.Join(", ", result.Errors.Select(e => e.Description))}");
            }
            await _userManager.AddToRoleAsync(user, RoleConstants.User);

            var roles = await _userManager.GetRolesAsync(user);
            var token = _tokenService.GenerateJwtToken(user, roles);
            var refreshToken = _tokenService.GenerateRefreshToken();
            user.RefreshToken = refreshToken;

            user.RefreshTokenExpiry = DateTime.UtcNow.AddDays(AppConstants.RefreshTokenExpiryDays);
            await _userManager.UpdateAsync(user);

            return new AuthResponse
            {
                Token = token,
                TokenExpiry = DateTime.UtcNow.AddMinutes(AppConstants.JwtTokenExpiryMinutes),
                RefreshToken = refreshToken,
                RefreshTokenExpiry = user.RefreshTokenExpiry.Value,
                User = new UserResponse
                {
                    Id = user.Id,
                    Email = user.Email!,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Roles = roles.ToList(),
                    CreatedAt = user.CreatedAt
                }
            };

        }

        public async Task<AuthResponse> RefreshTokenAsync(string refreshToken)
        {
            var user = await _context.Users.SingleOrDefaultAsync(u => u.RefreshToken == refreshToken && u.RefreshTokenExpiry > DateTime.UtcNow);
            if (user == null || !user.IsActive)
            {
                throw new UnauthorizedAccessException("Invalid refresh token.");
            }

            var roles = await _userManager.GetRolesAsync(user);
            var newJwtToken = _tokenService.GenerateJwtToken(user, roles);
            var newRefreshToken = _tokenService.GenerateRefreshToken();
            user.RefreshToken = newRefreshToken;
            user.RefreshTokenExpiry = DateTime.UtcNow.AddDays(AppConstants.RefreshTokenExpiryDays);
            await _userManager.UpdateAsync(user);

            return new AuthResponse
            {
                Token = newJwtToken,
                TokenExpiry = DateTime.UtcNow.AddMinutes(AppConstants.JwtTokenExpiryMinutes),
                RefreshToken = newRefreshToken,
                RefreshTokenExpiry = user.RefreshTokenExpiry.Value,
                User = new UserResponse
                {
                    Id = user.Id,
                    Email = user.Email!,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Roles = roles.ToList(),
                    CreatedAt = user.CreatedAt
                }
            };
        }

        public async Task<bool> ChangePasswordAsync(string UserId, ChangePasswordRequest request)
        {
            var user = await _userManager.FindByIdAsync(UserId);
            if (user == null)
            {
                throw new ArgumentException("User not found.");
            }
            var result = await _userManager.ChangePasswordAsync(user, request.CurrentPassword, request.NewPassword);
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                throw new ArgumentException($"Password change failed: {errors}");
            }

            return true;
        }

        public async Task LogoutAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null || !user.IsActive)
            {
                throw new ArgumentException("User not found.");
            }
            user.RefreshToken = null;
            user.RefreshTokenExpiry = null;
            await _userManager.UpdateAsync(user);
        }
        public async Task<UserResponse?> GetUserProfileAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return null;

            var roles = await _userManager.GetRolesAsync(user);

            return new UserResponse
            {
                Id = user.Id,
                Email = user.Email!,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Roles = roles.ToList(),
                CreatedAt = user.CreatedAt
            };
        }

    }
}