using Alcohol.DTOs.Auth;
using Alcohol.DTOs.Account;
using Alcohol.Models;
using Alcohol.Repositories.Interfaces;
using Alcohol.Services.Interfaces;

using Microsoft.AspNetCore.Http;
using System.Text.Json;

namespace Alcohol.Services
{
    public class OAuthService : IOAuthService
    {
        private readonly IAccountRepository _accountRepository;
        private readonly IAccountService _accountService;
        private readonly ITokenService _tokenService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly HttpClient _httpClient;

        public OAuthService(
            IAccountRepository accountRepository,
            IAccountService accountService,
            ITokenService tokenService,
            IHttpContextAccessor httpContextAccessor,
            HttpClient httpClient)
        {
            _accountRepository = accountRepository;
            _accountService = accountService;
            _tokenService = tokenService;
            _httpContextAccessor = httpContextAccessor;
            _httpClient = httpClient;
        }

        public async Task<LoginResponseDto?> AuthenticateGoogleAsync(string idToken)
        {
            try
            {
                var googleUserInfo = await VerifyGoogleIdTokenAsync(idToken);
                if (googleUserInfo == null) return null;
                var account = await CreateOrUpdateAccountFromOAuthAsync(
                    "Google",
                    googleUserInfo["sub"],
                    googleUserInfo["email"],
                    googleUserInfo["name"],
                    googleUserInfo.GetValueOrDefault("picture")
                );
                if (account == null) return null;
                var tokens = _tokenService.GenerateTokens(new Account
                {
                    Id = account.Id,
                    Email = account.Email,
                    FullName = account.FullName,
                    Role = account.Role
                });
                _tokenService.SetRefreshTokenCookie(tokens.RefreshToken, _httpContextAccessor.HttpContext);
                return new LoginResponseDto
                {
                    Id = account.Id,
                    FullName = account.FullName,
                    Email = account.Email,
                    Role = account.Role,
                    AccessToken = tokens.AccessToken
                };
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<LoginResponseDto?> AuthenticateFacebookAsync(string accessToken)
        {
            try
            {
                var facebookUserInfo = await GetFacebookUserInfoAsync(accessToken);
                if (facebookUserInfo == null) return null;
                if (!facebookUserInfo.ContainsKey("email") || string.IsNullOrEmpty(facebookUserInfo["email"]))
                {
                    // Facebook không trả về email
                    return null;
                }
                var account = await CreateOrUpdateAccountFromOAuthAsync(
                    "Facebook",
                    facebookUserInfo["id"],
                    facebookUserInfo["email"],
                    facebookUserInfo["name"],
                    facebookUserInfo.GetValueOrDefault("picture", null)
                );
                if (account == null) return null;
                var tokens = _tokenService.GenerateTokens(new Account
                {
                    Id = account.Id,
                    Email = account.Email,
                    FullName = account.FullName,
                    Role = account.Role
                });
                _tokenService.SetRefreshTokenCookie(tokens.RefreshToken, _httpContextAccessor.HttpContext);
                return new LoginResponseDto
                {
                    Id = account.Id,
                    FullName = account.FullName,
                    Email = account.Email,
                    Role = account.Role,
                    AccessToken = tokens.AccessToken
                };
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<AccountResponseDto?> CreateOrUpdateAccountFromOAuthAsync(string provider, string oauthId, string email, string fullName, string? avatarUrl = null)
        {
            try
            {
                // Check if account exists by OAuth ID
                var existingAccount = await _accountRepository.GetAccountByOAuthIdAsync(provider, oauthId);
                if (existingAccount != null)
                {
                    // Update existing account
                    var updateDto = new Alcohol.DTOs.Account.AccountUpdateDto
                    {
                        FullName = fullName,
                        DateOfBirth = existingAccount.DateOfBirth,
                        Address = existingAccount.Address,
                        Gender = existingAccount.Gender,
                        PhoneNumber = existingAccount.PhoneNumber,
                        Email = existingAccount.Email,
                        Status = existingAccount.Status,
                        Password = existingAccount.Password
                    };
                    var updated = await _accountService.UpdateAccountAsync(existingAccount.Id, updateDto);
                    return updated;
                }
                // Check if account exists by email
                var accountByEmail = await _accountService.GetAccountByEmailAsync(email);
                if (accountByEmail != null)
                {
                    // Link OAuth to existing account
                    var accountEntity = await _accountRepository.GetByIdAsync(accountByEmail.Id);
                    var updateDto = new Alcohol.DTOs.Account.AccountUpdateDto
                    {
                        FullName = fullName,
                        DateOfBirth = accountByEmail.DateOfBirth,
                        Address = accountByEmail.Address,
                        Gender = accountByEmail.Gender,
                        PhoneNumber = accountByEmail.PhoneNumber,
                        Email = accountByEmail.Email,
                        Status = accountByEmail.Status,
                        Password = accountEntity?.Password ?? string.Empty
                    };
                    var updated = await _accountService.UpdateAccountAsync(accountByEmail.Id, updateDto);
                    return updated;
                }
                // Create new account
                var createDto = new Alcohol.DTOs.Account.AccountCreateDto
                {
                    FullName = fullName,
                    Email = email,
                    Password = Guid.NewGuid().ToString(),
                    Role = Alcohol.Models.Enums.RoleType.User,
                    Status = true,
                    DateOfBirth = DateTime.UtcNow.AddYears(-18),
                    Address = "Not provided",
                    Gender = Alcohol.Models.Enums.GenderType.Other,
                    PhoneNumber = "Not provided"
                };
                var created = await _accountService.CreateAccountAsync(createDto);
                return created;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        private async Task<Dictionary<string, string>?> VerifyGoogleIdTokenAsync(string idToken)
        {
            try
            {
                var response = await _httpClient.GetAsync($"https://oauth2.googleapis.com/tokeninfo?id_token={idToken}");
                if (!response.IsSuccessStatusCode) return null;

                var content = await response.Content.ReadAsStringAsync();
                var tokenInfo = JsonSerializer.Deserialize<JsonElement>(content);
                
                return new Dictionary<string, string>
                {
                    ["sub"] = tokenInfo.GetProperty("sub").GetString()!,
                    ["email"] = tokenInfo.GetProperty("email").GetString()!,
                    ["name"] = tokenInfo.GetProperty("name").GetString()!,
                    ["picture"] = tokenInfo.GetProperty("picture").GetString()!
                };
            }
            catch
            {
                return null;
            }
        }

        private async Task<Dictionary<string, string>?> GetFacebookUserInfoAsync(string accessToken)
        {
            try
            {
                var response = await _httpClient.GetAsync($"https://graph.facebook.com/me?fields=id,name,email,picture&access_token={accessToken}");
                if (!response.IsSuccessStatusCode) return null;

                var content = await response.Content.ReadAsStringAsync();
                var userInfo = JsonSerializer.Deserialize<JsonElement>(content);
                
                return new Dictionary<string, string>
                {
                    ["id"] = userInfo.GetProperty("id").GetString()!,
                    ["email"] = userInfo.GetProperty("email").GetString()!,
                    ["name"] = userInfo.GetProperty("name").GetString()!,
                    ["picture"] = userInfo.GetProperty("picture").ToString()
                };
            }
            catch
            {
                return null;
            }
        }

        private AccountResponseDto MapToAccountResponseDto(Account account)
        {
            return new AccountResponseDto
            {
                Id = account.Id,
                FullName = account.FullName,
                Email = account.Email,
                PhoneNumber = account.PhoneNumber,
                Address = account.Address,
                DateOfBirth = account.DateOfBirth,
                Gender = account.Gender,
                Role = account.Role,
                Status = account.Status,
                CreatedAt = account.CreatedAt,
                UpdatedAt = account.UpdatedAt,
                OAuthProvider = account.OAuthProvider,
                AvatarUrl = account.AvatarUrl
            };
        }
    }
} 