using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Alcohol.Models;

namespace Alcohol.Repositories.Interfaces;

public interface IInventoryRepository
{
    Task<List<Inventory>> GetAllAsync();
    Task<Inventory> GetByIdAsync(int id);
    Task<Inventory> GetByProductIdAsync(int productId);
    Task<List<Inventory>> GetLowStockInventoriesAsync(int threshold);
    Task AddAsync(Inventory inventory);
    Task UpdateAsync(Inventory inventory);
    Task DeleteAsync(int id);
    Task SaveChangesAsync();
} 