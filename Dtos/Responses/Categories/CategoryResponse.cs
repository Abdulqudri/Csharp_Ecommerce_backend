using Ecommerce.API.Dtos.Responses.Products;

namespace Ecommerce.API.Dtos.Responses.Categories
{
    public class CategoryResponse
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string? ImageUrl { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public int? ParentCategoryId { get; set; }
        public string? ParentCategoryName { get; set; }
        public List<CategoryResponse> SubCategories { get; set; } = new List<CategoryResponse>();
        public List<ProductResponse> Products { get; set; } = new List<ProductResponse>();
    }

    
}