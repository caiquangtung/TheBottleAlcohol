using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Alcohol.Models;

namespace Alcohol.Repositories.Interfaces;

public interface IProductRepository : IGenericRepository<Product>
{
    Task<Product> GetBySlugAsync(string slug);
    Task<IEnumerable<Product>> GetByCategoryAsync(int categoryId);
    Task<IEnumerable<Product>> GetByBrandAsync(int brandId);
    Task<IEnumerable<Product>> GetActiveProductsAsync();
    Task<IEnumerable<Product>> GetFeaturedProductsAsync();
    Task<IEnumerable<Product>> GetNewProductsAsync();
    Task<IEnumerable<Product>> GetBestSellingProductsAsync();
    Task<IEnumerable<Product>> GetAllWithDetailsAsync();
    Task<Product> GetByIdWithDetailsAsync(int id);
} 