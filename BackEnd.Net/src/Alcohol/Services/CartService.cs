using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Alcohol.DTOs.Cart;
using Alcohol.Models;
using Alcohol.Repositories.Interfaces;
using Alcohol.Services.Interfaces;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Alcohol.DTOs;

namespace Alcohol.Services;

public class CartService : ICartService
{
    private readonly ICartRepository _cartRepository;
    private readonly ICartDetailRepository _cartDetailRepository;
    private readonly IProductRepository _productRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<CartService> _logger;

    public CartService(
        ICartRepository cartRepository,
        ICartDetailRepository cartDetailRepository,
        IProductRepository productRepository,
        IMapper mapper,
        ILogger<CartService> logger
    )
    {
        _cartRepository = cartRepository;
        _cartDetailRepository = cartDetailRepository;
        _productRepository = productRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<PagedResult<CartResponseDto>> GetAllCartsAsync(CartFilterDto filter)
    {
        var carts = await _cartRepository.GetAllAsync();
        
        // Apply filters
        var filteredCarts = carts.AsQueryable();
        
        if (!string.IsNullOrWhiteSpace(filter.SearchTerm))
        {
            filteredCarts = filteredCarts.Where(c => 
                c.Customer != null && c.Customer.FullName.Contains(filter.SearchTerm));
        }
        
        if (filter.CustomerId.HasValue)
        {
            filteredCarts = filteredCarts.Where(c => c.CustomerId == filter.CustomerId.Value);
        }
        
        if (filter.StartDate.HasValue)
        {
            filteredCarts = filteredCarts.Where(c => c.CreatedAt >= filter.StartDate.Value);
        }
        
        if (filter.EndDate.HasValue)
        {
            filteredCarts = filteredCarts.Where(c => c.CreatedAt <= filter.EndDate.Value);
        }
        
        if (filter.MinTotal.HasValue)
        {
            filteredCarts = filteredCarts.Where(c => c.TotalAmount >= filter.MinTotal.Value);
        }
        
        if (filter.MaxTotal.HasValue)
        {
            filteredCarts = filteredCarts.Where(c => c.TotalAmount <= filter.MaxTotal.Value);
        }
        
        // Apply sorting
        if (!string.IsNullOrWhiteSpace(filter.SortBy))
        {
            filteredCarts = filter.SortBy.ToLower() switch
            {
                "totalamount" => filter.SortOrder?.ToLower() == "desc" 
                    ? filteredCarts.OrderByDescending(c => c.TotalAmount)
                    : filteredCarts.OrderBy(c => c.TotalAmount),
                "createdat" => filter.SortOrder?.ToLower() == "desc"
                    ? filteredCarts.OrderByDescending(c => c.CreatedAt)
                    : filteredCarts.OrderBy(c => c.CreatedAt),
                _ => filteredCarts.OrderBy(c => c.Id)
            };
        }
        else
        {
            filteredCarts = filteredCarts.OrderBy(c => c.Id);
        }
        
        var totalRecords = filteredCarts.Count();
        var pagedCarts = filteredCarts
            .Skip((filter.PageNumber - 1) * filter.PageSize)
            .Take(filter.PageSize)
            .ToList();
        
        var cartDtos = _mapper.Map<List<CartResponseDto>>(pagedCarts);
        return new PagedResult<CartResponseDto>(cartDtos, totalRecords, filter.PageNumber, filter.PageSize);
    }

    public async Task<CartResponseDto> GetCartByIdAsync(int id)
    {
        var cart = await _cartRepository.GetByIdAsync(id);
        if (cart == null)
            return null;

        return _mapper.Map<CartResponseDto>(cart);
    }

    public async Task<CartResponseDto> GetCartsByCustomerAsync(int customerId)
    {
        var cart = await _cartRepository.GetByCustomerIdAsync(customerId);
        if (cart == null)
            return null;
            
        return _mapper.Map<CartResponseDto>(cart);
    }

    public async Task<CartResponseDto> CreateCartAsync(CartCreateDto createDto)
    {
        var cart = _mapper.Map<Cart>(createDto);
        cart.CreatedAt = DateTime.UtcNow;

        await _cartRepository.AddAsync(cart);
        await _cartRepository.SaveChangesAsync();

        return _mapper.Map<CartResponseDto>(cart);
    }

    public async Task<CartResponseDto> UpdateCartAsync(int id, CartUpdateDto updateDto)
    {
        var cart = await _cartRepository.GetByIdAsync(id);
        if (cart == null)
            return null;

        _mapper.Map(updateDto, cart);
        cart.UpdatedAt = DateTime.UtcNow;

        _cartRepository.Update(cart);
        await _cartRepository.SaveChangesAsync();

        return _mapper.Map<CartResponseDto>(cart);
    }

    public async Task<bool> DeleteCartAsync(int id)
    {
        var cart = await _cartRepository.GetByIdAsync(id);
        if (cart == null)
            return false;

        _cartRepository.Delete(cart);
        await _cartRepository.SaveChangesAsync();
        return true;
    }

    public async Task<CartResponseDto> SyncCartAsync(CartSyncDto syncDto)
    {
        var existingCart = await _cartRepository.GetByCustomerIdAsync(syncDto.CustomerId);
        
        if (existingCart == null)
        {
            // Create new cart
            var newCart = new Cart
            {
                CustomerId = syncDto.CustomerId,
                CreatedAt = DateTime.UtcNow
            };
            
            await _cartRepository.AddAsync(newCart);
            await _cartRepository.SaveChangesAsync();
            existingCart = newCart;
        }

		// Sync cart details (upsert + delete missing)
		var incomingProductIds = new HashSet<int>(syncDto.Items.Select(i => i.ProductId));

		// Upsert items from client
		foreach (var item in syncDto.Items)
		{
			var product = await _productRepository.GetByIdAsync(item.ProductId);
			if (product == null) continue;

			var existingDetail = existingCart.CartDetails?.FirstOrDefault(cd => cd.ProductId == item.ProductId);
			
			// If client sets quantity <= 0, treat as deletion
			if (item.Quantity <= 0)
			{
				if (existingDetail != null)
				{
					_cartDetailRepository.Delete(existingDetail);
				}
				continue;
			}

			if (existingDetail != null)
			{
				// Update existing item
				existingDetail.Quantity = item.Quantity;
				existingDetail.UpdatedAt = DateTime.UtcNow;
			}
			else
			{
				// Add new item
				var newDetail = new CartDetail
				{
					CartId = existingCart.Id,
					ProductId = item.ProductId,
					Quantity = item.Quantity,
					Price = product.Price,
					CreatedAt = DateTime.UtcNow
				};
				
				existingCart.CartDetails?.Add(newDetail);
			}
		}

		// Delete any items on the server that are not present in client payload
		var itemsToRemove = existingCart.CartDetails?
			.Where(cd => !incomingProductIds.Contains(cd.ProductId))
			.ToList();
		if (itemsToRemove != null && itemsToRemove.Count > 0)
		{
			_cartDetailRepository.DeleteRange(itemsToRemove);
		}

        await _cartRepository.SaveChangesAsync();
        
        // Recalculate total
        var updatedCart = await _cartRepository.GetByIdAsync(existingCart.Id);
        return _mapper.Map<CartResponseDto>(updatedCart);
    }

    public async Task<bool> ClearCartByCustomerAsync(int customerId)
    {
        try
        {
            var cart = await _cartRepository.GetByCustomerIdAsync(customerId);
            if (cart == null)
                return true; // No cart to clear

            // Clear all cart details
            if (cart.CartDetails != null && cart.CartDetails.Any())
            {
                _cartDetailRepository.DeleteRange(cart.CartDetails.ToList());
            }

            await _cartRepository.SaveChangesAsync();
            _logger.LogInformation("Cleared cart for customer {CustomerId}", customerId);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error clearing cart for customer {CustomerId}", customerId);
            return false;
        }
    }
} 