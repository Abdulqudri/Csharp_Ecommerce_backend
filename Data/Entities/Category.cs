using System.Diagnostics.Contracts;

namespace Ecommerce.API.Data.Entities;

public class Category
{
    public int Id {get; set;}
    public string Name {get; set;} = string.Empty;
    public string Description {get; set;} = string.Empty;
    public string? ImageUrl {get; set;}
    public bool IsActive {get; set;} = true;
    public DateTime CreatedAt {get; set;} = DateTime.UtcNow;

    public int? ParentCategoryId {get;set;} 
    public virtual Category? ParentCategory {get; set;} 
    public virtual ICollection<Category> SubCategories {get; set;} = new List<Category>();
    public virtual ICollection<Product> Products {get; set;} = new List<Product>();

}