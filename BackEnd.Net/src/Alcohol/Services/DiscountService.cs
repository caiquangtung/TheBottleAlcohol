using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Alcohol.DTOs.Discount;
using Alcohol.Models;
using Alcohol.Repositories.Interfaces;
using Alcohol.Services.Interfaces;
using AutoMapper;

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

    public async Task<IEnumerable<DiscountResponseDto>> GetAllDiscountsAsync()
    {
        var discounts = await _discountRepository.GetAllAsync();
        return _mapper.Map<IEnumerable<DiscountResponseDto>>(discounts);
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
        discount.IsActive = true;

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