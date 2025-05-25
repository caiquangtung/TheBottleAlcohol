using Alcohol.DTOs.Auth;
using Alcohol.DTOs.Account;
using Alcohol.Models;
using Alcohol.Models.Enums;
using Alcohol.Repositories.Interfaces;
using Alcohol.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System.Security.Claims;

namespace Alcohol.Services;

public class AuthService : IAuthService
{
    private readonly IConfiguration _configuration;
    private readonly IAccountService _accountService;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ITokenService _tokenService;
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private const int MAX_REFRESH_TOKENS_PER_USER = 5;

    public AuthService(
        IConfiguration configuration,
        IAccountService accountService,
        IPasswordHasher passwordHasher,
        ITokenService tokenService,
        IRefreshTokenRepository refreshTokenRepository)
    {
        _configuration = configuration;
        _accountService = accountService;
        _passwordHasher = passwordHasher;
        _tokenService = tokenService;
        _refreshTokenRepository = refreshTokenRepository;
    }

    // Auth operations
    public async Task<AccountResponseDto> Register(AccountCreateDto accountDto)
    {
        var existingAccount = await _accountService.GetAccountByEmailAsync(accountDto.Email);
        if (existingAccount != null)
        {
            throw new Exception("Email already exists");
        }

        // Hash password before creating account
        var hashedPassword = _passwordHasher.HashPassword(accountDto.Password);

        return await _accountService.CreateAccountAsync(new AccountCreateDto
        {
            Email = accountDto.Email,
            Password = hashedPassword, // Use hashed password
            FullName = accountDto.FullName,
            DateOfBirth = accountDto.DateOfBirth,
            Address = accountDto.Address,
            Gender = accountDto.Gender,
            PhoneNumber = accountDto.PhoneNumber,
            Role = RoleType.User,
            Status = true
        });
    }

    public async Task<LoginResponseDto> Login(LoginDto loginDto, HttpRequest request)
    {
        var account = await _accountService.GetAccountByEmailAsync(loginDto.Email);
        if (account == null)
            return null;

        if (!await _accountService.VerifyPassword(account.Id, loginDto.Password))
            return null;

        if (!account.Status)
            throw new Exception("Account is disabled");

        // Check number of active refresh tokens
        var tokenCount = await _refreshTokenRepository.GetUserTokenCountAsync(account.Id);
        if (tokenCount >= MAX_REFRESH_TOKENS_PER_USER)
        {
            // Revoke oldest tokens if limit reached
            await _refreshTokenRepository.RevokeAllUserTokensAsync(account.Id);
        }

        var tokens = _tokenService.GenerateTokens(new Account
        {
            Id = account.Id,
            Email = account.Email,
            FullName = account.FullName,
            Role = account.Role
        });

        // Store refresh token in database
        var refreshToken = new RefreshToken
        {
            Token = tokens.RefreshToken,
            UserId = account.Id,
            ExpiryDate = DateTime.UtcNow.AddDays(7), // 7 days expiry
            CreatedAt = DateTime.UtcNow,
            DeviceInfo = request.Headers["User-Agent"].ToString(),
            IpAddress = request.HttpContext.Connection.RemoteIpAddress?.ToString(),
            UserAgent = request.Headers["User-Agent"].ToString()
        };

        await _refreshTokenRepository.CreateAsync(refreshToken);

        // Set refresh token cookie
        _tokenService.SetRefreshTokenCookie(tokens.RefreshToken, request.HttpContext);

        return new LoginResponseDto
        {
            Id = account.Id,
            FullName = account.FullName,
            Email = account.Email,
            Role = account.Role,
            AccessToken = tokens.AccessToken
        };
    }

    public async Task<TokenResponseDto> RefreshToken(string refreshToken, HttpRequest request)
    {
        if (string.IsNullOrEmpty(refreshToken))
            return null;

        var storedToken = await _refreshTokenRepository.GetByTokenAsync(refreshToken);
        if (storedToken == null || storedToken.IsRevoked || storedToken.ExpiryDate < DateTime.UtcNow)
            return null;

        // Verify device info
        if (storedToken.UserAgent != request.Headers["User-Agent"].ToString() ||
            storedToken.IpAddress != request.HttpContext.Connection.RemoteIpAddress?.ToString())
        {
            // Revoke token if device info doesn't match
            await _refreshTokenRepository.RevokeTokenAsync(refreshToken);
            return null;
        }

        var account = await _accountService.GetAccountByIdAsync(storedToken.UserId);
        if (account == null)
            return null;

        // Generate new tokens
        var newTokens = _tokenService.GenerateTokens(new Account
        {
            Id = account.Id,
            Email = account.Email,
            FullName = account.FullName,
            Role = account.Role
        });

        // Revoke old token
        await _refreshTokenRepository.RevokeTokenAsync(refreshToken);

        // Store new refresh token
        var newRefreshToken = new RefreshToken
        {
            Token = newTokens.RefreshToken,
            UserId = account.Id,
            ExpiryDate = DateTime.UtcNow.AddDays(7),
            CreatedAt = DateTime.UtcNow,
            DeviceInfo = request.Headers["User-Agent"].ToString(),
            IpAddress = request.HttpContext.Connection.RemoteIpAddress?.ToString(),
            UserAgent = request.Headers["User-Agent"].ToString()
        };

        await _refreshTokenRepository.CreateAsync(newRefreshToken);

        // Set new refresh token cookie
        _tokenService.SetRefreshTokenCookie(newTokens.RefreshToken, request.HttpContext);

        return newTokens;
    }

    public async Task Logout(int userId, HttpRequest request)
    {
        var refreshToken = request.Cookies["refreshToken"];
        if (!string.IsNullOrEmpty(refreshToken))
        {
            await _refreshTokenRepository.RevokeTokenAsync(refreshToken);
        }
        _tokenService.RemoveRefreshTokenCookie(request.HttpContext);
    }

    public async Task<bool> ChangePassword(int accountId, ChangePasswordDto changePasswordDto)
    {
        var account = await _accountService.GetAccountByIdAsync(accountId);
        if (account == null)
            throw new Exception("Account not found");

        // Verify current password using AccountService's internal method
        if (!await _accountService.VerifyPassword(accountId, changePasswordDto.CurrentPassword))
            return false;

        await _accountService.UpdateAccountAsync(accountId, new AccountUpdateDto
        {
            Password = changePasswordDto.NewPassword
        });

        return true;
    }

    public async Task<bool> ForgotPassword(ForgotPasswordDto dto)
    {
        var account = await _accountService.GetAccountByEmailAsync(dto.Email);
        if (account == null)
        {
            throw new Exception("Account not found");
        }

        // TODO: Implement email sending logic
        return true;
    }

    public async Task<bool> ResetPassword(ResetPasswordDto dto)
    {
        var account = await _accountService.GetAccountByEmailAsync(dto.Email);
        if (account == null)
        {
            throw new Exception("Account not found");
        }

        // TODO: Implement token verification logic
        await _accountService.UpdateAccountAsync(account.Id, new AccountUpdateDto
        {
            Password = dto.NewPassword
        });

        return true;
    }
}