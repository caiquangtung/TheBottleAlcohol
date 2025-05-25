using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Alcohol.Data;
using Alcohol.Models;
using Alcohol.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Alcohol.Repositories;

public class CartRepository : ICartRepository
{
    private readonly MyDbContext _context;

    public CartRepository(MyDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Cart>> GetAllAsync()
    {
        return await _context.Carts
            .Include(c => c.Customer)
            .ToListAsync();
    }

    public async Task<Cart> GetByIdAsync(int id)
    {
        return await _context.Carts
            .Include(c => c.Customer)
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<IEnumerable<Cart>> GetByCustomerIdAsync(int customerId)
    {
        return await _context.Carts
            .Include(c => c.Customer)
            .Where(c => c.CustomerId == customerId)
            .ToListAsync();
    }

    public async Task AddAsync(Cart cart)
    {
        await _context.Carts.AddAsync(cart);
    }

    public void Update(Cart cart)
    {
        _context.Carts.Update(cart);
    }

    public void Delete(Cart cart)
    {
        _context.Carts.Remove(cart);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
} 