using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Alcohol.Data;
using Alcohol.Models;
using Alcohol.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Alcohol.Repositories;

public class ShippingRepository : IShippingRepository
{
    private readonly MyDbContext _context;

    public ShippingRepository(MyDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Shipping>> GetAllAsync()
    {
        return await _context.Shippings
            .Include(s => s.Order)
            .Include(s => s.Account)
            .ToListAsync();
    }

    public async Task<Shipping> GetByIdAsync(int id)
    {
        return await _context.Shippings
            .Include(s => s.Order)
            .Include(s => s.Account)
            .FirstOrDefaultAsync(s => s.Id == id);
    }

    public async Task<IEnumerable<Shipping>> GetByOrderIdAsync(int orderId)
    {
        return await _context.Shippings
            .Include(s => s.Order)
            .Include(s => s.Account)
            .Where(s => s.OrderId == orderId)
            .ToListAsync();
    }

    public async Task<IEnumerable<Shipping>> GetByCustomerIdAsync(int customerId)
    {
        return await _context.Shippings
            .Include(s => s.Order)
            .Include(s => s.Account)
            .Where(s => s.AccountId == customerId)
            .ToListAsync();
    }

    public async Task AddAsync(Shipping shipping)
    {
        await _context.Shippings.AddAsync(shipping);
    }

    public void Update(Shipping shipping)
    {
        _context.Shippings.Update(shipping);
    }

    public void Delete(Shipping shipping)
    {
        _context.Shippings.Remove(shipping);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
} 