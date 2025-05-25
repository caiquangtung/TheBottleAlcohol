using System.Collections.Generic;
using System.Threading.Tasks;
using Alcohol.Models;

namespace Alcohol.Repositories.Interfaces;

public interface ICartDetailRepository
{
    Task<IEnumerable<CartDetail>> GetAllAsync();
    Task<CartDetail> GetByIdAsync(int id);
    Task<IEnumerable<CartDetail>> GetByCartIdAsync(int cartId);
    Task AddAsync(CartDetail cartDetail);
    void Update(CartDetail cartDetail);
    void Delete(CartDetail cartDetail);
    Task SaveChangesAsync();
} 