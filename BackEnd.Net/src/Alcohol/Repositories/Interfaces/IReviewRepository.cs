using System.Collections.Generic;
using System.Threading.Tasks;
using Alcohol.Models;

namespace Alcohol.Repositories.Interfaces;

public interface IReviewRepository
{
    Task<IEnumerable<Review>> GetAllAsync();
    Task<Review> GetByIdAsync(int id);
    Task<IEnumerable<Review>> GetByProductIdAsync(int productId);
    Task<IEnumerable<Review>> GetByCustomerIdAsync(int customerId);
    Task<Review> GetByProductAndCustomerAsync(int productId, int customerId);
    Task AddAsync(Review review);
    void Update(Review review);
    void Delete(Review review);
    Task SaveChangesAsync();
} 