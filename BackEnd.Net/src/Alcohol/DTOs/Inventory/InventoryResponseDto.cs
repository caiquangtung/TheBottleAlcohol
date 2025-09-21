using System;

namespace Alcohol.DTOs.Inventory;

public class InventoryResponseDto
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public string ProductName { get; set; }
    public int Quantity { get; set; }
    public int ReorderLevel { get; set; }
    public decimal AverageCost { get; set; }
    public decimal TotalValue { get; set; }
    public DateTime LastUpdated { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string Notes { get; set; }
} 