using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Alcohol.DTOs.Statistics;

namespace Alcohol.Services.Interfaces;

public interface IStatisticsService
{
    Task<List<OrderAnalyticsDto>> GetOrderAnalytics(DateTime? startDate, DateTime? endDate);
    Task<List<RevenueAnalyticsDto>> GetRevenueAnalytics(DateTime? startDate, DateTime? endDate);
    Task<List<ProductAnalyticsDto>> GetProductAnalytics(DateTime? startDate, DateTime? endDate);
} 