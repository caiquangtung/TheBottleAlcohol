using System.ComponentModel.DataAnnotations;

namespace Alcohol.DTOs.Category;

public class CategoryCreateDto
{
    [Required(ErrorMessage = "Category name is required")]
    public string Name { get; set; }

    public string Description { get; set; }
    public string Slug { get; set; }
    public int? ParentId { get; set; }
    public bool Status { get; set; } = true;
} 