using System.ComponentModel.DataAnnotations;

namespace Ecommerce.API.Dtos.Requests.Products
{
    public class CreateReviewRequest
    {
        [Required]
        [Range(1, 5)]
        public int Rating { get; set; }

        [MaxLength(1000)]
        public string Comment { get; set; } = string.Empty;
    }
}