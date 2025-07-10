using System;

namespace Alcohol.DTOs.Recipe;

public class RecipeFilterDto : BaseFilterDto
{
    public int? CategoryId { get; set; }
    public bool? IsActive { get; set; }
    public string Difficulty { get; set; }
    public int? MinPrepTime { get; set; }
    public int? MaxPrepTime { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
} 