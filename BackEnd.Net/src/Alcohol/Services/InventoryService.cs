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

namespace Alcohol.Services;

public class InventoryService : IInventoryService
{
    private readonly IInventoryRepository _inventoryRepository;
    private readonly IInventoryTransactionRepository _transactionRepository;
    private readonly IMapper _mapper;

    public InventoryService(
        IInventoryRepository inventoryRepository,
        IInventoryTransactionRepository transactionRepository,
        IMapper mapper)
    {
        _inventoryRepository = inventoryRepository;
        _transactionRepository = transactionRepository;
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
        inventory.UpdatedAt = DateTime.UtcNow;
        inventory.LastUpdated = DateTime.UtcNow;

        await _inventoryRepository.UpdateAsync(inventory);
        return true;
    }

    public async Task<bool> AddStockAsync(int id, int quantity)
    {
        var inventory = await _inventoryRepository.GetByIdAsync(id);
        if (inventory == null)
            return false;

        inventory.Quantity += quantity;
        inventory.UpdatedAt = DateTime.UtcNow;
        inventory.LastUpdated = DateTime.UtcNow;

        await _inventoryRepository.UpdateAsync(inventory);
        return true;
    }

    public async Task<bool> RemoveStockAsync(int id, int quantity)
    {
        var inventory = await _inventoryRepository.GetByIdAsync(id);
        if (inventory == null || inventory.Quantity < quantity)
            return false;

        inventory.Quantity -= quantity;
        inventory.UpdatedAt = DateTime.UtcNow;
        inventory.LastUpdated = DateTime.UtcNow;

        await _inventoryRepository.UpdateAsync(inventory);
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
        await _inventoryRepository.UpdateAsync(inventory);

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
        await _inventoryRepository.UpdateAsync(inventory);

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

    public async Task ExportStockAsync(int productId, int quantity, int orderId, string notes = null)
    {
        var inventory = await _inventoryRepository.GetByProductIdAsync(productId);
        if (inventory == null)
        {
            throw new Exception($"Inventory not found for product {productId}");
        }

        if (inventory.Quantity < quantity)
        {
            throw new Exception($"Insufficient stock for product {productId}");
        }

        inventory.Quantity -= quantity;
        await _inventoryRepository.UpdateAsync(inventory);

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
} 