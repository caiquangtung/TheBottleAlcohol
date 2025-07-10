using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Alcohol.DTOs.Brand;
using Alcohol.Models;
using Alcohol.Repositories.Interfaces;
using Alcohol.Services.Interfaces;
using AutoMapper;
using System.Linq;
using Alcohol.DTOs;

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

    public async Task<PagedResult<BrandResponseDto>> GetAllBrandsAsync(BrandFilterDto filter)
    {
        var brands = await _brandRepository.GetAllAsync();
        
        // Apply filters
        var filteredBrands = brands.AsQueryable();
        
        if (!string.IsNullOrWhiteSpace(filter.SearchTerm))
        {
            filteredBrands = filteredBrands.Where(b => 
                b.Name.Contains(filter.SearchTerm) || 
                (b.Description != null && b.Description.Contains(filter.SearchTerm)));
        }
        
        if (filter.IsActive.HasValue)
        {
            filteredBrands = filteredBrands.Where(b => b.IsActive == filter.IsActive.Value);
        }
        
        // Apply sorting
        if (!string.IsNullOrWhiteSpace(filter.SortBy))
        {
            filteredBrands = filter.SortBy.ToLower() switch
            {
                "name" => filter.SortOrder?.ToLower() == "desc" 
                    ? filteredBrands.OrderByDescending(b => b.Name)
                    : filteredBrands.OrderBy(b => b.Name),
                "createdat" => filter.SortOrder?.ToLower() == "desc"
                    ? filteredBrands.OrderByDescending(b => b.CreatedAt)
                    : filteredBrands.OrderBy(b => b.CreatedAt),
                _ => filteredBrands.OrderBy(b => b.Id)
            };
        }
        else
        {
            filteredBrands = filteredBrands.OrderBy(b => b.Id);
        }
        
        var totalRecords = filteredBrands.Count();
        var pagedBrands = filteredBrands
            .Skip((filter.PageNumber - 1) * filter.PageSize)
            .Take(filter.PageSize)
            .ToList();
        
        var brandDtos = _mapper.Map<List<BrandResponseDto>>(pagedBrands);
        return new PagedResult<BrandResponseDto>(brandDtos, totalRecords, filter.PageNumber, filter.PageSize);
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