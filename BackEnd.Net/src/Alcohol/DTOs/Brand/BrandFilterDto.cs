using System;

namespace Alcohol.DTOs.Brand;

public class BrandFilterDto : BaseFilterDto
{
    public bool? IsActive { get; set; }
} 