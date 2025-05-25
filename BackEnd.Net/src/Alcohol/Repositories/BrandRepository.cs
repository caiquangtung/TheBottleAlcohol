using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Alcohol.Data;
using Alcohol.Models;
using Alcohol.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Alcohol.Repositories;

public class BrandRepository : GenericRepository<Brand>, IBrandRepository
{
    public BrandRepository(MyDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Brand>> GetActiveBrandsAsync()
    {
        return await _dbSet
            .Where(b => b.IsActive)
            .OrderBy(b => b.DisplayOrder)
            .ToListAsync();
    }

    public async Task<Brand> GetBrandWithProductsAsync(int id)
    {
        return await _dbSet
            .Include(b => b.Products)
            .FirstOrDefaultAsync(b => b.Id == id);
    }

    public async Task<bool> HasProductsAsync(int id)
    {
        return await _dbSet
            .Include(b => b.Products)
            .Where(b => b.Id == id)
            .Select(b => b.Products.Any())
            .FirstOrDefaultAsync();
    }
} 