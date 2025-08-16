using System;
using Alcohol.Models.Enums;

namespace Alcohol.DTOs.Payment;

public class PaymentResponseDto
{
    public int Id { get; set; }
    public int OrderId { get; set; }
    public int AccountId { get; set; }
    public decimal Amount { get; set; }
    public PaymentStatusType Status { get; set; }
    public PaymentMethodType PaymentMethod { get; set; }
    public string TransactionId { get; set; }
    public DateTime PaymentDate { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
} 