using System.Collections.Generic;
using System.Threading.Tasks;
using Alcohol.Models;

namespace Alcohol.Repositories.Interfaces;

public interface ICartRepository
{
    Task<IEnumerable<Cart>> GetAllAsync();
    Task<Cart> GetByIdAsync(int id);
    Task<IEnumerable<Cart>> GetByCustomerIdAsync(int customerId);
    Task AddAsync(Cart cart);
    void Update(Cart cart);
    void Delete(Cart cart);
    Task SaveChangesAsync();
} 