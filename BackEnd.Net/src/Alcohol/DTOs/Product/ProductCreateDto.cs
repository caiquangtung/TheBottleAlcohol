using System.ComponentModel.DataAnnotations;

namespace Alcohol.DTOs.Product;

public class ProductCreateDto
{
    [Required(ErrorMessage = "Product name is required")]
    public string Name { get; set; }

    public string Description { get; set; }
    public string Slug { get; set; }
    public string Origin { get; set; }

    [Required(ErrorMessage = "Volume is required")]
    [Range(0.01, 10.00, ErrorMessage = "Volume must be between 0.01 and 10.00")]
    [RegularExpression(@"^\d+(\.\d{1,2})?$", ErrorMessage = "Volume must have at most 2 decimal places")]
    public decimal Volume { get; set; }

    [Required(ErrorMessage = "Alcohol content is required")]
    [Range(0.0, 100.0, ErrorMessage = "Alcohol content must be between 0.0 and 100.0")]
    [RegularExpression(@"^\d+(\.\d{1})?$", ErrorMessage = "Alcohol content must have at most 1 decimal place")]
    public decimal AlcoholContent { get; set; }

    [Required(ErrorMessage = "Price is required")]
    [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0")]
    [RegularExpression(@"^\d+(\.\d{1,2})?$", ErrorMessage = "Price must have at most 2 decimal places")]
    public decimal Price { get; set; }

    [Required(ErrorMessage = "Stock quantity is required")]
    [Range(0, int.MaxValue, ErrorMessage = "Stock quantity must be greater than or equal to 0")]
    public int StockQuantity { get; set; }

    public bool Status { get; set; } = true;
    public string ImageUrl { get; set; }
    public string MetaTitle { get; set; }
    public string MetaDescription { get; set; }
    public int CategoryId { get; set; }
    public int BrandId { get; set; }
} 