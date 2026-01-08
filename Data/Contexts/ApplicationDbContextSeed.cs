using Ecommerce.API.Constants;
using Ecommerce.API.Data.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.API.Data.Contexts
{
    public static class ApplicationDbContextSeed
    {
        public static async Task SeedAsync(UserManager<ApplicationUser> userManager, 
            RoleManager<IdentityRole> roleManager, ApplicationDbContext context)
        {
            // Seed Roles
            if (!await roleManager.RoleExistsAsync(RoleConstants.Admin))
            {
                await roleManager.CreateAsync(new IdentityRole(RoleConstants.Admin));
            }

            if (!await roleManager.RoleExistsAsync(RoleConstants.User))
            {
                await roleManager.CreateAsync(new IdentityRole(RoleConstants.User));
            }

            // Seed Admin User
            var adminEmail = "admin@example.com";
            if (await userManager.FindByEmailAsync(adminEmail) == null)
            {
                var adminUser = new ApplicationUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    FirstName = "Admin",
                    LastName = "User",
                    EmailConfirmed = true,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                };

                var result = await userManager.CreateAsync(adminUser, "Admin@123");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, RoleConstants.Admin);
                }
            }

            // Seed Categories if none exist
            if (!await context.Categories.AnyAsync())
            {
                var categories = new List<Category>
                {
                    new Category { Name = "Electronics", Description = "Electronic devices", CreatedAt = DateTime.UtcNow, IsActive = true },
                    new Category { Name = "Clothing", Description = "Men and women clothing", CreatedAt = DateTime.UtcNow, IsActive = true },
                    new Category { Name = "Books", Description = "All kinds of books", CreatedAt = DateTime.UtcNow, IsActive = true }
                };

                await context.Categories.AddRangeAsync(categories);
                await context.SaveChangesAsync();
            }

            // Seed Products if none exist
            if (!await context.Products.AnyAsync())
            {
                var electronicsCategory = await context.Categories.FirstOrDefaultAsync(c => c.Name == "Electronics");
                var clothingCategory = await context.Categories.FirstOrDefaultAsync(c => c.Name == "Clothing");

                var products = new List<Product>
                {
                    new Product
                    {
                        Name = "Wireless Headphones",
                        Description = "Noise cancelling wireless headphones",
                        Price = 99.99m,
                        StockQuantity = 50,
                        SKU = "WH-001",
                        CategoryId = electronicsCategory?.Id ?? 1,
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow,
                        ImageUrl = "https://example.com/images/headphones.jpg"
                    },
                    new Product
                    {
                        Name = "T-Shirt",
                        Description = "Cotton T-Shirt",
                        Price = 19.99m,
                        StockQuantity = 100,
                        SKU = "TS-001",
                        CategoryId = clothingCategory?.Id ?? 2,
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow,
                        ImageUrl = "https://example.com/images/tshirt.jpg"
                    }
                };

                await context.Products.AddRangeAsync(products);
                await context.SaveChangesAsync();
            }
        }
    }
}