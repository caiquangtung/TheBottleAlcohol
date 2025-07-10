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
}

