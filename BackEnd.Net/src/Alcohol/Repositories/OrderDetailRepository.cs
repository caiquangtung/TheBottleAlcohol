using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Alcohol.Data;
using Alcohol.Models;
using Alcohol.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Alcohol.Repositories;

public class OrderDetailRepository : IOrderDetailRepository
{
    private readonly MyDbContext _context;

    public OrderDetailRepository(MyDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<OrderDetail>> GetAllAsync()
    {
        return await _context.OrderDetails
            .Include(od => od.Order)
            .Include(od => od.Product)
            .ToListAsync();
    }

    public async Task<OrderDetail> GetByIdAsync(int orderId, int productId)
    {
        return await _context.OrderDetails
            .Include(od => od.Order)
            .Include(od => od.Product)
            .FirstOrDefaultAsync(od => od.OrderId == orderId && od.ProductId == productId);
    }

    public async Task<IEnumerable<OrderDetail>> GetByOrderIdAsync(int orderId)
    {
        return await _context.OrderDetails
            .Include(od => od.Order)
            .Include(od => od.Product)
            .Where(od => od.OrderId == orderId)
            .ToListAsync();
    }

    public async Task AddAsync(OrderDetail orderDetail)
    {
        await _context.OrderDetails.AddAsync(orderDetail);
    }

    public async Task UpdateAsync(OrderDetail orderDetail)
    {
        _context.OrderDetails.Update(orderDetail);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int orderId, int productId)
    {
        var orderDetail = await _context.OrderDetails.FindAsync(orderId, productId);
        if (orderDetail != null)
        {
            _context.OrderDetails.Remove(orderDetail);
            await _context.SaveChangesAsync();
        }
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
} 