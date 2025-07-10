using Alcohol.Data;
using Alcohol.Models;
using Alcohol.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Alcohol.Repositories
{
    public class RecipeRepository : IRecipeRepository
    {
        private readonly MyDbContext _context;

        public RecipeRepository(MyDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Recipe>> GetAllAsync()
        {
            return await _context.Recipes
                .Include(r => r.Ingredients)
                .Include(r => r.Category)
                .ToListAsync();
        }

        public async Task<Recipe> GetByIdAsync(int id)
        {
            return await _context.Recipes.FindAsync(id);
        }

        public async Task AddAsync(Recipe recipe)
        {
            await _context.Recipes.AddAsync(recipe);
        }

        public void Update(Recipe recipe)
        {
            _context.Recipes.Update(recipe);
        }

        public void Delete(Recipe recipe)
        {
            _context.Recipes.Remove(recipe);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Recipe>> GetRecipesByCategoryAsync(int categoryId)
        {
            return await _context.Recipes
                .Where(r => r.CategoryId == categoryId)
                .OrderBy(r => r.DisplayOrder)
                .ToListAsync();
        }

        public async Task<Recipe> GetRecipeWithIngredientsAsync(int id)
        {
            return await _context.Recipes
                .Include(r => r.Ingredients)
                .ThenInclude(i => i.Product)
                .FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task<Recipe> GetRecipeWithCategoriesAsync(int id)
        {
            return await _context.Recipes
                .Include(r => r.Category)
                .FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task<IEnumerable<Recipe>> GetFeaturedRecipesAsync()
        {
            return await _context.Recipes
                .Where(r => r.IsActive)
                .OrderByDescending(r => r.DisplayOrder)
                .Take(6)
                .ToListAsync();
        }

        public async Task<IEnumerable<Recipe>> GetRecipesByDifficultyAsync(string difficulty)
        {
            return await _context.Recipes
                .Where(r => r.Difficulty == difficulty && r.IsActive)
                .OrderBy(r => r.DisplayOrder)
                .ToListAsync();
        }

        public async Task<Recipe> GetBySlugAsync(string slug)
        {
            return await _context.Recipes
                .Include(r => r.Ingredients)
                .Include(r => r.Category)
                .FirstOrDefaultAsync(r => r.Slug == slug);
        }

        public async Task<IEnumerable<Recipe>> GetByCategoryIdAsync(int categoryId)
        {
            return await _context.Recipes
                .Include(r => r.Ingredients)
                .Include(r => r.Category)
                .Where(r => r.CategoryId == categoryId)
                .ToListAsync();
        }
    }
} 