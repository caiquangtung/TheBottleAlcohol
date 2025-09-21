using System;
using Alcohol.Models.Enums;

namespace Alcohol.DTOs.InventoryTransaction;

public class InventoryTransactionResponseDto
{
    public int Id { get; set; }
    public string TransactionNumber { get; set; }
    public int ProductId { get; set; }
    public string ProductName { get; set; }
    public int Quantity { get; set; }
    public InventoryTransactionType TransactionType { get; set; }
    public ReferenceType ReferenceType { get; set; }
    public int ReferenceId { get; set; }
    public InventoryTransactionStatusType Status { get; set; }
    public string Notes { get; set; }
    public DateTime TransactionDate { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
} 