using Alcohol.DTOs.Account;
using Alcohol.DTOs.Auth;

namespace Alcohol.Services.Interfaces
{
    public interface IOAuthService
    {
        Task<LoginResponseDto?> AuthenticateGoogleAsync(string idToken);
        Task<LoginResponseDto?> AuthenticateFacebookAsync(string accessToken);
        Task<AccountResponseDto?> CreateOrUpdateAccountFromOAuthAsync(string provider, string oauthId, string email, string fullName, string? avatarUrl = null);
    }
} 