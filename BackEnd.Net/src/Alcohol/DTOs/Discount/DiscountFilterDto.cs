using System;

namespace Alcohol.DTOs.Discount;

public class DiscountFilterDto : BaseFilterDto
{
    public bool? IsActive { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public decimal? MinAmount { get; set; }
    public decimal? MaxAmount { get; set; }
} 