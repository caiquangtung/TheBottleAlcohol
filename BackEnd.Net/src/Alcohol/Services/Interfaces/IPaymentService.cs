using System.Collections.Generic;
using System.Threading.Tasks;
using Alcohol.DTOs.Payment;
using Alcohol.Models.Enums;

namespace Alcohol.Services.Interfaces;

public interface IPaymentService
{
    Task<IEnumerable<PaymentResponseDto>> GetAllPaymentsAsync();
    Task<PaymentResponseDto> GetPaymentByIdAsync(int id);
    Task<IEnumerable<PaymentResponseDto>> GetPaymentsByOrderAsync(int orderId);
    Task<IEnumerable<PaymentResponseDto>> GetPaymentsByCustomerAsync(int customerId);
    Task<PaymentResponseDto> CreatePaymentAsync(PaymentCreateDto createDto);
    Task<PaymentResponseDto> UpdatePaymentAsync(int id, PaymentUpdateDto updateDto);
    Task<bool> UpdatePaymentStatusAsync(int id, PaymentStatusType status);
    Task<bool> DeletePaymentAsync(int id);
} 