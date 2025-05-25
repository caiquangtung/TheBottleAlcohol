using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Alcohol.Models;
using Alcohol.Models.Enums;

namespace Alcohol.Repositories.Interfaces;

public interface IInventoryTransactionRepository
{
    Task<List<InventoryTransaction>> GetAllAsync();
    Task<InventoryTransaction> GetByIdAsync(int id);
    Task<List<InventoryTransaction>> GetByProductIdAsync(int productId);
    Task<List<InventoryTransaction>> GetByTypeAsync(InventoryTransactionType type);
    Task<List<InventoryTransaction>> GetByReferenceAsync(ReferenceType referenceType, int referenceId);
    Task AddAsync(InventoryTransaction transaction);
    Task UpdateAsync(InventoryTransaction transaction);
    Task DeleteAsync(int id);
    Task SaveChangesAsync();
} 