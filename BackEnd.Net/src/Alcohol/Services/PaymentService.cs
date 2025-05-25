using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Alcohol.DTOs.Payment;
using Alcohol.Models;
using Alcohol.Models.Enums;
using Alcohol.Repositories.Interfaces;
using Alcohol.Services.Interfaces;
using AutoMapper;

namespace Alcohol.Services;

public class PaymentService : IPaymentService
{
    private readonly IPaymentRepository _paymentRepository;
    private readonly IMapper _mapper;

    public PaymentService(IPaymentRepository paymentRepository, IMapper mapper)
    {
        _paymentRepository = paymentRepository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<PaymentResponseDto>> GetAllPaymentsAsync()
    {
        var payments = await _paymentRepository.GetAllAsync();
        return _mapper.Map<IEnumerable<PaymentResponseDto>>(payments);
    }

    public async Task<PaymentResponseDto> GetPaymentByIdAsync(int id)
    {
        var payment = await _paymentRepository.GetByIdAsync(id);
        if (payment == null)
            return null;

        return _mapper.Map<PaymentResponseDto>(payment);
    }

    public async Task<IEnumerable<PaymentResponseDto>> GetPaymentsByOrderAsync(int orderId)
    {
        var payments = await _paymentRepository.GetByOrderIdAsync(orderId);
        return _mapper.Map<IEnumerable<PaymentResponseDto>>(payments);
    }

    public async Task<IEnumerable<PaymentResponseDto>> GetPaymentsByCustomerAsync(int customerId)
    {
        var payments = await _paymentRepository.GetByCustomerIdAsync(customerId);
        return _mapper.Map<IEnumerable<PaymentResponseDto>>(payments);
    }

    public async Task<PaymentResponseDto> CreatePaymentAsync(PaymentCreateDto createDto)
    {
        var payment = _mapper.Map<Payment>(createDto);
        payment.CreatedAt = DateTime.UtcNow;
        payment.Status = PaymentStatusType.Pending;

        await _paymentRepository.AddAsync(payment);
        await _paymentRepository.SaveChangesAsync();

        return _mapper.Map<PaymentResponseDto>(payment);
    }

    public async Task<PaymentResponseDto> UpdatePaymentAsync(int id, PaymentUpdateDto updateDto)
    {
        var payment = await _paymentRepository.GetByIdAsync(id);
        if (payment == null)
            return null;

        _mapper.Map(updateDto, payment);
        payment.UpdatedAt = DateTime.UtcNow;

        _paymentRepository.Update(payment);
        await _paymentRepository.SaveChangesAsync();

        return _mapper.Map<PaymentResponseDto>(payment);
    }

    public async Task<bool> UpdatePaymentStatusAsync(int id, PaymentStatusType status)
    {
        var payment = await _paymentRepository.GetByIdAsync(id);
        if (payment == null)
            return false;

        payment.Status = status;
        payment.UpdatedAt = DateTime.UtcNow;

        _paymentRepository.Update(payment);
        await _paymentRepository.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeletePaymentAsync(int id)
    {
        var payment = await _paymentRepository.GetByIdAsync(id);
        if (payment == null)
            return false;

        _paymentRepository.Delete(payment);
        await _paymentRepository.SaveChangesAsync();
        return true;
    }
} 