using System.ComponentModel.DataAnnotations;

namespace Alcohol.DTOs.Product;

public class ProductCreateDto
{
    [Required(ErrorMessage = "Product name is required")]
    public string Name { get; set; }

    public string Description { get; set; }
    public string Slug { get; set; }
    public string Origin { get; set; }
    public decimal Volume { get; set; }
    public decimal AlcoholContent { get; set; }
    public decimal Price { get; set; }
    public int StockQuantity { get; set; }
    public bool Status { get; set; } = true;
    public string ImageUrl { get; set; }
    public string MetaTitle { get; set; }
    public string MetaDescription { get; set; }
    public int CategoryId { get; set; }
    public int BrandId { get; set; }
} 