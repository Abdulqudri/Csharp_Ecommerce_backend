using AutoMapper;
using Ecommerce.API.Constants;
using Ecommerce.API.Data.Entities;
using Ecommerce.API.Dtos.Requests.Products;
using Ecommerce.API.Dtos.Responses.Products;
using Ecommerce.API.Helpers;
using Ecommerce.API.Repository.Interfaces;
using Ecommerce.API.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.API.Services.Implementations
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly IGenericRepository<Category> _categoryRepository;
        private readonly IGenericRepository<Review> _reviewRepository;
        private readonly IMapper _mapper;
        private readonly ICurrentUserService _currentUserService;

        public ProductService(
            IProductRepository productRepository,
            IGenericRepository<Category> categoryRepository,
            IGenericRepository<Review> reviewRepository,
            IMapper mapper,
            ICurrentUserService currentUserService)
        {
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
            _reviewRepository = reviewRepository;
            _mapper = mapper;
            _currentUserService = currentUserService;
        }

        public async Task<PaginatedResponse<ProductListResponse>> GetAllProductsAsync(
            int page = 1, 
            int pageSize = 10, 
            string? category = null, 
            decimal? minPrice = null, 
            decimal? maxPrice = null, 
            string? search = null,
            string? sortBy = null,
            bool sortDesc = false)
        {
            page = Math.Max(1, page);
            pageSize = Math.Clamp(pageSize, 1, AppConstants.MaxPageSize);

            var query = _productRepository.GetProductsByFilterAsync(
                filter: p => p.IsActive &&
                    (string.IsNullOrEmpty(category) || p.Category.Name == category) &&
                    (!minPrice.HasValue || p.Price >= minPrice.Value) &&
                    (!maxPrice.HasValue || p.Price <= maxPrice.Value) &&
                    (string.IsNullOrEmpty(search) || 
                     p.Name.Contains(search) || 
                     p.Description.Contains(search) ||
                     p.Category.Name.Contains(search))
            );

            var products = await query;
            var totalCount = products.Count();

            // Apply sorting
            if (!string.IsNullOrEmpty(sortBy))
            {
                products = sortBy.ToLower() switch
                {
                    "price" => sortDesc ? products.OrderByDescending(p => p.Price).ToList() 
                                        : products.OrderBy(p => p.Price).ToList(),
                    "name" => sortDesc ? products.OrderByDescending(p => p.Name).ToList() 
                                       : products.OrderBy(p => p.Name).ToList(),
                    "date" => sortDesc ? products.OrderByDescending(p => p.CreatedAt).ToList() 
                                       : products.OrderBy(p => p.CreatedAt).ToList(),
                    "rating" => sortDesc ? products.OrderByDescending(p => 
                                        p.Reviews.Any() ? p.Reviews.Average(r => r.Rating) : 0).ToList()
                                        : products.OrderBy(p => 
                                        p.Reviews.Any() ? p.Reviews.Average(r => r.Rating) : 0).ToList(),
                    _ => products
                };
            }

            // Apply pagination
            var pagedProducts = products
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var productResponses = _mapper.Map<List<ProductListResponse>>(pagedProducts);

            return new PaginatedResponse<ProductListResponse>
            {
                Page = page,
                PageSize = pageSize,
                TotalCount = totalCount,
                Items = productResponses
            };
        }

        public async Task<ProductResponse?> GetProductByIdAsync(int id)
        {
            var product = await _productRepository.GetProductWithDetailsAsync(id);
            if (product == null)
                return null;

            var response = _mapper.Map<ProductResponse>(product);
            response.AverageRating = product.Reviews.Any() 
                ? product.Reviews.Average(r => r.Rating) 
                : 0;
            response.TotalReviews = product.Reviews.Count;

            return response;
        }

        public async Task<ProductResponse> CreateProductAsync(CreateProductRequest request)
        {
            // Validate category exists
            var category = await _categoryRepository.GetByIdAsync(request.CategoryId);
            if (category == null)
                throw new ArgumentException("Category not found");

            var product = new Product
            {
                Name = request.Name,
                Description = request.Description,
                Price = request.Price,
                DiscountPrice = request.DiscountPrice,
                StockQuantity = request.StockQuantity,
                SKU = request.SKU,
                ImageUrl = request.ImageUrl,
                CategoryId = request.CategoryId,
                IsActive = request.IsActive,
                CreatedAt = DateTime.UtcNow
            };

            await _productRepository.AddAsync(product);
            
            var createdProduct = await _productRepository.GetProductWithDetailsAsync(product.Id);
            return _mapper.Map<ProductResponse>(createdProduct!);
        }

        public async Task<ProductResponse?> UpdateProductAsync(int id, UpdateProductRequest request)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null)
                return null;

            // Update fields if provided
            if (!string.IsNullOrEmpty(request.Name))
                product.Name = request.Name;

            if (!string.IsNullOrEmpty(request.Description))
                product.Description = request.Description;

            if (request.Price.HasValue)
                product.Price = request.Price.Value;

            if (request.DiscountPrice.HasValue)
                product.DiscountPrice = request.DiscountPrice.Value;

            if (request.StockQuantity.HasValue)
                product.StockQuantity = request.StockQuantity.Value;

            if (!string.IsNullOrEmpty(request.SKU))
                product.SKU = request.SKU;

            if (!string.IsNullOrEmpty(request.ImageUrl))
                product.ImageUrl = request.ImageUrl;

            if (request.CategoryId.HasValue)
            {
                var category = await _categoryRepository.GetByIdAsync(request.CategoryId.Value);
                if (category == null)
                    throw new ArgumentException("Category not found");
                
                product.CategoryId = request.CategoryId.Value;
            }

            if (request.IsActive.HasValue)
                product.IsActive = request.IsActive.Value;

            product.UpdateAt = DateTime.UtcNow;

            await _productRepository.UpdateAsync(product);
            
            var updatedProduct = await _productRepository.GetProductWithDetailsAsync(product.Id);
            return _mapper.Map<ProductResponse>(updatedProduct!);
        }

        public async Task<bool> DeleteProductAsync(int id)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null)
                return false;

            // Soft delete by setting IsActive to false
            product.IsActive = false;
            product.UpdateAt = DateTime.UtcNow;

            await _productRepository.UpdateAsync(product);
            return true;
        }

        public async Task<bool> UpdateProductStockAsync(int id, int quantity)
        {
            return await _productRepository.UpdateStockAsync(id, quantity);
        }

        public async Task<List<ReviewResponse>> GetProductReviewsAsync(int productId)
        {
            var product = await _productRepository.GetProductWithDetailsAsync(productId);
            if (product == null)
                return new List<ReviewResponse>();

            return _mapper.Map<List<ReviewResponse>>(product.Reviews);
        }

        public async Task<ReviewResponse> AddReviewAsync(int productId, string userId, CreateReviewRequest request)
        {
            var product = await _productRepository.GetByIdAsync(productId);
            if (product == null)
                throw new ArgumentException("Product not found");

            // Check if user has already reviewed this product
            var existingReview = (await _reviewRepository.FindAsync(r => 
                r.ProductId == productId && r.UserId == userId)).FirstOrDefault();

            if (existingReview != null)
                throw new ArgumentException("You have already reviewed this product");

            var review = new Review
            {
                Rating = request.Rating,
                Comment = request.Comment,
                ProductId = productId,
                UserId = userId,
                CreatedAt = DateTime.UtcNow
            };

            await _reviewRepository.AddAsync(review);
            
            // Get the review with user details
            var createdReview = await _reviewRepository.FindAsync(r => r.Id == review.Id);
            return _mapper.Map<ReviewResponse>(review);
        }

        public async Task<bool> DeleteReviewAsync(int reviewId, string userId)
        {
            var review = await _reviewRepository.GetByIdAsync(reviewId);
            if (review == null)
                return false;

            // Check if the user is the owner of the review or an admin
            if (review.UserId != userId && !_currentUserService.IsInRole(RoleConstants.Admin))
                return false;

            await _reviewRepository.DeleteAsync(review);
            return true;
        }

        public async Task<List<ProductListResponse>> GetFeaturedProductsAsync(int count = 10)
        {
            var products = await _productRepository.GetFeaturedProductsAsync(count);
            return _mapper.Map<List<ProductListResponse>>(products);
        }

        public async Task<List<ProductListResponse>> GetRelatedProductsAsync(int productId, int count = 5)
        {
            var product = await _productRepository.GetByIdAsync(productId);
            if (product == null)
                return new List<ProductListResponse>();

            var relatedProducts = await _productRepository.GetRelatedProductsAsync(
                productId, product.CategoryId, count);
            
            return _mapper.Map<List<ProductListResponse>>(relatedProducts);
        }
    }
}