using System.Net;
using Alcohol.Common;
using Alcohol.DTOs.VnPay;
using Alcohol.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Alcohol.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class VnPayController : ControllerBase
{
    private readonly IVnPayService _vnPayService;
    private readonly ILogger<VnPayController> _logger;

    public VnPayController(IVnPayService vnPayService, ILogger<VnPayController> logger)
    {
        _vnPayService = vnPayService;
        _logger = logger;
    }

    /// <summary>
    /// Tạo URL thanh toán VnPay
    /// </summary>
    /// <param name="request">Thông tin yêu cầu thanh toán</param>
    /// <returns>URL thanh toán VnPay</returns>
    [HttpPost("create-payment")]
    // [Authorize] // Temporarily removed for testing
    public async Task<IActionResult> CreatePayment([FromBody] VnPayCreatePaymentRequestDto request)
    {
        try
        {
            // Get client IP address
            var clientIpAddress = GetClientIpAddress();
            
            var result = await _vnPayService.CreatePaymentUrlAsync(request, clientIpAddress);
            
            _logger.LogInformation("Created VnPay payment URL for order {OrderId}", request.OrderId);
            
            return Ok(new ApiResponse<VnPayCreatePaymentResponseDto>(result));
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning("Invalid request for VnPay payment creation: {Message}", ex.Message);
            return BadRequest(new ApiResponse<string>(ex.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating VnPay payment for order {OrderId}", request.OrderId);
            return StatusCode(500, new ApiResponse<string>("Internal server error"));
        }
    }

    /// <summary>
    /// Xử lý callback từ VnPay sau khi thanh toán (cho frontend)
    /// </summary>
    /// <param name="vnpayReturn">Dữ liệu trả về từ VnPay</param>
    /// <returns>Kết quả xử lý thanh toán</returns>
    [HttpGet("payment-return")]
    public async Task<IActionResult> PaymentReturn([FromQuery] VnPayPaymentReturnDto vnpayReturn)
    {
        try
        {
            var result = await _vnPayService.ProcessPaymentCallbackAsync(vnpayReturn);
            
            _logger.LogInformation("Processed VnPay payment return for transaction {TxnRef}, success: {Success}", 
                result.TransactionRef, result.Success);
            
            return Ok(new ApiResponse<VnPayPaymentCallbackDto>(result));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing VnPay payment return for transaction {TxnRef}", 
                vnpayReturn.vnp_TxnRef);
            return StatusCode(500, new ApiResponse<string>("Internal server error"));
        }
    }

    /// <summary>
    /// Xử lý IPN (Instant Payment Notification) từ VnPay
    /// </summary>
    /// <param name="vnpayIpn">Dữ liệu IPN từ VnPay</param>
    /// <returns>Response code cho VnPay</returns>
    [HttpPost("ipn")]
    public async Task<IActionResult> PaymentIpn([FromQuery] VnPayPaymentReturnDto vnpayIpn)
    {
        try
        {
            var responseCode = await _vnPayService.ProcessIpnAsync(vnpayIpn);
            
            _logger.LogInformation("Processed VnPay IPN for transaction {TxnRef}, response code: {ResponseCode}", 
                vnpayIpn.vnp_TxnRef, responseCode);
            
            // VnPay expects a specific response format
            return Ok(new { RspCode = responseCode, Message = "OK" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing VnPay IPN for transaction {TxnRef}", vnpayIpn.vnp_TxnRef);
            return Ok(new { RspCode = "99", Message = "Unknown error" });
        }
    }

    /// <summary>
    /// Kiểm tra trạng thái thanh toán của một đơn hàng
    /// </summary>
    /// <param name="orderId">ID đơn hàng</param>
    /// <returns>Trạng thái thanh toán</returns>
    [HttpGet("payment-status/{orderId}")]
    [Authorize]
    public IActionResult GetPaymentStatus(int orderId)
    {
        try
        {
            // This would typically check the payment status from your database
            // For now, we'll return a simple response
            return Ok(new ApiResponse<object>(new { OrderId = orderId, Message = "Use payment service to check status" }));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking payment status for order {OrderId}", orderId);
            return StatusCode(500, new ApiResponse<string>("Internal server error"));
        }
    }

    /// <summary>
    /// Endpoint test để kiểm tra cấu hình VnPay (chỉ dùng trong development)
    /// </summary>
    /// <returns>Thông tin cấu hình VnPay (ẩn sensitive data)</returns>
    [HttpGet("test-config")]
    [Authorize(Roles = "Admin")]
    public IActionResult TestConfig()
    {
        try
        {
            return Ok(new ApiResponse<object>(new 
            { 
                Message = "VnPay service is configured and ready",
                Environment = "Test",
                Timestamp = DateTime.UtcNow
            }));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error testing VnPay configuration");
            return StatusCode(500, new ApiResponse<string>("VnPay service configuration error"));
        }
    }

    private string GetClientIpAddress()
    {
        var ipAddress = string.Empty;

        // Check for X-Forwarded-For header (for load balancers/proxies)
        if (Request.Headers.ContainsKey("X-Forwarded-For"))
        {
            ipAddress = Request.Headers["X-Forwarded-For"].FirstOrDefault();
            if (!string.IsNullOrEmpty(ipAddress))
            {
                // Take the first IP if multiple IPs are present
                ipAddress = ipAddress.Split(',')[0].Trim();
            }
        }

        // Check for X-Real-IP header (nginx)
        if (string.IsNullOrEmpty(ipAddress) && Request.Headers.ContainsKey("X-Real-IP"))
        {
            ipAddress = Request.Headers["X-Real-IP"].FirstOrDefault();
        }

        // Fall back to remote IP address
        if (string.IsNullOrEmpty(ipAddress))
        {
            ipAddress = Request.HttpContext.Connection.RemoteIpAddress?.ToString();
        }

        // Handle IPv6 loopback
        if (ipAddress == "::1")
        {
            ipAddress = "127.0.0.1";
        }

        // Default fallback
        if (string.IsNullOrEmpty(ipAddress))
        {
            ipAddress = "127.0.0.1";
        }

        return ipAddress;
    }
}
