using System;

namespace Alcohol.DTOs.Category;

public class CategoryFilterDto : BaseFilterDto
{
    public int? ParentId { get; set; }
    public bool? IsActive { get; set; }
} 