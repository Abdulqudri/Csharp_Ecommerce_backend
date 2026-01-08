using System.ComponentModel.DataAnnotations;

namespace Ecommerce.API.Dtos.Requests.Orders
{
    public class UpdateOrderStatusRequest
    {
        [Required]
        public string Status { get; set; } = string.Empty;
    }
}