using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Alcohol.DTOs.DataMigration;
using Alcohol.Models;
using Alcohol.Repositories.Interfaces;
using Microsoft.Extensions.Logging;

namespace Alcohol.Services;

/// <summary>
/// Service for data migration and cleanup tasks
/// </summary>
public class DataMigrationService
{
    private readonly IInventoryRepository _inventoryRepository;
    private readonly IImportOrderDetailRepository _importOrderDetailRepository;
    private readonly IProductRepository _productRepository;
    private readonly ILogger<DataMigrationService> _logger;

    public DataMigrationService(
        IInventoryRepository inventoryRepository,
        IImportOrderDetailRepository importOrderDetailRepository,
        IProductRepository productRepository,
        ILogger<DataMigrationService> logger)
    {
        _inventoryRepository = inventoryRepository;
        _importOrderDetailRepository = importOrderDetailRepository;
        _productRepository = productRepository;
        _logger = logger;
    }

    /// <summary>
    /// Recalculate AverageCost for all inventories based on import history
    /// </summary>
    public async Task RecalculateAllAverageCostsAsync()
    {
        _logger.LogInformation("Starting AverageCost recalculation for all inventories");
        
        var inventories = await _inventoryRepository.GetAllAsync();
        var updatedCount = 0;

        foreach (var inventory in inventories)
        {
            try
            {
                var averageCost = await CalculateAverageCostForProductAsync(inventory.ProductId);
                
                if (averageCost != inventory.AverageCost)
                {
                    inventory.AverageCost = averageCost;
                    inventory.TotalValue = averageCost * inventory.Quantity;
                    inventory.UpdatedAt = DateTime.UtcNow;
                    inventory.LastUpdated = DateTime.UtcNow;
                    
                    await _inventoryRepository.UpdateAsync(inventory);
                    updatedCount++;
                    
                    _logger.LogInformation("Updated AverageCost for Product {ProductId}: {OldCost} -> {NewCost}", 
                        inventory.ProductId, inventory.AverageCost, averageCost);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to recalculate AverageCost for Product {ProductId}", inventory.ProductId);
            }
        }

        _logger.LogInformation("AverageCost recalculation completed. Updated {UpdatedCount} out of {TotalCount} inventories", 
            updatedCount, inventories.Count);
    }

    /// <summary>
    /// Calculate average cost for a product based on import history
    /// </summary>
    private async Task<decimal> CalculateAverageCostForProductAsync(int productId)
    {
        // Get all completed import order details for this product
        var importDetails = await _importOrderDetailRepository.GetByProductIdAsync(productId);
        var completedImports = importDetails.Where(d => d.Status == Models.Enums.ImportOrderStatusType.Completed).ToList();

        if (!completedImports.Any())
        {
            _logger.LogWarning("No completed imports found for Product {ProductId}", productId);
            return 0;
        }

        // Calculate weighted average cost
        var totalCost = completedImports.Sum(d => d.ImportPrice * d.Quantity);
        var totalQuantity = completedImports.Sum(d => d.Quantity);

        return totalQuantity > 0 ? totalCost / totalQuantity : 0;
    }

    /// <summary>
    /// Sync all Product.StockQuantity with Inventory.Quantity
    /// </summary>
    public async Task SyncAllProductStockQuantitiesAsync()
    {
        _logger.LogInformation("Starting Product.StockQuantity synchronization");
        
        var inventories = await _inventoryRepository.GetAllAsync();
        var updatedCount = 0;

        foreach (var inventory in inventories)
        {
            try
            {
                var product = await _productRepository.GetByIdAsync(inventory.ProductId);
                if (product != null && product.StockQuantity != inventory.Quantity)
                {
                    var oldQuantity = product.StockQuantity;
                    product.StockQuantity = inventory.Quantity;
                    product.UpdatedAt = DateTime.UtcNow;
                    
                    _productRepository.Update(product);
                    await _productRepository.SaveChangesAsync();
                    updatedCount++;
                    
                    _logger.LogInformation("Synced StockQuantity for Product {ProductId}: {OldQuantity} -> {NewQuantity}", 
                        product.Id, oldQuantity, inventory.Quantity);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to sync StockQuantity for Product {ProductId}", inventory.ProductId);
            }
        }

        _logger.LogInformation("Product.StockQuantity synchronization completed. Updated {UpdatedCount} products", updatedCount);
    }

    /// <summary>
    /// Create missing inventory records for products that don't have them
    /// </summary>
    public async Task CreateMissingInventoryRecordsAsync()
    {
        _logger.LogInformation("Creating missing inventory records");
        
        var products = await _productRepository.GetAllAsync();
        var inventories = await _inventoryRepository.GetAllAsync();
        var existingProductIds = inventories.Select(i => i.ProductId).ToHashSet();
        var createdCount = 0;

        foreach (var product in products)
        {
            if (!existingProductIds.Contains(product.Id))
            {
                try
                {
                    var inventory = new Inventory
                    {
                        ProductId = product.Id,
                        Quantity = product.StockQuantity,
                        ReorderLevel = 10, // Default
                        AverageCost = 0, // Will be calculated later
                        TotalValue = 0,
                        CreatedAt = DateTime.UtcNow,
                        LastUpdated = DateTime.UtcNow
                    };

                    await _inventoryRepository.AddAsync(inventory);
                    createdCount++;
                    
                    _logger.LogInformation("Created inventory record for Product {ProductId}", product.Id);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to create inventory record for Product {ProductId}", product.Id);
                }
            }
        }

        _logger.LogInformation("Created {CreatedCount} missing inventory records", createdCount);
    }

    /// <summary>
    /// Run all data migration tasks
    /// </summary>
    public async Task RunAllMigrationsAsync()
    {
        _logger.LogInformation("Starting all data migrations");
        
        try
        {
            await CreateMissingInventoryRecordsAsync();
            await RecalculateAllAverageCostsAsync();
            await SyncAllProductStockQuantitiesAsync();
            
            _logger.LogInformation("All data migrations completed successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Data migration failed");
            throw;
        }
    }

    /// <summary>
    /// Get migration status report
    /// </summary>
    public async Task<DataMigrationReport> GetMigrationReportAsync()
    {
        var products = await _productRepository.GetAllAsync();
        var inventories = await _inventoryRepository.GetAllAsync();
        
        var productsWithInventory = inventories.Count;
        var productsWithoutInventory = products.Count() - productsWithInventory;
        var inventoriesWithZeroAverageCost = inventories.Count(i => i.AverageCost == 0);
        var inventoriesWithMismatchedQuantity = 0;

        // Check for quantity mismatches
        foreach (var inventory in inventories)
        {
            var product = products.FirstOrDefault(p => p.Id == inventory.ProductId);
            if (product != null && product.StockQuantity != inventory.Quantity)
            {
                inventoriesWithMismatchedQuantity++;
            }
        }

        return new DataMigrationReport
        {
            TotalProducts = products.Count(),
            ProductsWithInventory = productsWithInventory,
            ProductsWithoutInventory = productsWithoutInventory,
            TotalInventories = inventories.Count,
            InventoriesWithZeroAverageCost = inventoriesWithZeroAverageCost,
            InventoriesWithMismatchedQuantity = inventoriesWithMismatchedQuantity,
            TotalInventoryValue = inventories.Sum(i => i.TotalValue)
        };
    }
}

