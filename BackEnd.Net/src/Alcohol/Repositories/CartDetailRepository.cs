using System.Collections.Generic;
using System.Threading.Tasks;
using Alcohol.Data;
using Alcohol.Models;
using Alcohol.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace Alcohol.Repositories;

public class CartDetailRepository : GenericRepository<CartDetail>, ICartDetailRepository
{
    public CartDetailRepository(MyDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<CartDetail>> GetByCartIdAsync(int cartId)
    {
        return await _context.CartDetails
            .Include(cd => cd.Product)
            .Where(cd => cd.CartId == cartId)
            .ToListAsync();
    }

    public void DeleteRange(IEnumerable<CartDetail> entities)
    {
        _context.CartDetails.RemoveRange(entities);
    }

    public async Task AddRangeAsync(IEnumerable<CartDetail> entities)
    {
        await _context.CartDetails.AddRangeAsync(entities);
    }

    public void UpdateRange(IEnumerable<CartDetail> entities)
    {
        _context.CartDetails.UpdateRange(entities);
    }
} 