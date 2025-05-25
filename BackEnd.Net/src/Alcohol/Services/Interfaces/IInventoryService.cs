using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Alcohol.DTOs.Inventory;

namespace Alcohol.Services.Interfaces;

public interface IInventoryService
{
    Task<IEnumerable<InventoryResponseDto>> GetAllInventoriesAsync();
    Task<InventoryResponseDto> GetInventoryByIdAsync(int id);
    Task<InventoryResponseDto> GetInventoryByProductAsync(int productId);
    Task<IEnumerable<InventoryResponseDto>> GetLowStockInventoriesAsync(int threshold);
    Task<InventoryResponseDto> CreateInventoryAsync(InventoryCreateDto createDto);
    Task<InventoryResponseDto> UpdateInventoryAsync(int id, InventoryUpdateDto updateDto);
    Task<bool> UpdateStockAsync(int id, int quantity);
    Task<bool> AddStockAsync(int id, int quantity);
    Task<bool> RemoveStockAsync(int id, int quantity);
    Task<bool> DeleteInventoryAsync(int id);
}

