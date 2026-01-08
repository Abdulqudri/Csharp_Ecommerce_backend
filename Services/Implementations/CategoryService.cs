using AutoMapper;
using Ecommerce.API.Data.Entities;
using Ecommerce.API.Dtos.Requests.Categories;
using Ecommerce.API.Dtos.Responses.Categories;
using Ecommerce.API.Repository.Interfaces;
using Ecommerce.API.Services.Interfaces;

namespace Ecommerce.API.Services.Implementations
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IMapper _mapper;

        public CategoryService(ICategoryRepository categoryRepository, IMapper mapper)
        {
            _categoryRepository = categoryRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<CategoryResponse>> GetAllCategoriesAsync()
        {
            var categories = await _categoryRepository.GetCategoriesWithProductsAsync();
            return _mapper.Map<IEnumerable<CategoryResponse>>(categories);
        }

        public async Task<CategoryResponse?> GetCategoryByIdAsync(int id)
        {
            var category = await _categoryRepository.GetCategoryWithProductsAsync(id);
            if (category == null)
                return null;

            return _mapper.Map<CategoryResponse>(category);
        }

        public async Task<CategoryResponse> CreateCategoryAsync(CreateCategoryRequest request)
        {
            var category = new Category
            {
                Name = request.Name,
                Description = request.Description,
                ImageUrl = request.ImageUrl,
                ParentCategoryId = request.ParentCategoryId,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            await _categoryRepository.AddAsync(category);
            return _mapper.Map<CategoryResponse>(category);
        }

        public async Task<CategoryResponse?> UpdateCategoryAsync(int id, UpdateCategoryRequest request)
        {
            var category = await _categoryRepository.GetByIdAsync(id);
            if (category == null)
                return null;

            if (!string.IsNullOrEmpty(request.Name))
                category.Name = request.Name;

            if (!string.IsNullOrEmpty(request.Description))
                category.Description = request.Description;

            if (!string.IsNullOrEmpty(request.ImageUrl))
                category.ImageUrl = request.ImageUrl;

            if (request.ParentCategoryId.HasValue)
                category.ParentCategoryId = request.ParentCategoryId;

            if (request.IsActive.HasValue)
                category.IsActive = request.IsActive.Value;

            await _categoryRepository.UpdateAsync(category);
            return _mapper.Map<CategoryResponse>(category);
        }

        public async Task<bool> DeleteCategoryAsync(int id)
        {
            var category = await _categoryRepository.GetByIdAsync(id);
            if (category == null)
                return false;

            category.IsActive = false;
            await _categoryRepository.UpdateAsync(category);
            return true;
        }

        public async Task<IEnumerable<CategoryResponse>> GetSubCategoriesAsync(int parentCategoryId)
        {
            var subCategories = await _categoryRepository.GetSubCategoriesAsync(parentCategoryId);
            return _mapper.Map<IEnumerable<CategoryResponse>>(subCategories);
        }
    }
}