using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Alcohol.DTOs.Inventory;
using Alcohol.Models;
using Alcohol.Models.Enums;
using Alcohol.Repositories.Interfaces;
using Alcohol.Services.Interfaces;
using AutoMapper;
using Alcohol.DTOs;
using Alcohol.Repositories.IRepositories;

namespace Alcohol.Services;

public class InventoryService : IInventoryService
{
    private readonly IInventoryRepository _inventoryRepository;
    private readonly IInventoryTransactionRepository _transactionRepository;
    private readonly IProductRepository _productRepository;
    private readonly IMapper _mapper;

    public InventoryService(
        IInventoryRepository inventoryRepository,
        IInventoryTransactionRepository transactionRepository,
        IProductRepository productRepository,
        IMapper mapper)
    {
        _inventoryRepository = inventoryRepository;
        _transactionRepository = transactionRepository;
        _productRepository = productRepository;
        _mapper = mapper;
    }

    public async Task<PagedResult<InventoryResponseDto>> GetAllInventoriesAsync(InventoryFilterDto filter)
    {
        var inventories = await _inventoryRepository.GetAllAsync();
        
        // Apply filters
        var filteredInventories = inventories.AsQueryable();
        
        if (!string.IsNullOrWhiteSpace(filter.SearchTerm))
        {
            filteredInventories = filteredInventories.Where(i => 
                i.Product != null && i.Product.Name.Contains(filter.SearchTerm));
        }
        
        if (filter.ProductId.HasValue)
        {
            filteredInventories = filteredInventories.Where(i => i.ProductId == filter.ProductId.Value);
        }
        
        if (filter.MinQuantity.HasValue)
        {
            filteredInventories = filteredInventories.Where(i => i.Quantity >= filter.MinQuantity.Value);
        }
        
        if (filter.MaxQuantity.HasValue)
        {
            filteredInventories = filteredInventories.Where(i => i.Quantity <= filter.MaxQuantity.Value);
        }
        
        if (filter.MinReorderLevel.HasValue)
        {
            filteredInventories = filteredInventories.Where(i => i.ReorderLevel >= filter.MinReorderLevel.Value);
        }
        
        if (filter.MaxReorderLevel.HasValue)
        {
            filteredInventories = filteredInventories.Where(i => i.ReorderLevel <= filter.MaxReorderLevel.Value);
        }
        
        // Apply sorting
        if (!string.IsNullOrWhiteSpace(filter.SortBy))
        {
            filteredInventories = filter.SortBy.ToLower() switch
            {
                "quantity" => filter.SortOrder?.ToLower() == "desc" 
                    ? filteredInventories.OrderByDescending(i => i.Quantity)
                    : filteredInventories.OrderBy(i => i.Quantity),
                "reorderlevel" => filter.SortOrder?.ToLower() == "desc"
                    ? filteredInventories.OrderByDescending(i => i.ReorderLevel)
                    : filteredInventories.OrderBy(i => i.ReorderLevel),
                "createdat" => filter.SortOrder?.ToLower() == "desc"
                    ? filteredInventories.OrderByDescending(i => i.CreatedAt)
                    : filteredInventories.OrderBy(i => i.CreatedAt),
                _ => filteredInventories.OrderBy(i => i.Id)
            };
        }
        else
        {
            filteredInventories = filteredInventories.OrderBy(i => i.Id);
        }
        
        var totalRecords = filteredInventories.Count();
        var pagedInventories = filteredInventories
            .Skip((filter.PageNumber - 1) * filter.PageSize)
            .Take(filter.PageSize)
            .ToList();
        
        var inventoryDtos = _mapper.Map<List<InventoryResponseDto>>(pagedInventories);
        return new PagedResult<InventoryResponseDto>(inventoryDtos, totalRecords, filter.PageNumber, filter.PageSize);
    }

    public async Task<InventoryResponseDto> GetInventoryByIdAsync(int id)
    {
        var inventory = await _inventoryRepository.GetByIdAsync(id);
        if (inventory == null)
            return null;

        return _mapper.Map<InventoryResponseDto>(inventory);
    }

    public async Task<InventoryResponseDto> GetInventoryByProductAsync(int productId)
    {
        var inventory = await _inventoryRepository.GetByProductIdAsync(productId);
        if (inventory == null)
            return null;
            
        return _mapper.Map<InventoryResponseDto>(inventory);
    }

    public async Task<InventoryResponseDto> CreateInventoryAsync(InventoryCreateDto createDto)
    {
        var inventory = _mapper.Map<Inventory>(createDto);
        inventory.CreatedAt = DateTime.UtcNow;
        inventory.LastUpdated = DateTime.UtcNow;

        await _inventoryRepository.AddAsync(inventory);
        await _inventoryRepository.SaveChangesAsync();

        return _mapper.Map<InventoryResponseDto>(inventory);
    }

