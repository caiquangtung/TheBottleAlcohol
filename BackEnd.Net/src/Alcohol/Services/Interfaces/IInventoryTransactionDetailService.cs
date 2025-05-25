using System.Collections.Generic;
using System.Threading.Tasks;
using Alcohol.DTOs.InventoryTransaction;

namespace Alcohol.Services.Interfaces;

public interface IInventoryTransactionDetailService
{
    Task<IEnumerable<InventoryTransactionDetailResponseDto>> GetDetailsByTransactionAsync(int transactionId);
    Task<InventoryTransactionDetailResponseDto> GetDetailByIdAsync(int id);
    Task<InventoryTransactionDetailResponseDto> CreateDetailAsync(InventoryTransactionDetailCreateDto createDto);
    Task<InventoryTransactionDetailResponseDto> UpdateDetailAsync(int id, InventoryTransactionDetailUpdateDto updateDto);
    Task<bool> DeleteDetailAsync(int id);
} 