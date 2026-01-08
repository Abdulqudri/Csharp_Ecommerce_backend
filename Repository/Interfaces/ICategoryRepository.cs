using Ecommerce.API.Data.Entities;

namespace Ecommerce.API.Repository.Interfaces
{
    public interface ICategoryRepository : IGenericRepository<Category>
    {
        Task<IEnumerable<Category>> GetCategoriesWithProductsAsync();
        Task<Category?> GetCategoryWithProductsAsync(int id);
        Task<IEnumerable<Category>> GetSubCategoriesAsync(int parentCategoryId);
    }
}