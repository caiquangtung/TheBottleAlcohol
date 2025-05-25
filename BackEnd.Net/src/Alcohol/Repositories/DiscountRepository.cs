using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Alcohol.Data;
using Alcohol.Models;
using Alcohol.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Alcohol.Repositories;

public class DiscountRepository : GenericRepository<Discount>, IDiscountRepository
{
    public DiscountRepository(MyDbContext context) : base(context)
    {
    }

    public async Task<Discount> GetByCodeAsync(string code)
    {
        return await _dbSet
            .Include(d => d.Products)
            .FirstOrDefaultAsync(d => d.Code == code);
    }

    public async Task<IEnumerable<Discount>> GetActiveDiscountsAsync()
    {
        var now = DateTime.UtcNow;
        return await _dbSet
            .Include(d => d.Products)
            .Where(d => d.IsActive && d.StartDate <= now && d.EndDate >= now)
            .ToListAsync();
    }

    public async Task<IEnumerable<Discount>> GetExpiredDiscountsAsync()
    {
        return await _dbSet
            .Where(d => d.EndDate < DateTime.UtcNow)
            .ToListAsync();
    }

    public async Task<bool> IsCodeUniqueAsync(string code)
    {
        return !await _dbSet.AnyAsync(d => d.Code == code);
    }
} 