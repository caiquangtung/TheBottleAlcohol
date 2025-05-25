using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Alcohol.DTOs.ImportOrder;
using Alcohol.Models.Enums;

namespace Alcohol.Services.Interfaces;

public interface IImportOrderService
{
    Task<IEnumerable<ImportOrderResponseDto>> GetAllImportOrdersAsync();
    Task<ImportOrderResponseDto> GetImportOrderByIdAsync(int id);
    Task<ImportOrderResponseDto> GetImportOrderWithDetailsAsync(int id);
    Task<IEnumerable<ImportOrderResponseDto>> GetImportOrdersBySupplierAsync(int supplierId);
    Task<IEnumerable<ImportOrderResponseDto>> GetImportOrdersByStatusAsync(ImportOrderStatusType status);
    Task<ImportOrderResponseDto> CreateImportOrderAsync(ImportOrderCreateDto createDto);
    Task<ImportOrderResponseDto> UpdateImportOrderAsync(int id, ImportOrderUpdateDto updateDto);
    Task<ImportOrderResponseDto> UpdateImportOrderStatusAsync(int id, ImportOrderStatusType status);
    Task<bool> DeleteImportOrderAsync(int id);
    Task<ImportOrderResponseDto> CompleteImportOrderAsync(int id);
    Task<List<ImportOrderDetailResponseDto>> GetImportOrderDetails(int id);
    Task<ImportOrderDetailResponseDto> AddImportOrderDetail(int id, ImportOrderDetailCreateDto detailDto);
    Task<bool> RemoveImportOrderDetail(int id, int detailId);
} 