using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Alcohol.DTOs.WishlistDetail;
using Alcohol.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Alcohol.Common;

namespace Alcohol.Controllers;

[ApiController]
[Route("api/v1/wishlist")]
public class WishlistDetailController : ControllerBase
{
    private readonly IWishlistDetailService _wishlistDetailService;
    private readonly IWishlistService _wishlistService;

    public WishlistDetailController(IWishlistDetailService wishlistDetailService, IWishlistService wishlistService)
    {
        _wishlistDetailService = wishlistDetailService;
        _wishlistService = wishlistService;
    }

    private int GetCurrentUserId()
    {
        return int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
    }

    [HttpGet("products")]
    [Authorize]
    public async Task<IActionResult> GetMyWishlistProducts()
    {
        try
        {
            var userId = GetCurrentUserId();
            var wishlist = await _wishlistService.GetWishlistByCustomerAsync(userId);
            
            if (wishlist == null)
                return Ok(new ApiResponse<IEnumerable<WishlistDetailResponseDto>>(new List<WishlistDetailResponseDto>()));

            var wishlistProducts = await _wishlistDetailService.GetWishlistDetailsByWishlistAsync(wishlist.Id);
            return Ok(new ApiResponse<IEnumerable<WishlistDetailResponseDto>>(wishlistProducts));
        }
        catch (Exception ex)
        {
            return BadRequest(new ApiResponse<string>(ex.Message));
        }
    }

    [HttpPost("products/{productId}")]
    [Authorize]
    public async Task<IActionResult> AddProductToMyWishlist(int productId)
    {
        try
        {
            var userId = GetCurrentUserId();
            var wishlist = await _wishlistService.GetOrCreateWishlistForUserAsync(userId);
            
            // Check if product already exists in wishlist
            var existingItems = await _wishlistDetailService.GetWishlistDetailsByWishlistAsync(wishlist.Id);
            if (existingItems.Any(item => item.ProductId == productId))
                return BadRequest(new ApiResponse<string>("Product already in wishlist"));

            var createDto = new WishlistDetailCreateDto
            {
                WishlistId = wishlist.Id,
                ProductId = productId
            };
            
            var wishlistDetail = await _wishlistDetailService.CreateWishlistDetailAsync(createDto);
            return Ok(new ApiResponse<WishlistDetailResponseDto>(wishlistDetail, "Product added to wishlist successfully"));
        }
        catch (Exception ex)
        {
            return BadRequest(new ApiResponse<string>(ex.Message));
        }
    }

    [HttpDelete("products/{productId}")]
    [Authorize]
    public async Task<IActionResult> RemoveProductFromMyWishlist(int productId)
    {
        try
        {
            var userId = GetCurrentUserId();
            var wishlist = await _wishlistService.GetWishlistByCustomerAsync(userId);
            
            if (wishlist == null)
                return NotFound(new ApiResponse<string>("Wishlist not found"));

            // Find the wishlist detail by productId
            var wishlistDetails = await _wishlistDetailService.GetWishlistDetailsByWishlistAsync(wishlist.Id);
            var wishlistDetail = wishlistDetails.FirstOrDefault(wd => wd.ProductId == productId);
            
            if (wishlistDetail == null)
                return NotFound(new ApiResponse<string>("Product not found in wishlist"));

            var result = await _wishlistDetailService.DeleteWishlistDetailAsync(wishlistDetail.Id);
            if (!result)
                return BadRequest(new ApiResponse<string>("Failed to remove product from wishlist"));
                
            return Ok(new ApiResponse<string>("Product removed from wishlist successfully"));
        }
        catch (Exception ex)
        {
            return BadRequest(new ApiResponse<string>(ex.Message));
        }
    }
}