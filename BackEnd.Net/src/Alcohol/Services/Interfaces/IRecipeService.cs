using System.Collections.Generic;
using System.Threading.Tasks;
using Alcohol.DTOs.Recipe;
using Alcohol.DTOs;

namespace Alcohol.Services.Interfaces;

public interface IRecipeService
{
    Task<PagedResult<RecipeResponseDto>> GetAllRecipesAsync(RecipeFilterDto filter);
    Task<RecipeResponseDto> GetRecipeByIdAsync(int id);
    Task<IEnumerable<RecipeResponseDto>> GetRecipesByCategoryAsync(int categoryId);
    Task<RecipeResponseDto> GetRecipeWithIngredientsAsync(int id);
    Task<RecipeResponseDto> GetRecipeWithCategoriesAsync(int id);
    Task<RecipeResponseDto> CreateRecipeAsync(RecipeCreateDto createDto);
    Task<RecipeResponseDto> UpdateRecipeAsync(int id, RecipeUpdateDto updateDto);
    Task<bool> DeleteRecipeAsync(int id);
} 