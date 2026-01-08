using AutoMapper;
using Ecommerce.API.Data.Entities;
using Ecommerce.API.Dtos.Responses.Auth;
using Ecommerce.API.Dtos.Responses.Products;

namespace Ecommerce.API.Helpers
{
    public class MappingProfiles : Profile
    {
        public MappingProfiles()
        {
            // Auth mappings
            CreateMap<ApplicationUser, UserResponse>()
                .ForMember(dest => dest.Roles, opt => opt.Ignore());

            // Product mappings
            CreateMap<Product, ProductResponse>()
                .ForMember(dest => dest.Category, opt => opt.MapFrom(src => new CategoryResponse
                {
                    Id = src.Category.Id,
                    Name = src.Category.Name
                }))
                .ForMember(dest => dest.FinalPrice, opt => opt.MapFrom(src => src.DiscountPrice ?? src.Price));

            // CreateMap<Category, CategoryResponse>();
            // CreateMap<Review, ReviewResponse>()
            //     .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => $"{src.User.FirstName} {src.User.LastName}"));

            // // Order mappings
            // CreateMap<Order, OrderResponse>();
            // CreateMap<OrderItem, OrderItemResponse>()
            //     .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product.Name));
        }
    }
}