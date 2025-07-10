using System;

namespace Alcohol.DTOs.ImportOrderDetail;

public class ImportOrderDetailFilterDto : BaseFilterDto
{
    public int? ImportOrderId { get; set; }
    public int? ProductId { get; set; }
    public decimal? MinQuantity { get; set; }
    public decimal? MaxQuantity { get; set; }
    public decimal? MinUnitPrice { get; set; }
    public decimal? MaxUnitPrice { get; set; }
    public decimal? MinTotalAmount { get; set; }
    public decimal? MaxTotalAmount { get; set; }
} 