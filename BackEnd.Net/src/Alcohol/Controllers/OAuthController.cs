using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Alcohol.DTOs.Auth;
using Alcohol.Services.Interfaces;
using Alcohol.Common;

namespace Alcohol.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class OAuthController : ControllerBase
    {
        private readonly IOAuthService _oauthService;
        private readonly ITokenService _tokenService;

        public OAuthController(IOAuthService oauthService, ITokenService tokenService)
        {
            _oauthService = oauthService;
            _tokenService = tokenService;
        }

        [HttpGet("google-callback")]
        public async Task<IActionResult> GoogleCallback()
        {
            try
            {
                // Get the authentication result
                var result = await HttpContext.AuthenticateAsync("Google");
                if (!result.Succeeded)
                {
                    return Redirect("http://localhost:3000/login?error=google_auth_failed");
                }

                var email = result.Principal.FindFirst(ClaimTypes.Email)?.Value;
                var name = result.Principal.FindFirst(ClaimTypes.Name)?.Value;
                var googleId = result.Principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(googleId))
                {
                    return Redirect("http://localhost:3000/login?error=invalid_google_data");
                }

                // Create or update account
                var account = await _oauthService.CreateOrUpdateAccountFromOAuthAsync(
                    "Google", googleId, email, name);

                if (account == null)
                {
                    return Redirect("http://localhost:3000/login?error=account_creation_failed");
                }

                // Generate tokens
                var tokens = _tokenService.GenerateTokens(new Alcohol.Models.Account
                {
                    Id = account.Id,
                    Email = account.Email,
                    FullName = account.FullName,
                    Role = account.Role
                });

                // Set refresh token cookie
                _tokenService.SetRefreshTokenCookie(tokens.RefreshToken, HttpContext);

                // Redirect to frontend with access token
                return Redirect($"http://localhost:3000/oauth-success?token={tokens.AccessToken}");
            }
            catch (Exception ex)
            {
                return Redirect($"http://localhost:3000/login?error={Uri.EscapeDataString(ex.Message)}");
            }
        }

        [HttpGet("facebook-callback")]
        public async Task<IActionResult> FacebookCallback()
        {
            try
            {
                // Get the authentication result
                var result = await HttpContext.AuthenticateAsync("Facebook");
                if (!result.Succeeded)
                {
                    return Redirect("http://localhost:3000/login?error=facebook_auth_failed");
                }

                var email = result.Principal.FindFirst(ClaimTypes.Email)?.Value;
                var name = result.Principal.FindFirst(ClaimTypes.Name)?.Value;
                var facebookId = result.Principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(facebookId))
                {
                    return Redirect("http://localhost:3000/login?error=invalid_facebook_data");
                }

                // Create or update account
                var account = await _oauthService.CreateOrUpdateAccountFromOAuthAsync(
                    "Facebook", facebookId, email, name);

                if (account == null)
                {
                    return Redirect("http://localhost:3000/login?error=account_creation_failed");
                }

                // Generate tokens
                var tokens = _tokenService.GenerateTokens(new Alcohol.Models.Account
                {
                    Id = account.Id,
                    Email = account.Email,
                    FullName = account.FullName,
                    Role = account.Role
                });

                // Set refresh token cookie
                _tokenService.SetRefreshTokenCookie(tokens.RefreshToken, HttpContext);

                // Redirect to frontend with access token
                return Redirect($"http://localhost:3000/oauth-success?token={tokens.AccessToken}");
            }
            catch (Exception ex)
            {
                return Redirect($"http://localhost:3000/login?error={Uri.EscapeDataString(ex.Message)}");
            }
        }

        [HttpGet("login/google")]
        public IActionResult GoogleLogin()
        {
            return Challenge(new AuthenticationProperties
            {
                RedirectUri = "/api/v1/oauth/google-callback"
            }, "Google");
        }

        [HttpGet("login/facebook")]
        public IActionResult FacebookLogin()
        {
            return Challenge(new AuthenticationProperties
            {
                RedirectUri = "/api/v1/oauth/facebook-callback"
            }, "Facebook");
        }
    }
} 