using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Alcohol.DTOs.Brand;
using Alcohol.Models;
using Alcohol.Repositories.Interfaces;
using Alcohol.Services.Interfaces;
using AutoMapper;

namespace Alcohol.Services;

public class BrandService : IBrandService
{
    private readonly IBrandRepository _brandRepository;
    private readonly IMapper _mapper;

    public BrandService(IBrandRepository brandRepository, IMapper mapper)
    {
        _brandRepository = brandRepository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<BrandResponseDto>> GetAllBrandsAsync()
    {
        var brands = await _brandRepository.GetAllAsync();
        return _mapper.Map<IEnumerable<BrandResponseDto>>(brands);
    }

    public async Task<BrandResponseDto> GetBrandByIdAsync(int id)
    {
        var brand = await _brandRepository.GetByIdAsync(id);
        if (brand == null)
            return null;

        return _mapper.Map<BrandResponseDto>(brand);
    }

    public async Task<IEnumerable<BrandResponseDto>> GetActiveBrandsAsync()
    {
        var brands = await _brandRepository.GetActiveBrandsAsync();
        return _mapper.Map<IEnumerable<BrandResponseDto>>(brands);
    }

    public async Task<BrandResponseDto> GetBrandWithProductsAsync(int id)
    {
        var brand = await _brandRepository.GetBrandWithProductsAsync(id);
        if (brand == null)
            return null;

        return _mapper.Map<BrandResponseDto>(brand);
    }

    public async Task<BrandResponseDto> CreateBrandAsync(BrandCreateDto createDto)
    {
        var brand = _mapper.Map<Brand>(createDto);
        brand.CreatedAt = DateTime.UtcNow;

        await _brandRepository.AddAsync(brand);
        await _brandRepository.SaveChangesAsync();

        return _mapper.Map<BrandResponseDto>(brand);
    }

    public async Task<BrandResponseDto> UpdateBrandAsync(int id, BrandUpdateDto updateDto)
    {
        var brand = await _brandRepository.GetByIdAsync(id);
        if (brand == null)
            return null;

        _mapper.Map(updateDto, brand);
        brand.UpdatedAt = DateTime.UtcNow;

        _brandRepository.Update(brand);
        await _brandRepository.SaveChangesAsync();

        return _mapper.Map<BrandResponseDto>(brand);
    }

    public async Task<bool> DeleteBrandAsync(int id)
    {
        var brand = await _brandRepository.GetByIdAsync(id);
        if (brand == null)
            return false;

        if (await _brandRepository.HasProductsAsync(id))
            return false;

        _brandRepository.Delete(brand);
        await _brandRepository.SaveChangesAsync();
        return true;
    }
} 