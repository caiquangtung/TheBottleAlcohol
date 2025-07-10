using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Alcohol.DTOs.Shipping;
using Alcohol.Models;
using Alcohol.Models.Enums;
using Alcohol.Repositories.Interfaces;
using Alcohol.Services.Interfaces;
using AutoMapper;
using Alcohol.DTOs;

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

    public async Task<PagedResult<ShippingResponseDto>> GetAllShippingsAsync(ShippingFilterDto filter)
    {
        var shippings = await _shippingRepository.GetAllAsync();
        
        // Apply filters
        var filteredShippings = shippings.AsQueryable();
        
        if (!string.IsNullOrWhiteSpace(filter.SearchTerm))
        {
            filteredShippings = filteredShippings.Where(s => 
                s.TrackingNumber.Contains(filter.SearchTerm) || 
                (s.Order != null && s.Order.OrderNumber.Contains(filter.SearchTerm)));
        }
        
        if (filter.OrderId.HasValue)
        {
            filteredShippings = filteredShippings.Where(s => s.OrderId == filter.OrderId.Value);
        }
        
        if (!string.IsNullOrWhiteSpace(filter.Status))
        {
            if (Enum.TryParse<ShippingStatusType>(filter.Status, out var status))
            {
                filteredShippings = filteredShippings.Where(s => s.Status == status);
            }
        }
        
        if (filter.StartDate.HasValue)
        {
            filteredShippings = filteredShippings.Where(s => s.ShippingDate >= filter.StartDate.Value);
        }
        
        if (filter.EndDate.HasValue)
        {
            filteredShippings = filteredShippings.Where(s => s.ShippingDate <= filter.EndDate.Value);
        }
        
        if (filter.MinCost.HasValue)
        {
            filteredShippings = filteredShippings.Where(s => s.ShippingCost >= filter.MinCost.Value);
        }
        
        if (filter.MaxCost.HasValue)
        {
            filteredShippings = filteredShippings.Where(s => s.ShippingCost <= filter.MaxCost.Value);
        }
        
        // Apply sorting
        if (!string.IsNullOrWhiteSpace(filter.SortBy))
        {
            filteredShippings = filter.SortBy.ToLower() switch
            {
                "trackingnumber" => filter.SortOrder?.ToLower() == "desc" 
                    ? filteredShippings.OrderByDescending(s => s.TrackingNumber)
                    : filteredShippings.OrderBy(s => s.TrackingNumber),
                "shippingcost" => filter.SortOrder?.ToLower() == "desc"
                    ? filteredShippings.OrderByDescending(s => s.ShippingCost)
                    : filteredShippings.OrderBy(s => s.ShippingCost),
                "shippingdate" => filter.SortOrder?.ToLower() == "desc"
                    ? filteredShippings.OrderByDescending(s => s.ShippingDate)
                    : filteredShippings.OrderBy(s => s.ShippingDate),
                "createdat" => filter.SortOrder?.ToLower() == "desc"
                    ? filteredShippings.OrderByDescending(s => s.CreatedAt)
                    : filteredShippings.OrderBy(s => s.CreatedAt),
                _ => filteredShippings.OrderBy(s => s.Id)
            };
        }
        else
        {
            filteredShippings = filteredShippings.OrderBy(s => s.Id);
        }
        
        var totalRecords = filteredShippings.Count();
        var pagedShippings = filteredShippings
            .Skip((filter.PageNumber - 1) * filter.PageSize)
            .Take(filter.PageSize)
            .ToList();
        
        var shippingDtos = _mapper.Map<List<ShippingResponseDto>>(pagedShippings);
        return new PagedResult<ShippingResponseDto>(shippingDtos, totalRecords, filter.PageNumber, filter.PageSize);
    }

    public async Task<ShippingResponseDto> GetShippingByIdAsync(int id)
    {
        var shipping = await _shippingRepository.GetByIdAsync(id);
        if (shipping == null)
            return null;

        return _mapper.Map<ShippingResponseDto>(shipping);
    }

    public async Task<ShippingResponseDto> GetShippingByOrderAsync(int orderId)
    {
        var shipping = await _shippingRepository.GetByOrderIdAsync(orderId);
        if (shipping == null)
            return null;
            
        return _mapper.Map<ShippingResponseDto>(shipping);
    }

    public async Task<ShippingResponseDto> CreateShippingAsync(ShippingCreateDto createDto)
    {
        var shipping = _mapper.Map<Shipping>(createDto);
        shipping.CreatedAt = DateTime.UtcNow;

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