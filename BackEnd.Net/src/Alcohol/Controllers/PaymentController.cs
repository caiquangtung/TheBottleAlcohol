using System;
using System.Threading.Tasks;
using Alcohol.DTOs.Payment;
using Alcohol.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Alcohol.Common;
using Alcohol.Models.Enums;

namespace Alcohol.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentService _paymentService;

        public PaymentController(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        [HttpGet]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> GetAll()
        {
            var payments = await _paymentService.GetAllPaymentsAsync();
            return Ok(new ApiResponse<IEnumerable<PaymentResponseDto>>(payments));
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetById(int id)
        {
            var payment = await _paymentService.GetPaymentByIdAsync(id);
            if (payment == null)
                return NotFound(new ApiResponse<string>("Payment not found"));
            return Ok(new ApiResponse<PaymentResponseDto>(payment));
        }

        [HttpGet("order/{orderId}")]
        [Authorize]
        public async Task<IActionResult> GetByOrder(int orderId)
        {
            var payments = await _paymentService.GetPaymentsByOrderAsync(orderId);
            return Ok(new ApiResponse<IEnumerable<PaymentResponseDto>>(payments));
        }

        [HttpGet("customer/{customerId}")]
        [Authorize]
        public async Task<IActionResult> GetByCustomer(int customerId)
        {
            var payments = await _paymentService.GetPaymentsByCustomerAsync(customerId);
            return Ok(new ApiResponse<IEnumerable<PaymentResponseDto>>(payments));
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create(PaymentCreateDto createDto)
        {
            var payment = await _paymentService.CreatePaymentAsync(createDto);
            return CreatedAtAction(nameof(GetById), new { id = payment.Id }, new ApiResponse<PaymentResponseDto>(payment));
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> Update(int id, PaymentUpdateDto updateDto)
        {
            var payment = await _paymentService.UpdatePaymentAsync(id, updateDto);
            if (payment == null)
                return NotFound(new ApiResponse<string>("Payment not found"));
            return Ok(new ApiResponse<PaymentResponseDto>(payment));
        }

        [HttpPut("{id}/status")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> UpdateStatus(int id, PaymentStatusType status)
        {
            var result = await _paymentService.UpdatePaymentStatusAsync(id, status);
            if (!result)
                return NotFound(new ApiResponse<string>("Payment not found"));
            return Ok(new ApiResponse<string>("Payment status updated successfully"));
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _paymentService.DeletePaymentAsync(id);
            if (!result)
                return NotFound(new ApiResponse<string>("Payment not found"));
            return Ok(new ApiResponse<string>("Payment deleted successfully"));
        }
    }
} 