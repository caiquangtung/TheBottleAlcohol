using System.Collections.Generic;
using System.Threading.Tasks;
using Alcohol.Models;

namespace Alcohol.Repositories.Interfaces;

public interface IBrandRepository : IGenericRepository<Brand>
{
    Task<IEnumerable<Brand>> GetActiveBrandsAsync();
    Task<Brand> GetBrandWithProductsAsync(int id);
    Task<bool> HasProductsAsync(int id);
} 