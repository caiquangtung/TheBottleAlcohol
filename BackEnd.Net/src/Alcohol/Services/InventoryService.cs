using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Alcohol.DTOs.Inventory;
using Alcohol.Models;
using Alcohol.Models.Enums;
using Alcohol.Repositories.Interfaces;
using Alcohol.Services.Interfaces;
using AutoMapper;

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

    public async Task<IEnumerable<InventoryResponseDto>> GetAllInventoriesAsync()
    {
        var inventories = await _inventoryRepository.GetAllAsync();
        return _mapper.Map<IEnumerable<InventoryResponseDto>>(inventories);
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

    public async Task<IEnumerable<InventoryResponseDto>> GetLowStockInventoriesAsync(int threshold)
    {
        var inventories = await _inventoryRepository.GetLowStockInventoriesAsync(threshold);
        return _mapper.Map<IEnumerable<InventoryResponseDto>>(inventories);
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
            Type = InventoryTransactionType.Adjustment,
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
            Type = InventoryTransactionType.Import,
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
            Type = InventoryTransactionType.Export,
            ReferenceType = ReferenceType.Order,
            ReferenceId = orderId,
            Notes = notes,
            Status = InventoryTransactionStatusType.Completed
        };

        await _transactionRepository.AddAsync(transaction);
    }
} 