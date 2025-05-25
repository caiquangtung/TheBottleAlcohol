using System.Collections.Generic;
using System.Threading.Tasks;
using Alcohol.Models;

namespace Alcohol.Repositories.Interfaces;

public interface IShippingRepository
{
    Task<IEnumerable<Shipping>> GetAllAsync();
    Task<Shipping> GetByIdAsync(int id);
    Task<IEnumerable<Shipping>> GetByOrderIdAsync(int orderId);
    Task<IEnumerable<Shipping>> GetByCustomerIdAsync(int customerId);
    Task AddAsync(Shipping shipping);
    void Update(Shipping shipping);
    void Delete(Shipping shipping);
    Task SaveChangesAsync();
} 