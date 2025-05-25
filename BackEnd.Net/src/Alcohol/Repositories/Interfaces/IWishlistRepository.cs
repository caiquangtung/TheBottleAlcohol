using System.Collections.Generic;
using System.Threading.Tasks;
using Alcohol.Models;

namespace Alcohol.Repositories.Interfaces;

public interface IWishlistRepository
{
    Task<IEnumerable<Wishlist>> GetAllAsync();
    Task<Wishlist> GetByIdAsync(int id);
    Task<IEnumerable<Wishlist>> GetByCustomerIdAsync(int customerId);
    Task AddAsync(Wishlist wishlist);
    void Update(Wishlist wishlist);
    void Delete(Wishlist wishlist);
    Task SaveChangesAsync();
} 