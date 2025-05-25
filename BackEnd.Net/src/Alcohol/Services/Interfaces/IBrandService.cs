using System.Collections.Generic;
using System.Threading.Tasks;
using Alcohol.DTOs.Brand;

namespace Alcohol.Services.Interfaces;

public interface IBrandService
{
    Task<IEnumerable<BrandResponseDto>> GetAllBrandsAsync();
    Task<BrandResponseDto> GetBrandByIdAsync(int id);
    Task<IEnumerable<BrandResponseDto>> GetActiveBrandsAsync();
    Task<BrandResponseDto> GetBrandWithProductsAsync(int id);
    Task<BrandResponseDto> CreateBrandAsync(BrandCreateDto createDto);
    Task<BrandResponseDto> UpdateBrandAsync(int id, BrandUpdateDto updateDto);
    Task<bool> DeleteBrandAsync(int id);
} 