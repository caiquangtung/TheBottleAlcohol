using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Alcohol.DTOs.Product;
using Alcohol.Models;
using Alcohol.Repositories.Interfaces;
using Microsoft.Extensions.Logging;

namespace Alcohol.Services;

/// <summary>
/// Service for profit analysis and margin calculations
/// </summary>
public class ProfitAnalysisService
{
    private readonly IProductRepository _productRepository;
    private readonly IInventoryRepository _inventoryRepository;
    private readonly ILogger<ProfitAnalysisService> _logger;

    public ProfitAnalysisService(
        IProductRepository productRepository,
        IInventoryRepository inventoryRepository,
        ILogger<ProfitAnalysisService> logger)
    {
        _productRepository = productRepository;
        _inventoryRepository = inventoryRepository;
        _logger = logger;
    }

    /// <summary>
    /// Get profit analysis for a specific product
    /// </summary>
    public async Task<ProfitAnalysisDto> GetProductProfitAnalysisAsync(int productId)
    {
        var product = await _productRepository.GetByIdAsync(productId);
        if (product == null)
            throw new Exception($"Product {productId} not found");

        var inventory = await _inventoryRepository.GetByProductIdAsync(productId);
        if (inventory == null)
            throw new Exception($"Inventory not found for product {productId}");

        return CalculateProfitAnalysis(product, inventory);
    }

    /// <summary>
    /// Get profit analysis for all products
    /// </summary>
    public async Task<List<ProfitAnalysisDto>> GetAllProductsProfitAnalysisAsync()
    {
        var products = await _productRepository.GetAllAsync();
        var inventories = await _inventoryRepository.GetAllAsync();
        
        var inventoryDict = inventories.ToDictionary(i => i.ProductId);
        var results = new List<ProfitAnalysisDto>();

        foreach (var product in products)
        {
            if (inventoryDict.TryGetValue(product.Id, out var inventory))
            {
                results.Add(CalculateProfitAnalysis(product, inventory));
            }
        }

        return results.OrderByDescending(r => r.ProfitMarginPercentage).ToList();
    }

    /// <summary>
    /// Get profit summary report
    /// </summary>
    public async Task<ProfitSummaryDto> GetProfitSummaryAsync()
    {
        var analyses = await GetAllProductsProfitAnalysisAsync();
        
        if (!analyses.Any())
        {
            return new ProfitSummaryDto
            {
                TopProfitableProducts = new List<ProfitAnalysisDto>(),
                LowProfitProducts = new List<ProfitAnalysisDto>()
            };
        }

        return new ProfitSummaryDto
        {
            TotalInventoryValue = analyses.Sum(a => a.AverageCost * a.CurrentStock),
            TotalPotentialRevenue = analyses.Sum(a => a.SellingPrice * a.CurrentStock),
            TotalPotentialProfit = analyses.Sum(a => a.PotentialTotalProfit),
            AverageMarginPercentage = analyses.Average(a => a.ProfitMarginPercentage),
            ProductsAboveTargetMargin = analyses.Count(a => a.IsMarginAchieved),
            ProductsBelowTargetMargin = analyses.Count(a => !a.IsMarginAchieved),
            TopProfitableProducts = analyses.OrderByDescending(a => a.ProfitMarginPercentage).Take(10).ToList(),
            LowProfitProducts = analyses.Where(a => a.ProfitMarginPercentage < 10).OrderBy(a => a.ProfitMarginPercentage).ToList()
        };
    }

    /// <summary>
    /// Set target margin percentage for a product
    /// </summary>
    public async Task<ProfitAnalysisDto> SetTargetMarginAsync(int productId, decimal targetMarginPercentage)
    {
        if (targetMarginPercentage < 0 || targetMarginPercentage > 100)
            throw new ArgumentException("Target margin percentage must be between 0 and 100");

        var product = await _productRepository.GetByIdAsync(productId);
        if (product == null)
            throw new Exception($"Product {productId} not found");

        product.TargetMarginPercentage = targetMarginPercentage;
        product.UpdatedAt = DateTime.UtcNow;
        
        _productRepository.Update(product);
        await _productRepository.SaveChangesAsync();

        _logger.LogInformation("Updated target margin for Product {ProductId}: {TargetMargin}%", 
            productId, targetMarginPercentage);

        return await GetProductProfitAnalysisAsync(productId);
    }

