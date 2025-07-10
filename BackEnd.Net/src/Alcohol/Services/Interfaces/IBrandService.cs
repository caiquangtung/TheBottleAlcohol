using System.Collections.Generic;
using System.Threading.Tasks;
using Alcohol.DTOs.Brand;
using Alcohol.DTOs;

namespace Alcohol.Services.Interfaces;

public interface IBrandService
{
    Task<PagedResult<BrandResponseDto>> GetAllBrandsAsync(BrandFilterDto filter);
    Task<BrandResponseDto> GetBrandByIdAsync(int id);
    Task<IEnumerable<BrandResponseDto>> GetActiveBrandsAsync();
    Task<BrandResponseDto> GetBrandWithProductsAsync(int id);
    Task<BrandResponseDto> CreateBrandAsync(BrandCreateDto createDto);
    Task<BrandResponseDto> UpdateBrandAsync(int id, BrandUpdateDto updateDto);
    Task<bool> DeleteBrandAsync(int id);
} 