using Alcohol.DTOs.Auth;
using Alcohol.DTOs.Account;
using Alcohol.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;
using Alcohol.Common;
using Alcohol.Models;

namespace Alcohol.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ITokenService _tokenService;
        private readonly IAuthService _authService;
        private readonly IAccountService _accountService;

        public AuthController(ITokenService tokenService, IAuthService authService, IAccountService accountService)
        {
            _tokenService = tokenService;
            _authService = authService;
            _accountService = accountService;
        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken()
        {
            try
            {
                var refreshToken = Request.Cookies["refreshToken"];
                if (string.IsNullOrEmpty(refreshToken))
                {
                    return Unauthorized(new ApiResponse<string>("Refresh token is required"));
                }

                var result = await _authService.RefreshToken(refreshToken, Request);
                if (result == null)
                {
                    return Unauthorized(new ApiResponse<string>("Invalid refresh token"));
                }

                return Ok(new ApiResponse<TokenResponseDto>(result));
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<string>(ex.Message));
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto loginDto)
        {
            try
            {
                var result = await _authService.Login(loginDto, Request);
                if (result == null)
                    return Unauthorized(new ApiResponse<string>("Invalid username or password"));
                return Ok(new ApiResponse<LoginResponseDto>(result, "Login successful"));
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<string>(ex.Message));
            }
        }

        [HttpPost("logout")]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                await _authService.Logout(userId, Request);
                return Ok(new ApiResponse<string>("Logged out successfully"));
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<string>(ex.Message));
            }
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(AccountCreateDto accountDto)
        {
            try
            {
                var account = await _authService.Register(accountDto);
                return CreatedAtAction(nameof(GetById), new { id = account.Id }, new ApiResponse<AccountResponseDto>(account, "Account registered successfully"));
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<string>(ex.Message));
            }
        }

        [HttpGet("profile")]
        [Authorize]
        public async Task<IActionResult> GetProfile()
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                var account = await _accountService.GetAccountByIdAsync(userId);
                if (account == null)
                    return NotFound(new ApiResponse<string>("Account not found"));
                return Ok(new ApiResponse<AccountResponseDto>(account));
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<string>(ex.Message));
            }
        }

        [HttpPut("profile")]
        [Authorize]
        public async Task<IActionResult> UpdateProfile(AccountUpdateDto accountDto)
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                var account = await _accountService.UpdateAccountAsync(userId, accountDto);
                if (account == null)
                    return NotFound(new ApiResponse<string>("Account not found"));
                return Ok(new ApiResponse<AccountResponseDto>(account));
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<string>(ex.Message));
            }
        }

        [HttpPost("change-password")]
        [Authorize]
        public async Task<IActionResult> ChangePassword(ChangePasswordDto changePasswordDto)
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                var result = await _authService.ChangePassword(userId, changePasswordDto);
                if (!result)
                    return BadRequest(new ApiResponse<string>("Invalid current password"));
                return Ok(new ApiResponse<string>("Password changed successfully"));
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<string>(ex.Message));
            }
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "CEO,Manager")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var account = await _accountService.GetAccountByIdAsync(id);
                if (account == null)
                    return NotFound(new ApiResponse<string>("Account not found"));
                return Ok(new ApiResponse<AccountResponseDto>(account));
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<string>(ex.Message));
            }
        }
    }
}