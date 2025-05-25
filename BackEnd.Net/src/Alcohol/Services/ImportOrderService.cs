using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Alcohol.DTOs.ImportOrder;
using Alcohol.Models;
using Alcohol.Models.Enums;
using Alcohol.Repositories.Interfaces;
using Alcohol.Services.Interfaces;
using AutoMapper;

namespace Alcohol.Services;

public class ImportOrderService : IImportOrderService
{
    private readonly IImportOrderRepository _importOrderRepository;
    private readonly IImportOrderDetailRepository _importOrderDetailRepository;
    private readonly IMapper _mapper;

    public ImportOrderService(
        IImportOrderRepository importOrderRepository,
        IImportOrderDetailRepository importOrderDetailRepository,
        IMapper mapper)
    {
        _importOrderRepository = importOrderRepository;
        _importOrderDetailRepository = importOrderDetailRepository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<ImportOrderResponseDto>> GetAllImportOrdersAsync()
    {
        var importOrders = await _importOrderRepository.GetAllAsync();
        return _mapper.Map<IEnumerable<ImportOrderResponseDto>>(importOrders);
    }

    public async Task<ImportOrderResponseDto> GetImportOrderByIdAsync(int id)
    {
        var importOrder = await _importOrderRepository.GetByIdAsync(id);
        if (importOrder == null)
            return null;

        return _mapper.Map<ImportOrderResponseDto>(importOrder);
    }

    public async Task<ImportOrderResponseDto> GetImportOrderWithDetailsAsync(int id)
    {
        var importOrder = await _importOrderRepository.GetImportOrderWithDetailsAsync(id);
        if (importOrder == null)
            return null;

        return _mapper.Map<ImportOrderResponseDto>(importOrder);
    }

    public async Task<IEnumerable<ImportOrderResponseDto>> GetImportOrdersBySupplierAsync(int supplierId)
    {
        var importOrders = await _importOrderRepository.GetImportOrdersBySupplierAsync(supplierId);
        return _mapper.Map<IEnumerable<ImportOrderResponseDto>>(importOrders);
    }

    public async Task<IEnumerable<ImportOrderResponseDto>> GetImportOrdersByStatusAsync(ImportOrderStatusType status)
    {
        var importOrders = await _importOrderRepository.GetImportOrdersByStatusAsync(status);
        return _mapper.Map<IEnumerable<ImportOrderResponseDto>>(importOrders);
    }

    public async Task<ImportOrderResponseDto> CreateImportOrderAsync(ImportOrderCreateDto createDto)
    {
        var importOrder = _mapper.Map<ImportOrder>(createDto);
        importOrder.CreatedAt = DateTime.UtcNow;
        importOrder.Status = ImportOrderStatusType.Pending;

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

    public async Task<ImportOrderResponseDto> UpdateImportOrderStatusAsync(int id, ImportOrderStatusType status)
    {
        var importOrder = await _importOrderRepository.GetByIdAsync(id);
        if (importOrder == null)
            return null;

        importOrder.Status = status;
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

    public async Task<ImportOrderResponseDto> CompleteImportOrderAsync(int id)
    {
        var importOrder = await _importOrderRepository.GetByIdAsync(id);
        if (importOrder == null)
            return null;

        importOrder.Status = ImportOrderStatusType.Completed;
        importOrder.UpdatedAt = DateTime.UtcNow;

        _importOrderRepository.Update(importOrder);
        await _importOrderRepository.SaveChangesAsync();

        return _mapper.Map<ImportOrderResponseDto>(importOrder);
    }

    public async Task<List<ImportOrderDetailResponseDto>> GetImportOrderDetails(int id)
    {
        var details = await _importOrderDetailRepository.GetByImportOrderIdAsync(id);
        return _mapper.Map<List<ImportOrderDetailResponseDto>>(details);
    }

    public async Task<ImportOrderDetailResponseDto> AddImportOrderDetail(int id, ImportOrderDetailCreateDto detailDto)
    {
        var importOrder = await _importOrderRepository.GetByIdAsync(id);
        if (importOrder == null)
            return null;

        var detail = _mapper.Map<ImportOrderDetail>(detailDto);
        detail.ImportOrderId = id;
        detail.CreatedAt = DateTime.UtcNow;

        await _importOrderDetailRepository.AddAsync(detail);
        await _importOrderDetailRepository.SaveChangesAsync();

        return _mapper.Map<ImportOrderDetailResponseDto>(detail);
    }

    public async Task<bool> RemoveImportOrderDetail(int id, int detailId)
    {
        var detail = await _importOrderDetailRepository.GetByIdAsync(detailId);
        if (detail == null || detail.ImportOrderId != id)
            return false;

        _importOrderDetailRepository.Delete(detail);
        await _importOrderDetailRepository.SaveChangesAsync();
        return true;
    }
}
