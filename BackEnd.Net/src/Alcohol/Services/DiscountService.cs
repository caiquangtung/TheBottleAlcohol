using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Alcohol.DTOs.Discount;
using Alcohol.Models;
using Alcohol.Repositories.Interfaces;
using Alcohol.Services.Interfaces;
using AutoMapper;
using System.Linq;
using Alcohol.DTOs;

namespace Alcohol.Services;

public class DiscountService : IDiscountService
{
    private readonly IDiscountRepository _discountRepository;
    private readonly IMapper _mapper;

    public DiscountService(IDiscountRepository discountRepository, IMapper mapper)
    {
        _discountRepository = discountRepository;
        _mapper = mapper;
    }

    public async Task<PagedResult<DiscountResponseDto>> GetAllDiscountsAsync(DiscountFilterDto filter)
    {
        var discounts = await _discountRepository.GetAllAsync();
        
        // Apply filters
        var filteredDiscounts = discounts.AsQueryable();
        
        if (!string.IsNullOrWhiteSpace(filter.SearchTerm))
        {
            filteredDiscounts = filteredDiscounts.Where(d => 
                d.Code.Contains(filter.SearchTerm));
        }
        
        if (filter.IsActive.HasValue)
        {
            filteredDiscounts = filteredDiscounts.Where(d => d.IsActive == filter.IsActive.Value);
        }
        
        if (filter.StartDate.HasValue)
        {
            filteredDiscounts = filteredDiscounts.Where(d => d.StartDate >= filter.StartDate.Value);
        }
        
        if (filter.EndDate.HasValue)
        {
            filteredDiscounts = filteredDiscounts.Where(d => d.EndDate <= filter.EndDate.Value);
        }
        
        if (filter.MinAmount.HasValue)
        {
            filteredDiscounts = filteredDiscounts.Where(d => d.DiscountAmount >= filter.MinAmount.Value);
        }
        
        if (filter.MaxAmount.HasValue)
        {
            filteredDiscounts = filteredDiscounts.Where(d => d.DiscountAmount <= filter.MaxAmount.Value);
        }
        
        // Apply sorting
        if (!string.IsNullOrWhiteSpace(filter.SortBy))
        {
            filteredDiscounts = filter.SortBy.ToLower() switch
            {
                "code" => filter.SortOrder?.ToLower() == "desc" 
                    ? filteredDiscounts.OrderByDescending(d => d.Code)
                    : filteredDiscounts.OrderBy(d => d.Code),
                "discountamount" => filter.SortOrder?.ToLower() == "desc"
                    ? filteredDiscounts.OrderByDescending(d => d.DiscountAmount)
                    : filteredDiscounts.OrderBy(d => d.DiscountAmount),
                "startdate" => filter.SortOrder?.ToLower() == "desc"
                    ? filteredDiscounts.OrderByDescending(d => d.StartDate)
                    : filteredDiscounts.OrderBy(d => d.StartDate),
                "enddate" => filter.SortOrder?.ToLower() == "desc"
                    ? filteredDiscounts.OrderByDescending(d => d.EndDate)
                    : filteredDiscounts.OrderBy(d => d.EndDate),
                _ => filteredDiscounts.OrderBy(d => d.Id)
            };
        }
        else
        {
            filteredDiscounts = filteredDiscounts.OrderBy(d => d.Id);
        }
        
        var totalRecords = filteredDiscounts.Count();
        var pagedDiscounts = filteredDiscounts
            .Skip((filter.PageNumber - 1) * filter.PageSize)
            .Take(filter.PageSize)
            .ToList();
        
        var discountDtos = _mapper.Map<List<DiscountResponseDto>>(pagedDiscounts);
        return new PagedResult<DiscountResponseDto>(discountDtos, totalRecords, filter.PageNumber, filter.PageSize);
    }

    public async Task<DiscountResponseDto> GetDiscountByIdAsync(int id)
    {
        var discount = await _discountRepository.GetByIdAsync(id);
        if (discount == null)
            return null;

        return _mapper.Map<DiscountResponseDto>(discount);
    }

    public async Task<DiscountResponseDto> GetDiscountByCodeAsync(string code)
    {
        var discount = await _discountRepository.GetByCodeAsync(code);
        if (discount == null)
            return null;

        return _mapper.Map<DiscountResponseDto>(discount);
    }

    public async Task<IEnumerable<DiscountResponseDto>> GetActiveDiscountsAsync()
    {
        var discounts = await _discountRepository.GetActiveDiscountsAsync();
        return _mapper.Map<IEnumerable<DiscountResponseDto>>(discounts);
    }

    public async Task<DiscountResponseDto> CreateDiscountAsync(DiscountCreateDto createDto)
    {
        var discount = _mapper.Map<Discount>(createDto);

        await _discountRepository.AddAsync(discount);
        await _discountRepository.SaveChangesAsync();

        return _mapper.Map<DiscountResponseDto>(discount);
    }

    public async Task<DiscountResponseDto> UpdateDiscountAsync(int id, DiscountUpdateDto updateDto)
    {
        var discount = await _discountRepository.GetByIdAsync(id);
        if (discount == null)
            return null;

        _mapper.Map(updateDto, discount);

        _discountRepository.Update(discount);
        await _discountRepository.SaveChangesAsync();

        return _mapper.Map<DiscountResponseDto>(discount);
    }

    public async Task<bool> ToggleDiscountStatusAsync(int id)
    {
        var discount = await _discountRepository.GetByIdAsync(id);
        if (discount == null)
            return false;

        discount.IsActive = !discount.IsActive;

        _discountRepository.Update(discount);
        await _discountRepository.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteDiscountAsync(int id)
    {
        var discount = await _discountRepository.GetByIdAsync(id);
        if (discount == null)
            return false;

        _discountRepository.Delete(discount);
        await _discountRepository.SaveChangesAsync();
        return true;
    }
} 