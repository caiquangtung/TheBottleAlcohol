using System.Collections.Generic;
using System.Threading.Tasks;
using Alcohol.DTOs.Shipping;
using Alcohol.Models.Enums;

namespace Alcohol.Services.Interfaces;

public interface IShippingService
{
    Task<IEnumerable<ShippingResponseDto>> GetAllShippingsAsync();
    Task<ShippingResponseDto> GetShippingByIdAsync(int id);
    Task<IEnumerable<ShippingResponseDto>> GetShippingsByOrderAsync(int orderId);
    Task<IEnumerable<ShippingResponseDto>> GetShippingsByCustomerAsync(int customerId);
    Task<ShippingResponseDto> CreateShippingAsync(ShippingCreateDto createDto);
    Task<ShippingResponseDto> UpdateShippingAsync(int id, ShippingUpdateDto updateDto);
    Task<bool> UpdateShippingStatusAsync(int id, ShippingStatusType status);
    Task<bool> DeleteShippingAsync(int id);
} 