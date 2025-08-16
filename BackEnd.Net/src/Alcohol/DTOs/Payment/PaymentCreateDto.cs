using System;
using System.ComponentModel.DataAnnotations;
using Alcohol.Models.Enums;

namespace Alcohol.DTOs.Payment;

public class PaymentCreateDto
{
    [Required]
    public int OrderId { get; set; }

    [Required]
    public int AccountId { get; set; }

    [Required]
    [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than 0")] 
    public decimal Amount { get; set; }

    public PaymentStatusType Status { get; set; } = PaymentStatusType.Pending;
    
    public PaymentMethodType PaymentMethod { get; set; } = PaymentMethodType.Cash;
    
    public string? TransactionId { get; set; }
    
    public DateTime PaymentDate { get; set; } = DateTime.UtcNow;
} 