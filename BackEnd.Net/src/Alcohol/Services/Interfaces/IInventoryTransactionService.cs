using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Alcohol.DTOs.InventoryTransaction;
using Alcohol.Models.Enums;

namespace Alcohol.Services.Interfaces;

public interface IInventoryTransactionService
{
    Task<IEnumerable<InventoryTransactionResponseDto>> GetAllTransactionsAsync();
    Task<InventoryTransactionResponseDto> GetTransactionByIdAsync(int id);
    Task<IEnumerable<InventoryTransactionResponseDto>> GetTransactionsByProductAsync(int productId);
    Task<IEnumerable<InventoryTransactionResponseDto>> GetTransactionsByTypeAsync(InventoryTransactionType type);
    Task<IEnumerable<InventoryTransactionResponseDto>> GetTransactionsByReferenceAsync(ReferenceType referenceType, int referenceId);
    Task<InventoryTransactionResponseDto> CreateTransactionAsync(InventoryTransactionCreateDto createDto);
    Task<InventoryTransactionResponseDto> UpdateTransactionAsync(int id, InventoryTransactionUpdateDto updateDto);
    Task<bool> UpdateTransactionStatusAsync(int id, InventoryTransactionStatusType status);
    Task<bool> DeleteTransactionAsync(int id);
    Task SaveChangesAsync();
} 