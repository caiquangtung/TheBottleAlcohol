using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Alcohol.DTOs.Payment;
using Alcohol.Models;
using Alcohol.Models.Enums;
using Alcohol.Repositories.Interfaces;
using Alcohol.Services.Interfaces;
using AutoMapper;
using System.Linq;
using Alcohol.DTOs;

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

    public async Task<PagedResult<PaymentResponseDto>> GetAllPaymentsAsync(PaymentFilterDto filter)
    {
        var payments = await _paymentRepository.GetAllAsync();
        
        // Apply filters
        var filteredPayments = payments.AsQueryable();
        
        if (!string.IsNullOrWhiteSpace(filter.SearchTerm))
        {
            filteredPayments = filteredPayments.Where(p => 
                p.TransactionId.Contains(filter.SearchTerm) || 
                p.PaymentMethod.ToString().Contains(filter.SearchTerm));
        }
        
        if (filter.OrderId.HasValue)
        {
            filteredPayments = filteredPayments.Where(p => p.OrderId == filter.OrderId.Value);
        }
        
        if (!string.IsNullOrWhiteSpace(filter.PaymentMethod))
        {
            if (Enum.TryParse<PaymentMethodType>(filter.PaymentMethod, out var paymentMethod))
            {
                filteredPayments = filteredPayments.Where(p => p.PaymentMethod == paymentMethod);
            }
        }
        
        if (!string.IsNullOrWhiteSpace(filter.Status))
        {
            if (Enum.TryParse<PaymentStatusType>(filter.Status, out var status))
            {
                filteredPayments = filteredPayments.Where(p => p.Status == status);
            }
        }
        
        if (filter.StartDate.HasValue)
        {
            filteredPayments = filteredPayments.Where(p => p.PaymentDate >= filter.StartDate.Value);
        }
        
        if (filter.EndDate.HasValue)
        {
            filteredPayments = filteredPayments.Where(p => p.PaymentDate <= filter.EndDate.Value);
        }
        
        if (filter.MinAmount.HasValue)
        {
            filteredPayments = filteredPayments.Where(p => p.Amount >= filter.MinAmount.Value);
        }
        
        if (filter.MaxAmount.HasValue)
        {
            filteredPayments = filteredPayments.Where(p => p.Amount <= filter.MaxAmount.Value);
        }
        
        // Apply sorting
        if (!string.IsNullOrWhiteSpace(filter.SortBy))
        {
            filteredPayments = filter.SortBy.ToLower() switch
            {
                "transactionid" => filter.SortOrder?.ToLower() == "desc" 
                    ? filteredPayments.OrderByDescending(p => p.TransactionId)
                    : filteredPayments.OrderBy(p => p.TransactionId),
                "amount" => filter.SortOrder?.ToLower() == "desc"
                    ? filteredPayments.OrderByDescending(p => p.Amount)
                    : filteredPayments.OrderBy(p => p.Amount),
                "paymentdate" => filter.SortOrder?.ToLower() == "desc"
                    ? filteredPayments.OrderByDescending(p => p.PaymentDate)
                    : filteredPayments.OrderBy(p => p.PaymentDate),
                "createdat" => filter.SortOrder?.ToLower() == "desc"
                    ? filteredPayments.OrderByDescending(p => p.CreatedAt)
                    : filteredPayments.OrderBy(p => p.CreatedAt),
                _ => filteredPayments.OrderBy(p => p.Id)
            };
        }
        else
        {
            filteredPayments = filteredPayments.OrderBy(p => p.Id);
        }
        
        var totalRecords = filteredPayments.Count();
        var pagedPayments = filteredPayments
            .Skip((filter.PageNumber - 1) * filter.PageSize)
            .Take(filter.PageSize)
            .ToList();
        
        var paymentDtos = _mapper.Map<List<PaymentResponseDto>>(pagedPayments);
        return new PagedResult<PaymentResponseDto>(paymentDtos, totalRecords, filter.PageNumber, filter.PageSize);
    }

    public async Task<PaymentResponseDto> GetPaymentByIdAsync(int id)
    {
        var payment = await _paymentRepository.GetByIdAsync(id);
        if (payment == null)
            return null;

        return _mapper.Map<PaymentResponseDto>(payment);
    }

    public async Task<PaymentResponseDto> GetPaymentByOrderAsync(int orderId)
    {
        var payment = await _paymentRepository.GetByOrderIdAsync(orderId);
        if (payment == null)
            return null;

        return _mapper.Map<PaymentResponseDto>(payment);
    }

    public async Task<PaymentResponseDto> CreatePaymentAsync(PaymentCreateDto createDto)
    {
        var payment = _mapper.Map<Payment>(createDto);
        payment.CreatedAt = DateTime.UtcNow;

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