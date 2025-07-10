using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Alcohol.DTOs.InventoryTransaction;
using Alcohol.DTOs;

namespace Alcohol.Services.Interfaces;

public interface IInventoryTransactionService
{
    Task<PagedResult<InventoryTransactionResponseDto>> GetAllInventoryTransactionsAsync(InventoryTransactionFilterDto filter);
    Task<InventoryTransactionResponseDto> GetInventoryTransactionByIdAsync(int id);
    Task<IEnumerable<InventoryTransactionResponseDto>> GetInventoryTransactionsByProductAsync(int productId);
    Task<InventoryTransactionResponseDto> CreateInventoryTransactionAsync(InventoryTransactionCreateDto createDto);
    Task<InventoryTransactionResponseDto> UpdateInventoryTransactionAsync(int id, InventoryTransactionUpdateDto updateDto);
    Task<bool> DeleteInventoryTransactionAsync(int id);
} 