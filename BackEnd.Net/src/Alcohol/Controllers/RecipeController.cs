using System;
using System.Threading.Tasks;
using Alcohol.DTOs.Recipe;
using Alcohol.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Alcohol.Common;
using Alcohol.DTOs;

namespace Alcohol.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class RecipeController : ControllerBase
{
    private readonly IRecipeService _recipeService;

    public RecipeController(IRecipeService recipeService)
    {
        _recipeService = recipeService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllRecipes([FromQuery] RecipeFilterDto filter)
    {
        var result = await _recipeService.GetAllRecipesAsync(filter);
        return Ok(new ApiResponse<PagedResult<RecipeResponseDto>>(result));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetRecipeById(int id)
    {
        var recipe = await _recipeService.GetRecipeByIdAsync(id);
        if (recipe == null)
            return NotFound(new ApiResponse<string>("Recipe not found"));
        return Ok(new ApiResponse<RecipeResponseDto>(recipe));
    }

    [HttpGet("category/{categoryId}")]
    public async Task<IActionResult> GetRecipesByCategory(int categoryId)
    {
        var recipes = await _recipeService.GetRecipesByCategoryAsync(categoryId);
        return Ok(new ApiResponse<IEnumerable<RecipeResponseDto>>(recipes));
    }

    [HttpGet("{id}/ingredients")]
    public async Task<IActionResult> GetRecipeWithIngredients(int id)
    {
        var recipe = await _recipeService.GetRecipeWithIngredientsAsync(id);
        if (recipe == null)
            return NotFound(new ApiResponse<string>("Recipe not found"));
        return Ok(new ApiResponse<RecipeResponseDto>(recipe));
    }

    [HttpGet("{id}/categories")]
    public async Task<IActionResult> GetRecipeWithCategories(int id)
    {
        var recipe = await _recipeService.GetRecipeWithCategoriesAsync(id);
        if (recipe == null)
            return NotFound(new ApiResponse<string>("Recipe not found"));
        return Ok(new ApiResponse<RecipeResponseDto>(recipe));
    }

    [HttpPost]
    [Authorize(Roles = "CEO,Manager")]
    public async Task<IActionResult> CreateRecipe(RecipeCreateDto createDto)
    {
        try
        {
            var recipe = await _recipeService.CreateRecipeAsync(createDto);
            return CreatedAtAction(nameof(GetRecipeById), new { id = recipe.Id }, new ApiResponse<RecipeResponseDto>(recipe));
        }
        catch (Exception ex)
        {
            return BadRequest(new ApiResponse<string>(ex.Message));
        }
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "CEO,Manager")]
    public async Task<IActionResult> UpdateRecipe(int id, RecipeUpdateDto updateDto)
    {
        var recipe = await _recipeService.UpdateRecipeAsync(id, updateDto);
        if (recipe == null)
            return NotFound(new ApiResponse<string>("Recipe not found"));
        return Ok(new ApiResponse<RecipeResponseDto>(recipe));
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "CEO,Manager")]
    public async Task<IActionResult> DeleteRecipe(int id)
    {
        var result = await _recipeService.DeleteRecipeAsync(id);
        if (!result)
            return NotFound(new ApiResponse<string>("Recipe not found"));
        return Ok(new ApiResponse<string>("Recipe deleted successfully"));
    }
} 