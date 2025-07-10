using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Alcohol.DTOs.InventoryTransaction;
using Alcohol.Models;
using Alcohol.Models.Enums;
using Alcohol.Repositories.Interfaces;
using Alcohol.Services.Interfaces;
using AutoMapper;
using Alcohol.DTOs;

namespace Alcohol.Services;

public class InventoryTransactionService : IInventoryTransactionService
{
    private readonly IInventoryTransactionRepository _inventoryTransactionRepository;
    private readonly IMapper _mapper;

    public InventoryTransactionService(IInventoryTransactionRepository inventoryTransactionRepository, IMapper mapper)
    {
        _inventoryTransactionRepository = inventoryTransactionRepository;
        _mapper = mapper;
    }

    public async Task<PagedResult<InventoryTransactionResponseDto>> GetAllInventoryTransactionsAsync(InventoryTransactionFilterDto filter)
    {
        var inventoryTransactions = await _inventoryTransactionRepository.GetAllAsync();
        
        // Apply filters
        var filteredTransactions = inventoryTransactions.AsQueryable();
        
        if (!string.IsNullOrWhiteSpace(filter.SearchTerm))
        {
            filteredTransactions = filteredTransactions.Where(it => 
                it.TransactionNumber.Contains(filter.SearchTerm) || 
                (it.Product != null && it.Product.Name.Contains(filter.SearchTerm)));
        }
        
        if (filter.ProductId.HasValue)
        {
            filteredTransactions = filteredTransactions.Where(it => it.ProductId == filter.ProductId.Value);
        }
        
        if (!string.IsNullOrWhiteSpace(filter.TransactionType))
        {
            if (Enum.TryParse<InventoryTransactionType>(filter.TransactionType, out var transactionType))
            {
                filteredTransactions = filteredTransactions.Where(it => it.TransactionType == transactionType);
            }
        }
        
        if (!string.IsNullOrWhiteSpace(filter.Status))
        {
            if (Enum.TryParse<InventoryTransactionStatusType>(filter.Status, out var status))
            {
                filteredTransactions = filteredTransactions.Where(it => it.Status == status);
            }
        }
        
        if (filter.StartDate.HasValue)
        {
            filteredTransactions = filteredTransactions.Where(it => it.TransactionDate >= filter.StartDate.Value);
        }
        
        if (filter.EndDate.HasValue)
        {
            filteredTransactions = filteredTransactions.Where(it => it.TransactionDate <= filter.EndDate.Value);
        }
        
        if (filter.MinQuantity.HasValue)
        {
            filteredTransactions = filteredTransactions.Where(it => it.Quantity >= filter.MinQuantity.Value);
        }
        
        if (filter.MaxQuantity.HasValue)
        {
            filteredTransactions = filteredTransactions.Where(it => it.Quantity <= filter.MaxQuantity.Value);
        }
        
        // Apply sorting
        if (!string.IsNullOrWhiteSpace(filter.SortBy))
        {
            filteredTransactions = filter.SortBy.ToLower() switch
            {
                "transactionnumber" => filter.SortOrder?.ToLower() == "desc" 
                    ? filteredTransactions.OrderByDescending(it => it.TransactionNumber)
                    : filteredTransactions.OrderBy(it => it.TransactionNumber),
                "quantity" => filter.SortOrder?.ToLower() == "desc"
                    ? filteredTransactions.OrderByDescending(it => it.Quantity)
                    : filteredTransactions.OrderBy(it => it.Quantity),
                "transactiondate" => filter.SortOrder?.ToLower() == "desc"
                    ? filteredTransactions.OrderByDescending(it => it.TransactionDate)
                    : filteredTransactions.OrderBy(it => it.TransactionDate),
                "createdat" => filter.SortOrder?.ToLower() == "desc"
                    ? filteredTransactions.OrderByDescending(it => it.CreatedAt)
                    : filteredTransactions.OrderBy(it => it.CreatedAt),
                _ => filteredTransactions.OrderBy(it => it.Id)
            };
        }
        else
        {
            filteredTransactions = filteredTransactions.OrderBy(it => it.Id);
        }
        
        var totalRecords = filteredTransactions.Count();
        var pagedTransactions = filteredTransactions
            .Skip((filter.PageNumber - 1) * filter.PageSize)
            .Take(filter.PageSize)
            .ToList();
        
        var transactionDtos = _mapper.Map<List<InventoryTransactionResponseDto>>(pagedTransactions);
        return new PagedResult<InventoryTransactionResponseDto>(transactionDtos, totalRecords, filter.PageNumber, filter.PageSize);
    }

    public async Task<InventoryTransactionResponseDto> GetInventoryTransactionByIdAsync(int id)
    {
        var inventoryTransaction = await _inventoryTransactionRepository.GetByIdAsync(id);
        if (inventoryTransaction == null)
            return null;

        return _mapper.Map<InventoryTransactionResponseDto>(inventoryTransaction);
    }

    public async Task<IEnumerable<InventoryTransactionResponseDto>> GetInventoryTransactionsByProductAsync(int productId)
    {
        var inventoryTransactions = await _inventoryTransactionRepository.GetByProductIdAsync(productId);
        return _mapper.Map<IEnumerable<InventoryTransactionResponseDto>>(inventoryTransactions);
    }

    public async Task<InventoryTransactionResponseDto> CreateInventoryTransactionAsync(InventoryTransactionCreateDto createDto)
    {
        var inventoryTransaction = _mapper.Map<InventoryTransaction>(createDto);
        inventoryTransaction.CreatedAt = DateTime.UtcNow;

        await _inventoryTransactionRepository.AddAsync(inventoryTransaction);
        await _inventoryTransactionRepository.SaveChangesAsync();

        return _mapper.Map<InventoryTransactionResponseDto>(inventoryTransaction);
    }

    public async Task<InventoryTransactionResponseDto> UpdateInventoryTransactionAsync(int id, InventoryTransactionUpdateDto updateDto)
    {
        var inventoryTransaction = await _inventoryTransactionRepository.GetByIdAsync(id);
        if (inventoryTransaction == null)
            return null;

        _mapper.Map(updateDto, inventoryTransaction);
        inventoryTransaction.UpdatedAt = DateTime.UtcNow;

        await _inventoryTransactionRepository.UpdateAsync(inventoryTransaction);
        await _inventoryTransactionRepository.SaveChangesAsync();

        return _mapper.Map<InventoryTransactionResponseDto>(inventoryTransaction);
    }

    public async Task<bool> DeleteInventoryTransactionAsync(int id)
    {
        var inventoryTransaction = await _inventoryTransactionRepository.GetByIdAsync(id);
        if (inventoryTransaction == null)
            return false;

        await _inventoryTransactionRepository.DeleteAsync(id);
        await _inventoryTransactionRepository.SaveChangesAsync();
        return true;
    }
} 