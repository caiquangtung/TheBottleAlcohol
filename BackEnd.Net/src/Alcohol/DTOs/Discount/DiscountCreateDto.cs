using System;
using System.ComponentModel.DataAnnotations;

namespace Alcohol.DTOs.Discount;

public class DiscountCreateDto
{
    [Required(ErrorMessage = "Discount code is required")]
    public string Code { get; set; }

    public string Description { get; set; }

    [Required(ErrorMessage = "Discount amount is required")]
    [Range(0, double.MaxValue, ErrorMessage = "Discount amount must be greater than or equal to 0")]
    public decimal DiscountAmount { get; set; }

    [Required(ErrorMessage = "Minimum order amount is required")]
    [Range(0, double.MaxValue, ErrorMessage = "Minimum order amount must be greater than or equal to 0")]
    public decimal MinimumOrderAmount { get; set; }

    [Required(ErrorMessage = "Start date is required")]
    public DateTime StartDate { get; set; }

    [Required(ErrorMessage = "End date is required")]
    public DateTime EndDate { get; set; }
} 