using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Alcohol.DTOs.Supplier;
using Alcohol.Models;
using Alcohol.Repositories.Interfaces;
using Alcohol.Services.Interfaces;
using AutoMapper;

namespace Alcohol.Services;

public class SupplierService : ISupplierService
{
    private readonly ISupplierRepository _supplierRepository;
    private readonly IMapper _mapper;

    public SupplierService(ISupplierRepository supplierRepository, IMapper mapper)
    {
        _supplierRepository = supplierRepository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<SupplierResponseDto>> GetAllSuppliersAsync()
    {
        var suppliers = await _supplierRepository.GetAllAsync();
        return _mapper.Map<IEnumerable<SupplierResponseDto>>(suppliers);
    }

    public async Task<SupplierResponseDto> GetSupplierByIdAsync(int id)
    {
        var supplier = await _supplierRepository.GetByIdAsync(id);
        if (supplier == null)
            return null;

        return _mapper.Map<SupplierResponseDto>(supplier);
    }

    public async Task<SupplierResponseDto> CreateSupplierAsync(SupplierCreateDto dto)
    {
        var supplier = _mapper.Map<Supplier>(dto);

        await _supplierRepository.AddAsync(supplier);
        await _supplierRepository.SaveChangesAsync();

        return _mapper.Map<SupplierResponseDto>(supplier);
    }

    public async Task<SupplierResponseDto> UpdateSupplierAsync(int id, SupplierUpdateDto dto)
    {
        var supplier = await _supplierRepository.GetByIdAsync(id);
        if (supplier == null)
            return null;

        _mapper.Map(dto, supplier);

        _supplierRepository.Update(supplier);
        await _supplierRepository.SaveChangesAsync();

        return _mapper.Map<SupplierResponseDto>(supplier);
    }

    public async Task<bool> DeleteSupplierAsync(int id)
    {
        var supplier = await _supplierRepository.GetByIdAsync(id);
        if (supplier == null)
            return false;

        if (await _supplierRepository.HasImportOrdersAsync(id))
            return false;

        _supplierRepository.Delete(supplier);
        await _supplierRepository.SaveChangesAsync();
        return true;
    }
} 