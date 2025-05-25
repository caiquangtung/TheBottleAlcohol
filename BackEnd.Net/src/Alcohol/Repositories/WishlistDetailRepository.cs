using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Alcohol.Data;
using Alcohol.Models;
using Alcohol.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Alcohol.Repositories;

public class WishlistDetailRepository : IWishlistDetailRepository
{
    private readonly MyDbContext _context;

    public WishlistDetailRepository(MyDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<WishlistDetail>> GetAllAsync()
    {
        return await _context.WishlistDetails
            .Include(wd => wd.Wishlist)
            .Include(wd => wd.Product)
            .ToListAsync();
    }

    public async Task<WishlistDetail> GetByIdAsync(int id)
    {
        return await _context.WishlistDetails
            .Include(wd => wd.Wishlist)
            .Include(wd => wd.Product)
            .FirstOrDefaultAsync(wd => wd.Id == id);
    }

    public async Task<IEnumerable<WishlistDetail>> GetByWishlistIdAsync(int wishlistId)
    {
        return await _context.WishlistDetails
            .Include(wd => wd.Wishlist)
            .Include(wd => wd.Product)
            .Where(wd => wd.WishlistId == wishlistId)
            .ToListAsync();
    }

    public async Task AddAsync(WishlistDetail wishlistDetail)
    {
        await _context.WishlistDetails.AddAsync(wishlistDetail);
    }

    public void Update(WishlistDetail wishlistDetail)
    {
        _context.WishlistDetails.Update(wishlistDetail);
    }

    public void Delete(WishlistDetail wishlistDetail)
    {
        _context.WishlistDetails.Remove(wishlistDetail);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
} 