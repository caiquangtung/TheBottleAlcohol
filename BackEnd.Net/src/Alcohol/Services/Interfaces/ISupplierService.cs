using System.Collections.Generic;
using System.Threading.Tasks;
using Alcohol.DTOs.Supplier;

namespace Alcohol.Services.Interfaces;

public interface ISupplierService
{
    Task<IEnumerable<SupplierResponseDto>> GetAllSuppliersAsync();
    Task<SupplierResponseDto> GetSupplierByIdAsync(int id);
    Task<SupplierResponseDto> CreateSupplierAsync(SupplierCreateDto dto);
    Task<SupplierResponseDto> UpdateSupplierAsync(int id, SupplierUpdateDto dto);
    Task<bool> DeleteSupplierAsync(int id);
} 