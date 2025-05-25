using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Alcohol.Models.Enums;

namespace Alcohol.Models;

public class ImportOrderDetail
{
    public int Id { get; set; }
    public int ImportOrderId { get; set; }
    public int ProductId { get; set; }
    public int Quantity { get; set; }
    public decimal ImportPrice { get; set; }
    public decimal TotalAmount { get; set; }
    public ImportOrderStatusType Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    // Navigation properties
    public virtual ImportOrder ImportOrder { get; set; }
    public virtual Product Product { get; set; }

    public ImportOrderDetail()
    {
        CreatedAt = DateTime.UtcNow;
        Status = ImportOrderStatusType.Pending;
    }
}