    public async Task<InventoryResponseDto> UpdateInventoryAsync(int id, InventoryUpdateDto updateDto)
    {
        var inventory = await _inventoryRepository.GetByIdAsync(id);
        if (inventory == null)
            return null;

        _mapper.Map(updateDto, inventory);
        inventory.UpdatedAt = DateTime.UtcNow;
        inventory.LastUpdated = DateTime.UtcNow;

        await _inventoryRepository.UpdateAsync(inventory);
        return _mapper.Map<InventoryResponseDto>(inventory);
    }

    public async Task<bool> UpdateStockAsync(int id, int quantity)
    {
        var inventory = await _inventoryRepository.GetByIdAsync(id);
        if (inventory == null)
            return false;

        inventory.Quantity = quantity;
        inventory.TotalValue = inventory.AverageCost * inventory.Quantity;
        inventory.UpdatedAt = DateTime.UtcNow;
        inventory.LastUpdated = DateTime.UtcNow;

        await _inventoryRepository.UpdateAsync(inventory);
        
        // Sync Product.StockQuantity
        await SyncProductStockQuantityAsync(inventory.ProductId, inventory.Quantity);
        
        return true;
    }

    public async Task<bool> AddStockAsync(int id, int quantity)
    {
        var inventory = await _inventoryRepository.GetByIdAsync(id);
        if (inventory == null)
            return false;

        inventory.Quantity += quantity;
        inventory.TotalValue = inventory.AverageCost * inventory.Quantity;
        inventory.UpdatedAt = DateTime.UtcNow;
        inventory.LastUpdated = DateTime.UtcNow;

        await _inventoryRepository.UpdateAsync(inventory);
        
        // Sync Product.StockQuantity
        await SyncProductStockQuantityAsync(inventory.ProductId, inventory.Quantity);
        
        return true;
    }

    public async Task<bool> RemoveStockAsync(int id, int quantity)
    {
        var inventory = await _inventoryRepository.GetByIdAsync(id);
        if (inventory == null || inventory.Quantity < quantity)
            return false;

        inventory.Quantity -= quantity;
        inventory.TotalValue = inventory.AverageCost * inventory.Quantity;
        inventory.UpdatedAt = DateTime.UtcNow;
        inventory.LastUpdated = DateTime.UtcNow;

        await _inventoryRepository.UpdateAsync(inventory);
        
        // Sync Product.StockQuantity
        await SyncProductStockQuantityAsync(inventory.ProductId, inventory.Quantity);
        
        return true;
    }

    public async Task<bool> DeleteInventoryAsync(int id)
    {
        var inventory = await _inventoryRepository.GetByIdAsync(id);
        if (inventory == null)
            return false;

        await _inventoryRepository.DeleteAsync(id);
        return true;
    }

    public async Task AdjustStockAsync(int productId, int quantity, string notes = null)
    {
        var inventory = await _inventoryRepository.GetByProductIdAsync(productId);
        if (inventory == null)
        {
            throw new Exception($"Inventory not found for product {productId}");
        }

        inventory.Quantity += quantity;
        inventory.TotalValue = inventory.AverageCost * inventory.Quantity;
        inventory.UpdatedAt = DateTime.UtcNow;
        inventory.LastUpdated = DateTime.UtcNow;
        
        await _inventoryRepository.UpdateAsync(inventory);

        // Sync Product.StockQuantity
        await SyncProductStockQuantityAsync(productId, inventory.Quantity);

        // Create transaction record
        var transaction = new InventoryTransaction
        {
            ProductId = productId,
            Quantity = quantity,
            TransactionType = InventoryTransactionType.Adjustment,
            ReferenceType = ReferenceType.Manual,
            Notes = notes,
            Status = InventoryTransactionStatusType.Completed
        };

        await _transactionRepository.AddAsync(transaction);
    }

    public async Task ImportStockAsync(int productId, int quantity, int importOrderId, string notes = null)
    {
        var inventory = await _inventoryRepository.GetByProductIdAsync(productId);
        if (inventory == null)
        {
            throw new Exception($"Inventory not found for product {productId}");
        }

        inventory.Quantity += quantity;
        inventory.UpdatedAt = DateTime.UtcNow;
        inventory.LastUpdated = DateTime.UtcNow;
        
        // Update TotalValue if AverageCost is set
        if (inventory.AverageCost > 0)
        {
            inventory.TotalValue = inventory.AverageCost * inventory.Quantity;
        }

        await _inventoryRepository.UpdateAsync(inventory);

        // Sync Product.StockQuantity with Inventory.Quantity
        await SyncProductStockQuantityAsync(productId, inventory.Quantity);

        // Create transaction record
        var transaction = new InventoryTransaction
        {
            ProductId = productId,
            Quantity = quantity,
            TransactionType = InventoryTransactionType.Import,
            ReferenceType = ReferenceType.ImportOrder,
            ReferenceId = importOrderId,
            Notes = notes,
            Status = InventoryTransactionStatusType.Completed
        };

        await _transactionRepository.AddAsync(transaction);
    }

