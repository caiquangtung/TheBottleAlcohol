using System;
using System.Collections.Generic;
using Alcohol.Models.Enums;
using Alcohol.DTOs.Discount;

namespace Alcohol.DTOs.Product;

public class ProductResponseDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string Slug { get; set; }
    public string Origin { get; set; }
    public decimal Volume { get; set; }
    public decimal AlcoholContent { get; set; }
    public decimal Price { get; set; }
    public int StockQuantity { get; set; }
    public bool Status { get; set; }
    public string ImageUrl { get; set; }
    public string MetaTitle { get; set; }
    public string MetaDescription { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    // Category information
    public int CategoryId { get; set; }
    public string CategoryName { get; set; }

    // Brand information
    public int BrandId { get; set; }
    public string BrandName { get; set; }

    public int? Age { get; set; }
    public string Flavor { get; set; }
    
    // Discount information
    public decimal OriginalPrice { get; set; }
    public decimal? DiscountedPrice { get; set; }
    public List<DiscountResponseDto> ActiveDiscounts { get; set; }
    public bool HasDiscount { get; set; }
    public decimal? DiscountAmount { get; set; }
    public decimal? DiscountPercentage { get; set; }
    
    public ProductResponseDto()
    {
        ActiveDiscounts = new List<DiscountResponseDto>();
    }
} 