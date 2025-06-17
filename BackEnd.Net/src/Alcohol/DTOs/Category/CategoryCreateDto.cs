using System.ComponentModel.DataAnnotations;

namespace Alcohol.DTOs.Category;

public class CategoryCreateDto
{
    [Required(ErrorMessage = "Category name is required")]
    [StringLength(100)]
    public string Name { get; set; }

    [StringLength(500)]
    public string Description { get; set; }
    
    public string Slug { get; set; }
    
    public int? ParentId { get; set; }
    
    public bool IsActive { get; set; } = true;
    
    public int DisplayOrder { get; set; }
    
    public string ImageUrl { get; set; }
    
    [StringLength(200)]
    public string MetaTitle { get; set; }
    
    [StringLength(500)]
    public string MetaDescription { get; set; }
} 