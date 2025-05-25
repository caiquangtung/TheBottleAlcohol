using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Alcohol.DTOs.Wishlist;
using Alcohol.Models;
using Alcohol.Repositories.Interfaces;
using Alcohol.Services.Interfaces;
using AutoMapper;

namespace Alcohol.Services;

public class WishlistService : IWishlistService
{
    private readonly IWishlistRepository _wishlistRepository;
    private readonly IMapper _mapper;

    public WishlistService(IWishlistRepository wishlistRepository, IMapper mapper)
    {
        _wishlistRepository = wishlistRepository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<WishlistResponseDto>> GetAllWishlistsAsync()
    {
        var wishlists = await _wishlistRepository.GetAllAsync();
        return _mapper.Map<IEnumerable<WishlistResponseDto>>(wishlists);
    }

    public async Task<WishlistResponseDto> GetWishlistByIdAsync(int id)
    {
        var wishlist = await _wishlistRepository.GetByIdAsync(id);
        if (wishlist == null)
            return null;

        return _mapper.Map<WishlistResponseDto>(wishlist);
    }

    public async Task<IEnumerable<WishlistResponseDto>> GetWishlistsByCustomerAsync(int customerId)
    {
        var wishlists = await _wishlistRepository.GetByCustomerIdAsync(customerId);
        return _mapper.Map<IEnumerable<WishlistResponseDto>>(wishlists);
    }

    public async Task<WishlistResponseDto> CreateWishlistAsync(WishlistCreateDto createDto)
    {
        var wishlist = _mapper.Map<Wishlist>(createDto);

        await _wishlistRepository.AddAsync(wishlist);
        await _wishlistRepository.SaveChangesAsync();

        return _mapper.Map<WishlistResponseDto>(wishlist);
    }

    public async Task<WishlistResponseDto> UpdateWishlistAsync(int id, WishlistUpdateDto updateDto)
    {
        var wishlist = await _wishlistRepository.GetByIdAsync(id);
        if (wishlist == null)
            return null;

        _mapper.Map(updateDto, wishlist);

        _wishlistRepository.Update(wishlist);
        await _wishlistRepository.SaveChangesAsync();

        return _mapper.Map<WishlistResponseDto>(wishlist);
    }

    public async Task<bool> DeleteWishlistAsync(int id)
    {
        var wishlist = await _wishlistRepository.GetByIdAsync(id);
        if (wishlist == null)
            return false;

        _wishlistRepository.Delete(wishlist);
        await _wishlistRepository.SaveChangesAsync();
        return true;
    }
} 