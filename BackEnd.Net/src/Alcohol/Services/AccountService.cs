using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Alcohol.Common;
using Alcohol.Data;
using Alcohol.DTOs.Account;
using Alcohol.Models;
using Alcohol.Models.Enums;
using Alcohol.Repositories.Interfaces;
using Alcohol.Services.Interfaces;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace Alcohol.Services;

public class AccountService : IAccountService
{
    private readonly MyDbContext _context;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IAccountRepository _accountRepository;
    private readonly IMapper _mapper;

    public AccountService(MyDbContext context, IPasswordHasher passwordHasher, IAccountRepository accountRepository, IMapper mapper)
    {
        _context = context;
        _passwordHasher = passwordHasher;
        _accountRepository = accountRepository;
        _mapper = mapper;
    }

    public async Task<PagedResult<AccountResponseDto>> GetAccounts(AccountFilterDto filter)
    {
        var query = _context.Accounts.AsQueryable();

        if (!string.IsNullOrEmpty(filter.SearchTerm))
        {
            query = query.Where(a => a.FullName.Contains(filter.SearchTerm) || 
                                   a.Email.Contains(filter.SearchTerm) ||
                                   a.PhoneNumber.Contains(filter.SearchTerm));
        }

        if (filter.Status.HasValue)
        {
            query = query.Where(a => a.Status == filter.Status.Value);
        }

        var totalItems = await query.CountAsync();
        var items = await query
            .Skip((filter.PageNumber - 1) * filter.PageSize)
            .Take(filter.PageSize)
            .Select(a => new AccountResponseDto
            {
                Id = a.Id,
                FullName = a.FullName,
                DateOfBirth = a.DateOfBirth,
                Address = a.Address,
                Gender = a.Gender,
                PhoneNumber = a.PhoneNumber,
                Email = a.Email,
                Role = a.Role,
                Status = a.Status,
                CreatedAt = a.CreatedAt
            })
            .ToListAsync();

        return new PagedResult<AccountResponseDto>
        {
            Items = items,
            TotalItems = totalItems,
            PageNumber = filter.PageNumber,
            PageSize = filter.PageSize
        };
    }

    public async Task<AccountResponseDto> GetAccountById(int id)
    {
        var account = await _context.Accounts.FindAsync(id);
        if (account == null)
            return null;

        return new AccountResponseDto
        {
            Id = account.Id,
            FullName = account.FullName,
            DateOfBirth = account.DateOfBirth,
            Address = account.Address,
            Gender = account.Gender,
            PhoneNumber = account.PhoneNumber,
            Email = account.Email,
            Role = account.Role,
            Status = account.Status,
            CreatedAt = account.CreatedAt
        };
    }

    public async Task<AccountResponseDto> CreateAccount(AccountCreateDto dto)
    {
        if (await _accountRepository.ExistsByEmailAsync(dto.Email))
        {
            throw new Exception("Email already exists");
        }

        var account = new Account
        {
            FullName = dto.FullName,
            DateOfBirth = dto.DateOfBirth,
            Address = dto.Address,
            Gender = dto.Gender,
            PhoneNumber = dto.PhoneNumber,
            Email = dto.Email,
            Password = _passwordHasher.HashPassword(dto.Password),
            Role = RoleType.User,
            Status = true,
            CreatedAt = DateTime.Now
        };

        _context.Accounts.Add(account);
        await _context.SaveChangesAsync();

        return new AccountResponseDto
        {
            Id = account.Id,
            FullName = account.FullName,
            DateOfBirth = account.DateOfBirth,
            Address = account.Address,
            Gender = account.Gender,
            PhoneNumber = account.PhoneNumber,
            Email = account.Email,
            Role = account.Role,
            Status = account.Status,
            CreatedAt = account.CreatedAt
        };
    }

    public async Task<AccountResponseDto> UpdateAccount(int id, AccountUpdateDto dto)
    {
        var account = await _context.Accounts.FindAsync(id);
        if (account == null)
            return null;

        account.FullName = dto.FullName;
        account.DateOfBirth = dto.DateOfBirth;
        account.Address = dto.Address;
        account.Gender = dto.Gender;
        account.PhoneNumber = dto.PhoneNumber;
        account.Email = dto.Email;
        account.Status = dto.Status;

        if (!string.IsNullOrEmpty(dto.Password))
        {
            account.Password = _passwordHasher.HashPassword(dto.Password);
        }

        await _context.SaveChangesAsync();

        return new AccountResponseDto
        {
            Id = account.Id,
            FullName = account.FullName,
            DateOfBirth = account.DateOfBirth,
            Address = account.Address,
            Gender = account.Gender,
            PhoneNumber = account.PhoneNumber,
            Email = account.Email,
            Role = account.Role,
            Status = account.Status,
            CreatedAt = account.CreatedAt
        };
    }

