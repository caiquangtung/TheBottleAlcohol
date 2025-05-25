using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Alcohol.Models;

public class Inventory
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public int Quantity { get; set; }
    public decimal AverageCost { get; set; }
    public decimal TotalValue { get; set; }
    public DateTime LastUpdated { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    // Navigation properties
    public virtual Product Product { get; set; }

    public Inventory()
    {
        CreatedAt = DateTime.UtcNow;
        LastUpdated = DateTime.UtcNow;
        Quantity = 0;
        AverageCost = 0;
        TotalValue = 0;
    }
} 