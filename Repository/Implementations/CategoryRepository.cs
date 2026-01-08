using Ecommerce.API.Data.Contexts;
using Ecommerce.API.Data.Entities;
using Ecommerce.API.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.API.Repository.Implementations
{
    public class CategoryRepository : GenericRepository<Category>, ICategoryRepository
    {
        public CategoryRepository(ApplicationDbContext context) : base(context) { }

        public async Task<IEnumerable<Category>> GetCategoriesWithProductsAsync()
        {
            return await _context.Categories
                .Include(c => c.Products.Where(p => p.IsActive))
                .Where(c => c.IsActive)
                .ToListAsync();
        }

        public async Task<Category?> GetCategoryWithProductsAsync(int id)
        {
            return await _context.Categories
                .Include(c => c.Products.Where(p => p.IsActive))
                .FirstOrDefaultAsync(c => c.Id == id && c.IsActive);
        }

        public async Task<IEnumerable<Category>> GetSubCategoriesAsync(int parentCategoryId)
        {
            return await _context.Categories
                .Include(c => c.SubCategories)
                .Where(c => c.ParentCategoryId == parentCategoryId && c.IsActive)
                .ToListAsync();
        }
    }
}