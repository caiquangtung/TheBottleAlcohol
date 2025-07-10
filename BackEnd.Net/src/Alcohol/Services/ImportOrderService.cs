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
    private readonly IMapper _mapper;

    public ImportOrderService(IImportOrderRepository importOrderRepository, IMapper mapper)
    {
        _importOrderRepository = importOrderRepository;
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
}
