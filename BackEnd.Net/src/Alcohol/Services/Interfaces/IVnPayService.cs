using Alcohol.DTOs.VnPay;

namespace Alcohol.Services.Interfaces;

public interface IVnPayService
{
    /// <summary>
    /// Tạo URL thanh toán VnPay
    /// </summary>
    /// <param name="request">Thông tin yêu cầu thanh toán</param>
    /// <param name="clientIpAddress">Địa chỉ IP của client</param>
    /// <returns>Response chứa URL thanh toán</returns>
    Task<VnPayCreatePaymentResponseDto> CreatePaymentUrlAsync(VnPayCreatePaymentRequestDto request, string clientIpAddress);

    /// <summary>
    /// Xử lý callback từ VnPay sau khi thanh toán
    /// </summary>
    /// <param name="vnpayData">Dữ liệu trả về từ VnPay</param>
    /// <returns>Kết quả xử lý callback</returns>
    Task<VnPayPaymentCallbackDto> ProcessPaymentCallbackAsync(VnPayPaymentReturnDto vnpayData);

    /// <summary>
    /// Xác thực chữ ký từ VnPay
    /// </summary>
    /// <param name="vnpayData">Dữ liệu từ VnPay</param>
    /// <param name="inputHash">Chữ ký cần xác thực</param>
    /// <returns>True nếu chữ ký hợp lệ</returns>
    bool ValidateSignature(VnPayPaymentReturnDto vnpayData, string inputHash);

    /// <summary>
    /// Xử lý IPN (Instant Payment Notification) từ VnPay
    /// </summary>
    /// <param name="vnpayData">Dữ liệu IPN từ VnPay</param>
    /// <returns>Response code cho VnPay</returns>
    Task<string> ProcessIpnAsync(VnPayPaymentReturnDto vnpayData);
}
