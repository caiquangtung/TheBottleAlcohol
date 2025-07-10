using System;

namespace Alcohol.DTOs.Payment;

public class PaymentFilterDto : BaseFilterDto
{
    public int? OrderId { get; set; }
    public string PaymentMethod { get; set; }
    public string Status { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public decimal? MinAmount { get; set; }
    public decimal? MaxAmount { get; set; }
} 