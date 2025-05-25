using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Alcohol.Data;
using Alcohol.Models;
using Alcohol.Models.Enums;
using Alcohol.Repositories.Interfaces;
using Alcohol.Repositories.IRepositories;
using Microsoft.EntityFrameworkCore;

namespace Alcohol.Repositories;

public class InventoryTransactionRepository : IInventoryTransactionRepository
{
    private readonly MyDbContext _context;

    public InventoryTransactionRepository(MyDbContext context)
    {
        _context = context;
    }

    public async Task<List<InventoryTransaction>> GetAllAsync()
    {
        return await _context.InventoryTransactions
            .Include(t => t.Product)
            .ToListAsync();
    }

    public async Task<InventoryTransaction> GetByIdAsync(int id)
    {
        return await _context.InventoryTransactions
            .Include(t => t.Product)
            .FirstOrDefaultAsync(t => t.Id == id);
    }

    public async Task<List<InventoryTransaction>> GetByProductIdAsync(int productId)
    {
        return await _context.InventoryTransactions
            .Include(t => t.Product)
            .Where(t => t.ProductId == productId)
            .ToListAsync();
    }

    public async Task<List<InventoryTransaction>> GetByTypeAsync(InventoryTransactionType type)
    {
        return await _context.InventoryTransactions
            .Include(t => t.Product)
            .Where(t => t.Type == type)
            .ToListAsync();
    }

    public async Task<List<InventoryTransaction>> GetByReferenceAsync(ReferenceType referenceType, int referenceId)
    {
        return await _context.InventoryTransactions
            .Include(t => t.Product)
            .Where(t => t.ReferenceType == referenceType && t.ReferenceId == referenceId)
            .ToListAsync();
    }

    public async Task AddAsync(InventoryTransaction transaction)
    {
        await _context.InventoryTransactions.AddAsync(transaction);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(InventoryTransaction transaction)
    {
        _context.InventoryTransactions.Update(transaction);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var transaction = await _context.InventoryTransactions.FindAsync(id);
        if (transaction != null)
        {
            _context.InventoryTransactions.Remove(transaction);
            await _context.SaveChangesAsync();
        }
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
} 