using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Alcohol.DTOs.Recipe;
using Alcohol.Models;
using Alcohol.Repositories.Interfaces;
using Alcohol.Services.Interfaces;
using AutoMapper;

namespace Alcohol.Services;

public class RecipeService : IRecipeService
{
    private readonly IRecipeRepository _recipeRepository;
    private readonly IMapper _mapper;

    public RecipeService(IRecipeRepository recipeRepository, IMapper mapper)
    {
        _recipeRepository = recipeRepository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<RecipeResponseDto>> GetAllRecipesAsync()
    {
        var recipes = await _recipeRepository.GetAllAsync();
        return _mapper.Map<IEnumerable<RecipeResponseDto>>(recipes);
    }

    public async Task<RecipeResponseDto> GetRecipeByIdAsync(int id)
    {
        var recipe = await _recipeRepository.GetByIdAsync(id);
        if (recipe == null)
            return null;

        return _mapper.Map<RecipeResponseDto>(recipe);
    }

    public async Task<IEnumerable<RecipeResponseDto>> GetRecipesByCategoryAsync(int categoryId)
    {
        var recipes = await _recipeRepository.GetRecipesByCategoryAsync(categoryId);
        return _mapper.Map<IEnumerable<RecipeResponseDto>>(recipes);
    }

    public async Task<RecipeResponseDto> GetRecipeWithIngredientsAsync(int id)
    {
        var recipe = await _recipeRepository.GetRecipeWithIngredientsAsync(id);
        if (recipe == null)
            return null;

        return _mapper.Map<RecipeResponseDto>(recipe);
    }

    public async Task<RecipeResponseDto> GetRecipeWithCategoriesAsync(int id)
    {
        var recipe = await _recipeRepository.GetRecipeWithCategoriesAsync(id);
        if (recipe == null)
            return null;

        return _mapper.Map<RecipeResponseDto>(recipe);
    }

    public async Task<IEnumerable<RecipeResponseDto>> GetFeaturedRecipesAsync()
    {
        var recipes = await _recipeRepository.GetFeaturedRecipesAsync();
        return _mapper.Map<IEnumerable<RecipeResponseDto>>(recipes);
    }

    public async Task<IEnumerable<RecipeResponseDto>> GetRecipesByDifficultyAsync(string difficulty)
    {
        var recipes = await _recipeRepository.GetRecipesByDifficultyAsync(difficulty);
        return _mapper.Map<IEnumerable<RecipeResponseDto>>(recipes);
    }

    public async Task<RecipeResponseDto> CreateRecipeAsync(RecipeCreateDto createDto)
    {
        var recipe = _mapper.Map<Recipe>(createDto);
        recipe.CreatedAt = DateTime.UtcNow;

        await _recipeRepository.AddAsync(recipe);
        await _recipeRepository.SaveChangesAsync();

        return _mapper.Map<RecipeResponseDto>(recipe);
    }

    public async Task<RecipeResponseDto> UpdateRecipeAsync(int id, RecipeUpdateDto updateDto)
    {
        var recipe = await _recipeRepository.GetByIdAsync(id);
        if (recipe == null)
            return null;

        _mapper.Map(updateDto, recipe);
        recipe.UpdatedAt = DateTime.UtcNow;

        _recipeRepository.Update(recipe);
        await _recipeRepository.SaveChangesAsync();

        return _mapper.Map<RecipeResponseDto>(recipe);
    }

    public async Task<bool> DeleteRecipeAsync(int id)
    {
        var recipe = await _recipeRepository.GetByIdAsync(id);
        if (recipe == null)
            return false;

        _recipeRepository.Delete(recipe);
        await _recipeRepository.SaveChangesAsync();
        return true;
    }
} 