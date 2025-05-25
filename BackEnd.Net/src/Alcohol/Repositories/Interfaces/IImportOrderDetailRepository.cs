using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Alcohol.Models;

namespace Alcohol.Repositories.Interfaces;

public interface IImportOrderDetailRepository
{
    Task<IEnumerable<ImportOrderDetail>> GetAllAsync();
    Task<ImportOrderDetail> GetByIdAsync(int id);
    Task<IEnumerable<ImportOrderDetail>> GetByImportOrderIdAsync(int importOrderId);
    Task<List<ImportOrderDetail>> GetByProductIdAsync(int productId);
    Task AddAsync(ImportOrderDetail importOrderDetail);
    void Update(ImportOrderDetail importOrderDetail);
    void Delete(ImportOrderDetail importOrderDetail);
    Task SaveChangesAsync();
} 