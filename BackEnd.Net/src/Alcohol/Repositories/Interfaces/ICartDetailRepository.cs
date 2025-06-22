using System.Collections.Generic;
using System.Threading.Tasks;
using Alcohol.Models;

namespace Alcohol.Repositories.Interfaces;

public interface ICartDetailRepository : IGenericRepository<CartDetail>
{
    Task<IEnumerable<CartDetail>> GetByCartIdAsync(int cartId);
    void DeleteRange(IEnumerable<CartDetail> entities);
    Task AddRangeAsync(IEnumerable<CartDetail> entities);
    void UpdateRange(IEnumerable<CartDetail> entities);
} 