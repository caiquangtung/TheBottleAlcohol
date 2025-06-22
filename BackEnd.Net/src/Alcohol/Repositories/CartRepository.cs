using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Alcohol.Data;
using Alcohol.Models;
using Alcohol.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Alcohol.Repositories;

public class CartRepository : GenericRepository<Cart>, ICartRepository
{
    public CartRepository(MyDbContext context) : base(context)
    {
    }

    public async Task<Cart> GetByCustomerIdAsync(int customerId)
    {
        return await _context.Carts
            .Include(c => c.CartDetails)
            .ThenInclude(cd => cd.Product)
            .FirstOrDefaultAsync(c => c.CustomerId == customerId);
    }

    public async Task<Cart> GetCartWithDetailsAsync(int cartId)
    {
        return await _context.Carts
            .Include(c => c.CartDetails)
                .ThenInclude(cd => cd.Product)
            .FirstOrDefaultAsync(c => c.Id == cartId);
    }
} 