using System.Collections.Generic;
using System.Threading.Tasks;
using Alcohol.Models;

namespace Alcohol.Repositories.Interfaces;

public interface IOrderDetailRepository
{
    Task<IEnumerable<OrderDetail>> GetAllAsync();
    Task<OrderDetail> GetByIdAsync(int orderId, int productId);
    Task<IEnumerable<OrderDetail>> GetByOrderIdAsync(int orderId);
    Task AddAsync(OrderDetail orderDetail);
    Task UpdateAsync(OrderDetail orderDetail);
    Task DeleteAsync(int orderId, int productId);
    Task SaveChangesAsync();
} 