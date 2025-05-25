using System.ComponentModel.DataAnnotations;

namespace Alcohol.DTOs.Recipe;

public class RecipeIngredientUpdateDto
{
    public int Id { get; set; }

    [Required]
    public string Name { get; set; }

    [Required]
    [Range(0.01, double.MaxValue)]
    public decimal Quantity { get; set; }

    [Required]
    public string Unit { get; set; }

    public string Notes { get; set; }
} 