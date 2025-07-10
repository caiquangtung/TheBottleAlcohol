using System;

namespace Alcohol.DTOs.Product;

public class ProductFilterDto : BaseFilterDto
{
    public int? CategoryId { get; set; }
    public int? BrandId { get; set; }
    public decimal? MinPrice { get; set; }
    public decimal? MaxPrice { get; set; }
    public bool? Status { get; set; }
} 