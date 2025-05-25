using System.Collections.Generic;
using System.Threading.Tasks;
using Alcohol.Models;

namespace Alcohol.Repositories.Interfaces;

public interface IOrderStatusRepository
{
    Task<IEnumerable<OrderStatus>> GetAllAsync();
    Task<OrderStatus> GetByIdAsync(int id);
    Task<OrderStatus> GetByNameAsync(string name);
    Task AddAsync(OrderStatus orderStatus);
    void Update(OrderStatus orderStatus);
    void Delete(OrderStatus orderStatus);
    Task SaveChangesAsync();
} 