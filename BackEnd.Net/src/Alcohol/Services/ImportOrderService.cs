using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Alcohol.DTOs.ImportOrder;
using Alcohol.Models;
using Alcohol.Models.Enums;
using Alcohol.Repositories.Interfaces;
using Alcohol.Services.Interfaces;
using AutoMapper;
using Alcohol.DTOs;

namespace Alcohol.Services;

public class ImportOrderService : IImportOrderService
{
    private readonly IImportOrderRepository _importOrderRepository;
    private readonly IImportOrderDetailRepository _importOrderDetailRepository;
    private readonly IInventoryService _inventoryService;
    private readonly IMapper _mapper;

    public ImportOrderService(
        IImportOrderRepository importOrderRepository,
        IImportOrderDetailRepository importOrderDetailRepository,
        IInventoryService inventoryService,
        IMapper mapper)
    {
        _importOrderRepository = importOrderRepository;
        _importOrderDetailRepository = importOrderDetailRepository;
        _inventoryService = inventoryService;
        _mapper = mapper;
    }

    public async Task<PagedResult<ImportOrderResponseDto>> GetAllImportOrdersAsync(ImportOrderFilterDto filter)
    {
        var importOrders = await _importOrderRepository.GetAllAsync();
        
        // Apply filters
        var filteredImportOrders = importOrders.AsQueryable();
        
        if (!string.IsNullOrWhiteSpace(filter.SearchTerm))
        {
            filteredImportOrders = filteredImportOrders.Where(io => 
                io.OrderNumber.Contains(filter.SearchTerm) || 
                (io.Supplier != null && io.Supplier.Name.Contains(filter.SearchTerm)));
        }
        
        if (filter.SupplierId.HasValue)
        {
            filteredImportOrders = filteredImportOrders.Where(io => io.SupplierId == filter.SupplierId.Value);
        }
        
        if (!string.IsNullOrWhiteSpace(filter.Status))
        {
            if (Enum.TryParse<ImportOrderStatusType>(filter.Status, out var status))
            {
                filteredImportOrders = filteredImportOrders.Where(io => io.Status == status);
            }
        }
        
        if (filter.StartDate.HasValue)
        {
            filteredImportOrders = filteredImportOrders.Where(io => io.OrderDate >= filter.StartDate.Value);
        }
        
        if (filter.EndDate.HasValue)
        {
            filteredImportOrders = filteredImportOrders.Where(io => io.OrderDate <= filter.EndDate.Value);
        }
        
        if (filter.MinTotal.HasValue)
        {
            filteredImportOrders = filteredImportOrders.Where(io => io.TotalAmount >= filter.MinTotal.Value);
        }
        
        if (filter.MaxTotal.HasValue)
        {
            filteredImportOrders = filteredImportOrders.Where(io => io.TotalAmount <= filter.MaxTotal.Value);
        }
        
        // Apply sorting
        if (!string.IsNullOrWhiteSpace(filter.SortBy))
        {
            filteredImportOrders = filter.SortBy.ToLower() switch
            {
                "ordernumber" => filter.SortOrder?.ToLower() == "desc" 
                    ? filteredImportOrders.OrderByDescending(io => io.OrderNumber)
                    : filteredImportOrders.OrderBy(io => io.OrderNumber),
                "totalamount" => filter.SortOrder?.ToLower() == "desc"
                    ? filteredImportOrders.OrderByDescending(io => io.TotalAmount)
                    : filteredImportOrders.OrderBy(io => io.TotalAmount),
                "orderdate" => filter.SortOrder?.ToLower() == "desc"
                    ? filteredImportOrders.OrderByDescending(io => io.OrderDate)
                    : filteredImportOrders.OrderBy(io => io.OrderDate),
                "createdat" => filter.SortOrder?.ToLower() == "desc"
                    ? filteredImportOrders.OrderByDescending(io => io.CreatedAt)
                    : filteredImportOrders.OrderBy(io => io.CreatedAt),
                _ => filteredImportOrders.OrderBy(io => io.Id)
            };
        }
        else
        {
            filteredImportOrders = filteredImportOrders.OrderBy(io => io.Id);
        }
        
        var totalRecords = filteredImportOrders.Count();
        var pagedImportOrders = filteredImportOrders
            .Skip((filter.PageNumber - 1) * filter.PageSize)
            .Take(filter.PageSize)
            .ToList();
        
        var importOrderDtos = _mapper.Map<List<ImportOrderResponseDto>>(pagedImportOrders);
        return new PagedResult<ImportOrderResponseDto>(importOrderDtos, totalRecords, filter.PageNumber, filter.PageSize);
    }

