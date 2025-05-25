using System;
using System.Collections.Generic;

namespace Alcohol.DTOs.Recipe;

public class RecipeResponseDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string Instructions { get; set; }
    public string Difficulty { get; set; }
    public int PrepTime { get; set; }
    public int CookTime { get; set; }
    public int Servings { get; set; }
    public string ImageUrl { get; set; }
    public bool IsFeatured { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public ICollection<RecipeCategoryDto> Categories { get; set; }
    public ICollection<RecipeIngredientDto> Ingredients { get; set; }
} 