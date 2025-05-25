using System.Collections.Generic;
using System.Threading.Tasks;
using Alcohol.Models;

namespace Alcohol.Repositories.Interfaces;

public interface IAccountRepository
{
    Task<IEnumerable<Account>> GetAllAsync();
    Task<Account> GetByIdAsync(int id);
    Task<Account> GetByEmailAsync(string email);
    Task<Account> GetByPhoneAsync(string phone);
    Task<IEnumerable<Account>> GetByRoleAsync(string role);
    Task AddAsync(Account account);
    void Update(Account account);
    void Delete(Account account);
    Task SaveChangesAsync();
    Task<bool> ExistsByEmailAsync(string email);
} 