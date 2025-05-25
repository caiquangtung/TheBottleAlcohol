using System.Collections.Generic;
using System.Threading.Tasks;
using Alcohol.DTOs.Category;

namespace Alcohol.Services.Interfaces
{
    public interface ICategoryService
    {
        Task<IEnumerable<CategoryResponseDto>> GetAllCategoriesAsync();
        Task<IEnumerable<CategoryResponseDto>> GetRootCategoriesAsync();
        Task<IEnumerable<CategoryResponseDto>> GetSubCategoriesAsync(int parentId);
        Task<CategoryResponseDto> GetCategoryByIdAsync(int id);
        Task<CategoryResponseDto> GetCategoryWithChildrenAsync(int id);
        Task<CategoryResponseDto> GetCategoryWithProductsAsync(int id);
        Task<CategoryResponseDto> CreateCategoryAsync(CategoryCreateDto createDto);
        Task<CategoryResponseDto> UpdateCategoryAsync(int id, CategoryUpdateDto updateDto);
        Task<bool> DeleteCategoryAsync(int id);
        Task<bool> HasChildrenAsync(int id);
        Task<bool> HasProductsAsync(int id);
    }
} 