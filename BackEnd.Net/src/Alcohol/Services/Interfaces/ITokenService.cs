using System.Security.Claims;
using Alcohol.DTOs.Auth;
using Alcohol.Models;
using Microsoft.AspNetCore.Http;

namespace Alcohol.Services.Interfaces;

public interface ITokenService
{
    string GenerateAccessToken(Account account);
    string GenerateRefreshToken();
    TokenResponseDto GenerateTokens(Account account);
    bool ValidateRefreshToken(string token);
    ClaimsPrincipal GetPrincipalFromExpiredToken(string token);
    string GetEmailFromToken(ClaimsPrincipal principal);
    void SetRefreshTokenCookie(string token, HttpContext context);
    void RemoveRefreshTokenCookie(HttpContext context);
}