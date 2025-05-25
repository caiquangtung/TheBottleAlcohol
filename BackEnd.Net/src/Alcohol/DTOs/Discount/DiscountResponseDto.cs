using System;

namespace Alcohol.DTOs.Discount;

public class DiscountResponseDto
{
    public int Id { get; set; }
    public string Code { get; set; }
    public string Description { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal MinimumOrderAmount { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
} 