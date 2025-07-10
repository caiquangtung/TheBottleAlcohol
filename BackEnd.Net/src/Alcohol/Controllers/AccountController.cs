using Alcohol.DTOs.Account;
using Alcohol.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Alcohol.Common;
using Alcohol.DTOs;

namespace Alcohol.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    [Authorize(Roles = "CEO,Manager")]
    public class AccountController : ControllerBase
    {
        private readonly IAccountService _accountService;

        public AccountController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] AccountFilterDto filter)
        {
            try
            {
                var result = await _accountService.GetAllAccounts(filter);
                return Ok(new ApiResponse<PagedResult<AccountResponseDto>>(result));
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<string>(ex.Message));
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var account = await _accountService.GetAccountById(id);
                if (account == null)
                    return NotFound(new ApiResponse<string>("Account not found"));
                return Ok(new ApiResponse<AccountResponseDto>(account));
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<string>(ex.Message));
            }
        }

        [HttpPost("create-with-role")]
        public async Task<IActionResult> CreateWithRole(AccountCreateWithRoleDto accountDto)
        {
            try
            {
                var account = await _accountService.CreateAccountWithRole(accountDto);
                return CreatedAtAction(nameof(GetById), new { id = account.Id }, new ApiResponse<AccountResponseDto>(account));
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<string>(ex.Message));
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, AccountUpdateDto accountDto)
        {
            try
            {
                var account = await _accountService.UpdateAccount(id, accountDto);
                if (account == null)
                    return NotFound(new ApiResponse<string>("Account not found"));
                return Ok(new ApiResponse<AccountResponseDto>(account));
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<string>(ex.Message));
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var result = await _accountService.DeleteAccount(id);
                if (!result)
                    return NotFound(new ApiResponse<string>("Account not found"));
                return Ok(new ApiResponse<string>("Account deleted successfully"));
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<string>(ex.Message));
            }
        }

        [HttpPut("{id}/status")]
        [Authorize(Roles = "CEO,Manager")]
        public async Task<IActionResult> UpdateStatus(int id, [FromBody] bool status)
        {
            try
            {
                var account = await _accountService.UpdateAccountStatus(id, status);
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