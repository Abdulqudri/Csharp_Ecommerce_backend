using System.ComponentModel.DataAnnotations.Schema;

using Ecommerce.API.Constants;

namespace Ecommerce.API.Data.Entities;

public class Order
{
    public int Id { get; set; }
    public string OrderNumber { get; set; } = GenerateOrderNumber();
    public DateTime OrderDate { get; set; } = DateTime.UtcNow;


    [Column(TypeName = "decimal(18,2)")]
    public decimal TotalAmount { get; set; }

    public string Status { get; set; } = OrderStatus.Pending;
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdateAt { get; set; }
    public string UserId { get; set; } = string.Empty;
    public int? ShippingAddressId { get; set; }
    public int? BillingAddressId { get; set; }
    public virtual ApplicationUser User { get; set; } = null!;
    public virtual Address? ShippingAddress { get; set; }
    public virtual Address? BillingAddress { get; set; }
    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

    private static string GenerateOrderNumber()
    {
        return $"ORD-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString().Substring(0, 8).ToUpper()}";
    }


}
