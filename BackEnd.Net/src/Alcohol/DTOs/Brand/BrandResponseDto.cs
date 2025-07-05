using System;
using System.Collections.Generic;
using Alcohol.DTOs.Product;

namespace Alcohol.DTOs.Brand;

public class BrandResponseDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string LogoUrl { get; set; }
    public string Website { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public ICollection<ProductResponseDto> Products { get; set; }
} 