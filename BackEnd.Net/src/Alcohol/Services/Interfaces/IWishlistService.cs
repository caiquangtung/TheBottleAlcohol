using System.Collections.Generic;
using System.Threading.Tasks;
using Alcohol.DTOs.Wishlist;
using Alcohol.DTOs;

namespace Alcohol.Services.Interfaces;

public interface IWishlistService
{
    Task<PagedResult<WishlistResponseDto>> GetAllWishlistsAsync(WishlistFilterDto filter);
    Task<WishlistResponseDto> GetWishlistByIdAsync(int id);
    Task<WishlistResponseDto> GetWishlistByCustomerAsync(int customerId);
    Task<WishlistResponseDto> GetOrCreateWishlistForUserAsync(int userId);
    Task<WishlistResponseDto> CreateWishlistAsync(WishlistCreateDto createDto);
    Task<WishlistResponseDto> UpdateWishlistAsync(int id, WishlistUpdateDto updateDto);
    Task<bool> DeleteWishlistAsync(int id);
} 