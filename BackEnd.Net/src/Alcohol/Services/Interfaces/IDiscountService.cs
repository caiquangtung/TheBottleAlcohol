using System.Collections.Generic;
using System.Threading.Tasks;
using Alcohol.DTOs.Discount;

namespace Alcohol.Services.Interfaces;

public interface IDiscountService
{
    Task<IEnumerable<DiscountResponseDto>> GetAllDiscountsAsync();
    Task<DiscountResponseDto> GetDiscountByIdAsync(int id);
    Task<DiscountResponseDto> GetDiscountByCodeAsync(string code);
    Task<IEnumerable<DiscountResponseDto>> GetActiveDiscountsAsync();
    Task<DiscountResponseDto> CreateDiscountAsync(DiscountCreateDto createDto);
    Task<DiscountResponseDto> UpdateDiscountAsync(int id, DiscountUpdateDto updateDto);
    Task<bool> ToggleDiscountStatusAsync(int id);
    Task<bool> DeleteDiscountAsync(int id);
} 