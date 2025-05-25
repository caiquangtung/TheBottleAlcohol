using System;
using Alcohol.Models.Enums;

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
    public int CategoryId { get; set; }
    public string CategoryName { get; set; }
    public int BrandId { get; set; }
    public string BrandName { get; set; }
} 