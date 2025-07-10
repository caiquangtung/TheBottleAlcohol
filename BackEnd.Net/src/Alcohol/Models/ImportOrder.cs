using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Alcohol.Models.Enums;

namespace Alcohol.Models;

public class ImportOrder
{
    public int Id { get; set; }
    
    [Required]
    [StringLength(50)]
    public string OrderNumber { get; set; }
    
    public int SupplierId { get; set; }
    public int ManagerId { get; set; }
    
    [Required]
    public DateTime OrderDate { get; set; }
    
    public DateTime ImportDate { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal Profit { get; set; }
    public ImportOrderStatusType Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    // Navigation properties
    public virtual Supplier Supplier { get; set; }
    public virtual Account Manager { get; set; }
    public virtual ICollection<ImportOrderDetail> ImportOrderDetails { get; set; }

    public ImportOrder()
    {
        OrderDate = DateTime.UtcNow;
        ImportDate = DateTime.Now;
        CreatedAt = DateTime.UtcNow;
        Status = ImportOrderStatusType.Pending;
        ImportOrderDetails = new HashSet<ImportOrderDetail>();
        OrderNumber = GenerateOrderNumber();
    }
    
    private string GenerateOrderNumber()
    {
        return $"IMP{DateTime.UtcNow:yyyyMMddHHmmss}{new Random().Next(1000, 9999)}";
    }
}