    public async Task<ImportOrderResponseDto> GetImportOrderByIdAsync(int id)
    {
        var importOrder = await _importOrderRepository.GetByIdAsync(id);
        if (importOrder == null)
            return null;

        return _mapper.Map<ImportOrderResponseDto>(importOrder);
    }

    public async Task<IEnumerable<ImportOrderResponseDto>> GetImportOrdersBySupplierAsync(int supplierId)
    {
        var importOrders = await _importOrderRepository.GetBySupplierIdAsync(supplierId);
        return _mapper.Map<IEnumerable<ImportOrderResponseDto>>(importOrders);
    }

    public async Task<ImportOrderResponseDto> CreateImportOrderAsync(ImportOrderCreateDto createDto)
    {
        var importOrder = _mapper.Map<ImportOrder>(createDto);
        importOrder.CreatedAt = DateTime.UtcNow;

        await _importOrderRepository.AddAsync(importOrder);
        await _importOrderRepository.SaveChangesAsync();

        return _mapper.Map<ImportOrderResponseDto>(importOrder);
    }

    public async Task<ImportOrderResponseDto> UpdateImportOrderAsync(int id, ImportOrderUpdateDto updateDto)
    {
        var importOrder = await _importOrderRepository.GetByIdAsync(id);
        if (importOrder == null)
            return null;

        _mapper.Map(updateDto, importOrder);
        importOrder.UpdatedAt = DateTime.UtcNow;

        _importOrderRepository.Update(importOrder);
        await _importOrderRepository.SaveChangesAsync();

        return _mapper.Map<ImportOrderResponseDto>(importOrder);
    }

    public async Task<bool> DeleteImportOrderAsync(int id)
    {
        var importOrder = await _importOrderRepository.GetByIdAsync(id);
        if (importOrder == null)
            return false;

        _importOrderRepository.Delete(importOrder);
        await _importOrderRepository.SaveChangesAsync();
        return true;
    }

    /// <summary>
    /// Complete import order and update inventory
    /// </summary>
    public async Task<ImportOrderResponseDto> CompleteImportOrderAsync(int id)
    {
        var importOrder = await _importOrderRepository.GetByIdAsync(id);
        if (importOrder == null)
            throw new Exception($"Import order {id} not found");

        if (importOrder.Status != ImportOrderStatusType.Approved)
            throw new Exception($"Import order {id} must be approved before completion");

        // Get import order details
        var importOrderDetails = await _importOrderDetailRepository.GetByImportOrderIdAsync(id);
        
        // Process each import order detail
        foreach (var detail in importOrderDetails)
        {
            try
            {
                // Import stock with cost calculation
                await _inventoryService.ImportStockWithCostAsync(
                    detail.ProductId, 
                    detail.Quantity, 
                    detail.ImportPrice, 
                    importOrder.Id, 
                    $"Import from order {importOrder.OrderNumber}");
                
                // Update detail status to completed
                detail.Status = ImportOrderStatusType.Completed;
                detail.UpdatedAt = DateTime.UtcNow;
                _importOrderDetailRepository.Update(detail);
            }
            catch (Exception ex)
            {
                // Log error and mark detail as failed
                detail.Status = ImportOrderStatusType.Cancelled;
                detail.UpdatedAt = DateTime.UtcNow;
                _importOrderDetailRepository.Update(detail);
                throw new Exception($"Failed to import product {detail.ProductId}: {ex.Message}");
            }
        }

        // Update import order status
        importOrder.Status = ImportOrderStatusType.Completed;
        importOrder.ImportDate = DateTime.UtcNow;
        importOrder.UpdatedAt = DateTime.UtcNow;
        
        _importOrderRepository.Update(importOrder);
        await _importOrderRepository.SaveChangesAsync();
        await _importOrderDetailRepository.SaveChangesAsync();

        return _mapper.Map<ImportOrderResponseDto>(importOrder);
    }

