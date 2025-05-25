using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Alcohol.DTOs.InventoryTransaction;
using Alcohol.Models;
using Alcohol.Models.Enums;
using Alcohol.Repositories.Interfaces;
using Alcohol.Services.Interfaces;
using AutoMapper;

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

    public async Task<IEnumerable<InventoryTransactionResponseDto>> GetAllTransactionsAsync()
    {
        var transactions = await _inventoryTransactionRepository.GetAllAsync();
        return _mapper.Map<IEnumerable<InventoryTransactionResponseDto>>(transactions);
    }

    public async Task<InventoryTransactionResponseDto> GetTransactionByIdAsync(int id)
    {
        var transaction = await _inventoryTransactionRepository.GetByIdAsync(id);
        if (transaction == null)
            return null;

        return _mapper.Map<InventoryTransactionResponseDto>(transaction);
    }

    public async Task<IEnumerable<InventoryTransactionResponseDto>> GetTransactionsByProductAsync(int productId)
    {
        var transactions = await _inventoryTransactionRepository.GetByProductIdAsync(productId);
        return _mapper.Map<IEnumerable<InventoryTransactionResponseDto>>(transactions);
    }

    public async Task<IEnumerable<InventoryTransactionResponseDto>> GetTransactionsByTypeAsync(InventoryTransactionType type)
    {
        var transactions = await _inventoryTransactionRepository.GetByTypeAsync(type);
        return _mapper.Map<IEnumerable<InventoryTransactionResponseDto>>(transactions);
    }

    public async Task<IEnumerable<InventoryTransactionResponseDto>> GetTransactionsByReferenceAsync(ReferenceType referenceType, int referenceId)
    {
        var transactions = await _inventoryTransactionRepository.GetByReferenceAsync(referenceType, referenceId);
        return _mapper.Map<IEnumerable<InventoryTransactionResponseDto>>(transactions);
    }

    public async Task<InventoryTransactionResponseDto> CreateTransactionAsync(InventoryTransactionCreateDto createDto)
    {
        var transaction = _mapper.Map<InventoryTransaction>(createDto);
        transaction.CreatedAt = DateTime.UtcNow;
        transaction.Status = InventoryTransactionStatusType.Pending;

        await _inventoryTransactionRepository.AddAsync(transaction);
        await _inventoryTransactionRepository.SaveChangesAsync();

        return _mapper.Map<InventoryTransactionResponseDto>(transaction);
    }

    public async Task<InventoryTransactionResponseDto> UpdateTransactionAsync(int id, InventoryTransactionUpdateDto updateDto)
    {
        var transaction = await _inventoryTransactionRepository.GetByIdAsync(id);
        if (transaction == null)
            return null;

        _mapper.Map(updateDto, transaction);
        transaction.UpdatedAt = DateTime.UtcNow;

        await _inventoryTransactionRepository.UpdateAsync(transaction);
        return _mapper.Map<InventoryTransactionResponseDto>(transaction);
    }

    public async Task<bool> UpdateTransactionStatusAsync(int id, InventoryTransactionStatusType status)
    {
        var transaction = await _inventoryTransactionRepository.GetByIdAsync(id);
        if (transaction == null)
            return false;

        transaction.Status = status;
        transaction.UpdatedAt = DateTime.UtcNow;

        await _inventoryTransactionRepository.UpdateAsync(transaction);
        return true;
    }

    public async Task<bool> DeleteTransactionAsync(int id)
    {
        var transaction = await _inventoryTransactionRepository.GetByIdAsync(id);
        if (transaction == null)
            return false;

        await _inventoryTransactionRepository.DeleteAsync(id);
        return true;
    }

    public async Task SaveChangesAsync()
    {
        await _inventoryTransactionRepository.SaveChangesAsync();
    }
} 