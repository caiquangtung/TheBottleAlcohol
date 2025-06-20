using System.Collections.Generic;
using System.Threading.Tasks;
using Alcohol.Common;
using Alcohol.DTOs.Account;
using Alcohol.Models.Enums;
using Alcohol.Models;
using Alcohol.DTOs;

namespace Alcohol.Services.Interfaces;

public interface IAccountService
{
    Task<PagedResult<AccountResponseDto>> GetAccounts(AccountFilterDto filter);
    Task<AccountResponseDto> GetAccountById(int id);
    Task<AccountResponseDto> CreateAccount(AccountCreateDto dto);
    Task<AccountResponseDto> CreateAccountWithRole(AccountCreateWithRoleDto dto);
    Task<AccountResponseDto> UpdateAccount(int id, AccountUpdateDto dto);
    Task<bool> DeleteAccount(int id);
    Task<IEnumerable<AccountResponseDto>> GetAllAccountsAsync();
    Task<AccountResponseDto> GetAccountByIdAsync(int id);
    Task<AccountResponseDto> GetAccountByEmailAsync(string email);
    Task<AccountResponseDto> GetAccountByPhoneAsync(string phone);
    Task<IEnumerable<AccountResponseDto>> GetAccountsByRoleAsync(RoleType role);
    Task<AccountResponseDto> CreateAccountAsync(AccountCreateDto createDto);
    Task<AccountResponseDto> UpdateAccountAsync(int id, AccountUpdateDto updateDto);
    Task<bool> DeleteAccountAsync(int id);
    Task<AccountResponseDto> UpdateAccountStatus(int id, bool status);
    Task<IEnumerable<AccountResponseDto>> GetAllAccounts();
    Task<bool> VerifyPassword(int accountId, string password);
} 