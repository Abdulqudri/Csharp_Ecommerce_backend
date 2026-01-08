using Ecommerce.API.Data.Entities;
using System.Linq.Expressions;

namespace Ecommerce.API.Repository.Interfaces
{
    public interface IProductRepository : IGenericRepository<Product>
    {
        Task<IEnumerable<Product>> GetProductsWithCategoryAsync();
        Task<Product?> GetProductWithDetailsAsync(int id);
        Task<IEnumerable<Product>> GetProductsByCategoryAsync(int categoryId);
        Task<IEnumerable<Product>> GetFeaturedProductsAsync(int count);
        Task<IEnumerable<Product>> GetRelatedProductsAsync(int productId, int categoryId, int count);
        Task<IEnumerable<Product>> SearchProductsAsync(string searchTerm);
        Task<bool> UpdateStockAsync(int productId, int quantity);
        Task<IEnumerable<Product>> GetProductsByFilterAsync(
            Expression<Func<Product, bool>>? filter = null,
            Func<IQueryable<Product>, IOrderedQueryable<Product>>? orderBy = null,
            int? skip = null,
            int? take = null);
    }
}