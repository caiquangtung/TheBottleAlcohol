using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Alcohol.DTOs.WishlistDetail;
using Alcohol.Models;
using Alcohol.Repositories.Interfaces;
using Alcohol.Services.Interfaces;
using AutoMapper;

namespace Alcohol.Services;

public class WishlistDetailService : IWishlistDetailService
{
    private readonly IWishlistDetailRepository _wishlistDetailRepository;
    private readonly IMapper _mapper;

    public WishlistDetailService(IWishlistDetailRepository wishlistDetailRepository, IMapper mapper)
    {
        _wishlistDetailRepository = wishlistDetailRepository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<WishlistDetailResponseDto>> GetAllWishlistDetailsAsync()
    {
        var wishlistDetails = await _wishlistDetailRepository.GetAllAsync();
        return _mapper.Map<IEnumerable<WishlistDetailResponseDto>>(wishlistDetails);
    }

    public async Task<WishlistDetailResponseDto> GetWishlistDetailByIdAsync(int id)
    {
        var wishlistDetail = await _wishlistDetailRepository.GetByIdAsync(id);
        if (wishlistDetail == null)
            return null;

        return _mapper.Map<WishlistDetailResponseDto>(wishlistDetail);
    }

    public async Task<IEnumerable<WishlistDetailResponseDto>> GetWishlistDetailsByWishlistAsync(int wishlistId)
    {
        var wishlistDetails = await _wishlistDetailRepository.GetByWishlistIdAsync(wishlistId);
        return _mapper.Map<IEnumerable<WishlistDetailResponseDto>>(wishlistDetails);
    }

    public async Task<WishlistDetailResponseDto> CreateWishlistDetailAsync(WishlistDetailCreateDto createDto)
    {
        var wishlistDetail = _mapper.Map<WishlistDetail>(createDto);
        wishlistDetail.CreatedAt = DateTime.UtcNow;

        await _wishlistDetailRepository.AddAsync(wishlistDetail);
        await _wishlistDetailRepository.SaveChangesAsync();

        return _mapper.Map<WishlistDetailResponseDto>(wishlistDetail);
    }

    public async Task<WishlistDetailResponseDto> UpdateWishlistDetailAsync(int id, WishlistDetailUpdateDto updateDto)
    {
        var wishlistDetail = await _wishlistDetailRepository.GetByIdAsync(id);
        if (wishlistDetail == null)
            return null;

        _mapper.Map(updateDto, wishlistDetail);
        wishlistDetail.UpdatedAt = DateTime.UtcNow;

        _wishlistDetailRepository.Update(wishlistDetail);
        await _wishlistDetailRepository.SaveChangesAsync();

        return _mapper.Map<WishlistDetailResponseDto>(wishlistDetail);
    }

    public async Task<bool> DeleteWishlistDetailAsync(int id)
    {
        var wishlistDetail = await _wishlistDetailRepository.GetByIdAsync(id);
        if (wishlistDetail == null)
            return false;

        _wishlistDetailRepository.Delete(wishlistDetail);
        await _wishlistDetailRepository.SaveChangesAsync();
        return true;
    }
} 