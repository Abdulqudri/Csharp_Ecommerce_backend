using Ecommerce.API.Dtos.Requests.Categories;
using Ecommerce.API.Dtos.Responses.Categories;

namespace Ecommerce.API.Services.Interfaces
{
    public interface ICategoryService
    {
        Task<IEnumerable<CategoryResponse>> GetAllCategoriesAsync();
        Task<CategoryResponse?> GetCategoryByIdAsync(int id);
        Task<CategoryResponse> CreateCategoryAsync(CreateCategoryRequest request);
        Task<CategoryResponse?> UpdateCategoryAsync(int id, UpdateCategoryRequest request);
        Task<bool> DeleteCategoryAsync(int id);
        Task<IEnumerable<CategoryResponse>> GetSubCategoriesAsync(int parentCategoryId);
    }
}