using System.Collections.Generic;
using System.Threading.Tasks;
using Alcohol.DTOs.WishlistDetail;

namespace Alcohol.Services.Interfaces;

public interface IWishlistDetailService
{
    Task<IEnumerable<WishlistDetailResponseDto>> GetAllWishlistDetailsAsync();
    Task<WishlistDetailResponseDto> GetWishlistDetailByIdAsync(int id);
    Task<IEnumerable<WishlistDetailResponseDto>> GetWishlistDetailsByWishlistAsync(int wishlistId);
    Task<WishlistDetailResponseDto> CreateWishlistDetailAsync(WishlistDetailCreateDto createDto);
    Task<WishlistDetailResponseDto> UpdateWishlistDetailAsync(int id, WishlistDetailUpdateDto updateDto);
    Task<bool> DeleteWishlistDetailAsync(int id);
} 