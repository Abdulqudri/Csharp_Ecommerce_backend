using System.ComponentModel.DataAnnotations;

namespace Ecommerce.API.Dtos.Requests.Orders
{
    public class CreateOrderRequest
    {
        [Required]
        public List<OrderItemRequest> Items { get; set; } = new List<OrderItemRequest>();

        public string? Notes { get; set; }

        public int? ShippingAddressId { get; set; }
        public int? BillingAddressId { get; set; }
    }

    public class OrderItemRequest
    {
        [Required]
        public int ProductId { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int Quantity { get; set; }
    }
}