    /// <summary>
    /// Calculate recommended selling price based on target margin
    /// </summary>
    public async Task<decimal> CalculateRecommendedPriceAsync(int productId, decimal targetMarginPercentage)
    {
        var inventory = await _inventoryRepository.GetByProductIdAsync(productId);
        if (inventory == null)
            throw new Exception($"Inventory not found for product {productId}");

        if (inventory.AverageCost <= 0)
            throw new Exception($"Average cost not available for product {productId}");

        // Target Price = Average Cost / (1 - Target Margin%)
        var recommendedPrice = inventory.AverageCost / (1 - (targetMarginPercentage / 100));
        
        return Math.Round(recommendedPrice, 0); // Round to nearest VND
    }

    /// <summary>
    /// Update product price based on target margin
    /// </summary>
    public async Task<ProfitAnalysisDto> UpdatePriceByTargetMarginAsync(int productId, decimal targetMarginPercentage)
    {
        var product = await _productRepository.GetByIdAsync(productId);
        if (product == null)
            throw new Exception($"Product {productId} not found");

        var recommendedPrice = await CalculateRecommendedPriceAsync(productId, targetMarginPercentage);
        
        var oldPrice = product.Price;
        product.Price = recommendedPrice;
        product.TargetMarginPercentage = targetMarginPercentage;
        product.UpdatedAt = DateTime.UtcNow;
        
        _productRepository.Update(product);
        await _productRepository.SaveChangesAsync();

        _logger.LogInformation("Updated price for Product {ProductId}: {OldPrice} -> {NewPrice} (Target: {TargetMargin}%)", 
            productId, oldPrice, recommendedPrice, targetMarginPercentage);

        return await GetProductProfitAnalysisAsync(productId);
    }

    /// <summary>
    /// Get products with low profit margins
    /// </summary>
    public async Task<List<ProfitAnalysisDto>> GetLowProfitProductsAsync(decimal thresholdPercentage = 15.0m)
    {
        var analyses = await GetAllProductsProfitAnalysisAsync();
        return analyses.Where(a => a.ProfitMarginPercentage < thresholdPercentage).ToList();
    }

    /// <summary>
    /// Get products exceeding target margins
    /// </summary>
    public async Task<List<ProfitAnalysisDto>> GetHighPerformingProductsAsync()
    {
        var analyses = await GetAllProductsProfitAnalysisAsync();
        return analyses.Where(a => a.IsMarginAchieved && a.ProfitMarginPercentage > a.TargetMarginPercentage + 5).ToList();
    }

    /// <summary>
    /// Calculate profit analysis for a product
    /// </summary>
    private ProfitAnalysisDto CalculateProfitAnalysis(Product product, Inventory inventory)
    {
        var profitPerUnit = product.Price - inventory.AverageCost;
        var profitMarginPercentage = product.Price > 0 
            ? (profitPerUnit / product.Price) * 100 
            : 0;

        var recommendedPrice = inventory.AverageCost > 0 && product.TargetMarginPercentage > 0
            ? inventory.AverageCost / (1 - (product.TargetMarginPercentage / 100))
            : product.Price;

        return new ProfitAnalysisDto
        {
            ProductId = product.Id,
            ProductName = product.Name,
            AverageCost = inventory.AverageCost,
            SellingPrice = product.Price,
            ProfitPerUnit = profitPerUnit,
            ProfitMarginPercentage = Math.Round(profitMarginPercentage, 2),
            TargetMarginPercentage = product.TargetMarginPercentage,
            RecommendedSellingPrice = Math.Round(recommendedPrice, 0),
            IsMarginAchieved = profitMarginPercentage >= product.TargetMarginPercentage,
            CurrentStock = inventory.Quantity,
            PotentialTotalProfit = profitPerUnit * inventory.Quantity
        };
    }
}
