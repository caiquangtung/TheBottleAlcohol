using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Alcohol.DTOs;

namespace Alcohol.Helpers;

public static class FilterHelper
{
    public static IQueryable<T> ApplySearchFilter<T>(this IQueryable<T> query, string searchTerm, Func<T, string> searchProperty)
    {
        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            return query.Where(item => searchProperty(item).Contains(searchTerm));
        }
        return query;
    }

    public static IQueryable<T> ApplyPagination<T>(this IQueryable<T> query, BaseFilterDto filter)
    {
        return query
            .Skip((filter.PageNumber - 1) * filter.PageSize)
            .Take(filter.PageSize);
    }

    public static IQueryable<T> ApplySorting<T>(this IQueryable<T> query, BaseFilterDto filter, string defaultSortBy = "Id")
    {
        if (!string.IsNullOrWhiteSpace(filter.SortBy))
        {
            var sortOrder = filter.SortOrder?.ToLower() == "desc" ? "desc" : "asc";
            // Note: Dynamic sorting would require more complex implementation
            // For now, we'll use the default sorting
        }
        
        return query.OrderBy(x => EF.Property<object>(x, defaultSortBy));
    }

    public static IQueryable<T> ApplyDateRangeFilter<T>(this IQueryable<T> query, DateTime? startDate, DateTime? endDate, Func<T, DateTime> dateProperty)
    {
        if (startDate.HasValue)
            query = query.Where(item => dateProperty(item) >= startDate.Value);
        
        if (endDate.HasValue)
            query = query.Where(item => dateProperty(item) <= endDate.Value);
        
        return query;
    }

    public static IQueryable<T> ApplyNumericRangeFilter<T>(this IQueryable<T> query, decimal? minValue, decimal? maxValue, Func<T, decimal> valueProperty)
    {
        if (minValue.HasValue)
            query = query.Where(item => valueProperty(item) >= minValue.Value);
        
        if (maxValue.HasValue)
            query = query.Where(item => valueProperty(item) <= maxValue.Value);
        
        return query;
    }
} 