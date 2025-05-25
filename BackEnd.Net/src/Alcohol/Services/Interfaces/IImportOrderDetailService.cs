using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Alcohol.DTOs.ImportOrder;

namespace Alcohol.Services.Interfaces;

public interface IImportOrderDetailService
{
    Task<IEnumerable<ImportOrderDetailResponseDto>> GetImportOrderDetailsByOrderAsync(int orderId);
    Task<ImportOrderDetailResponseDto> GetImportOrderDetailByIdAsync(int id);
    Task<ImportOrderDetailResponseDto> CreateImportOrderDetailAsync(ImportOrderDetailCreateDto createDto);
    Task<ImportOrderDetailResponseDto> UpdateImportOrderDetailAsync(int id, ImportOrderDetailUpdateDto updateDto);
    Task<bool> DeleteImportOrderDetailAsync(int id);
} 