using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Alcohol.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Alcohol.Services.Extensions;

public static class RepositoryExtensions
{
    public static async Task<T> GetActiveAsync<T>(this IGenericRepository<T> repository, int id) where T : class
    {
        var entity = await repository.GetByIdAsync(id);
        if (entity == null || !IsActive(entity))
        {
            throw new ArgumentException($"Active entity with ID {id} does not exist.");
        }
        return entity;
    }

    public static async Task<IEnumerable<T>> GetActiveAllAsync<T>(this IGenericRepository<T> repository) where T : class
    {
        var entities = await repository.GetAllAsync();
        return entities.Where(IsActive);
    }

    public static async Task<IEnumerable<T>> GetByDateRangeAsync<T>(
        this IGenericRepository<T> repository,
        DateTime startDate,
        DateTime endDate,
        Func<T, DateTime> dateSelector) where T : class
    {
        var entities = await repository.GetAllAsync();
        return entities.Where(e => dateSelector(e) >= startDate && dateSelector(e) <= endDate);
    }

    public static async Task<IEnumerable<T>> GetByStatusAsync<T>(
        this IGenericRepository<T> repository,
        Enum status,
        Func<T, Enum> statusSelector) where T : class
    {
        var entities = await repository.GetAllAsync();
        return entities.Where(e => statusSelector(e).Equals(status));
    }

    public static async Task<IEnumerable<T>> GetByUserIdAsync<T>(
        this IGenericRepository<T> repository,
        int userId,
        Func<T, int> userIdSelector) where T : class
    {
        var entities = await repository.GetAllAsync();
        return entities.Where(e => userIdSelector(e) == userId);
    }

    public static async Task<IEnumerable<T>> GetByCategoryIdAsync<T>(
        this IGenericRepository<T> repository,
        int categoryId,
        Func<T, int> categoryIdSelector) where T : class
    {
        var entities = await repository.GetAllAsync();
        return entities.Where(e => categoryIdSelector(e) == categoryId);
    }

    public static async Task<IEnumerable<T>> GetByBrandIdAsync<T>(
        this IGenericRepository<T> repository,
        int brandId,
        Func<T, int> brandIdSelector) where T : class
    {
        var entities = await repository.GetAllAsync();
        return entities.Where(e => brandIdSelector(e) == brandId);
    }

    public static async Task<IEnumerable<T>> GetBySupplierIdAsync<T>(
        this IGenericRepository<T> repository,
        int supplierId,
        Func<T, int> supplierIdSelector) where T : class
    {
        var entities = await repository.GetAllAsync();
        return entities.Where(e => supplierIdSelector(e) == supplierId);
    }

    public static async Task<IEnumerable<T>> GetByProductIdAsync<T>(
        this IGenericRepository<T> repository,
        int productId,
        Func<T, int> productIdSelector) where T : class
    {
        var entities = await repository.GetAllAsync();
        return entities.Where(e => productIdSelector(e) == productId);
    }

    public static async Task<IEnumerable<T>> GetByOrderIdAsync<T>(
        this IGenericRepository<T> repository,
        int orderId,
        Func<T, int> orderIdSelector) where T : class
    {
        var entities = await repository.GetAllAsync();
        return entities.Where(e => orderIdSelector(e) == orderId);
    }

    public static async Task<IEnumerable<T>> GetByImportOrderIdAsync<T>(
        this IGenericRepository<T> repository,
        int importOrderId,
        Func<T, int> importOrderIdSelector) where T : class
    {
        var entities = await repository.GetAllAsync();
        return entities.Where(e => importOrderIdSelector(e) == importOrderId);
    }

    private static bool IsActive<T>(T entity) where T : class
    {
        var property = typeof(T).GetProperty("IsActive");
        if (property == null)
        {
            return true;
        }
        return (bool)property.GetValue(entity);
    }
} 