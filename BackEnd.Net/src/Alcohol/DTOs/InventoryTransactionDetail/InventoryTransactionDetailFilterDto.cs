using System;
using Alcohol.Models.Enums;

namespace Alcohol.DTOs.InventoryTransactionDetail;

public class InventoryTransactionDetailFilterDto : BaseFilterDto
{
    public int? InventoryTransactionId { get; set; }
    public int? ProductId { get; set; }
    public InventoryTransactionStatusType? Status { get; set; }
    public decimal? MinQuantity { get; set; }
    public decimal? MaxQuantity { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
} 