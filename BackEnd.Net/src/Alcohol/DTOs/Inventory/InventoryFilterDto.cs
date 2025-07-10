using System;

namespace Alcohol.DTOs.Inventory;

public class InventoryFilterDto : BaseFilterDto
{
    public int? ProductId { get; set; }
    public int? MinQuantity { get; set; }
    public int? MaxQuantity { get; set; }
    public int? MinReorderLevel { get; set; }
    public int? MaxReorderLevel { get; set; }
    public bool? IsLowStock { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
} 