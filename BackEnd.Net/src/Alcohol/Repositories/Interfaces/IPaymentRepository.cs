using System.Collections.Generic;
using System.Threading.Tasks;
using Alcohol.Models;

namespace Alcohol.Repositories.Interfaces;

public interface IPaymentRepository
{
    Task<IEnumerable<Payment>> GetAllAsync();
    Task<Payment> GetByIdAsync(int id);
    Task<IEnumerable<Payment>> GetByOrderIdAsync(int orderId);
    Task<IEnumerable<Payment>> GetByCustomerIdAsync(int customerId);
    Task AddAsync(Payment payment);
    void Update(Payment payment);
    void Delete(Payment payment);
    Task SaveChangesAsync();
} 