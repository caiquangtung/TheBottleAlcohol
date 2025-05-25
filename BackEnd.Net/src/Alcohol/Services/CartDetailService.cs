using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Alcohol.DTOs.CartDetail;
using Alcohol.Models;
using Alcohol.Repositories.Interfaces;
using Alcohol.Services.Interfaces;
using AutoMapper;

namespace Alcohol.Services;

public class CartDetailService : ICartDetailService
{
    private readonly ICartDetailRepository _cartDetailRepository;
    private readonly IMapper _mapper;

    public CartDetailService(ICartDetailRepository cartDetailRepository, IMapper mapper)
    {
        _cartDetailRepository = cartDetailRepository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<CartDetailResponseDto>> GetAllCartDetailsAsync()
    {
        var cartDetails = await _cartDetailRepository.GetAllAsync();
        return _mapper.Map<IEnumerable<CartDetailResponseDto>>(cartDetails);
    }

    public async Task<CartDetailResponseDto> GetCartDetailByIdAsync(int id)
    {
        var cartDetail = await _cartDetailRepository.GetByIdAsync(id);
        if (cartDetail == null)
            return null;

        return _mapper.Map<CartDetailResponseDto>(cartDetail);
    }

    public async Task<IEnumerable<CartDetailResponseDto>> GetCartDetailsByCartAsync(int cartId)
    {
        var cartDetails = await _cartDetailRepository.GetByCartIdAsync(cartId);
        return _mapper.Map<IEnumerable<CartDetailResponseDto>>(cartDetails);
    }

    public async Task<CartDetailResponseDto> CreateCartDetailAsync(CartDetailCreateDto createDto)
    {
        var cartDetail = _mapper.Map<CartDetail>(createDto);
        cartDetail.CreatedAt = DateTime.UtcNow;

        await _cartDetailRepository.AddAsync(cartDetail);
        await _cartDetailRepository.SaveChangesAsync();

        return _mapper.Map<CartDetailResponseDto>(cartDetail);
    }

    public async Task<CartDetailResponseDto> UpdateCartDetailAsync(int id, CartDetailUpdateDto updateDto)
    {
        var cartDetail = await _cartDetailRepository.GetByIdAsync(id);
        if (cartDetail == null)
            return null;

        _mapper.Map(updateDto, cartDetail);
        cartDetail.UpdatedAt = DateTime.UtcNow;

        _cartDetailRepository.Update(cartDetail);
        await _cartDetailRepository.SaveChangesAsync();

        return _mapper.Map<CartDetailResponseDto>(cartDetail);
    }

    public async Task<bool> DeleteCartDetailAsync(int id)
    {
        var cartDetail = await _cartDetailRepository.GetByIdAsync(id);
        if (cartDetail == null)
            return false;

        _cartDetailRepository.Delete(cartDetail);
        await _cartDetailRepository.SaveChangesAsync();
        return true;
    }
} 