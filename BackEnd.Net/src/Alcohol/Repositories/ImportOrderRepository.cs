using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Alcohol.Data;
using Alcohol.Models;
using Alcohol.Models.Enums;
using Alcohol.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Alcohol.Repositories;

public class ImportOrderRepository : IImportOrderRepository
{
    private readonly MyDbContext _context;

    public ImportOrderRepository(MyDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<ImportOrder>> GetAllAsync()
    {
        return await _context.ImportOrders
            .Include(io => io.Supplier)
            .Include(io => io.Manager)
            .Include(io => io.ImportOrderDetails)
            .ToListAsync();
    }

    public async Task<ImportOrder> GetByIdAsync(int id)
    {
        return await _context.ImportOrders
            .Include(io => io.Supplier)
            .Include(io => io.Manager)
            .Include(io => io.ImportOrderDetails)
            .FirstOrDefaultAsync(io => io.Id == id);
    }

    public async Task<ImportOrder> GetImportOrderWithDetailsAsync(int id)
    {
        return await _context.ImportOrders
            .Include(io => io.Supplier)
            .Include(io => io.Manager)
            .Include(io => io.ImportOrderDetails)
                .ThenInclude(d => d.Product)
            .FirstOrDefaultAsync(io => io.Id == id);
    }

    public async Task<IEnumerable<ImportOrder>> GetImportOrdersBySupplierAsync(int supplierId)
    {
        return await _context.ImportOrders
            .Include(io => io.Supplier)
            .Include(io => io.Manager)
            .Include(io => io.ImportOrderDetails)
            .Where(io => io.SupplierId == supplierId)
            .ToListAsync();
    }

    public async Task<IEnumerable<ImportOrder>> GetImportOrdersByStatusAsync(ImportOrderStatusType status)
    {
        return await _context.ImportOrders
            .Include(io => io.Supplier)
            .Include(io => io.Manager)
            .Include(io => io.ImportOrderDetails)
            .Where(io => io.Status == status)
            .ToListAsync();
    }

    public async Task<List<ImportOrder>> GetImportOrdersForStatsAsync(DateTime? minDate, DateTime? maxDate)
    {
        var query = _context.ImportOrders
            .Include(io => io.ImportOrderDetails)
            .AsQueryable();

        if (minDate.HasValue)
            query = query.Where(io => io.ImportDate >= minDate.Value);

        if (maxDate.HasValue)
            query = query.Where(io => io.ImportDate <= maxDate.Value);

        return await query.ToListAsync();
    }

    public async Task<IEnumerable<ImportOrder>> GetBySupplierIdAsync(int supplierId)
    {
        return await _context.ImportOrders
            .Include(io => io.Supplier)
            .Include(io => io.Manager)
            .Include(io => io.ImportOrderDetails)
            .Where(io => io.SupplierId == supplierId)
            .ToListAsync();
    }

    public async Task AddAsync(ImportOrder importOrder)
    {
        await _context.ImportOrders.AddAsync(importOrder);
    }

    public void Update(ImportOrder importOrder)
    {
        _context.ImportOrders.Update(importOrder);
    }

    public void Delete(ImportOrder importOrder)
    {
        _context.ImportOrders.Remove(importOrder);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}