using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Alcohol.Data;
using Alcohol.Models;
using Alcohol.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Alcohol.Repositories;

public class AccountRepository : IAccountRepository
{
    private readonly MyDbContext _context;

    public AccountRepository(MyDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Account>> GetAllAsync()
    {
        return await _context.Accounts.ToListAsync();
    }

    public async Task<Account> GetByIdAsync(int id)
    {
        return await _context.Accounts.FindAsync(id);
    }

    public async Task<Account> GetByEmailAsync(string email)
    {
        return await _context.Accounts
            .FirstOrDefaultAsync(a => a.Email == email);
    }

    public async Task<Account> GetByPhoneAsync(string phone)
    {
        return await _context.Accounts
            .FirstOrDefaultAsync(a => a.PhoneNumber == phone);
    }

    public async Task<IEnumerable<Account>> GetByRoleAsync(string role)
    {
        return await _context.Accounts
            .Where(a => a.Role.ToString() == role)
            .ToListAsync();
    }

    public async Task AddAsync(Account account)
    {
        await _context.Accounts.AddAsync(account);
    }

    public void Update(Account account)
    {
        _context.Accounts.Update(account);
    }

    public void Delete(Account account)
    {
        _context.Accounts.Remove(account);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }

    public async Task<bool> ExistsAsync(int id)
    {
        return await _context.Accounts.AnyAsync(a => a.Id == id);
    }

    public async Task<bool> ExistsByEmailAsync(string email)
    {
        return await _context.Accounts.AnyAsync(a => a.Email == email);
    }

    public async Task<Account?> GetAccountByOAuthIdAsync(string provider, string oauthId)
    {
        return await _context.Accounts
            .FirstOrDefaultAsync(a => a.OAuthProvider == provider && a.OAuthId == oauthId);
    }
} 