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

public class OrderRepository : IOrderRepository
{
    private readonly MyDbContext _context;

    public OrderRepository(MyDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Order>> GetAllAsync()
    {
        return await _context.Orders.ToListAsync();
    }

    public async Task<Order> GetByIdAsync(int id)
    {
        return await _context.Orders.FindAsync(id);
    }

    public async Task<Order> GetOrderWithDetailsAsync(int id)
    {
        return await _context.Orders
            .Include(o => o.Account)
            .Include(o => o.OrderDetails)
                .ThenInclude(od => od.Product)
            .FirstOrDefaultAsync(o => o.Id == id);
    }

    public async Task<IEnumerable<Order>> GetOrdersByCustomerAsync(int customerId)
    {
        return await _context.Orders
            .Where(o => o.AccountId == customerId)
            .OrderByDescending(o => o.OrderDate)
            .ToListAsync();
    }

    public async Task<IEnumerable<Order>> GetOrdersByStatusAsync(string status)
    {
        return await _context.Orders
            .Where(o => o.OrderStatuses.Any(os => Enum.Parse<OrderStatusType>(os.Status.ToString()) == Enum.Parse<OrderStatusType>(status)))
            .OrderByDescending(o => o.OrderDate)
            .ToListAsync();
    }

    public async Task AddAsync(Order order)
    {
        await _context.Orders.AddAsync(order);
    }

    public void Update(Order order)
    {
        _context.Orders.Update(order);
    }

    public void Delete(Order order)
    {
        _context.Orders.Remove(order);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
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
} 