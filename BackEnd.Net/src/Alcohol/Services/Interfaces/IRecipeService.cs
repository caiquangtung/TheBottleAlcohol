using System.Collections.Generic;
using System.Threading.Tasks;
using Alcohol.DTOs.Recipe;

namespace Alcohol.Services.Interfaces;

public interface IRecipeService
{
    Task<IEnumerable<RecipeResponseDto>> GetAllRecipesAsync();
    Task<RecipeResponseDto> GetRecipeByIdAsync(int id);
    Task<IEnumerable<RecipeResponseDto>> GetRecipesByCategoryAsync(int categoryId);
    Task<RecipeResponseDto> GetRecipeWithIngredientsAsync(int id);
    Task<RecipeResponseDto> GetRecipeWithCategoriesAsync(int id);
    Task<IEnumerable<RecipeResponseDto>> GetFeaturedRecipesAsync();
    Task<IEnumerable<RecipeResponseDto>> GetRecipesByDifficultyAsync(string difficulty);
    Task<RecipeResponseDto> CreateRecipeAsync(RecipeCreateDto createDto);
    Task<RecipeResponseDto> UpdateRecipeAsync(int id, RecipeUpdateDto updateDto);
    Task<bool> DeleteRecipeAsync(int id);
} 