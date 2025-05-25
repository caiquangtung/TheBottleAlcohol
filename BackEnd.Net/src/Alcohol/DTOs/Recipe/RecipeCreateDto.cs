using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Alcohol.DTOs.Recipe;

public class RecipeCreateDto
{
    [Required]
    [StringLength(100)]
    public string Name { get; set; }

    [Required]
    public string Description { get; set; }

    [Required]
    public string Instructions { get; set; }

    [Required]
    public string Difficulty { get; set; }

    [Required]
    [Range(0, int.MaxValue)]
    public int PrepTime { get; set; }

    [Required]
    [Range(0, int.MaxValue)]
    public int CookTime { get; set; }

    [Required]
    [Range(1, int.MaxValue)]
    public int Servings { get; set; }

    public string ImageUrl { get; set; }
    public bool IsFeatured { get; set; }
    public ICollection<int> CategoryIds { get; set; }
    public ICollection<RecipeIngredientCreateDto> Ingredients { get; set; }
} 