    public async Task<bool> DeleteAccount(int id)
    {
        var account = await _context.Accounts.FindAsync(id);
        if (account == null)
            return false;

        _context.Accounts.Remove(account);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<IEnumerable<AccountResponseDto>> GetAllAccountsAsync()
    {
        var accounts = await _accountRepository.GetAllAsync();
        return _mapper.Map<IEnumerable<AccountResponseDto>>(accounts);
    }

    public async Task<AccountResponseDto> GetAccountByIdAsync(int id)
    {
        var account = await _accountRepository.GetByIdAsync(id);
        if (account == null)
            return null;

        return _mapper.Map<AccountResponseDto>(account);
    }

    public async Task<AccountResponseDto> GetAccountByEmailAsync(string email)
    {
        var account = await _accountRepository.GetByEmailAsync(email);
        if (account == null)
            return null;

        return _mapper.Map<AccountResponseDto>(account);
    }

    public async Task<AccountResponseDto> GetAccountByPhoneAsync(string phone)
    {
        var account = await _accountRepository.GetByPhoneAsync(phone);
        if (account == null)
            return null;

        return _mapper.Map<AccountResponseDto>(account);
    }

    public async Task<IEnumerable<AccountResponseDto>> GetAccountsByRoleAsync(RoleType role)
    {
        var accounts = await _accountRepository.GetByRoleAsync(role.ToString());
        return _mapper.Map<IEnumerable<AccountResponseDto>>(accounts);
    }

    public async Task<AccountResponseDto> CreateAccountAsync(AccountCreateDto createDto)
    {
        var account = _mapper.Map<Account>(createDto);
        account.CreatedAt = DateTime.UtcNow;

        await _accountRepository.AddAsync(account);
        await _accountRepository.SaveChangesAsync();

        return _mapper.Map<AccountResponseDto>(account);
    }

    public async Task<AccountResponseDto> UpdateAccountAsync(int id, AccountUpdateDto updateDto)
    {
        var account = await _accountRepository.GetByIdAsync(id);
        if (account == null)
            return null;

        _mapper.Map(updateDto, account);
        account.CreatedAt = DateTime.UtcNow;

        _accountRepository.Update(account);
        await _accountRepository.SaveChangesAsync();

        return _mapper.Map<AccountResponseDto>(account);
    }

    public async Task<bool> DeleteAccountAsync(int id)
    {
        var account = await _accountRepository.GetByIdAsync(id);
        if (account == null)
            return false;

        _accountRepository.Delete(account);
        await _accountRepository.SaveChangesAsync();
        return true;
    }

    public async Task<AccountResponseDto> UpdateAccountStatus(int id, bool status)
    {
        var account = await _accountRepository.GetByIdAsync(id);
        if (account == null)
            return null;

        account.Status = status;
        account.CreatedAt = DateTime.UtcNow;

        _accountRepository.Update(account);
        await _accountRepository.SaveChangesAsync();

        return _mapper.Map<AccountResponseDto>(account);
    }

    public async Task<IEnumerable<AccountResponseDto>> GetAllAccounts()
    {
        var accounts = await _accountRepository.GetAllAsync();
        return accounts.Select(a => new AccountResponseDto
        {
            Id = a.Id,
            FullName = a.FullName,
            DateOfBirth = a.DateOfBirth,
            Address = a.Address,
            Gender = a.Gender,
            PhoneNumber = a.PhoneNumber,
            Email = a.Email,
            Role = a.Role,
            Status = a.Status,
            CreatedAt = a.CreatedAt
        });
    }

    public async Task<bool> VerifyPassword(int accountId, string password)
    {
        var account = await _context.Accounts.FindAsync(accountId);
        if (account == null)
            return false;

        return _passwordHasher.VerifyPassword(password, account.Password);
    }

    public async Task<AccountResponseDto> CreateAccountWithRole(AccountCreateWithRoleDto dto)
    {
        if (await _accountRepository.ExistsByEmailAsync(dto.Email))
        {
            throw new Exception("Email already exists");
        }

        var account = new Account
        {
            FullName = dto.FullName,
            DateOfBirth = dto.DateOfBirth,
            Address = dto.Address,
            Gender = dto.Gender,
            PhoneNumber = dto.PhoneNumber,
            Email = dto.Email,
            Password = _passwordHasher.HashPassword(dto.Password),
            Role = dto.Role,
            Status = true,
            CreatedAt = DateTime.Now
        };

        _context.Accounts.Add(account);
        await _context.SaveChangesAsync();

        return new AccountResponseDto
        {
            Id = account.Id,
            FullName = account.FullName,
            DateOfBirth = account.DateOfBirth,
            Address = account.Address,
            Gender = account.Gender,
            PhoneNumber = account.PhoneNumber,
            Email = account.Email,
            Role = account.Role,
            Status = account.Status,
            CreatedAt = account.CreatedAt
        };
    }
} 