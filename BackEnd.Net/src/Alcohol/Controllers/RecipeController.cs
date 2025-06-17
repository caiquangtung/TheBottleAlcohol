using System;
using System.Threading.Tasks;
using Alcohol.DTOs.Recipe;
using Alcohol.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Alcohol.Common;

namespace Alcohol.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class RecipeController : ControllerBase
    {
        private readonly IRecipeService _recipeService;

        public RecipeController(IRecipeService recipeService)
        {
            _recipeService = recipeService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var recipes = await _recipeService.GetAllRecipesAsync();
            return Ok(new ApiResponse<IEnumerable<RecipeResponseDto>>(recipes));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var recipe = await _recipeService.GetRecipeByIdAsync(id);
            if (recipe == null)
                return NotFound(new ApiResponse<string>("Recipe not found"));
            return Ok(new ApiResponse<RecipeResponseDto>(recipe));
        }

        [HttpGet("category/{categoryId}")]
        public async Task<IActionResult> GetByCategory(int categoryId)
        {
            var recipes = await _recipeService.GetRecipesByCategoryAsync(categoryId);
            return Ok(new ApiResponse<IEnumerable<RecipeResponseDto>>(recipes));
        }

        [HttpGet("featured")]
        public async Task<IActionResult> GetFeatured()
        {
            var recipes = await _recipeService.GetFeaturedRecipesAsync();
            return Ok(new ApiResponse<IEnumerable<RecipeResponseDto>>(recipes));
        }

        [HttpGet("difficulty/{difficulty}")]
        public async Task<IActionResult> GetByDifficulty(string difficulty)
        {
            var recipes = await _recipeService.GetRecipesByDifficultyAsync(difficulty);
            return Ok(new ApiResponse<IEnumerable<RecipeResponseDto>>(recipes));
        }

        [HttpGet("{id}/ingredients")]
        public async Task<IActionResult> GetWithIngredients(int id)
        {
            var recipe = await _recipeService.GetRecipeWithIngredientsAsync(id);
            if (recipe == null)
                return NotFound(new ApiResponse<string>("Recipe not found"));
            return Ok(new ApiResponse<RecipeResponseDto>(recipe));
        }

        [HttpGet("{id}/categories")]
        public async Task<IActionResult> GetWithCategories(int id)
        {
            var recipe = await _recipeService.GetRecipeWithCategoriesAsync(id);
            if (recipe == null)
                return NotFound(new ApiResponse<string>("Recipe not found"));
            return Ok(new ApiResponse<RecipeResponseDto>(recipe));
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> Create(RecipeCreateDto createDto)
        {
            var recipe = await _recipeService.CreateRecipeAsync(createDto);
            return CreatedAtAction(nameof(GetById), new { id = recipe.Id }, new ApiResponse<RecipeResponseDto>(recipe));
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> Update(int id, RecipeUpdateDto updateDto)
        {
            var recipe = await _recipeService.UpdateRecipeAsync(id, updateDto);
            if (recipe == null)
                return NotFound(new ApiResponse<string>("Recipe not found"));
            return Ok(new ApiResponse<RecipeResponseDto>(recipe));
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _recipeService.DeleteRecipeAsync(id);
            if (!result)
                return NotFound(new ApiResponse<string>("Recipe not found"));
            return Ok(new ApiResponse<string>("Recipe deleted successfully"));
        }
    }
} 