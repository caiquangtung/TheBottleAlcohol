using System.Collections.Generic;
using System.Threading.Tasks;
using Alcohol.Models;

namespace Alcohol.Repositories.Interfaces;

public interface IOrderRepository
{
    Task<IEnumerable<Order>> GetAllAsync();
    Task<Order> GetByIdAsync(int id);
    Task<Order> GetOrderWithDetailsAsync(int id);
    Task<IEnumerable<Order>> GetOrdersByCustomerAsync(int customerId);
    Task<IEnumerable<Order>> GetOrdersByStatusAsync(string status);
    Task AddAsync(Order order);
    void Update(Order order);
    void Delete(Order order);
    Task SaveChangesAsync();
} 