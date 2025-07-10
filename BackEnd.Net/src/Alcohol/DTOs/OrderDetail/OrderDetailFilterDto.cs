using System;

namespace Alcohol.DTOs.OrderDetail;

public class OrderDetailFilterDto : BaseFilterDto
{
    public int? OrderId { get; set; }
    public int? ProductId { get; set; }
    public decimal? MinQuantity { get; set; }
    public decimal? MaxQuantity { get; set; }
    public decimal? MinTotalAmount { get; set; }
    public decimal? MaxTotalAmount { get; set; }
} 