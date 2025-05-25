using System;
using Alcohol.Models;

namespace Alcohol.Repositories.IRepositories;

public interface IStatisticsRepository
{
    Task<List<Order>> GetOrdersForStatsAsync(DateTime? minDate, DateTime? maxDate);
    Task<List<Product>> GetProductsForStatsAsync();
} 