using System;
using System.Threading.Tasks;
using Alcohol.DTOs.Cart;
using Alcohol.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Alcohol.Common;
using Alcohol.DTOs;

namespace Alcohol.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class CartController : ControllerBase
{
    private readonly ICartService _cartService;

    public CartController(ICartService cartService)
    {
        _cartService = cartService;
    }

    [HttpGet]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> GetAllCarts([FromQuery] CartFilterDto filter)
    {
        var result = await _cartService.GetAllCartsAsync(filter);
        return Ok(new ApiResponse<PagedResult<CartResponseDto>>(result));
    }

    [HttpGet("{id}")]
    [Authorize]
    public async Task<IActionResult> GetCartById(int id)
    {
        var cart = await _cartService.GetCartByIdAsync(id);
        if (cart == null)
            return NotFound(new ApiResponse<string>("Cart not found"));
        return Ok(new ApiResponse<CartResponseDto>(cart));
    }

    [HttpGet("customer/{customerId}")]
    [Authorize]
    public async Task<IActionResult> GetCartByCustomer(int customerId)
    {
        var cart = await _cartService.GetCartsByCustomerAsync(customerId);
        if (cart == null)
            return NotFound(new ApiResponse<string>("Cart not found"));
        return Ok(new ApiResponse<CartResponseDto>(cart));
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> CreateCart(CartCreateDto createDto)
    {
        try
        {
            var cart = await _cartService.CreateCartAsync(createDto);
            return CreatedAtAction(nameof(GetCartById), new { id = cart.Id }, new ApiResponse<CartResponseDto>(cart));
        }
        catch (Exception ex)
        {
            return BadRequest(new ApiResponse<string>(ex.Message));
        }
    }

    [HttpPut("{id}")]
    [Authorize]
    public async Task<IActionResult> UpdateCart(int id, CartUpdateDto updateDto)
    {
        var cart = await _cartService.UpdateCartAsync(id, updateDto);
        if (cart == null)
            return NotFound(new ApiResponse<string>("Cart not found"));
        return Ok(new ApiResponse<CartResponseDto>(cart));
    }

    [HttpDelete("{id}")]
    [Authorize]
    public async Task<IActionResult> DeleteCart(int id)
    {
        var result = await _cartService.DeleteCartAsync(id);
        if (!result)
            return NotFound(new ApiResponse<string>("Cart not found"));
        return Ok(new ApiResponse<string>("Cart deleted successfully"));
    }

    [HttpPost("sync")]
    [Authorize]
    public async Task<IActionResult> SyncCart(CartSyncDto syncDto)
    {
        try
        {
            var cart = await _cartService.SyncCartAsync(syncDto);
            return Ok(new ApiResponse<CartResponseDto>(cart));
        }
        catch (Exception ex)
        {
            return BadRequest(new ApiResponse<string>(ex.Message));
        }
    }
} 