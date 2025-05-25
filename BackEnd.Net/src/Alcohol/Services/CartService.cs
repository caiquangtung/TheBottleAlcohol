using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Alcohol.DTOs.Cart;
using Alcohol.Models;
using Alcohol.Repositories.Interfaces;
using Alcohol.Services.Interfaces;
using AutoMapper;

namespace Alcohol.Services;

public class CartService : ICartService
{
    private readonly ICartRepository _cartRepository;
    private readonly IMapper _mapper;

    public CartService(ICartRepository cartRepository, IMapper mapper)
    {
        _cartRepository = cartRepository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<CartResponseDto>> GetAllCartsAsync()
    {
        var carts = await _cartRepository.GetAllAsync();
        return _mapper.Map<IEnumerable<CartResponseDto>>(carts);
    }

    public async Task<CartResponseDto> GetCartByIdAsync(int id)
    {
        var cart = await _cartRepository.GetByIdAsync(id);
        if (cart == null)
            return null;

        return _mapper.Map<CartResponseDto>(cart);
    }

    public async Task<IEnumerable<CartResponseDto>> GetCartsByCustomerAsync(int customerId)
    {
        var carts = await _cartRepository.GetByCustomerIdAsync(customerId);
        return _mapper.Map<IEnumerable<CartResponseDto>>(carts);
    }

    public async Task<CartResponseDto> CreateCartAsync(CartCreateDto createDto)
    {
        var cart = _mapper.Map<Cart>(createDto);
        cart.CreatedAt = DateTime.UtcNow;

        await _cartRepository.AddAsync(cart);
        await _cartRepository.SaveChangesAsync();

        return _mapper.Map<CartResponseDto>(cart);
    }

    public async Task<CartResponseDto> UpdateCartAsync(int id, CartUpdateDto updateDto)
    {
        var cart = await _cartRepository.GetByIdAsync(id);
        if (cart == null)
            return null;

        _mapper.Map(updateDto, cart);
        cart.UpdatedAt = DateTime.UtcNow;

        _cartRepository.Update(cart);
        await _cartRepository.SaveChangesAsync();

        return _mapper.Map<CartResponseDto>(cart);
    }

    public async Task<bool> DeleteCartAsync(int id)
    {
        var cart = await _cartRepository.GetByIdAsync(id);
        if (cart == null)
            return false;

        _cartRepository.Delete(cart);
        await _cartRepository.SaveChangesAsync();
        return true;
    }
} 