using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Alcohol.Models;
using Alcohol.Models.Enums;

namespace Alcohol.Repositories.Interfaces;

public interface IImportOrderRepository
{
    Task<IEnumerable<ImportOrder>> GetAllAsync();
    Task<ImportOrder> GetByIdAsync(int id);
    Task<ImportOrder> GetImportOrderWithDetailsAsync(int id);
    Task<IEnumerable<ImportOrder>> GetImportOrdersBySupplierAsync(int supplierId);
    Task<IEnumerable<ImportOrder>> GetBySupplierIdAsync(int supplierId);
    Task<IEnumerable<ImportOrder>> GetImportOrdersByStatusAsync(ImportOrderStatusType status);
    Task<List<ImportOrder>> GetImportOrdersForStatsAsync(DateTime? minDate, DateTime? maxDate);
    Task AddAsync(ImportOrder importOrder);
    void Update(ImportOrder importOrder);
    void Delete(ImportOrder importOrder);
    Task SaveChangesAsync();
}
