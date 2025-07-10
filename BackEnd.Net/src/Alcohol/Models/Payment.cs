using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Alcohol.Models.Enums;

namespace Alcohol.Models;

public class Payment
{
    [Key]
    public int Id { get; set; }

    [Required]
    [StringLength(100)]
    public string TransactionId { get; set; }

    [Required]
    public int OrderId { get; set; }

    [ForeignKey("OrderId")]
    public Order Order { get; set; }

    [Required]
    public int AccountId { get; set; }

    [ForeignKey("AccountId")]
    public Account Account { get; set; }

    [Required]
    public decimal Amount { get; set; }

    [Required]
    public PaymentMethodType PaymentMethod { get; set; }

    [Required]
    public PaymentStatusType Status { get; set; }

    [Required]
    public DateTime PaymentDate { get; set; }

    [Required]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedAt { get; set; }

    public Payment()
    {
        Status = PaymentStatusType.Pending; // Default value
        PaymentDate = DateTime.UtcNow;
        TransactionId = GenerateTransactionId();
    }

    private string GenerateTransactionId()
    {
        return $"PAY{DateTime.UtcNow:yyyyMMddHHmmss}{new Random().Next(1000, 9999)}";
    }
} 