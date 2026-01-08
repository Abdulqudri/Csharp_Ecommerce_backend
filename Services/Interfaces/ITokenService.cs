using Ecommerce.API.Data.Entities;

namespace Ecommerce.API.Services.Interfaces
{
    public interface ITokenService
    {
        string GenerateJwtToken(ApplicationUser user, IList<string> roles);
        string GenerateRefreshToken();
        Task<ApplicationUser?> GetUserFromTokenAsync(string token);
    }
}