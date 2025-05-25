using System.Collections.Generic;
using System.Threading.Tasks;
using Alcohol.DTOs.Wishlist;

namespace Alcohol.Services.Interfaces;

public interface IWishlistService
{
    Task<IEnumerable<WishlistResponseDto>> GetAllWishlistsAsync();
    Task<WishlistResponseDto> GetWishlistByIdAsync(int id);
    Task<IEnumerable<WishlistResponseDto>> GetWishlistsByCustomerAsync(int customerId);
    Task<WishlistResponseDto> CreateWishlistAsync(WishlistCreateDto createDto);
    Task<WishlistResponseDto> UpdateWishlistAsync(int id, WishlistUpdateDto updateDto);
    Task<bool> DeleteWishlistAsync(int id);
} 