    /// <summary>
    /// Import stock with cost calculation - calculates AverageCost automatically
    /// </summary>
    public async Task ImportStockWithCostAsync(int productId, int quantity, decimal importPrice, int importOrderId, string notes = null)
    {
        var inventory = await _inventoryRepository.GetByProductIdAsync(productId);
        if (inventory == null)
        {
            throw new Exception($"Inventory not found for product {productId}");
        }

        // Calculate new AverageCost using weighted average method
        var oldTotalValue = inventory.AverageCost * inventory.Quantity;
        var newImportValue = importPrice * quantity;
        var newTotalValue = oldTotalValue + newImportValue;
        var newQuantity = inventory.Quantity + quantity;

        // Update inventory with new values
        inventory.Quantity = newQuantity;
        inventory.AverageCost = newQuantity > 0 ? newTotalValue / newQuantity : 0;
        inventory.TotalValue = newTotalValue;
        inventory.UpdatedAt = DateTime.UtcNow;
        inventory.LastUpdated = DateTime.UtcNow;

        await _inventoryRepository.UpdateAsync(inventory);

        // Sync Product.StockQuantity with Inventory.Quantity
        await SyncProductStockQuantityAsync(productId, inventory.Quantity);

        // Create transaction record
        var transaction = new InventoryTransaction
        {
            ProductId = productId,
            Quantity = quantity,
            TransactionType = InventoryTransactionType.Import,
            ReferenceType = ReferenceType.ImportOrder,
            ReferenceId = importOrderId,
            Notes = $"{notes ?? ""} - Import Price: {importPrice:C}",
            Status = InventoryTransactionStatusType.Completed
        };

        await _transactionRepository.AddAsync(transaction);
    }

    public async Task ExportStockAsync(int productId, int quantity, int orderId, string notes = null)
    {
        var inventory = await _inventoryRepository.GetByProductIdAsync(productId);
        if (inventory == null)
        {
            throw new Exception($"Inventory not found for product {productId}");
        }

        if (inventory.Quantity < quantity)
        {
            throw new Exception($"Insufficient stock for product {productId}. Available: {inventory.Quantity}, Requested: {quantity}");
        }

        inventory.Quantity -= quantity;
        inventory.UpdatedAt = DateTime.UtcNow;
        inventory.LastUpdated = DateTime.UtcNow;
        
        // Update TotalValue
        inventory.TotalValue = inventory.AverageCost * inventory.Quantity;

        await _inventoryRepository.UpdateAsync(inventory);

        // Sync Product.StockQuantity with Inventory.Quantity
        await SyncProductStockQuantityAsync(productId, inventory.Quantity);

        // Create transaction record
        var transaction = new InventoryTransaction
        {
            ProductId = productId,
            Quantity = -quantity,
            TransactionType = InventoryTransactionType.Export,
            ReferenceType = ReferenceType.Order,
            ReferenceId = orderId,
            Notes = notes,
            Status = InventoryTransactionStatusType.Completed
        };

        await _transactionRepository.AddAsync(transaction);
    }

    /// <summary>
    /// Sync Product.StockQuantity with Inventory.Quantity
    /// </summary>
    private async Task SyncProductStockQuantityAsync(int productId, int inventoryQuantity)
    {
        var product = await _productRepository.GetByIdAsync(productId);
        if (product != null && product.StockQuantity != inventoryQuantity)
        {
            product.StockQuantity = inventoryQuantity;
            product.UpdatedAt = DateTime.UtcNow;
            _productRepository.Update(product);
            await _productRepository.SaveChangesAsync();
        }
    }

    /// <summary>
    /// Recalculate and update TotalValue for all inventories
    /// </summary>
    public async Task RecalculateAllTotalValuesAsync()
    {
        var inventories = await _inventoryRepository.GetAllAsync();
        
        foreach (var inventory in inventories)
        {
            inventory.TotalValue = inventory.AverageCost * inventory.Quantity;
            inventory.UpdatedAt = DateTime.UtcNow;
            inventory.LastUpdated = DateTime.UtcNow;
        }

        // Bulk update - assuming repository supports it
        foreach (var inventory in inventories)
        {
            await _inventoryRepository.UpdateAsync(inventory);
        }
    }

    /// <summary>
    /// Get inventory valuation report
    /// </summary>
    public async Task<decimal> GetTotalInventoryValueAsync()
    {
        var inventories = await _inventoryRepository.GetAllAsync();
        return inventories.Sum(i => i.TotalValue);
    }

    /// <summary>
    /// Get low stock items based on reorder level
    /// </summary>
    public async Task<List<InventoryResponseDto>> GetLowStockItemsAsync()
    {
        var inventories = await _inventoryRepository.GetAllAsync();
        var lowStockItems = inventories.Where(i => i.Quantity <= i.ReorderLevel).ToList();
        return _mapper.Map<List<InventoryResponseDto>>(lowStockItems);
    }
} 