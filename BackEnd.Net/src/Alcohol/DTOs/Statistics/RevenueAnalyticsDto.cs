using System;

namespace Alcohol.DTOs.Statistics;

public class RevenueAnalyticsDto
{
    public DateTime Date { get; set; }
    public decimal TotalRevenue { get; set; }
    public decimal TotalProfit { get; set; }
    public decimal AverageOrderValue { get; set; }
    public int TotalOrders { get; set; }
} 