namespace Alcohol.DTOs.VnPay;

public class VnPayCreatePaymentResponseDto
{
    public string PaymentUrl { get; set; } = string.Empty;
    public string TxnRef { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public int OrderId { get; set; }
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
}
