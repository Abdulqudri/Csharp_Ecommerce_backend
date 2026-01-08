using Ecommerce.API.Dtos.Requests.Auth;
using Ecommerce.API.Dtos.Responses.Auth;

namespace Ecommerce.API.Services.Interfaces
{
    public interface IAuthService
    {
        Task<AuthResponse> LoginAsync(LoginRequest request);
        Task<AuthResponse> RegisterAsync(RegisterRequest request);
        Task<AuthResponse> RefreshTokenAsync(string refreshToken);
        Task<bool> ChangePasswordAsync(string UserId, ChangePasswordRequest request);
        Task LogoutAsync(string UserId);
        Task<UserResponse?> GetUserProfileAsync(string userId);
    }
}