using Ecommerce.API.Data.Contexts;
using Ecommerce.API.Data.Entities;
using Ecommerce.API.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.API.Repository.Implementations
{
    public class ReviewRepository : GenericRepository<Review>, IReviewRepository
    {
        public ReviewRepository(ApplicationDbContext context) : base(context) { }

        public async Task<IEnumerable<Review>> GetReviewsByProductIdAsync(int productId)
        {
            return await _context.Reviews
                .Include(r => r.User)
                .Where(r => r.ProductId == productId)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Review>> GetReviewsByUserIdAsync(string userId)
        {
            return await _context.Reviews
                .Include(r => r.Product)
                .Where(r => r.UserId == userId)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();
        }

        public async Task<Review?> GetReviewWithDetailsAsync(int id)
        {
            return await _context.Reviews
                .Include(r => r.User)
                .Include(r => r.Product)
                .FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task<double> GetAverageRatingAsync(int productId)
        {
            var average = await _context.Reviews
                .Where(r => r.ProductId == productId)
                .AverageAsync(r => (double?)r.Rating) ?? 0.0;
            
            return Math.Round(average, 1);
        }

        public async Task<int> GetReviewCountAsync(int productId)
        {
            return await _context.Reviews
                .CountAsync(r => r.ProductId == productId);
        }

        public async Task<bool> HasUserReviewedProductAsync(string userId, int productId)
        {
            return await _context.Reviews
                .AnyAsync(r => r.UserId == userId && r.ProductId == productId);
        }
    }
}