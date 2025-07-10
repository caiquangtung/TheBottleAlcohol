using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Alcohol.DTOs.Wishlist;
using Alcohol.Models;
using Alcohol.Repositories.Interfaces;
using Alcohol.Services.Interfaces;
using AutoMapper;
using Alcohol.DTOs;

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

    public async Task<PagedResult<WishlistResponseDto>> GetAllWishlistsAsync(WishlistFilterDto filter)
    {
        var wishlists = await _wishlistRepository.GetAllAsync();
        
        // Apply filters
        var filteredWishlists = wishlists.AsQueryable();
        
        if (!string.IsNullOrWhiteSpace(filter.SearchTerm))
        {
            filteredWishlists = filteredWishlists.Where(w => 
                w.Customer != null && w.Customer.FullName.Contains(filter.SearchTerm));
        }
        
        if (filter.CustomerId.HasValue)
        {
            filteredWishlists = filteredWishlists.Where(w => w.CustomerId == filter.CustomerId.Value);
        }
        
        if (filter.StartDate.HasValue)
        {
            filteredWishlists = filteredWishlists.Where(w => w.CreatedAt >= filter.StartDate.Value);
        }
        
        if (filter.EndDate.HasValue)
        {
            filteredWishlists = filteredWishlists.Where(w => w.CreatedAt <= filter.EndDate.Value);
        }
        
        // Apply sorting
        if (!string.IsNullOrWhiteSpace(filter.SortBy))
        {
            filteredWishlists = filter.SortBy.ToLower() switch
            {
                "createdat" => filter.SortOrder?.ToLower() == "desc" 
                    ? filteredWishlists.OrderByDescending(w => w.CreatedAt)
                    : filteredWishlists.OrderBy(w => w.CreatedAt),
                "updatedat" => filter.SortOrder?.ToLower() == "desc"
                    ? filteredWishlists.OrderByDescending(w => w.UpdatedAt)
                    : filteredWishlists.OrderBy(w => w.UpdatedAt),
                _ => filteredWishlists.OrderBy(w => w.Id)
            };
        }
        else
        {
            filteredWishlists = filteredWishlists.OrderBy(w => w.Id);
        }
        
        var totalRecords = filteredWishlists.Count();
        var pagedWishlists = filteredWishlists
            .Skip((filter.PageNumber - 1) * filter.PageSize)
            .Take(filter.PageSize)
            .ToList();
        
        var wishlistDtos = _mapper.Map<List<WishlistResponseDto>>(pagedWishlists);
        return new PagedResult<WishlistResponseDto>(wishlistDtos, totalRecords, filter.PageNumber, filter.PageSize);
    }

    public async Task<WishlistResponseDto> GetWishlistByIdAsync(int id)
    {
        var wishlist = await _wishlistRepository.GetByIdAsync(id);
        if (wishlist == null)
            return null;

        return _mapper.Map<WishlistResponseDto>(wishlist);
    }

    public async Task<WishlistResponseDto> GetWishlistByCustomerAsync(int customerId)
    {
        var wishlists = await _wishlistRepository.GetByCustomerIdAsync(customerId);
        var wishlist = wishlists.FirstOrDefault();
        if (wishlist == null)
            return null;
            
        return _mapper.Map<WishlistResponseDto>(wishlist);
    }

    public async Task<WishlistResponseDto> CreateWishlistAsync(WishlistCreateDto createDto)
    {
        var wishlist = _mapper.Map<Wishlist>(createDto);
        wishlist.CreatedAt = DateTime.UtcNow;

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
        wishlist.UpdatedAt = DateTime.UtcNow;

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