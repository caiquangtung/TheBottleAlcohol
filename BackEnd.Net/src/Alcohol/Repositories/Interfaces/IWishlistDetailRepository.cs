using System.Collections.Generic;
using System.Threading.Tasks;
using Alcohol.Models;

namespace Alcohol.Repositories.Interfaces;

public interface IWishlistDetailRepository
{
    Task<IEnumerable<WishlistDetail>> GetAllAsync();
    Task<WishlistDetail> GetByIdAsync(int id);
    Task<IEnumerable<WishlistDetail>> GetByWishlistIdAsync(int wishlistId);
    Task AddAsync(WishlistDetail wishlistDetail);
    void Update(WishlistDetail wishlistDetail);
    void Delete(WishlistDetail wishlistDetail);
    Task SaveChangesAsync();
} 