using System.Collections.Generic;
using System.Threading.Tasks;
using Alcohol.Data;
using Alcohol.Models;
using Alcohol.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Alcohol.Repositories;

public class WishlistRepository : IWishlistRepository
{
    private readonly MyDbContext _context;

    public WishlistRepository(MyDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Wishlist>> GetAllAsync()
    {
        return await _context.Wishlists
            .Include(w => w.WishlistDetails)
            .ToListAsync();
    }

    public async Task<Wishlist> GetByIdAsync(int id)
    {
        return await _context.Wishlists
            .Include(w => w.WishlistDetails)
            .FirstOrDefaultAsync(w => w.Id == id);
    }

    public async Task<IEnumerable<Wishlist>> GetByCustomerIdAsync(int customerId)
    {
        return await _context.Wishlists
            .Include(w => w.WishlistDetails)
            .Where(w => w.CustomerId == customerId)
            .ToListAsync();
    }

    public async Task AddAsync(Wishlist wishlist)
    {
        await _context.Wishlists.AddAsync(wishlist);
    }

    public void Update(Wishlist wishlist)
    {
        _context.Wishlists.Update(wishlist);
    }

    public void Delete(Wishlist wishlist)
    {
        _context.Wishlists.Remove(wishlist);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
} 