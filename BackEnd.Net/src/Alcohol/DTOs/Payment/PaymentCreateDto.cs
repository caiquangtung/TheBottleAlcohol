using System.ComponentModel.DataAnnotations;

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
} 