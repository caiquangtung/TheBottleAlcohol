using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Alcohol.DTOs.Cart;
using Alcohol.DTOs.CartDetail;
using Alcohol.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Alcohol.Common;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace Alcohol.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class CartController : ControllerBase
    {
        private readonly ICartService _cartService;
        private readonly ICartDetailService _cartDetailService;

        public CartController(ICartService cartService, ICartDetailService cartDetailService)
        {
            _cartService = cartService;
            _cartDetailService = cartDetailService;
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetAll()
        {
            var carts = await _cartService.GetAllCartsAsync();
            return Ok(new ApiResponse<IEnumerable<CartResponseDto>>(carts));
        }

        [HttpGet("current")]
        [Authorize]
        public async Task<IActionResult> GetCurrentUserCart()
        {
            var customerIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(customerIdString, out var customerId))
            {
                return Unauthorized(new ApiResponse<string>("Invalid user identifier."));
            }

            var cart = await _cartService.GetCartsByCustomerAsync(customerId);
            if (cart == null)
            {
                // Trả về cart rỗng nếu user chưa có cart
                return Ok(new ApiResponse<CartResponseDto>(new CartResponseDto
                {
                    Id = 0,
                    CustomerId = customerId,
                    CustomerName = null,
                    TotalAmount = 0,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = null,
                    RowVersion = null,
                    CartDetails = new List<CartDetailResponseDto>()
                }));
            }
            return Ok(new ApiResponse<CartResponseDto>(cart));
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetById(int id)
        {
            var cart = await _cartService.GetCartByIdAsync(id);
            if (cart == null)
                return NotFound(new ApiResponse<string>("Cart not found"));
            return Ok(new ApiResponse<CartResponseDto>(cart));
        }

        [HttpGet("customer/{customerId}")]
        [Authorize]
        public async Task<IActionResult> GetByCustomer(int customerId)
        {
            var cart = await _cartService.GetCartsByCustomerAsync(customerId);
            if (cart == null)
                return NotFound(new ApiResponse<string>("Cart not found"));
            return Ok(new ApiResponse<CartResponseDto>(cart));
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create(CartCreateDto createDto)
        {
            var cart = await _cartService.CreateCartAsync(createDto);
            return CreatedAtAction(nameof(GetById), new { id = cart.Id }, new ApiResponse<CartResponseDto>(cart));
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> Update(int id, CartUpdateDto updateDto)
        {
            var cart = await _cartService.UpdateCartAsync(id, updateDto);
            if (cart == null)
                return NotFound(new ApiResponse<string>("Cart not found"));
            return Ok(new ApiResponse<CartResponseDto>(cart));
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _cartService.DeleteCartAsync(id);
            if (!result)
                return NotFound(new ApiResponse<string>("Cart not found"));
            return Ok(new ApiResponse<string>("Cart deleted successfully"));
        }

        [HttpGet("{id}/items")]
        [Authorize]
        public async Task<IActionResult> GetItems(int id)
        {
            var items = await _cartDetailService.GetCartDetailsByCartAsync(id);
            return Ok(new ApiResponse<IEnumerable<CartDetailResponseDto>>(items));
        }

        [HttpPost("{id}/items")]
        [Authorize]
        public async Task<IActionResult> AddItem(int id, CartDetailCreateDto itemDto)
        {
            var item = await _cartDetailService.CreateCartDetailAsync(itemDto);
            return Ok(new ApiResponse<CartDetailResponseDto>(item));
        }

        [HttpPut("{id}/items/{itemId}")]
        [Authorize]
        public async Task<IActionResult> UpdateItem(int id, int itemId, CartDetailUpdateDto updateDto)
        {
            var item = await _cartDetailService.UpdateCartDetailAsync(itemId, updateDto);
            if (item == null)
                return NotFound(new ApiResponse<string>("Item not found"));
            return Ok(new ApiResponse<CartDetailResponseDto>(item));
        }

        [HttpDelete("{id}/items/{itemId}")]
        [Authorize]
        public async Task<IActionResult> RemoveItem(int id, int itemId)
        {
            var result = await _cartDetailService.DeleteCartDetailAsync(itemId);
            if (!result)
                return NotFound(new ApiResponse<string>("Item not found"));
            return Ok(new ApiResponse<string>("Item removed successfully"));
        }

        [HttpPost("sync")]
        [Authorize]
        public async Task<IActionResult> SyncCart([FromBody] CartSyncDto syncDto)
        {
            var customerIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(customerIdString, out var customerId))
            {
                return Unauthorized(new ApiResponse<string>("Invalid user identifier."));
            }

            try
            {
                var cart = await _cartService.SyncCartAsync(customerId, syncDto);
                return Ok(new ApiResponse<CartResponseDto>(cart));
            }
            catch (DbUpdateConcurrencyException ex)
            {
                return Conflict(new ApiResponse<string>(ex.Message));
            }
            catch (Exception ex)
            {
                // Log the exception
                return BadRequest(new ApiResponse<string>($"An error occurred while syncing the cart: {ex.Message}"));
            }
        }
    }
} 