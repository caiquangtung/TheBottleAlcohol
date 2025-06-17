using System.ComponentModel.DataAnnotations;

namespace Alcohol.DTOs.Brand;

public class BrandUpdateDto
{
    [Required(ErrorMessage = "Brand name is required")]
    public string Name { get; set; }

    public string Description { get; set; }
    public string LogoUrl { get; set; }
    public bool IsActive { get; set; }
} 