using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Alcohol.Data;
using Alcohol.Models;
using Alcohol.Models.Enums;
using Alcohol.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Alcohol.Repositories;

public class SupplierRepository : GenericRepository<Supplier>, ISupplierRepository
{
    public SupplierRepository(MyDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Supplier>> GetActiveSuppliersAsync()
    {
        return await _dbSet.Where(s => s.Status == SupplierStatusType.Active).ToListAsync();
    }

    public async Task<Supplier> GetSupplierWithImportOrdersAsync(int id)
    {
        return await _dbSet
            .Include(s => s.ImportOrders)
            .FirstOrDefaultAsync(s => s.Id == id);
    }

    public async Task<bool> HasImportOrdersAsync(int id)
    {
        return await _dbSet
            .Include(s => s.ImportOrders)
            .Where(s => s.Id == id)
            .Select(s => s.ImportOrders.Any())
            .FirstOrDefaultAsync();
    }
} 