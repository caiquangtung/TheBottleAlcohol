using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using Alcohol.DTOs.VnPay;
using Alcohol.Models;
using Alcohol.Models.Enums;
using Alcohol.Services.Interfaces;
using AutoMapper;
using Microsoft.Extensions.Options;
using Alcohol.Helpers;

namespace Alcohol.Services;

public class VnPayService : IVnPayService
{
    private readonly VnPayConfig _vnPayConfig;
    private readonly IPaymentService _paymentService;
    private readonly IOrderService _orderService;
    private readonly ICartService _cartService;
    private readonly ILogger<VnPayService> _logger;
    private readonly IMapper _mapper;

    public VnPayService(
        IOptions<VnPayConfig> vnPayConfig,
        IPaymentService paymentService,
        IOrderService orderService,
        ICartService cartService,
        ILogger<VnPayService> logger,
        IMapper mapper)
    {
        _vnPayConfig = vnPayConfig.Value;
        _paymentService = paymentService;
        _orderService = orderService;
        _cartService = cartService;
        _logger = logger;
        _mapper = mapper;
    }

    public async Task<VnPayCreatePaymentResponseDto> CreatePaymentUrlAsync(VnPayCreatePaymentRequestDto request, string clientIpAddress)
    {
        try
        {
            // Verify order exists
            var order = await _orderService.GetOrderByIdAsync(request.OrderId);
            if (order == null)
            {
                throw new ArgumentException("Order not found");
            }

            // Generate unique transaction reference
            var txnRef = GenerateTransactionReference(request.OrderId);
            var createDate = DateTime.Now.ToString("yyyyMMddHHmmss");
            var expireDate = DateTime.Now.AddMinutes(15).ToString("yyyyMMddHHmmss");

            // Normalize amount to integer VND then multiply by 100 per VNPAY spec
            var amountVnd = (long)Math.Round(request.Amount, MidpointRounding.AwayFromZero);
            if (amountVnd <= 0)
            {
                throw new ArgumentException("Invalid amount for VNPAY. Amount must be positive in VND.");
            }

            // Use VnPayLibrary for consistent parameter handling
            var vnpayLib = new VnPayLibrary();
            vnpayLib.AddRequestData("vnp_Version", _vnPayConfig.Version);
            vnpayLib.AddRequestData("vnp_Command", _vnPayConfig.Command);
            vnpayLib.AddRequestData("vnp_TmnCode", _vnPayConfig.TmnCode);
            vnpayLib.AddRequestData("vnp_Amount", (amountVnd * 100).ToString());
            vnpayLib.AddRequestData("vnp_CurrCode", _vnPayConfig.CurrCode);
            vnpayLib.AddRequestData("vnp_TxnRef", txnRef);
            vnpayLib.AddRequestData("vnp_OrderInfo", $"Thanh toan don hang {request.OrderId}");
            vnpayLib.AddRequestData("vnp_OrderType", "other");
            vnpayLib.AddRequestData("vnp_Locale", _vnPayConfig.Locale);
            vnpayLib.AddRequestData("vnp_ReturnUrl", _vnPayConfig.ReturnUrl);
            vnpayLib.AddRequestData("vnp_IpAddr", clientIpAddress);
            vnpayLib.AddRequestData("vnp_CreateDate", createDate);
            vnpayLib.AddRequestData("vnp_ExpireDate", expireDate);

            // Add bank code if specified
            if (!string.IsNullOrEmpty(request.BankCode))
            {
                vnpayLib.AddRequestData("vnp_BankCode", request.BankCode);
            }

                         // Generate payment URL using VnPayLibrary
             var paymentUrl = vnpayLib.CreateRequestUrl(_vnPayConfig.PaymentUrl, _vnPayConfig.HashSecret);

             // DEBUG LOGS
             _logger.LogDebug("[VNPAY][Create] OrderId={OrderId}, TxnRef={TxnRef}, Amount={Amount}, IP={Ip}", request.OrderId, txnRef, request.Amount, clientIpAddress);
             
             // Additional debug: log all parameters that were added
             var debugParams = vnpayLib.GetRequestDataForDebug();
             _logger.LogDebug("[VNPAY][Create] AllParams={Params}", string.Join("&", debugParams.Select(kv => $"{kv.Key}={kv.Value}")));
             _logger.LogDebug("[VNPAY][Create] RawDataForHash={RawData}", vnpayLib.LastRawData);
             _logger.LogDebug("[VNPAY][Create] SecureHash={Hash}", vnpayLib.LastSecureHash);
             _logger.LogDebug("[VNPAY][Create] URL={Url}", paymentUrl);

            _logger.LogInformation("Created VnPay payment URL for order {OrderId}, txnRef: {TxnRef}", request.OrderId, txnRef);

            return new VnPayCreatePaymentResponseDto
            {
                PaymentUrl = paymentUrl,
                TxnRef = txnRef,
                Amount = request.Amount,
                OrderId = request.OrderId,
                CreatedDate = DateTime.UtcNow
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating VnPay payment URL for order {OrderId}", request.OrderId);
            throw;
        }
    }

    public async Task<VnPayPaymentCallbackDto> ProcessPaymentCallbackAsync(VnPayPaymentReturnDto vnpayData)
    {
        try
        {
            var isValidSignature = ValidateSignature(vnpayData, vnpayData.vnp_SecureHash);
            if (!isValidSignature)
            {
                _logger.LogWarning("Invalid VnPay signature for transaction {TxnRef}", vnpayData.vnp_TxnRef);
                return new VnPayPaymentCallbackDto
                {
                    Success = false,
                    Message = "Invalid signature",
                    TransactionRef = vnpayData.vnp_TxnRef
                };
            }

            var orderId = ExtractOrderIdFromTxnRef(vnpayData.vnp_TxnRef);
            var amount = decimal.Parse(vnpayData.vnp_Amount) / 100; // Convert back from VnPay format
            var paymentDate = ParseVnPayDate(vnpayData.vnp_PayDate);

            var callback = new VnPayPaymentCallbackDto
            {
                Success = vnpayData.vnp_ResponseCode == "00",
                ResponseCode = vnpayData.vnp_ResponseCode,
                TransactionRef = vnpayData.vnp_TxnRef,
                VnPayTransactionNo = vnpayData.vnp_TransactionNo,
                Amount = amount,
                OrderId = orderId,
                BankCode = vnpayData.vnp_BankCode,
                BankTransactionNo = vnpayData.vnp_BankTranNo,
                CardType = vnpayData.vnp_CardType,
                PaymentDate = paymentDate,
                OrderInfo = vnpayData.vnp_OrderInfo,
                Message = GetResponseMessage(vnpayData.vnp_ResponseCode)
            };

            // Update payment status in database
            if (callback.Success)
            {
                await UpdatePaymentStatusAsync(orderId, vnpayData);
            }

            _logger.LogInformation("Processed VnPay callback for order {OrderId}, success: {Success}", orderId, callback.Success);

            return callback;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing VnPay callback for transaction {TxnRef}", vnpayData.vnp_TxnRef);
            return new VnPayPaymentCallbackDto
            {
                Success = false,
                Message = "Internal server error",
                TransactionRef = vnpayData.vnp_TxnRef
            };
        }
    }

    public bool ValidateSignature(VnPayPaymentReturnDto vnpayData, string inputHash)
    {
        var all = new SortedDictionary<string, string>(StringComparer.Ordinal);
        foreach (var prop in typeof(VnPayPaymentReturnDto).GetProperties())
        {
            var value = prop.GetValue(vnpayData)?.ToString();
            if (value != null && prop.Name.StartsWith("vnp_", StringComparison.Ordinal)) all[prop.Name] = value;
        }
        
        _logger.LogDebug("[VNPAY][Validate] All params: {AllParams}", string.Join("&", all.Select(kv => $"{kv.Key}={kv.Value}")));
        
        var ok = VnPayLibrary.ValidateSignature(all, _vnPayConfig.HashSecret, out var rawData, out var expected, out var received);
        _logger.LogDebug("[VNPAY][Validate] TxnRef={TxnRef}, InputHash={Input}, Calculated={Calc}, Raw={Raw}, IsValid={IsValid}", vnpayData.vnp_TxnRef, received, expected, rawData, ok);
        return ok;
    }

    public async Task<string> ProcessIpnAsync(VnPayPaymentReturnDto vnpayData)
    {
        try
        {
            var isValidSignature = ValidateSignature(vnpayData, vnpayData.vnp_SecureHash);
            if (!isValidSignature)
            {
                _logger.LogWarning("Invalid VnPay IPN signature for transaction {TxnRef}", vnpayData.vnp_TxnRef);
                return "97"; // Invalid signature
            }

            var orderId = ExtractOrderIdFromTxnRef(vnpayData.vnp_TxnRef);
            var order = await _orderService.GetOrderByIdAsync(orderId);
            
            if (order == null)
            {
                _logger.LogWarning("Order not found for VnPay IPN transaction {TxnRef}", vnpayData.vnp_TxnRef);
                return "01"; // Order not found
            }

            var amount = decimal.Parse(vnpayData.vnp_Amount) / 100;
            if (amount != order.TotalAmount)
            {
                _logger.LogWarning("Amount mismatch for VnPay IPN transaction {TxnRef}. Expected: {Expected}, Actual: {Actual}", 
                    vnpayData.vnp_TxnRef, order.TotalAmount, amount);
                return "04"; // Invalid amount
            }

            // Check if payment already processed
            var existingPayment = await _paymentService.GetPaymentByOrderAsync(orderId);
            if (existingPayment != null && existingPayment.Status == PaymentStatusType.Completed)
            {
                _logger.LogInformation("Payment already processed for order {OrderId}", orderId);
                return "02"; // Order already confirmed
            }

            if (vnpayData.vnp_ResponseCode == "00")
            {
                await UpdatePaymentStatusAsync(orderId, vnpayData);
                _logger.LogInformation("Successfully processed VnPay IPN for order {OrderId}", orderId);
                return "00"; // Success
            }
            else
            {
                _logger.LogWarning("VnPay payment failed for order {OrderId}, response code: {ResponseCode}", 
                    orderId, vnpayData.vnp_ResponseCode);
                return "99"; // Unknown error
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing VnPay IPN for transaction {TxnRef}", vnpayData.vnp_TxnRef);
            return "99"; // Unknown error
        }
    }

    // Hash helpers moved to VnPayHelper

    private string GenerateTransactionReference(int orderId)
    {
        var timestamp = DateTime.Now.ToString("yyyyMMddHHmmss");
        return $"{orderId}_{timestamp}";
    }

    private int ExtractOrderIdFromTxnRef(string txnRef)
    {
        var parts = txnRef.Split('_');
        if (parts.Length > 0 && int.TryParse(parts[0], out var orderId))
        {
            return orderId;
        }
        throw new ArgumentException($"Invalid transaction reference format: {txnRef}");
    }

    private DateTime ParseVnPayDate(string vnPayDate)
    {
        if (DateTime.TryParseExact(vnPayDate, "yyyyMMddHHmmss", 
            CultureInfo.InvariantCulture, DateTimeStyles.None, out var result))
        {
            return result;
        }
        return DateTime.UtcNow;
    }

    private async Task UpdatePaymentStatusAsync(int orderId, VnPayPaymentReturnDto vnpayData)
    {
        try
        {
            var order = await _orderService.GetOrderWithDetailsAsync(orderId);
            if (order == null)
            {
                _logger.LogWarning("Order {OrderId} not found when updating payment status", orderId);
                return;
            }

            var existingPayment = await _paymentService.GetPaymentByOrderAsync(orderId);
            
            if (existingPayment != null)
            {
                // Update existing payment
                var updateDto = new DTOs.Payment.PaymentUpdateDto
                {
                    Status = PaymentStatusType.Completed,
                    Amount = decimal.Parse(vnpayData.vnp_Amount) / 100
                };
                await _paymentService.UpdatePaymentAsync(existingPayment.Id, updateDto);
            }
            else
            {
                // Create new payment record
                var createDto = new DTOs.Payment.PaymentCreateDto
                {
                    OrderId = orderId,
                    AccountId = order.CustomerId,
                    Amount = decimal.Parse(vnpayData.vnp_Amount) / 100,
                    Status = PaymentStatusType.Completed,
                    PaymentMethod = PaymentMethodType.VnPay,
                    TransactionId = vnpayData.vnp_TransactionNo ?? vnpayData.vnp_TxnRef,
                    PaymentDate = ParseVnPayDate(vnpayData.vnp_PayDate)
                };
                await _paymentService.CreatePaymentAsync(createDto);
            }

            // 1. Update order status to Paid
            await _orderService.UpdateOrderStatusAsync(orderId, OrderStatusType.Paid);
            
            // 2. Clear cart items for the customer
            await _cartService.ClearCartByCustomerAsync(order.CustomerId);

            _logger.LogInformation("Payment completed for order {OrderId}: updated payment status, set order to Paid, cleared cart for customer {CustomerId}", 
                orderId, order.CustomerId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating payment status for order {OrderId}", orderId);
            throw;
        }
    }

    private string GetResponseMessage(string responseCode)
    {
        return responseCode switch
        {
            "00" => "Giao dịch thành công",
            "07" => "Trừ tiền thành công. Giao dịch bị nghi ngờ (liên quan tới lừa đảo, giao dịch bất thường).",
            "09" => "Giao dịch không thành công do: Thẻ/Tài khoản của khách hàng chưa đăng ký dịch vụ InternetBanking tại ngân hàng.",
            "10" => "Giao dịch không thành công do: Khách hàng xác thực thông tin thẻ/tài khoản không đúng quá 3 lần",
            "11" => "Giao dịch không thành công do: Đã hết hạn chờ thanh toán. Xin quý khách vui lòng thực hiện lại giao dịch.",
            "12" => "Giao dịch không thành công do: Thẻ/Tài khoản của khách hàng bị khóa.",
            "13" => "Giao dịch không thành công do Quý khách nhập sai mật khẩu xác thực giao dịch (OTP). Xin quý khách vui lòng thực hiện lại giao dịch.",
            "24" => "Giao dịch không thành công do: Khách hàng hủy giao dịch",
            "51" => "Giao dịch không thành công do: Tài khoản của quý khách không đủ số dư để thực hiện giao dịch.",
            "65" => "Giao dịch không thành công do: Tài khoản của Quý khách đã vượt quá hạn mức giao dịch trong ngày.",
            "75" => "Ngân hàng thanh toán đang bảo trì.",
            "79" => "Giao dịch không thành công do: KH nhập sai mật khẩu thanh toán quá số lần quy định. Xin quý khách vui lòng thực hiện lại giao dịch",
            "99" => "Các lỗi khác (lỗi còn lại, không có trong danh sách mã lỗi đã liệt kê)",
            _ => "Lỗi không xác định"
        };
    }
}
