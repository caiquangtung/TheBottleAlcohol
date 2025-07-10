using System;
using System.Threading.Tasks;
using Alcohol.DTOs.Wishlist;
using Alcohol.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Alcohol.Common;
using Alcohol.DTOs;

namespace Alcohol.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class WishlistController : ControllerBase
{
    private readonly IWishlistService _wishlistService;

    public WishlistController(IWishlistService wishlistService)
    {
        _wishlistService = wishlistService;
    }

    [HttpGet]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> GetAllWishlists([FromQuery] WishlistFilterDto filter)
    {
        var result = await _wishlistService.GetAllWishlistsAsync(filter);
        return Ok(new ApiResponse<PagedResult<WishlistResponseDto>>(result));
    }

    [HttpGet("{id}")]
    [Authorize]
    public async Task<IActionResult> GetWishlistById(int id)
    {
        var wishlist = await _wishlistService.GetWishlistByIdAsync(id);
        if (wishlist == null)
            return NotFound(new ApiResponse<string>("Wishlist not found"));
        return Ok(new ApiResponse<WishlistResponseDto>(wishlist));
    }

    [HttpGet("customer/{customerId}")]
    [Authorize]
    public async Task<IActionResult> GetWishlistByCustomer(int customerId)
    {
        var wishlist = await _wishlistService.GetWishlistByCustomerAsync(customerId);
        if (wishlist == null)
            return NotFound(new ApiResponse<string>("Wishlist not found"));
        return Ok(new ApiResponse<WishlistResponseDto>(wishlist));
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> CreateWishlist(WishlistCreateDto createDto)
    {
        try
        {
            var wishlist = await _wishlistService.CreateWishlistAsync(createDto);
            return CreatedAtAction(nameof(GetWishlistById), new { id = wishlist.Id }, new ApiResponse<WishlistResponseDto>(wishlist));
        }
        catch (Exception ex)
        {
            return BadRequest(new ApiResponse<string>(ex.Message));
        }
    }

    [HttpPut("{id}")]
    [Authorize]
    public async Task<IActionResult> UpdateWishlist(int id, WishlistUpdateDto updateDto)
    {
        var wishlist = await _wishlistService.UpdateWishlistAsync(id, updateDto);
        if (wishlist == null)
            return NotFound(new ApiResponse<string>("Wishlist not found"));
        return Ok(new ApiResponse<WishlistResponseDto>(wishlist));
    }

    [HttpDelete("{id}")]
    [Authorize]
    public async Task<IActionResult> DeleteWishlist(int id)
    {
        var result = await _wishlistService.DeleteWishlistAsync(id);
        if (!result)
            return NotFound(new ApiResponse<string>("Wishlist not found"));
        return Ok(new ApiResponse<string>("Wishlist deleted successfully"));
    }
} 