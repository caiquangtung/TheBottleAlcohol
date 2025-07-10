using System;

namespace Alcohol.DTOs.CartDetail;

public class CartDetailFilterDto : BaseFilterDto
{
    public int? CartId { get; set; }
    public int? ProductId { get; set; }
    public decimal? MinQuantity { get; set; }
    public decimal? MaxQuantity { get; set; }
    public decimal? MinPrice { get; set; }
    public decimal? MaxPrice { get; set; }
} 