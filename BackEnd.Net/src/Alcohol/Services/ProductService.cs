using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Alcohol.Services.Interfaces;
using Alcohol.Repositories.Interfaces;
using Alcohol.Models;
using Alcohol.DTOs.Product;
using Alcohol.DTOs;
using System.Threading.Tasks;
using System.Collections.Generic;
using System;
namespace Alcohol.Services;

public class ProductService : IProductService
{
    private readonly IProductRepository _productRepository;
    private readonly IMapper _mapper;

    public ProductService(IProductRepository productRepository, IMapper mapper)
    {
        _productRepository = productRepository;
        _mapper = mapper;
    }

    public async Task<PagedResult<ProductResponseDto>> GetAllProductsAsync(ProductFilterDto filter)
    {
        var pagedResult = await _productRepository.GetFilteredAsync(filter);
        var productDtos = _mapper.Map<List<ProductResponseDto>>(pagedResult.Items);
        
        return new PagedResult<ProductResponseDto>(
            productDtos,
            pagedResult.TotalRecords,
            pagedResult.PageNumber,
            pagedResult.PageSize
        );
    }

    public async Task<ProductResponseDto> GetProductByIdAsync(int id)
    {
        var product = await _productRepository.GetByIdWithDetailsAsync(id);
        if (product == null)
            return null;

        return _mapper.Map<ProductResponseDto>(product);
    }

    public async Task<ProductResponseDto> GetProductBySlugAsync(string slug)
    {
        var product = await _productRepository.GetBySlugAsync(slug);
        if (product == null)
            return null;

        return _mapper.Map<ProductResponseDto>(product);
    }

    public async Task<IEnumerable<ProductResponseDto>> GetProductsByCategoryAsync(int categoryId)
    {
        var products = await _productRepository.GetByCategoryAsync(categoryId);
        return _mapper.Map<IEnumerable<ProductResponseDto>>(products);
    }

    public async Task<IEnumerable<ProductResponseDto>> GetProductsByBrandAsync(int brandId)
    {
        var products = await _productRepository.GetByBrandAsync(brandId);
        return _mapper.Map<IEnumerable<ProductResponseDto>>(products);
    }

    public async Task<IEnumerable<ProductResponseDto>> GetActiveProductsAsync()
    {
        var products = await _productRepository.GetActiveProductsAsync();
        return _mapper.Map<IEnumerable<ProductResponseDto>>(products);
    }

    public async Task<IEnumerable<ProductResponseDto>> GetFeaturedProductsAsync()
    {
        var products = await _productRepository.GetFeaturedProductsAsync();
        return _mapper.Map<IEnumerable<ProductResponseDto>>(products);
    }

    public async Task<IEnumerable<ProductResponseDto>> GetNewProductsAsync()
    {
        var products = await _productRepository.GetNewProductsAsync();
        return _mapper.Map<IEnumerable<ProductResponseDto>>(products);
    }

    public async Task<IEnumerable<ProductResponseDto>> GetBestSellingProductsAsync()
    {
        var products = await _productRepository.GetBestSellingProductsAsync();
        return _mapper.Map<IEnumerable<ProductResponseDto>>(products);
    }

    public async Task<ProductResponseDto> CreateProductAsync(ProductCreateDto createDto)
    {
        var product = _mapper.Map<Product>(createDto);
        product.CreatedAt = DateTime.UtcNow;

        await _productRepository.AddAsync(product);
        await _productRepository.SaveChangesAsync();

        // Get the created product with details
        var createdProduct = await _productRepository.GetByIdWithDetailsAsync(product.Id);
        return _mapper.Map<ProductResponseDto>(createdProduct);
    }

    public async Task<ProductResponseDto> UpdateProductAsync(int id, ProductUpdateDto updateDto)
    {
        var product = await _productRepository.GetByIdAsync(id);
        if (product == null)
            return null;

        _mapper.Map(updateDto, product);
        product.UpdatedAt = DateTime.UtcNow;

        _productRepository.Update(product);
        await _productRepository.SaveChangesAsync();

        // Get the updated product with details
        var updatedProduct = await _productRepository.GetByIdWithDetailsAsync(id);
        return _mapper.Map<ProductResponseDto>(updatedProduct);
    }

    public async Task<bool> DeleteProductAsync(int id)
    {
        var product = await _productRepository.GetByIdAsync(id);
        if (product == null)
            return false;

        _productRepository.Delete(product);
        await _productRepository.SaveChangesAsync();
        return true;
    }
}
