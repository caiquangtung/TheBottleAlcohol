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

namespace Alcohol.Services;

public class CartService : ICartService
{
    private readonly ICartRepository _cartRepository;
    private readonly ICartDetailRepository _cartDetailRepository;
    private readonly IProductRepository _productRepository;
    private readonly IMapper _mapper;

    public CartService(
        ICartRepository cartRepository,
        ICartDetailRepository cartDetailRepository,
        IProductRepository productRepository,
        IMapper mapper
    )
    {
        _cartRepository = cartRepository;
        _cartDetailRepository = cartDetailRepository;
        _productRepository = productRepository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<CartResponseDto>> GetAllCartsAsync()
    {
        var carts = await _cartRepository.GetAllAsync();
        return _mapper.Map<IEnumerable<CartResponseDto>>(carts);
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

    public async Task<CartResponseDto> SyncCartAsync(int customerId, CartSyncDto syncDto)
    {
        // 1. Get current cart from DB
        var cart = await _cartRepository.GetByCustomerIdAsync(customerId);

        // 2. Create cart if it doesn't exist
        if (cart == null)
        {
            cart = new Cart 
            { 
                CustomerId = customerId, 
                CreatedAt = DateTime.UtcNow
            };
            await _cartRepository.AddAsync(cart);
            // Save the cart first to get the ID
            await _cartRepository.SaveChangesAsync();
        }
        else
        {
            // 3. Concurrency Check for existing cart
            if (syncDto.RowVersion == null || !cart.RowVersion.SequenceEqual(syncDto.RowVersion))
            {
                throw new DbUpdateConcurrencyException("The cart has been modified by another user. Please refresh and try again.");
            }
            
            // Ensure CartDetails is initialized
            if (cart.CartDetails == null)
            {
                cart.CartDetails = new List<CartDetail>();
            }
        }

        // 4. Smart Diff Logic
        var existingItemsMap = cart.CartDetails.ToDictionary(cd => cd.ProductId);
        var itemsDtoMap = syncDto.Items.ToDictionary(i => i.ProductId);
        
        var itemsToAdd = new List<CartDetail>();
        var itemsToUpdate = new List<CartDetail>();
        var itemsToRemove = new List<CartDetail>();

        // Find items to add or update
        foreach (var itemDto in syncDto.Items)
        {
            var product = await _productRepository.GetByIdAsync(itemDto.ProductId);
            if (product == null) continue; // Skip if product doesn't exist

            if (existingItemsMap.TryGetValue(itemDto.ProductId, out var existingItem))
            {
                // Item exists, check for quantity update
                if (existingItem.Quantity != itemDto.Quantity)
                {
                    existingItem.Quantity = Math.Min(itemDto.Quantity, product.StockQuantity);
                    existingItem.UpdatedAt = DateTime.UtcNow;
                    itemsToUpdate.Add(existingItem);
                }
                // Mark as processed
                existingItemsMap.Remove(itemDto.ProductId);
            }
            else
            {
                // Item is new, add it
                itemsToAdd.Add(new CartDetail
                {
                    CartId = cart.Id,
                    ProductId = itemDto.ProductId,
                    Quantity = Math.Min(itemDto.Quantity, product.StockQuantity),
                    Price = product.Price,
                    CreatedAt = DateTime.UtcNow
                });
            }
        }

        // 5. Find items to remove
        itemsToRemove.AddRange(existingItemsMap.Values);

        // 6. Batch DB Operations
        if (itemsToAdd.Any()) 
        {
            await _cartDetailRepository.AddRangeAsync(itemsToAdd);
        }
        if (itemsToUpdate.Any()) 
        {
            _cartDetailRepository.UpdateRange(itemsToUpdate);
        }
        if (itemsToRemove.Any()) 
        {
            _cartDetailRepository.DeleteRange(itemsToRemove);
        }
        
        // 7. Save all changes and update RowVersion
        try
        {
            // Use the same context for all operations
            await _cartDetailRepository.SaveChangesAsync();
        }
        catch(DbUpdateConcurrencyException ex)
        {
            // Re-throw with a more specific message if needed, or handle here
            throw new DbUpdateConcurrencyException("Failed to save changes due to a concurrency conflict.", ex);
        }
        catch (Exception ex)
        {
            // Log the detailed exception for debugging
            var errorDetails = $"Items to add: {itemsToAdd.Count}, Items to update: {itemsToUpdate.Count}, Items to remove: {itemsToRemove.Count}. ";
            throw new InvalidOperationException($"{errorDetails}Failed to save cart changes: {ex.Message}", ex);
        }

        // 8. Return updated cart with new RowVersion
        var updatedCart = await _cartRepository.GetCartWithDetailsAsync(cart.Id);
        if (updatedCart == null)
        {
            throw new InvalidOperationException("Failed to retrieve updated cart after sync operation.");
        }
        
        // Ensure CartDetails is initialized
        if (updatedCart.CartDetails == null)
        {
            updatedCart.CartDetails = new List<CartDetail>();
        }
        
        return _mapper.Map<CartResponseDto>(updatedCart);
    }
} 