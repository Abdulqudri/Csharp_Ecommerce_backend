using Ecommerce.API.Data.Entities;

namespace Ecommerce.API.Repository.Interfaces
{
    public interface IReviewRepository : IGenericRepository<Review>
    {
        Task<IEnumerable<Review>> GetReviewsByProductIdAsync(int productId);
        Task<IEnumerable<Review>> GetReviewsByUserIdAsync(string userId);
        Task<Review?> GetReviewWithDetailsAsync(int id);
        Task<double> GetAverageRatingAsync(int productId);
        Task<int> GetReviewCountAsync(int productId);
        Task<bool> HasUserReviewedProductAsync(string userId, int productId);
    }
}