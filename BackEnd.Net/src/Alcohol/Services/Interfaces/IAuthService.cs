using System.Security.Claims;
using Alcohol.DTOs.Auth;
using Alcohol.DTOs.Account;
using Alcohol.Models;
using Microsoft.AspNetCore.Http;

namespace Alcohol.Services.Interfaces
{
    public interface IAuthService
    {
        // Auth operations
        Task<LoginResponseDto> Login(LoginDto loginDto, HttpRequest request);
        Task<AccountResponseDto> Register(AccountCreateDto accountDto);
        Task<bool> ChangePassword(int userId, ChangePasswordDto changePasswordDto);
        Task<TokenResponseDto> RefreshToken(string refreshToken, HttpRequest request);
        Task<bool> ForgotPassword(ForgotPasswordDto dto);
        Task<bool> ResetPassword(ResetPasswordDto dto);
        Task Logout(int userId, HttpRequest request);
    }
}