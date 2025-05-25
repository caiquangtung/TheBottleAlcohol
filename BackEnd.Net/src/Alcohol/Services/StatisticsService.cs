using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Alcohol.Data;
using Alcohol.DTOs.Statistics;
using Alcohol.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Alcohol.Services;

public class StatisticsService : IStatisticsService
{
    private readonly MyDbContext _context;

    public StatisticsService(MyDbContext context)
    {
        _context = context;
    }

    public async Task<List<OrderAnalyticsDto>> GetOrderAnalytics(DateTime? startDate, DateTime? endDate)
    {
        var query = _context.Orders.AsQueryable();

        if (startDate.HasValue)
            query = query.Where(o => o.OrderDate >= startDate.Value);

        if (endDate.HasValue)
            query = query.Where(o => o.OrderDate <= endDate.Value);

        var analytics = await query
            .GroupBy(o => o.OrderDate.Date)
            .Select(g => new OrderAnalyticsDto
            {
                Date = g.Key,
                TotalOrders = g.Count(),
                TotalRevenue = g.Sum(o => o.TotalAmount)
            })
            .ToListAsync();

        return analytics;
    }

    public async Task<List<RevenueAnalyticsDto>> GetRevenueAnalytics(DateTime? startDate, DateTime? endDate)
    {
        var query = _context.Orders
            .Include(o => o.OrderDetails)
            .AsQueryable();

        if (startDate.HasValue)
            query = query.Where(o => o.OrderDate >= startDate.Value);

        if (endDate.HasValue)
            query = query.Where(o => o.OrderDate <= endDate.Value);

        var analytics = await query
            .GroupBy(o => o.OrderDate.Date)
            .Select(g => new RevenueAnalyticsDto
            {
                Date = g.Key,
                TotalRevenue = g.Sum(o => o.TotalAmount),
                TotalProfit = g.Sum(o => o.TotalAmount - o.OrderDetails.Sum(d => d.UnitPrice * d.Quantity))
            })
            .ToListAsync();

        return analytics;
    }

    public async Task<List<ProductAnalyticsDto>> GetProductAnalytics(DateTime? startDate, DateTime? endDate)
    {
        var query = _context.OrderDetails
            .Include(d => d.Order)
            .Include(d => d.Product)
            .AsQueryable();

        if (startDate.HasValue)
            query = query.Where(d => d.Order.OrderDate >= startDate.Value);

        if (endDate.HasValue)
            query = query.Where(d => d.Order.OrderDate <= endDate.Value);

        var analytics = await query
            .GroupBy(d => d.Product)
            .Select(g => new ProductAnalyticsDto
            {
                ProductId = g.Key.Id,
                ProductName = g.Key.Name,
                TotalQuantity = g.Sum(d => d.Quantity),
                TotalRevenue = g.Sum(d => d.TotalAmount)
            })
            .ToListAsync();

        return analytics;
    }

    public async Task<List<SpendAnalyticsDto>> GetSpendAnalytics(AnalyticsFilterDto filter)
    {
        var query = _context.ImportOrders
            .Where(o => o.ImportDate >= filter.StartDate && o.ImportDate <= filter.EndDate);

        var analytics = await query
            .GroupBy(o => o.ImportDate.Date)
            .Select(g => new SpendAnalyticsDto
            {
                Date = g.Key,
                TotalSpend = g.Sum(o => o.TotalAmount)
            })
            .ToListAsync();

        return analytics;
    }

    public async Task<List<IncomeAnalyticsDto>> GetIncomeAnalytics(AnalyticsFilterDto filter)
    {
        var query = _context.Orders
            .Include(o => o.OrderDetails)
            .Where(o => o.OrderDate >= filter.StartDate && o.OrderDate <= filter.EndDate);

        var analytics = await query
            .GroupBy(o => o.OrderDate.Date)
            .Select(g => new IncomeAnalyticsDto
            {
                Date = g.Key,
                TotalIncome = g.Sum(o => o.TotalAmount),
                TotalProfit = g.Sum(o => o.TotalAmount - o.OrderDetails.Sum(d => d.UnitPrice * d.Quantity))
            })
            .ToListAsync();

        return analytics;
    }
} 