using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Alcohol.Data;
using Alcohol.Models;
using Alcohol.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Alcohol.Repositories;

public class ImportOrderDetailRepository : IImportOrderDetailRepository
{
    private readonly MyDbContext _context;

    public ImportOrderDetailRepository(MyDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<ImportOrderDetail>> GetAllAsync()
    {
        return await _context.ImportOrderDetails
            .Include(d => d.ImportOrder)
            .Include(d => d.Product)
            .ToListAsync();
    }

    public async Task<ImportOrderDetail> GetByIdAsync(int id)
    {
        return await _context.ImportOrderDetails
            .Include(d => d.ImportOrder)
            .Include(d => d.Product)
            .FirstOrDefaultAsync(d => d.Id == id);
    }

    public async Task<IEnumerable<ImportOrderDetail>> GetByImportOrderIdAsync(int importOrderId)
    {
        return await _context.ImportOrderDetails
            .Include(d => d.ImportOrder)
            .Include(d => d.Product)
            .Where(d => d.ImportOrderId == importOrderId)
            .ToListAsync();
    }

    public async Task<List<ImportOrderDetail>> GetByProductIdAsync(int productId)
    {
        return await _context.ImportOrderDetails
            .Include(d => d.ImportOrder)
            .Include(d => d.Product)
            .Where(d => d.ProductId == productId)
            .ToListAsync();
    }

    public async Task AddAsync(ImportOrderDetail importOrderDetail)
    {
        await _context.ImportOrderDetails.AddAsync(importOrderDetail);
    }

    public void Update(ImportOrderDetail importOrderDetail)
    {
        _context.ImportOrderDetails.Update(importOrderDetail);
    }

    public void Delete(ImportOrderDetail importOrderDetail)
    {
        _context.ImportOrderDetails.Remove(importOrderDetail);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
} 