using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Alcohol.DTOs.Product;
using Alcohol.DTOs.Discount;
using Alcohol.Repositories.Interfaces;
using Alcohol.Services.Interfaces;
using AutoMapper;
using Microsoft.Extensions.Logging;

namespace Alcohol.Services;

public class ProductDiscountService : IProductDiscountService
{
    private readonly IProductRepository _productRepository;
    private readonly IDiscountRepository _discountRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<ProductDiscountService> _logger;

    public ProductDiscountService(
        IProductRepository productRepository,
        IDiscountRepository discountRepository,
        IMapper mapper,
        ILogger<ProductDiscountService> logger)
    {
        _productRepository = productRepository;
        _discountRepository = discountRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<ProductResponseDto> GetProductWithDiscountAsync(int productId)
    {
        try
        {
            var product = await _productRepository.GetByIdWithDetailsAsync(productId);
            if (product == null)
            {
                _logger.LogWarning("Product with ID {ProductId} not found", productId);
                return null;
            }

            var productDto = _mapper.Map<ProductResponseDto>(product);
            return await ApplyDiscountToProductAsync(productDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting product with discount for ID {ProductId}", productId);
            throw;
        }
    }

    public async Task<List<ProductResponseDto>> GetProductsWithDiscountAsync()
    {
        try
        {
            var products = await _productRepository.GetAllAsync();
            var productDtos = _mapper.Map<List<ProductResponseDto>>(products);
            
            var result = new List<ProductResponseDto>();
            foreach (var productDto in productDtos)
            {
                var productWithDiscount = await ApplyDiscountToProductAsync(productDto);
                result.Add(productWithDiscount);
            }
            
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting products with discount");
            throw;
        }
    }

    public async Task<decimal> CalculateDiscountedPriceAsync(int productId)
    {
        try
        {
            var product = await _productRepository.GetByIdAsync(productId);
            if (product == null)
            {
                _logger.LogWarning("Product with ID {ProductId} not found", productId);
                return 0;
            }

            var activeDiscounts = await GetActiveDiscountsForProductAsync(productId);

            if (!activeDiscounts.Any())
                return product.Price;

            // Apply best discount (highest discount amount)
            var bestDiscount = activeDiscounts.OrderByDescending(d => d.DiscountAmount).First();
            var discountedPrice = Math.Max(0, product.Price - bestDiscount.DiscountAmount);
            
            _logger.LogDebug("Calculated discounted price for Product {ProductId}: {OriginalPrice} -> {DiscountedPrice}", 
                productId, product.Price, discountedPrice);
                
            return discountedPrice;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calculating discounted price for Product {ProductId}", productId);
            throw;
        }
    }

    public async Task<List<DiscountResponseDto>> GetActiveDiscountsForProductAsync(int productId)
    {
        try
        {
            var now = DateTime.UtcNow;
            var discounts = await _discountRepository.GetActiveDiscountsAsync();

            var applicableDiscounts = discounts
                .Where(d => d.Products.Any(p => p.Id == productId))
                .ToList();

            return _mapper.Map<List<DiscountResponseDto>>(applicableDiscounts);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting active discounts for Product {ProductId}", productId);
            throw;
        }
    }

    public async Task<bool> IsDiscountApplicableAsync(int discountId, int productId)
    {
        try
        {
            var discount = await _discountRepository.GetByIdAsync(discountId);
            if (discount == null || !discount.IsActive)
            {
                _logger.LogDebug("Discount {DiscountId} not found or not active", discountId);
                return false;
            }

            var now = DateTime.UtcNow;
            if (now < discount.StartDate || now > discount.EndDate)
            {
                _logger.LogDebug("Discount {DiscountId} is not within valid date range", discountId);
                return false;
            }

            var isApplicable = discount.Products.Any(p => p.Id == productId);
            _logger.LogDebug("Discount {DiscountId} applicable to Product {ProductId}: {IsApplicable}", 
                discountId, productId, isApplicable);
                
            return isApplicable;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking if discount {DiscountId} is applicable to Product {ProductId}", 
                discountId, productId);
            throw;
        }
    }

    public async Task<ProductResponseDto> ApplyDiscountToProductAsync(ProductResponseDto product)
    {
        try
        {
            var activeDiscounts = await GetActiveDiscountsForProductAsync(product.Id);
            
            product.OriginalPrice = product.Price;
            product.ActiveDiscounts = activeDiscounts;
            product.HasDiscount = activeDiscounts.Any();

            if (product.HasDiscount)
            {
                var bestDiscount = activeDiscounts.OrderByDescending(d => d.DiscountAmount).First();
                product.DiscountedPrice = Math.Max(0, product.Price - bestDiscount.DiscountAmount);
                product.DiscountAmount = bestDiscount.DiscountAmount;
                product.DiscountPercentage = product.Price > 0 
                    ? Math.Round((bestDiscount.DiscountAmount / product.Price) * 100, 2) 
                    : 0;
                
                _logger.LogDebug("Applied discount to Product {ProductId}: {OriginalPrice} -> {DiscountedPrice} (-{DiscountAmount})", 
                    product.Id, product.OriginalPrice, product.DiscountedPrice, product.DiscountAmount);
            }
            else
            {
                product.DiscountedPrice = null;
                product.DiscountAmount = null;
                product.DiscountPercentage = null;
            }

            return product;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error applying discount to Product {ProductId}", product.Id);
            throw;
        }
    }
}
