using System.Collections.Generic;
using System.Threading.Tasks;
using Alcohol.DTOs.CartDetail;

namespace Alcohol.Services.Interfaces;

public interface ICartDetailService
{
    Task<IEnumerable<CartDetailResponseDto>> GetAllCartDetailsAsync();
    Task<CartDetailResponseDto> GetCartDetailByIdAsync(int id);
    Task<IEnumerable<CartDetailResponseDto>> GetCartDetailsByCartAsync(int cartId);
    Task<CartDetailResponseDto> CreateCartDetailAsync(CartDetailCreateDto createDto);
    Task<CartDetailResponseDto> UpdateCartDetailAsync(int id, CartDetailUpdateDto updateDto);
    Task<bool> DeleteCartDetailAsync(int id);
} 