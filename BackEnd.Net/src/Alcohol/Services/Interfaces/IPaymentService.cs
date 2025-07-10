using System.Collections.Generic;
using System.Threading.Tasks;
using Alcohol.DTOs.Payment;
using Alcohol.DTOs;

namespace Alcohol.Services.Interfaces;

public interface IPaymentService
{
    Task<PagedResult<PaymentResponseDto>> GetAllPaymentsAsync(PaymentFilterDto filter);
    Task<PaymentResponseDto> GetPaymentByIdAsync(int id);
    Task<PaymentResponseDto> GetPaymentByOrderAsync(int orderId);
    Task<PaymentResponseDto> CreatePaymentAsync(PaymentCreateDto createDto);
    Task<PaymentResponseDto> UpdatePaymentAsync(int id, PaymentUpdateDto updateDto);
    Task<bool> DeletePaymentAsync(int id);
} 