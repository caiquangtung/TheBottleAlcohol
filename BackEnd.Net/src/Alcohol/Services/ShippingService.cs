using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Alcohol.DTOs.Shipping;
using Alcohol.Models;
using Alcohol.Models.Enums;
using Alcohol.Repositories.Interfaces;
using Alcohol.Services.Interfaces;
using AutoMapper;

namespace Alcohol.Services;

public class ShippingService : IShippingService
{
    private readonly IShippingRepository _shippingRepository;
    private readonly IMapper _mapper;

    public ShippingService(IShippingRepository shippingRepository, IMapper mapper)
    {
        _shippingRepository = shippingRepository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<ShippingResponseDto>> GetAllShippingsAsync()
    {
        var shippings = await _shippingRepository.GetAllAsync();
        return _mapper.Map<IEnumerable<ShippingResponseDto>>(shippings);
    }

    public async Task<ShippingResponseDto> GetShippingByIdAsync(int id)
    {
        var shipping = await _shippingRepository.GetByIdAsync(id);
        if (shipping == null)
            return null;

        return _mapper.Map<ShippingResponseDto>(shipping);
    }

    public async Task<IEnumerable<ShippingResponseDto>> GetShippingsByOrderAsync(int orderId)
    {
        var shippings = await _shippingRepository.GetByOrderIdAsync(orderId);
        return _mapper.Map<IEnumerable<ShippingResponseDto>>(shippings);
    }

    public async Task<IEnumerable<ShippingResponseDto>> GetShippingsByCustomerAsync(int customerId)
    {
        var shippings = await _shippingRepository.GetByCustomerIdAsync(customerId);
        return _mapper.Map<IEnumerable<ShippingResponseDto>>(shippings);
    }

    public async Task<ShippingResponseDto> CreateShippingAsync(ShippingCreateDto createDto)
    {
        var shipping = _mapper.Map<Shipping>(createDto);
        shipping.CreatedAt = DateTime.UtcNow;
        shipping.Status = ShippingStatusType.Pending;

        await _shippingRepository.AddAsync(shipping);
        await _shippingRepository.SaveChangesAsync();

        return _mapper.Map<ShippingResponseDto>(shipping);
    }

    public async Task<ShippingResponseDto> UpdateShippingAsync(int id, ShippingUpdateDto updateDto)
    {
        var shipping = await _shippingRepository.GetByIdAsync(id);
        if (shipping == null)
            return null;

        _mapper.Map(updateDto, shipping);
        shipping.UpdatedAt = DateTime.UtcNow;

        _shippingRepository.Update(shipping);
        await _shippingRepository.SaveChangesAsync();

        return _mapper.Map<ShippingResponseDto>(shipping);
    }

    public async Task<bool> UpdateShippingStatusAsync(int id, ShippingStatusType status)
    {
        var shipping = await _shippingRepository.GetByIdAsync(id);
        if (shipping == null)
            return false;

        shipping.Status = status;
        shipping.UpdatedAt = DateTime.UtcNow;

        _shippingRepository.Update(shipping);
        await _shippingRepository.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteShippingAsync(int id)
    {
        var shipping = await _shippingRepository.GetByIdAsync(id);
        if (shipping == null)
            return false;

        _shippingRepository.Delete(shipping);
        await _shippingRepository.SaveChangesAsync();
        return true;
    }
} 