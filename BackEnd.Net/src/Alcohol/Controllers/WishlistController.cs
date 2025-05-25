using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Alcohol.DTOs.Wishlist;
using Alcohol.DTOs.WishlistDetail;
using Alcohol.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Alcohol.Common;

namespace Alcohol.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class WishlistController : ControllerBase
{
    private readonly IWishlistService _wishlistService;
    private readonly IWishlistDetailService _wishlistDetailService;

    public WishlistController(
        IWishlistService wishlistService,
        IWishlistDetailService wishlistDetailService)
    {
        _wishlistService = wishlistService;
        _wishlistDetailService = wishlistDetailService;
    }

    [HttpGet]
    [Authorize]
    public async Task<ActionResult<IEnumerable<WishlistResponseDto>>> GetWishlists()
    {
        var wishlists = await _wishlistService.GetAllWishlistsAsync();
        return Ok(new ApiResponse<IEnumerable<WishlistResponseDto>>(wishlists));
    }

    [HttpGet("{id}")]
    [Authorize]
    public async Task<ActionResult<WishlistResponseDto>> GetWishlistById(int id)
    {
        var wishlist = await _wishlistService.GetWishlistByIdAsync(id);
        if (wishlist == null)
            return NotFound(new ApiResponse<string>("Wishlist not found"));

        return Ok(new ApiResponse<WishlistResponseDto>(wishlist));
    }

    [HttpGet("customer/{customerId}")]
    [Authorize]
    public async Task<ActionResult<IEnumerable<WishlistResponseDto>>> GetWishlistsByCustomer(int customerId)
    {
        var wishlists = await _wishlistService.GetWishlistsByCustomerAsync(customerId);
        return Ok(new ApiResponse<IEnumerable<WishlistResponseDto>>(wishlists));
    }

    [HttpPost]
    [Authorize]
    public async Task<ActionResult<WishlistResponseDto>> CreateWishlist(WishlistCreateDto createDto)
    {
        var wishlist = await _wishlistService.CreateWishlistAsync(createDto);
        return CreatedAtAction(nameof(GetWishlistById), 
            new { id = wishlist.Id }, 
            new ApiResponse<WishlistResponseDto>(wishlist));
    }

    [HttpPut("{id}")]
    [Authorize]
    public async Task<ActionResult<WishlistResponseDto>> UpdateWishlist(int id, WishlistUpdateDto updateDto)
    {
        var wishlist = await _wishlistService.UpdateWishlistAsync(id, updateDto);
        if (wishlist == null)
            return NotFound(new ApiResponse<string>("Wishlist not found"));

        return Ok(new ApiResponse<WishlistResponseDto>(wishlist));
    }

    [HttpDelete("{id}")]
    [Authorize]
    public async Task<ActionResult> DeleteWishlist(int id)
    {
        var result = await _wishlistService.DeleteWishlistAsync(id);
        if (!result)
            return NotFound(new ApiResponse<string>("Wishlist not found"));

        return Ok(new ApiResponse<string>("Wishlist deleted successfully"));
    }

    [HttpGet("{wishlistId}/products")]
    [Authorize]
    public async Task<ActionResult<IEnumerable<WishlistDetailResponseDto>>> GetWishlistProducts(int wishlistId)
    {
        var products = await _wishlistDetailService.GetWishlistDetailsByWishlistAsync(wishlistId);
        return Ok(new ApiResponse<IEnumerable<WishlistDetailResponseDto>>(products));
    }

    [HttpPost("{wishlistId}/products")]
    [Authorize]
    public async Task<ActionResult<WishlistDetailResponseDto>> AddProductToWishlist(int wishlistId, [FromBody] WishlistDetailCreateDto createDto)
    {
        createDto.WishlistId = wishlistId;
        var wishlistDetail = await _wishlistDetailService.CreateWishlistDetailAsync(createDto);
        return Ok(new ApiResponse<WishlistDetailResponseDto>(wishlistDetail));
    }

    [HttpDelete("{wishlistId}/products/{productId}")]
    [Authorize]
    public async Task<ActionResult> RemoveProductFromWishlist(int wishlistId, int productId)
    {
        var wishlistDetails = await _wishlistDetailService.GetWishlistDetailsByWishlistAsync(wishlistId);
        var wishlistDetail = wishlistDetails.FirstOrDefault(wd => wd.ProductId == productId);
        
        if (wishlistDetail == null)
            return NotFound(new ApiResponse<string>("Product not found in wishlist"));

        var result = await _wishlistDetailService.DeleteWishlistDetailAsync(wishlistDetail.Id);
        if (!result)
            return NotFound(new ApiResponse<string>("Failed to remove product from wishlist"));

        return Ok(new ApiResponse<string>("Product removed from wishlist successfully"));
    }
} 