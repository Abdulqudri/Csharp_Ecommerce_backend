using System.Data.SqlTypes;
using Ecommerce.API.Dtos.Requests.Auth;
using Ecommerce.API.Dtos.Responses.Auth;

namespace Ecommerce.API.Services.Interface
{
    public interface IAuthService
    {
        Task<AuthResponse> LoginAsync(LoginRequest request);
        Task<AuthResponse> RegisterAsync(RegisterRequest request);
        Task<AuthResponse> RefreshTokenAsync(string refreshToken);
        Task<bool> ChangePasswordAsync(string UserId, string currentPassword, string newPassword);
        Task LogoutAsync(string UserId);
    }
}