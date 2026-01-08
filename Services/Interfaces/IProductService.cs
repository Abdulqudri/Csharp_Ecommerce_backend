using Ecommerce.API.Dtos.Requests.Products;
using Ecommerce.API.Dtos.Responses.Products;
using Ecommerce.API.Helpers;

namespace Ecommerce.API.Services.Interfaces
{
    public interface IProductService
    {
        Task<PaginatedResponse<ProductListResponse>> GetAllProductsAsync(
            int page = 1, 
            int pageSize = 10, 
            string? category = null, 
            decimal? minPrice = null, 
            decimal? maxPrice = null, 
            string? search = null,
            string? sortBy = null,
            bool sortDesc = false);

        Task<ProductResponse?> GetProductByIdAsync(int id);
        Task<ProductResponse> CreateProductAsync(CreateProductRequest request);
        Task<ProductResponse?> UpdateProductAsync(int id, UpdateProductRequest request);
        Task<bool> DeleteProductAsync(int id);
        Task<bool> UpdateProductStockAsync(int id, int quantity);
        Task<List<ReviewResponse>> GetProductReviewsAsync(int productId);
        Task<ReviewResponse> AddReviewAsync(int productId, string userId, CreateReviewRequest request);
        Task<bool> DeleteReviewAsync(int reviewId, string userId);
        Task<List<ProductListResponse>> GetFeaturedProductsAsync(int count = 10);
        Task<List<ProductListResponse>> GetRelatedProductsAsync(int productId, int count = 5);
    }
}