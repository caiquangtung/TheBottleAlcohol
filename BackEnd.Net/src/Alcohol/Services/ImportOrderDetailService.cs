using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Alcohol.Common;
using Alcohol.Data;
using Alcohol.DTOs.ImportOrder;
using Alcohol.Models;
using Alcohol.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Alcohol.Repositories.Interfaces;
using AutoMapper;

namespace Alcohol.Services;

public class ImportOrderDetailService : IImportOrderDetailService
{
    private readonly IImportOrderDetailRepository _importOrderDetailRepository;
    private readonly IMapper _mapper;

    public ImportOrderDetailService(IImportOrderDetailRepository importOrderDetailRepository, IMapper mapper)
    {
        _importOrderDetailRepository = importOrderDetailRepository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<ImportOrderDetailResponseDto>> GetAllImportOrderDetailsAsync()
    {
        var importOrderDetails = await _importOrderDetailRepository.GetAllAsync();
        return _mapper.Map<IEnumerable<ImportOrderDetailResponseDto>>(importOrderDetails);
    }

    public async Task<ImportOrderDetailResponseDto> GetImportOrderDetailByIdAsync(int id)
    {
        var importOrderDetail = await _importOrderDetailRepository.GetByIdAsync(id);
        if (importOrderDetail == null)
            return null;

        return _mapper.Map<ImportOrderDetailResponseDto>(importOrderDetail);
    }

    public async Task<IEnumerable<ImportOrderDetailResponseDto>> GetImportOrderDetailsByOrderAsync(int orderId)
    {
        var importOrderDetails = await _importOrderDetailRepository.GetByImportOrderIdAsync(orderId);
        return _mapper.Map<IEnumerable<ImportOrderDetailResponseDto>>(importOrderDetails);
    }

    public async Task<ImportOrderDetailResponseDto> CreateImportOrderDetailAsync(ImportOrderDetailCreateDto createDto)
    {
        var importOrderDetail = _mapper.Map<ImportOrderDetail>(createDto);
        importOrderDetail.CreatedAt = DateTime.UtcNow;

        await _importOrderDetailRepository.AddAsync(importOrderDetail);
        await _importOrderDetailRepository.SaveChangesAsync();

        return _mapper.Map<ImportOrderDetailResponseDto>(importOrderDetail);
    }

    public async Task<ImportOrderDetailResponseDto> UpdateImportOrderDetailAsync(int id, ImportOrderDetailUpdateDto updateDto)
    {
        var importOrderDetail = await _importOrderDetailRepository.GetByIdAsync(id);
        if (importOrderDetail == null)
            return null;

        _mapper.Map(updateDto, importOrderDetail);
        importOrderDetail.UpdatedAt = DateTime.UtcNow;

        _importOrderDetailRepository.Update(importOrderDetail);
        await _importOrderDetailRepository.SaveChangesAsync();

        return _mapper.Map<ImportOrderDetailResponseDto>(importOrderDetail);
    }

    public async Task<bool> DeleteImportOrderDetailAsync(int id)
    {
        var importOrderDetail = await _importOrderDetailRepository.GetByIdAsync(id);
        if (importOrderDetail == null)
            return false;

        _importOrderDetailRepository.Delete(importOrderDetail);
        await _importOrderDetailRepository.SaveChangesAsync();
        return true;
    }
}