    /// <summary>
    /// Approve import order
    /// </summary>
    public async Task<ImportOrderResponseDto> ApproveImportOrderAsync(int id)
    {
        var importOrder = await _importOrderRepository.GetByIdAsync(id);
        if (importOrder == null)
            throw new Exception($"Import order {id} not found");

        if (importOrder.Status != ImportOrderStatusType.Pending)
            throw new Exception($"Import order {id} must be pending to approve");

        importOrder.Status = ImportOrderStatusType.Approved;
        importOrder.UpdatedAt = DateTime.UtcNow;
        
        _importOrderRepository.Update(importOrder);
        await _importOrderRepository.SaveChangesAsync();

        return _mapper.Map<ImportOrderResponseDto>(importOrder);
    }

    /// <summary>
    /// Cancel import order
    /// </summary>
    public async Task<ImportOrderResponseDto> CancelImportOrderAsync(int id, string reason = null)
    {
        var importOrder = await _importOrderRepository.GetByIdAsync(id);
        if (importOrder == null)
            throw new Exception($"Import order {id} not found");

        if (importOrder.Status == ImportOrderStatusType.Completed)
            throw new Exception($"Cannot cancel completed import order {id}");

        // Cancel all details
        var importOrderDetails = await _importOrderDetailRepository.GetByImportOrderIdAsync(id);
        foreach (var detail in importOrderDetails)
        {
            detail.Status = ImportOrderStatusType.Cancelled;
            detail.UpdatedAt = DateTime.UtcNow;
            _importOrderDetailRepository.Update(detail);
        }

        // Cancel import order
        importOrder.Status = ImportOrderStatusType.Cancelled;
        importOrder.UpdatedAt = DateTime.UtcNow;
        
        _importOrderRepository.Update(importOrder);
        await _importOrderRepository.SaveChangesAsync();
        await _importOrderDetailRepository.SaveChangesAsync();

        return _mapper.Map<ImportOrderResponseDto>(importOrder);
    }

    /// <summary>
    /// Get import order statistics
    /// </summary>
    public async Task<ImportOrderStatsDto> GetImportOrderStatsAsync()
    {
        var importOrders = await _importOrderRepository.GetAllAsync();
        
        var importOrdersList = importOrders.ToList();
        
        return new ImportOrderStatsDto
        {
            TotalOrders = importOrdersList.Count,
            PendingOrders = importOrdersList.Count(io => io.Status == ImportOrderStatusType.Pending),
            ApprovedOrders = importOrdersList.Count(io => io.Status == ImportOrderStatusType.Approved),
            CompletedOrders = importOrdersList.Count(io => io.Status == ImportOrderStatusType.Completed),
            CancelledOrders = importOrdersList.Count(io => io.Status == ImportOrderStatusType.Cancelled),
            TotalValue = importOrdersList.Where(io => io.Status == ImportOrderStatusType.Completed).Sum(io => io.TotalAmount),
            AverageOrderValue = importOrdersList.Any(io => io.Status == ImportOrderStatusType.Completed) 
                ? importOrdersList.Where(io => io.Status == ImportOrderStatusType.Completed).Average(io => io.TotalAmount) 
                : 0
        };
    }
}

