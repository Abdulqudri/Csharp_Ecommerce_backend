using System.ComponentModel.DataAnnotations;

namespace Ecommerce.API.Dtos.Requests.Categories
{
    public class CreateCategoryRequest
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(500)]
        public string Description { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? ImageUrl { get; set; }

        public int? ParentCategoryId { get; set; }
    }
}