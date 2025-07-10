using System;

namespace Alcohol.DTOs.InventoryTransaction;

public class InventoryTransactionFilterDto : BaseFilterDto
{
    public int? ProductId { get; set; }
    public string TransactionType { get; set; }
    public string Status { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public int? MinQuantity { get; set; }
    public int? MaxQuantity { get; set; }
} 