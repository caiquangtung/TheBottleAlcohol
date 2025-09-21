using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Alcohol.DTOs.Inventory;
using Alcohol.DTOs;

namespace Alcohol.Services.Interfaces;

public interface IInventoryService
{
    Task<PagedResult<InventoryResponseDto>> GetAllInventoriesAsync(InventoryFilterDto filter);
    Task<InventoryResponseDto> GetInventoryByIdAsync(int id);
    Task<InventoryResponseDto> GetInventoryByProductAsync(int productId);
    Task<InventoryResponseDto> CreateInventoryAsync(InventoryCreateDto createDto);
    Task<InventoryResponseDto> UpdateInventoryAsync(int id, InventoryUpdateDto updateDto);
    Task<bool> DeleteInventoryAsync(int id);
    
    // Stock management methods
    Task<bool> UpdateStockAsync(int id, int quantity);
    Task<bool> AddStockAsync(int id, int quantity);
    Task<bool> RemoveStockAsync(int id, int quantity);
    Task AdjustStockAsync(int productId, int quantity, string notes = null);
    Task ImportStockAsync(int productId, int quantity, int importOrderId, string notes = null);
    Task ImportStockWithCostAsync(int productId, int quantity, decimal importPrice, int importOrderId, string notes = null);
    Task ExportStockAsync(int productId, int quantity, int orderId, string notes = null);
    
    // Inventory analysis methods
    Task RecalculateAllTotalValuesAsync();
    Task<decimal> GetTotalInventoryValueAsync();
    Task<List<InventoryResponseDto>> GetLowStockItemsAsync();
}

