using Ecommerce.API.Data.Entities;

namespace Ecommerce.API.Services.Interface
{
    public interface ITokenService
    {
        string GenerateJwtToken(ApplicationUser user, IList<string> roles);
        string GenerateRefreshToken();
        Task<ApplicationUser?> GetUserFromToken(string token);
    }
}