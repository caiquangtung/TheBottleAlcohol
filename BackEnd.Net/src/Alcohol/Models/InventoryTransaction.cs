using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Alcohol.Models.Enums;

namespace Alcohol.Models;

public class InventoryTransaction
{
    public int Id { get; set; }

    [Required]
    [StringLength(50)]
    public string TransactionNumber { get; set; }

    [Required]
    [ForeignKey("Product")]
    public int ProductId { get; set; }
    
    public virtual Product Product { get; set; }

    [Required]
    public InventoryTransactionType TransactionType { get; set; }

    [Required]
    public int Quantity { get; set; }

    [Required]
    public ReferenceType ReferenceType { get; set; }

    [Required]
    public int ReferenceId { get; set; }

    [Required]
    public InventoryTransactionStatusType Status { get; set; }

    public string Notes { get; set; }

    [Required]
    public DateTime TransactionDate { get; set; }

    [Required]
    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public InventoryTransaction()
    {
        CreatedAt = DateTime.UtcNow;
        TransactionDate = DateTime.UtcNow;
        Status = InventoryTransactionStatusType.Pending;
        TransactionNumber = GenerateTransactionNumber();
    }

    private string GenerateTransactionNumber()
    {
        return $"TXN{DateTime.UtcNow:yyyyMMddHHmmss}{new Random().Next(1000, 9999)}";
    }
} 