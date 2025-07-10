using System.Collections.Generic;
using System.Threading.Tasks;
using Alcohol.DTOs.Shipping;
using Alcohol.DTOs;

namespace Alcohol.Services.Interfaces;

public interface IShippingService
{
    Task<PagedResult<ShippingResponseDto>> GetAllShippingsAsync(ShippingFilterDto filter);
    Task<ShippingResponseDto> GetShippingByIdAsync(int id);
    Task<ShippingResponseDto> GetShippingByOrderAsync(int orderId);
    Task<ShippingResponseDto> CreateShippingAsync(ShippingCreateDto createDto);
    Task<ShippingResponseDto> UpdateShippingAsync(int id, ShippingUpdateDto updateDto);
    Task<bool> DeleteShippingAsync(int id);
} 