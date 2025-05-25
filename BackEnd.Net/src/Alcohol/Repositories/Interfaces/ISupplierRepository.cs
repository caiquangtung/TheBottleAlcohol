using System.Collections.Generic;
using System.Threading.Tasks;
using Alcohol.Models;

namespace Alcohol.Repositories.Interfaces;

public interface ISupplierRepository : IGenericRepository<Supplier>
{
    Task<IEnumerable<Supplier>> GetActiveSuppliersAsync();
    Task<Supplier> GetSupplierWithImportOrdersAsync(int id);
    Task<bool> HasImportOrdersAsync(int id);
} 