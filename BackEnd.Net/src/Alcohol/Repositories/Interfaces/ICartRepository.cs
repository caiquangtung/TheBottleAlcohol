using System.Collections.Generic;
using System.Threading.Tasks;
using Alcohol.Models;
using System;
using System.Linq.Expressions;

namespace Alcohol.Repositories.Interfaces;

public interface ICartRepository : IGenericRepository<Cart>
{
    Task<Cart> GetByCustomerIdAsync(int customerId);
    Task<Cart> GetCartWithDetailsAsync(int cartId);
} 