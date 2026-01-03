namespace Ecommerce.API.Services.Interface
{
    public interface ICurrentUserService
    {
        string? UserId { get; }
        string? Email { get; }
        List<string> Roles { get; }
        bool IsAuthenticated { get; }
        bool IsInRole(string role);
    }
}