namespace Alcohol.DTOs.VnPay;

public class VnPayPaymentCallbackDto
{
    public bool Success { get; set; }
    public string ResponseCode { get; set; } = string.Empty;
    public string TransactionRef { get; set; } = string.Empty;
    public string VnPayTransactionNo { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public int OrderId { get; set; }
    public string BankCode { get; set; } = string.Empty;
    public string BankTransactionNo { get; set; } = string.Empty;
    public string CardType { get; set; } = string.Empty;
    public DateTime PaymentDate { get; set; }
    public string OrderInfo { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
}
