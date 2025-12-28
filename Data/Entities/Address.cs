
namespace Ecommerce.API.Data.Entities;
public class Address
{
    public int Id { get; set; }
    public string Street { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public string ZipCode { get; set; } = string.Empty;
    public bool IsDefault { get; set; }
    public string AddressType { get; set; } = "Shipping"; // Shipping or Billing

    // Foreign key
    public string UserId { get; set; } = string.Empty;

    // Navigation property
    public virtual ApplicationUser User { get; set; } = null!;
}