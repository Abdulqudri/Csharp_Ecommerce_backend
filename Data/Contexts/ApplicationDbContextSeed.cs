using Ecommerce.API.Constants;
using Ecommerce.API.Data.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.API.Data.Contexts;
    public static class ApplicationDbContextSeed
    {
        public static void Seed(this ModelBuilder builder)
        {
            // Seed Roles
            var adminRole = new IdentityRole
            {
                Id = "admin-role-id",
                Name = RoleConstants.Admin,
                NormalizedName = RoleConstants.Admin.ToUpper()
            };

            var userRole = new IdentityRole
            {
                Id = "user-role-id",
                Name = RoleConstants.User,
                NormalizedName = RoleConstants.User.ToUpper()
            };

            builder.Entity<IdentityRole>().HasData(adminRole, userRole);

            // Seed Admin User
            var hasher = new PasswordHasher<ApplicationUser>();
            var adminUser = new ApplicationUser
            {
                Id = "admin-user-id",
                UserName = "admin@example.com",
                NormalizedUserName = "ADMIN@EXAMPLE.COM",
                Email = "admin@example.com",
                NormalizedEmail = "ADMIN@EXAMPLE.COM",
                EmailConfirmed = true,
                FirstName = "Admin",
                LastName = "User",
                SecurityStamp = Guid.NewGuid().ToString(),
                PasswordHash = hasher.HashPassword(null!, "Admin@123")
            };

            builder.Entity<ApplicationUser>().HasData(adminUser);

            // Assign Admin Role to Admin User
            builder.Entity<IdentityUserRole<string>>().HasData(
                new IdentityUserRole<string>
                {
                    RoleId = adminRole.Id,
                    UserId = adminUser.Id
                }
            );

            // Seed Categories
            var categories = new List<Category>
            {
                new Category { Id = 1, Name = "Electronics", Description = "Electronic devices", CreatedAt = DateTime.UtcNow, IsActive = true },
                new Category { Id = 2, Name = "Clothing", Description = "Men and women clothing", CreatedAt = DateTime.UtcNow, IsActive = true },
                new Category { Id = 3, Name = "Books", Description = "All kinds of books", CreatedAt = DateTime.UtcNow, IsActive = true }
            };

            builder.Entity<Category>().HasData(categories);

            // Seed Products
            var products = new List<Product>
            {
                new Product
                {
                    Id = 1,
                    Name = "Wireless Headphones",
                    Description = "Noise cancelling wireless headphones",
                    Price = 99.99m,
                    StockQuantity = 50,
                    SKU = "WH-001",
                    CategoryId = 1,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    ImageUrl = "https://example.com/images/headphones.jpg"
                },
                new Product
                {
                    Id = 2,
                    Name = "T-Shirt",
                    Description = "Cotton T-Shirt",
                    Price = 19.99m,
                    StockQuantity = 100,
                    SKU = "TS-001",
                    CategoryId = 2,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    ImageUrl = "https://example.com/images/tshirt.jpg"
                }
            };

            builder.Entity<Product>().HasData(products);
        }
    }
