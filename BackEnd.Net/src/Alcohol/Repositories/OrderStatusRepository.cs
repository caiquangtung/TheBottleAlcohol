using System.Collections.Generic;
using System.Threading.Tasks;
using Alcohol.Data;
using Alcohol.Models;
using Alcohol.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Alcohol.Repositories;

public class OrderStatusRepository : IOrderStatusRepository
{
    private readonly MyDbContext _context;

    public OrderStatusRepository(MyDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<OrderStatus>> GetAllAsync()
    {
        return await _context.OrderStatuses
            .Include(os => os.Order)
            .ToListAsync();
    }

    public async Task<OrderStatus> GetByIdAsync(int id)
    {
        return await _context.OrderStatuses
            .Include(os => os.Order)
            .FirstOrDefaultAsync(os => os.OrderId == id);
    }

    public async Task<OrderStatus> GetByNameAsync(string name)
    {
        return await _context.OrderStatuses
            .Include(os => os.Order)
            .FirstOrDefaultAsync(os => os.Status.ToString() == name);
    }

    public async Task AddAsync(OrderStatus orderStatus)
    {
        await _context.OrderStatuses.AddAsync(orderStatus);
    }

    public void Update(OrderStatus orderStatus)
    {
        _context.OrderStatuses.Update(orderStatus);
    }

    public void Delete(OrderStatus orderStatus)
    {
        _context.OrderStatuses.Remove(orderStatus);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
} 