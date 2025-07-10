using System;

namespace Alcohol.DTOs.Shipping;

public class ShippingFilterDto : BaseFilterDto
{
    public int? OrderId { get; set; }
    public string Status { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public decimal? MinCost { get; set; }
    public decimal? MaxCost { get; set; }
} 