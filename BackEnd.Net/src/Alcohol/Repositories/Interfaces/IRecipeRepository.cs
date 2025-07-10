using System.Collections.Generic;
using System.Threading.Tasks;
using Alcohol.Models;

namespace Alcohol.Repositories.Interfaces
{
    public interface IRecipeRepository
    {
        Task<IEnumerable<Recipe>> GetAllAsync();
        Task<Recipe> GetByIdAsync(int id);
        Task<Recipe> GetBySlugAsync(string slug);
        Task<IEnumerable<Recipe>> GetRecipesByCategoryAsync(int categoryId);
        Task<IEnumerable<Recipe>> GetByCategoryIdAsync(int categoryId);
        Task<Recipe> GetRecipeWithIngredientsAsync(int id);
        Task<Recipe> GetRecipeWithCategoriesAsync(int id);
        Task<IEnumerable<Recipe>> GetFeaturedRecipesAsync();
        Task<IEnumerable<Recipe>> GetRecipesByDifficultyAsync(string difficulty);
        Task AddAsync(Recipe recipe);
        void Update(Recipe recipe);
        void Delete(Recipe recipe);
        Task SaveChangesAsync();
    }
} 