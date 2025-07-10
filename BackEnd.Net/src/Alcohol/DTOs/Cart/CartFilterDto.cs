using System;

namespace Alcohol.DTOs.Cart;

public class CartFilterDto : BaseFilterDto
{
    public int? CustomerId { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public decimal? MinTotal { get; set; }
    public decimal? MaxTotal { get; set; }
} 