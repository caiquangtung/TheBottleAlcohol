using System.Collections.Generic;
using System.Threading.Tasks;
using Alcohol.DTOs.Product;

namespace Alcohol.Services.Interfaces;

public interface IProductService
{
    Task<IEnumerable<ProductResponseDto>> GetAllProductsAsync();
    Task<ProductResponseDto> GetProductByIdAsync(int id);
    Task<ProductResponseDto> GetProductBySlugAsync(string slug);
    Task<IEnumerable<ProductResponseDto>> GetProductsByCategoryAsync(int categoryId);
    Task<IEnumerable<ProductResponseDto>> GetProductsByBrandAsync(int brandId);
    Task<IEnumerable<ProductResponseDto>> GetActiveProductsAsync();
    Task<IEnumerable<ProductResponseDto>> GetFeaturedProductsAsync();
    Task<IEnumerable<ProductResponseDto>> GetNewProductsAsync();
    Task<IEnumerable<ProductResponseDto>> GetBestSellingProductsAsync();
    Task<ProductResponseDto> CreateProductAsync(ProductCreateDto createDto);
    Task<ProductResponseDto> UpdateProductAsync(int id, ProductUpdateDto updateDto);
    Task<bool> DeleteProductAsync(int id);
} 