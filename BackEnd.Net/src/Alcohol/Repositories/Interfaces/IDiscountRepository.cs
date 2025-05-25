using System.Collections.Generic;
using System.Threading.Tasks;
using Alcohol.Models;

namespace Alcohol.Repositories.Interfaces;

public interface IDiscountRepository : IGenericRepository<Discount>
{
    Task<Discount> GetByCodeAsync(string code);
    Task<IEnumerable<Discount>> GetActiveDiscountsAsync();
    Task<IEnumerable<Discount>> GetExpiredDiscountsAsync();
    Task<bool> IsCodeUniqueAsync(string code);
} 