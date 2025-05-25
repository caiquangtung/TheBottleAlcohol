using System.Collections.Generic;
using System.Threading.Tasks;
using Alcohol.DTOs.Cart;

namespace Alcohol.Services.Interfaces;

public interface ICartService
{
    Task<IEnumerable<CartResponseDto>> GetAllCartsAsync();
    Task<CartResponseDto> GetCartByIdAsync(int id);
    Task<IEnumerable<CartResponseDto>> GetCartsByCustomerAsync(int customerId);
    Task<CartResponseDto> CreateCartAsync(CartCreateDto createDto);
    Task<CartResponseDto> UpdateCartAsync(int id, CartUpdateDto updateDto);
    Task<bool> DeleteCartAsync(int id);
} 