using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Alcohol.Data;
using Alcohol.Models;
using Alcohol.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using Alcohol.DTOs.Product;
using Alcohol.DTOs;

namespace Alcohol.Repositories;

public class ProductRepository : GenericRepository<Product>, IProductRepository
{
    public ProductRepository(MyDbContext context) : base(context)
    {
    }

    public async Task<PagedResult<Product>> GetFilteredAsync(ProductFilterDto filter)
    {
        IQueryable<Product> query = _dbSet
            .Include(p => p.Category)
            .Include(p => p.Brand);

        // Filter
        if (filter.CategoryId.HasValue)
            query = query.Where(p => p.CategoryId == filter.CategoryId.Value);
        if (filter.BrandId.HasValue)
            query = query.Where(p => p.BrandId == filter.BrandId.Value);
        if (filter.MinPrice.HasValue)
            query = query.Where(p => p.Price >= filter.MinPrice.Value);
        if (filter.MaxPrice.HasValue)
            query = query.Where(p => p.Price <= filter.MaxPrice.Value);
        if (filter.Status.HasValue)
            query = query.Where(p => p.Status == filter.Status.Value);

        // Search
        if (!string.IsNullOrWhiteSpace(filter.SearchTerm))
            query = query.Where(p => p.Name.Contains(filter.SearchTerm) || p.Description.Contains(filter.SearchTerm));

        // Sort
        query = (filter.SortBy?.ToLower(), filter.SortOrder?.ToLower()) switch
        {
            ("price", "desc") => query.OrderByDescending(p => p.Price),
            ("price", _)      => query.OrderBy(p => p.Price),
            ("name", "desc")  => query.OrderByDescending(p => p.Name),
            ("name", _)       => query.OrderBy(p => p.Name),
            ("createdat", "desc") => query.OrderByDescending(p => p.CreatedAt),
            ("createdat", _)      => query.OrderBy(p => p.CreatedAt),
            _                 => query.OrderBy(p => p.Id)
        };

        // Pagination
        var totalRecords = await query.CountAsync();
        var data = await query
            .Skip((filter.PageNumber - 1) * filter.PageSize)
            .Take(filter.PageSize)
            .ToListAsync();

        return new PagedResult<Product>(data, totalRecords, filter.PageNumber, filter.PageSize);
    }

    public async Task<IEnumerable<Product>> GetAllWithDetailsAsync()
    {
        return await _dbSet
            .Include(p => p.Category)
            .Include(p => p.Brand)
            .ToListAsync();
    }

    public async Task<Product> GetByIdWithDetailsAsync(int id)
    {
        return await _dbSet
            .Include(p => p.Category)
            .Include(p => p.Brand)
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public override async Task<IEnumerable<Product>> GetAllAsync()
    {
        return await _context.Products
            .Include(p => p.Category)
            .Include(p => p.Brand)
            .ToListAsync();
    }

    public override async Task<Product> GetByIdAsync(int id)
    {
        return await _context.Products
            .Include(p => p.Category)
            .Include(p => p.Brand)
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<IEnumerable<Product>> GetByCategoryAsync(int categoryId)
    {
        return await _context.Products
            .Include(p => p.Category)
            .Include(p => p.Brand)
            .Where(p => p.CategoryId == categoryId)
            .ToListAsync();
    }

    public async Task<IEnumerable<Product>> GetByBrandAsync(int brandId)
    {
        return await _context.Products
            .Include(p => p.Category)
            .Include(p => p.Brand)
            .Where(p => p.BrandId == brandId)
            .ToListAsync();
    }

    public async Task<IEnumerable<Product>> GetLowStockProductsAsync(int threshold)
    {
        return await _context.Products
            .Include(p => p.Category)
            .Include(p => p.Brand)
            .Where(p => p.StockQuantity <= threshold)
            .ToListAsync();
    }

    public override async Task AddAsync(Product product)
    {
        await _context.Products.AddAsync(product);
    }

    public override void Update(Product product)
    {
        _context.Products.Update(product);
    }

    public override void Delete(Product product)
    {
        _context.Products.Remove(product);
    }

    public override async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<Product>> GetProductsForStatsAsync()
    {
        return await _context.Products
            .Include(p => p.OrderDetails)
            .ToListAsync();
    }

    public async Task<bool> ExistsAsync(int id)
    {
        return await _context.Products.AnyAsync(p => p.Id == id);
    }

    public async Task<bool> ExistsByNameAsync(string name)
    {
        return await _context.Products.AnyAsync(p => p.Name == name);
    }

    public async Task<Product> GetBySlugAsync(string slug)
    {
        return await _context.Products
            .FirstOrDefaultAsync(p => p.Slug == slug);
    }

    public async Task<IEnumerable<Product>> GetActiveProductsAsync()
    {
        return await _context.Products
            .Where(p => p.Status)
            .ToListAsync();
    }

    public async Task<IEnumerable<Product>> GetFeaturedProductsAsync()
    {
        return await _context.Products
            .Where(p => p.Status)
            .OrderByDescending(p => p.CreatedAt)
            .Take(8)
            .ToListAsync();
    }

    public async Task<IEnumerable<Product>> GetNewProductsAsync()
    {
        return await _context.Products
            .Where(p => p.Status)
            .OrderByDescending(p => p.CreatedAt)
            .Take(8)
            .ToListAsync();
    }

    public async Task<IEnumerable<Product>> GetBestSellingProductsAsync()
    {
        return await _context.Products
            .Where(p => p.Status)
            .OrderByDescending(p => p.OrderDetails.Sum(od => od.Quantity))
            .Take(8)
            .ToListAsync();
    }

    public async Task<List<Product>> GetProductsByIdsAsync(List<int> ids)
    {
        return await _context.Products
            .Where(p => ids.Contains(p.Id))
            .ToListAsync();
    }
} 