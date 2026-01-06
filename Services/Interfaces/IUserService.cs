using Ecommerce.API.Dtos.Requests.Users;
using Ecommerce.API.Dtos.Responses.Auth;

namespace Ecommerce.API.Services.Interfaces
{
    public interface IUserService
    {
        Task<UserResponse?> GetUserByIdAsync(string userId);
        Task<UserResponse?> GetUserByEmailAsync(string email);
        Task<List<UserResponse>> GetAllUsersAsync();
        Task<bool> UpdateUserAsync(string userId, UpdateUserRequest request);
        Task<bool> DeleteUserAsync(string userId);
    }

}