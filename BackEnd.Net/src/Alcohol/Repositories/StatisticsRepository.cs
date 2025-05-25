using System;
using Alcohol.Data;
using Alcohol.Models;
using Alcohol.Repositories.IRepositories;
using Microsoft.EntityFrameworkCore;

namespace Alcohol.Repositories;

public class StatisticsRepository : IStatisticsRepository
{
    private readonly MyDbContext _context;

    public StatisticsRepository(MyDbContext context)
    {
        _context = context;
    }

    public async Task<List<Order>> GetOrdersForStatsAsync(DateTime? minDate, DateTime? maxDate)
    {
        var query = _context.Orders
            .Include(o => o.OrderDetails)
            .AsQueryable();

        if (minDate.HasValue)
        {
            query = query.Where(o => o.OrderDate >= minDate.Value);
        }

        if (maxDate.HasValue)
        {
            query = query.Where(o => o.OrderDate <= maxDate.Value);
        }

        return await query.ToListAsync();
    }

    public async Task<List<Product>> GetProductsForStatsAsync()
    {
        return await _context.Products
            .Include(p => p.OrderDetails)
            .ToListAsync();
    }
} 