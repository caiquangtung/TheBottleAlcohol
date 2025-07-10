using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Alcohol.DTOs.Recipe;
using Alcohol.Models;
using Alcohol.Repositories.Interfaces;
using Alcohol.Services.Interfaces;
using AutoMapper;
using Alcohol.DTOs;

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

    public async Task<PagedResult<RecipeResponseDto>> GetAllRecipesAsync(RecipeFilterDto filter)
    {
        var recipes = await _recipeRepository.GetAllAsync();
        
        // Apply filters
        var filteredRecipes = recipes.AsQueryable();
        
        if (!string.IsNullOrWhiteSpace(filter.SearchTerm))
        {
            filteredRecipes = filteredRecipes.Where(r => 
                r.Name.Contains(filter.SearchTerm) || 
                r.Description.Contains(filter.SearchTerm) ||
                r.Instructions.Contains(filter.SearchTerm));
        }
        
        if (filter.CategoryId.HasValue)
        {
            filteredRecipes = filteredRecipes.Where(r => r.CategoryId == filter.CategoryId.Value);
        }
        
        if (!string.IsNullOrWhiteSpace(filter.Difficulty))
        {
            filteredRecipes = filteredRecipes.Where(r => r.Difficulty == filter.Difficulty);
        }
        
        if (filter.MinPrepTime.HasValue)
        {
            filteredRecipes = filteredRecipes.Where(r => r.PreparationTime >= filter.MinPrepTime.Value);
        }
        
        if (filter.MaxPrepTime.HasValue)
        {
            filteredRecipes = filteredRecipes.Where(r => r.PreparationTime <= filter.MaxPrepTime.Value);
        }
        
        if (filter.StartDate.HasValue)
        {
            filteredRecipes = filteredRecipes.Where(r => r.CreatedAt >= filter.StartDate.Value);
        }
        
        if (filter.EndDate.HasValue)
        {
            filteredRecipes = filteredRecipes.Where(r => r.CreatedAt <= filter.EndDate.Value);
        }
        
        // Apply sorting
        if (!string.IsNullOrWhiteSpace(filter.SortBy))
        {
            filteredRecipes = filter.SortBy.ToLower() switch
            {
                "name" => filter.SortOrder?.ToLower() == "desc" 
                    ? filteredRecipes.OrderByDescending(r => r.Name)
                    : filteredRecipes.OrderBy(r => r.Name),
                "difficulty" => filter.SortOrder?.ToLower() == "desc"
                    ? filteredRecipes.OrderByDescending(r => r.Difficulty)
                    : filteredRecipes.OrderBy(r => r.Difficulty),
                "preparationtime" => filter.SortOrder?.ToLower() == "desc"
                    ? filteredRecipes.OrderByDescending(r => r.PreparationTime)
                    : filteredRecipes.OrderBy(r => r.PreparationTime),
                "createdat" => filter.SortOrder?.ToLower() == "desc"
                    ? filteredRecipes.OrderByDescending(r => r.CreatedAt)
                    : filteredRecipes.OrderBy(r => r.CreatedAt),
                _ => filteredRecipes.OrderBy(r => r.Id)
            };
        }
        else
        {
            filteredRecipes = filteredRecipes.OrderBy(r => r.Id);
        }
        
        var totalRecords = filteredRecipes.Count();
        var pagedRecipes = filteredRecipes
            .Skip((filter.PageNumber - 1) * filter.PageSize)
            .Take(filter.PageSize)
            .ToList();
        
        var recipeDtos = _mapper.Map<List<RecipeResponseDto>>(pagedRecipes);
        return new PagedResult<RecipeResponseDto>(recipeDtos, totalRecords, filter.PageNumber, filter.PageSize);
    }

    public async Task<RecipeResponseDto> GetRecipeByIdAsync(int id)
    {
        var recipe = await _recipeRepository.GetByIdAsync(id);
        if (recipe == null)
            return null;

        return _mapper.Map<RecipeResponseDto>(recipe);
    }

    public async Task<RecipeResponseDto> GetRecipeBySlugAsync(string slug)
    {
        var recipe = await _recipeRepository.GetBySlugAsync(slug);
        if (recipe == null)
            return null;

        return _mapper.Map<RecipeResponseDto>(recipe);
    }

    public async Task<IEnumerable<RecipeResponseDto>> GetRecipesByCategoryAsync(int categoryId)
    {
        var recipes = await _recipeRepository.GetByCategoryIdAsync(categoryId);
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
} 