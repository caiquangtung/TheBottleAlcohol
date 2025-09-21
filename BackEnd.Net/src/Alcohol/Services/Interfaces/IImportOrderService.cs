using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Alcohol.DTOs.ImportOrder;
using Alcohol.DTOs;

namespace Alcohol.Services.Interfaces;

public interface IImportOrderService
{
    Task<PagedResult<ImportOrderResponseDto>> GetAllImportOrdersAsync(ImportOrderFilterDto filter);
    Task<ImportOrderResponseDto> GetImportOrderByIdAsync(int id);
    Task<IEnumerable<ImportOrderResponseDto>> GetImportOrdersBySupplierAsync(int supplierId);
    Task<ImportOrderResponseDto> CreateImportOrderAsync(ImportOrderCreateDto createDto);
    Task<ImportOrderResponseDto> UpdateImportOrderAsync(int id, ImportOrderUpdateDto updateDto);
    Task<bool> DeleteImportOrderAsync(int id);
    
    // Workflow methods
    Task<ImportOrderResponseDto> ApproveImportOrderAsync(int id);
    Task<ImportOrderResponseDto> CompleteImportOrderAsync(int id);
    Task<ImportOrderResponseDto> CancelImportOrderAsync(int id, string reason = null);
    Task<ImportOrderStatsDto> GetImportOrderStatsAsync();
} 