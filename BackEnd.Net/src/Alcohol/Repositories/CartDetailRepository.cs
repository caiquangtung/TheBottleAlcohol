using System.Collections.Generic;
using System.Threading.Tasks;
using Alcohol.Data;
using Alcohol.Models;
using Alcohol.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Alcohol.Repositories;

public class CartDetailRepository : ICartDetailRepository
{
    private readonly MyDbContext _context;

    public CartDetailRepository(MyDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<CartDetail>> GetAllAsync()
    {
        return await _context.CartDetails
            .Include(cd => cd.Product)
            .ToListAsync();
    }

    public async Task<CartDetail> GetByIdAsync(int id)
    {
        return await _context.CartDetails
            .Include(cd => cd.Product)
            .FirstOrDefaultAsync(cd => cd.Id == id);
    }

    public async Task<IEnumerable<CartDetail>> GetByCartIdAsync(int cartId)
    {
        return await _context.CartDetails
            .Include(cd => cd.Product)
            .Where(cd => cd.CartId == cartId)
            .ToListAsync();
    }

    public async Task AddAsync(CartDetail cartDetail)
    {
        await _context.CartDetails.AddAsync(cartDetail);
    }

    public void Update(CartDetail cartDetail)
    {
        _context.CartDetails.Update(cartDetail);
    }

    public void Delete(CartDetail cartDetail)
    {
        _context.CartDetails.Remove(cartDetail);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
} 