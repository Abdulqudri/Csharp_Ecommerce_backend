using Ecommerce.API.Services.Implementations;
using Ecommerce.API.Services.Interface;

namespace Ecommerce.API.Extensions
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            // Register services
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<ITokenService, TokenService>();
            // services.AddScoped<IProductService, ProductService>();
            // services.AddScoped<IOrderService, OrderService>();
            // services.AddScoped<ICategoryService, CategoryService>();
            services.AddScoped<ICurrentUserService, CurrentUserService>();

            // Register repositories
            // services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            // services.AddScoped<IProductRepository, ProductRepository>();
            // services.AddScoped<IOrderRepository, OrderRepository>();

            return services;
        }
    }
}