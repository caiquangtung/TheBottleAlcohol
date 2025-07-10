using System;
using System.Threading.Tasks;
using Alcohol.DTOs.Payment;
using Alcohol.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Alcohol.Common;
using Alcohol.DTOs;

namespace Alcohol.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class PaymentController : ControllerBase
{
    private readonly IPaymentService _paymentService;

    public PaymentController(IPaymentService paymentService)
    {
        _paymentService = paymentService;
    }

    [HttpGet]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> GetAllPayments([FromQuery] PaymentFilterDto filter)
    {
        var result = await _paymentService.GetAllPaymentsAsync(filter);
        return Ok(new ApiResponse<PagedResult<PaymentResponseDto>>(result));
    }

    [HttpGet("{id}")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> GetPaymentById(int id)
    {
        var payment = await _paymentService.GetPaymentByIdAsync(id);
        if (payment == null)
            return NotFound(new ApiResponse<string>("Payment not found"));
        return Ok(new ApiResponse<PaymentResponseDto>(payment));
    }

    [HttpGet("order/{orderId}")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> GetPaymentByOrder(int orderId)
    {
        var payment = await _paymentService.GetPaymentByOrderAsync(orderId);
        if (payment == null)
            return NotFound(new ApiResponse<string>("Payment not found"));
        return Ok(new ApiResponse<PaymentResponseDto>(payment));
    }

    [HttpPost]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> CreatePayment(PaymentCreateDto createDto)
    {
        try
        {
            var payment = await _paymentService.CreatePaymentAsync(createDto);
            return CreatedAtAction(nameof(GetPaymentById), new { id = payment.Id }, new ApiResponse<PaymentResponseDto>(payment));
        }
        catch (Exception ex)
        {
            return BadRequest(new ApiResponse<string>(ex.Message));
        }
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> UpdatePayment(int id, PaymentUpdateDto updateDto)
    {
        var payment = await _paymentService.UpdatePaymentAsync(id, updateDto);
        if (payment == null)
            return NotFound(new ApiResponse<string>("Payment not found"));
        return Ok(new ApiResponse<PaymentResponseDto>(payment));
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> DeletePayment(int id)
    {
        var result = await _paymentService.DeletePaymentAsync(id);
        if (!result)
            return NotFound(new ApiResponse<string>("Payment not found"));
        return Ok(new ApiResponse<string>("Payment deleted successfully"));
    }
} 